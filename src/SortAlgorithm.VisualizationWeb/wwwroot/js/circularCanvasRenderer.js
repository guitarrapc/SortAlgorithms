// Canvas 2D 円形レンダラー - 高速円形ビジュアライゼーション（複数Canvas対応）

window.circularCanvasRenderer = {
    instances: new Map(), // Canvas ID -> インスタンスのマップ
    resizeObserver: null, // ResizeObserver インスタンス
    lastRenderParams: new Map(), // Canvas ID -> 最後の描画パラメータ

    // rAFループ用
    dirtyCanvases: new Set(),  // 再描画が必要なCanvas
    isLoopRunning: false,      // rAFループが実行中かどうか
    rafId: null,               // requestAnimationFrame ID

    // JS 側配列コピー（Phase 3c）
    arrays: new Map(), // canvasId → { main: Int32Array, buffers: Map<bufferId, Int32Array> }

    // キャッシュされた Canvas サイズ（getBoundingClientRect をフレーム毎に呼ばないため）
    cachedSizes: new Map(), // canvasId → { width: number, height: number }
    
    // 色定義（操作に基づく）
    colors: {
        normal: null,           // HSLで動的に計算
        compare: '#A855F7',     // 紫
        swap: '#EF4444',        // 赤
        write: '#F97316',       // 橙
        read: '#FBBF24',        // 黄
        sorted: '#10B981'       // 緑 - ソート完了
    },
    
    /**
     * Canvasを初期化
     * @param {string} canvasId - Canvas要素のID
     */
    initialize: function(canvasId) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            window.debugHelper.error('Circular Canvas element not found:', canvasId);
            return false;
        }
        
        const ctx = canvas.getContext('2d', {
            alpha: false,           // 透明度不要（高速化）
            desynchronized: true    // 非同期描画（高速化）
        });
        
        // 高DPI対応
        const dpr = window.devicePixelRatio || 1;
        const rect = canvas.getBoundingClientRect();
        canvas.width = rect.width * dpr;
        canvas.height = rect.height * dpr;
        ctx.scale(dpr, dpr);
        
        // インスタンスを保存
        this.instances.set(canvasId, { canvas, ctx });
        this.cachedSizes.set(canvasId, { width: rect.width, height: rect.height });
        
        // ResizeObserverを初期化（まだ存在しない場合）
        if (!this.resizeObserver) {
            this.resizeObserver = new ResizeObserver(entries => {
                for (const entry of entries) {
                    const canvas = entry.target;
                    const canvasId = canvas.id;
                    const instance = this.instances.get(canvasId);
                    
                    if (instance) {
                        const { ctx } = instance;
                        const dpr = window.devicePixelRatio || 1;
                        const rect = canvas.getBoundingClientRect();
                        
                        // サイズが実際に変わった場合のみリサイズ
                        const newWidth = rect.width * dpr;
                        const newHeight = rect.height * dpr;
                        
                        if (canvas.width !== newWidth || canvas.height !== newHeight) {
                            canvas.width = newWidth;
                            canvas.height = newHeight;
                            ctx.scale(dpr, dpr);
                            this.cachedSizes.set(canvasId, { width: rect.width, height: rect.height });
                            
                            window.debugHelper.log('Circular Canvas auto-resized:', canvasId, rect.width, 'x', rect.height);
                            
                            // リサイズ後、最後の描画パラメータで即座に再描画（黒画面を防ぐ）
                            const lastParams = this.lastRenderParams.get(canvasId);
                            if (lastParams) {
                                requestAnimationFrame(() => {
                                    this.renderInternal(canvasId, lastParams);
                                });
                            }
                        }
                    }
                }
            });
        }
        
        // このCanvasを監視対象に追加
        this.resizeObserver.observe(canvas);
        
        window.debugHelper.log('Circular Canvas initialized:', canvasId, rect.width, 'x', rect.height, 'DPR:', dpr);
        return true;
    },
    
    /**
     * 値をHSL色に変換（0-maxValue -> 0-360度）
     * @param {number} value - 要素の値
     * @param {number} maxValue - 配列の最大値
     * @returns {string} HSL色文字列
     */
    valueToHSL: function(value, maxValue) {
        const hue = (value / maxValue) * 360;
        return `hsl(${hue}, 70%, 60%)`;
    },
    
    /**
     * 円形ビジュアライゼーションを描画
     * @param {string} canvasId - Canvas要素のID
     * @param {number[]} array - 描画する配列
     * @param {number[]} compareIndices - 比較中のインデックス
     * @param {number[]} swapIndices - スワップ中のインデックス
     * @param {number[]} readIndices - 読み取り中のインデックス
     * @param {number[]} writeIndices - 書き込み中のインデックス
     * @param {boolean} isSortCompleted - ソートが完了したかどうか
     * @param {Object} bufferArrays - バッファー配列（BufferId -> 配列）
     * @param {boolean} showCompletionHighlight - 完了ハイライトを表示するか
     */
    render: function(canvasId, array, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, bufferArrays, showCompletionHighlight) {
        // 後方互換用: updateData に委譲
        this.updateData(canvasId, array, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, bufferArrays, showCompletionHighlight);
    },

    /**
     * 新しいソートがロードされたとき（SortVersion 変化時）に C# から呼ばれる。
     * JS 側の配列コピーを初期化し、次フレームで再描画をスケジュールする。
     */
    setArray: function(canvasId, mainArray, bufferArrays, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight) {
        let entry = this.arrays.get(canvasId);
        if (!entry) {
            entry = { main: null, buffers: new Map() };
            this.arrays.set(canvasId, entry);
        }
        entry.main = new Int32Array(mainArray);
        entry.buffers.clear();
        if (bufferArrays) {
            for (const [idStr, arr] of Object.entries(bufferArrays)) {
                entry.buffers.set(parseInt(idStr), new Int32Array(arr));
            }
        }
        this._scheduleRender(canvasId, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight);
    },

    /**
     * 通常の再生フレームで C# から呼ばれる（高速パス）。
     * 差分を JS 側配列に適用し、次フレームで再描画をスケジュールする。
     */
    applyFrame: function(canvasId, mainDelta, bufferDeltas, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight) {
        const entry = this.arrays.get(canvasId);
        if (!entry || !entry.main) return;

        if (mainDelta) {
            for (let k = 0; k < mainDelta.length; k += 2) {
                entry.main[mainDelta[k]] = mainDelta[k + 1];
            }
        }
        if (bufferDeltas) {
            for (const [idStr, delta] of Object.entries(bufferDeltas)) {
                const bid = parseInt(idStr);
                let buf = entry.buffers.get(bid);
                if (!buf) {
                    buf = new Int32Array(entry.main.length);
                    entry.buffers.set(bid, buf);
                }
                for (let k = 0; k < delta.length; k += 2) {
                    buf[delta[k]] = delta[k + 1];
                }
            }
        }
        if (isSortCompleted && entry.buffers.size > 0) {
            entry.buffers.clear();
        }
        this._scheduleRender(canvasId, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight);
    },

    _scheduleRender: function(canvasId, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight) {
        this.lastRenderParams.set(canvasId, {
            compareIndices,
            swapIndices,
            readIndices,
            writeIndices,
            isSortCompleted: isSortCompleted || false,
            showCompletionHighlight: showCompletionHighlight !== undefined ? showCompletionHighlight : false
        });
        this.dirtyCanvases.add(canvasId);
        if (!this.isLoopRunning) this.startLoop();
    },

    /**
     * データを更新して rAF ループで再描画をスケジュール（シーク後・リセット後の全量更新フォールバック）
     * C# から呼ばれる主要エントリポイント（render の代替）
     */
    updateData: function(canvasId, array, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, bufferArrays, showCompletionHighlight) {
        this.setArray(canvasId, array, bufferArrays, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight);
    },

    /**
     * rAF 駆動の描画ループを開始する
     * dirty なCanvasのみ描画し、すべてが clean になったら停止する
     */
    startLoop: function() {
        if (this.isLoopRunning) return;
        this.isLoopRunning = true;

        const self = this;
        const tick = () => {
            if (self.dirtyCanvases.size > 0) {
                self.dirtyCanvases.forEach(canvasId => {
                    if (self.instances.has(canvasId)) {
                        const params = self.lastRenderParams.get(canvasId);
                        if (params) self.renderInternal(canvasId, params);
                    }
                });
                self.dirtyCanvases.clear();
                self.rafId = requestAnimationFrame(tick);
            } else {
                self.isLoopRunning = false;
                self.rafId = null;
            }
        };

        this.rafId = requestAnimationFrame(tick);
    },

    /**
     * 内部描画処理（実際のCanvas描画）
     * @param {string} canvasId - Canvas要素のID
     * @param {Object} params - 描画パラメータ
     */
    renderInternal: function(canvasId, params) {
        const instance = this.instances.get(canvasId);
        if (!instance) {
            window.debugHelper.error('Circular Canvas instance not found:', canvasId);
            return;
        }
        
        const { canvas, ctx } = instance;
        if (!canvas || !ctx) {
            window.debugHelper.error('Circular Canvas not initialized:', canvasId);
            return;
        }
        
        // パラメータ展開（ハイライト情報のみ；配列は arrays マップから取得）
        const { compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight } = params;

        // JS 側配列コピーを取得
        const entry = this.arrays.get(canvasId);
        if (!entry || !entry.main) return;
        const array = entry.main;
        
        const size = this.cachedSizes.get(canvasId);
        if (!size) return;
        const width = size.width;
        const height = size.height;
        const arrayLength = array.length;
        
        // バッファー配列の数を取得
        const bufferCount = entry.buffers.size;
        
        // 背景をクリア（黒）
        ctx.fillStyle = '#1A1A1A';
        ctx.fillRect(0, 0, width, height);
        
        // 配列が空の場合は何もしない
        if (arrayLength === 0) return;
        
        // 円の中心と半径を計算
        const centerX = width / 2;
        const centerY = height / 2;
        const maxRadius = Math.min(width, height) * 0.45; // 90%の直径を使用（余白考慮）
        const minRadius = maxRadius * 0.2; // 内側の空白（ドーナツ型）
        
        // バッファー配列が表示されている場合のみ同心円リングとして配置
        const showBuffers = bufferCount > 0 && !isSortCompleted;
        let mainMinRadius, mainMaxRadius;
        let ringWidth = 0;
        
        if (showBuffers) {
            // バッファーがある場合: リング分割
            const totalRings = 1 + bufferCount; // メイン + バッファー
            ringWidth = (maxRadius * 0.8) / totalRings; // 各リングの幅
            mainMinRadius = minRadius;
            mainMaxRadius = minRadius + ringWidth;
        } else {
            // バッファーがない場合: メイン配列が外側まで広がる
            mainMinRadius = minRadius;
            mainMaxRadius = maxRadius; // 最大半径まで使用
        }
        
        // 最大値を取得（スプレッド演算子は大配列でスタックオーバーフローのリスクがあるためループで計算）
        let maxValue = 0;
        for (let i = 0; i < arrayLength; i++) {
            if (array[i] > maxValue) maxValue = array[i];
        }

        // Set を使って高速な存在チェック
        const compareSet = new Set(compareIndices);
        const swapSet = new Set(swapIndices);
        const readSet = new Set(readIndices);
        const writeSet = new Set(writeIndices);
        
        // 各要素を円周上に配置
        const angleStep = (2 * Math.PI) / arrayLength;

        // 線の太さを配列サイズに応じて事前に1回計算
        const lineWidth = arrayLength <= 64 ? 3 : arrayLength <= 256 ? 2 : arrayLength <= 1024 ? 1.5 : 1;

        // メイン配列の線を描画（同色バッチ描画: strokeStyle 変更と stroke 呼び出しを最小化）
        ctx.lineWidth = lineWidth;
        if (showCompletionHighlight) {
            // 完了ハイライト: 全線を1色・1パスで一括描画
            ctx.strokeStyle = this.colors.sorted;
            ctx.beginPath();
            for (let i = 0; i < arrayLength; i++) {
                const angle = i * angleStep - Math.PI / 2;
                const radius = mainMinRadius + (array[i] / maxValue) * (mainMaxRadius - mainMinRadius);
                ctx.moveTo(centerX + Math.cos(angle) * mainMinRadius, centerY + Math.sin(angle) * mainMinRadius);
                ctx.lineTo(centerX + Math.cos(angle) * radius,        centerY + Math.sin(angle) * radius);
            }
            ctx.stroke();
        } else {
            // インデックスを色バケツに振り分け
            const swapBucket    = [];
            const compareBucket = [];
            const writeBucket   = [];
            const readBucket    = [];
            const normalBucket  = [];

            for (let i = 0; i < arrayLength; i++) {
                if      (swapSet.has(i))    swapBucket.push(i);
                else if (compareSet.has(i)) compareBucket.push(i);
                else if (writeSet.has(i))   writeBucket.push(i);
                else if (readSet.has(i))    readBucket.push(i);
                else                        normalBucket.push(i);
            }

            // 1. 通常色（HSLグラデーション）を描画 - 各要素で strokeStyle が異なるため個別描画
            for (const i of normalBucket) {
                const angle = i * angleStep - Math.PI / 2;
                const radius = mainMinRadius + (array[i] / maxValue) * (mainMaxRadius - mainMinRadius);
                ctx.strokeStyle = this.valueToHSL(array[i], maxValue);
                ctx.beginPath();
                ctx.moveTo(centerX + Math.cos(angle) * mainMinRadius, centerY + Math.sin(angle) * mainMinRadius);
                ctx.lineTo(centerX + Math.cos(angle) * radius,        centerY + Math.sin(angle) * radius);
                ctx.stroke();
            }

            // 2. ハイライト色をバッチ描画（1色につき1パス・1回の stroke）
            const highlightBuckets = [
                [compareBucket, this.colors.compare],
                [writeBucket,   this.colors.write],
                [readBucket,    this.colors.read],
                [swapBucket,    this.colors.swap],
            ];

            for (const [indices, color] of highlightBuckets) {
                if (indices.length === 0) continue;
                ctx.strokeStyle = color;
                ctx.beginPath();
                for (const i of indices) {
                    const angle = i * angleStep - Math.PI / 2;
                    const radius = mainMinRadius + (array[i] / maxValue) * (mainMaxRadius - mainMinRadius);
                    ctx.moveTo(centerX + Math.cos(angle) * mainMinRadius, centerY + Math.sin(angle) * mainMinRadius);
                    ctx.lineTo(centerX + Math.cos(angle) * radius,        centerY + Math.sin(angle) * radius);
                }
                ctx.stroke();
            }
        }
        
        // バッファー配列を同心円リングとして描画（ソート完了時は非表示）
        if (showBuffers) {
            const sortedBufferIds = [...entry.buffers.keys()].sort((a, b) => a - b);
            
            for (let bufferIndex = 0; bufferIndex < sortedBufferIds.length; bufferIndex++) {
                const bufferId = sortedBufferIds[bufferIndex];
                const bufferArray = entry.buffers.get(bufferId);
                
                if (!bufferArray || bufferArray.length === 0) continue;
                
                // このバッファー配列のリング範囲
                const ringIndex = bufferIndex + 1; // メイン配列の外側
                const bufferMinRadius = minRadius + ringIndex * ringWidth;
                const bufferMaxRadius = bufferMinRadius + ringWidth;
                
                // バッファー配列の最大値（ループで安全に）
                let bufferMaxValue = 0;
                const bufferLength = bufferArray.length;
                for (let i = 0; i < bufferLength; i++) {
                    if (bufferArray[i] > bufferMaxValue) bufferMaxValue = bufferArray[i];
                }
                const bufferAngleStep = (2 * Math.PI) / bufferLength;
                const bufferLineWidth = bufferLength <= 64 ? 3 : bufferLength <= 256 ? 2 : bufferLength <= 1024 ? 1.5 : 1;
                
                // バッファー配列の線を描画（単色なので1パス・1回の stroke で一括）
                ctx.strokeStyle = '#06B6D4';
                ctx.lineWidth = bufferLineWidth;
                ctx.beginPath();
                for (let i = 0; i < bufferLength; i++) {
                    const angle = i * bufferAngleStep - Math.PI / 2;
                    const radius = bufferMinRadius + (bufferArray[i] / bufferMaxValue) * (bufferMaxRadius - bufferMinRadius);
                    ctx.moveTo(centerX + Math.cos(angle) * bufferMinRadius, centerY + Math.sin(angle) * bufferMinRadius);
                    ctx.lineTo(centerX + Math.cos(angle) * radius,          centerY + Math.sin(angle) * radius);
                }
                ctx.stroke();
                
                // バッファーIDラベルを表示（円の外側）
                const labelAngle = -Math.PI / 2; // 12時の位置
                const labelRadius = bufferMaxRadius + 15;
                const labelX = centerX + Math.cos(labelAngle) * labelRadius;
                const labelY = centerY + Math.sin(labelAngle) * labelRadius;
                
                ctx.fillStyle = '#888';
                ctx.font = '12px monospace';
                ctx.textAlign = 'center';
                ctx.fillText(`Buf#${bufferId}`, labelX, labelY);
            }
        }
        
        // 中心円を描画（オプション、視覚的なアクセント）
        ctx.fillStyle = '#2A2A2A';
        ctx.beginPath();
        ctx.arc(centerX, centerY, minRadius, 0, 2 * Math.PI);
        ctx.fill();
    },
    
    /**
     * クリーンアップ
     */
    dispose: function(canvasId) {
        if (canvasId) {
            // Canvas要素を取得
            const canvas = document.getElementById(canvasId);
            
            // ResizeObserverの監視を解除
            if (canvas && this.resizeObserver) {
                this.resizeObserver.unobserve(canvas);
            }
            
            // 特定のCanvasインスタンスを削除
            const deleted = this.instances.delete(canvasId);
            if (deleted) {
                console.log('Circular Canvas instance disposed:', canvasId);
            } else {
                console.warn('Circular Canvas instance not found for disposal:', canvasId);
            }
            
            // 描画パラメータと dirty フラグ、JS 側配列コピーも削除
            this.lastRenderParams.delete(canvasId);
            this.dirtyCanvases.delete(canvasId);
            this.arrays.delete(canvasId);
            this.cachedSizes.delete(canvasId);
        } else {
            // rAFループを停止
            if (this.rafId) {
                cancelAnimationFrame(this.rafId);
                this.rafId = null;
            }
            this.isLoopRunning = false;
            this.dirtyCanvases.clear();

            // ResizeObserverをリセット
            if (this.resizeObserver) {
                this.resizeObserver.disconnect();
                this.resizeObserver = null;
            }
            
            // すべてのインスタンスをクリア
            this.instances.clear();
            this.lastRenderParams.clear();
            this.arrays.clear();
            this.cachedSizes.clear();
        }
    }
};

// ウィンドウリサイズ時の処理は不要（ResizeObserverが自動処理）
