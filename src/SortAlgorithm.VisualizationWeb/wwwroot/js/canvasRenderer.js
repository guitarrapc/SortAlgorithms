// Canvas 2D レンダラー - 高速バーチャート描画（複数Canvas対応）

window.canvasRenderer = {
instances: new Map(), // Canvas ID -> インスタンスのマップ
resizeObserver: null, // ResizeObserver インスタンス
lastRenderParams: new Map(), // Canvas ID -> 最後の描画パラメータ
    
// デバッグ用：FPS計測
renderCounts: new Map(),
lastFpsLogs: new Map(),

// rAFループ用
dirtyCanvases: new Set(),  // 再描画が必要なCanvas
isLoopRunning: false,      // rAFループが実行中かどうか
rafId: null,               // requestAnimationFrame ID
    
// 色定義
colors: {
    normal: '#3B82F6',      // 青
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
            window.debugHelper.error('Canvas element not found:', canvasId);
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
                            
                            window.debugHelper.log('Canvas auto-resized:', canvasId, rect.width, 'x', rect.height);
                            
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
        
        window.debugHelper.log('Canvas initialized:', canvasId, rect.width, 'x', rect.height, 'DPR:', dpr);
        return true;
    },
    
    /**
     * リサイズ処理
     * @param {string} canvasId - リサイズするCanvas要素のID（省略時は全Canvas）
     */
    resize: function(canvasId) {
        // レイアウトの更新を待つためにrequestAnimationFrameを使用
        requestAnimationFrame(() => {
            if (canvasId) {
                // 特定のCanvasをリサイズ
                const instance = this.instances.get(canvasId);
                if (instance) {
                    const { canvas, ctx } = instance;
                    if (canvas) {
                        const dpr = window.devicePixelRatio || 1;
                        const rect = canvas.getBoundingClientRect();
                        canvas.width = rect.width * dpr;
                        canvas.height = rect.height * dpr;
                        ctx.scale(dpr, dpr);
                        
                        console.log('Canvas resized:', canvasId, rect.width, 'x', rect.height);
                    }
                } else {
                    console.warn('Canvas instance not found for resize:', canvasId);
                }
            } else {
                // すべてのCanvasをリサイズ
                this.instances.forEach((instance, id) => {
                    const { canvas, ctx } = instance;
                    if (!canvas) return;
                    
                    const dpr = window.devicePixelRatio || 1;
                    const rect = canvas.getBoundingClientRect();
                    canvas.width = rect.width * dpr;
                    canvas.height = rect.height * dpr;
                    ctx.scale(dpr, dpr);
                    
                    console.log('Canvas resized:', id, rect.width, 'x', rect.height);
                });
            }
        });
    },
    
    /**
     * バーチャートを描画
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
     * データを更新して rAF ループで再描画をスケジュール
     * C# から呼ばれる主要エントリポイント（render の代替）
     */
    updateData: function(canvasId, array, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, bufferArrays, showCompletionHighlight) {
        const params = {
            array,
            compareIndices,
            swapIndices,
            readIndices,
            writeIndices,
            isSortCompleted: isSortCompleted || false,
            bufferArrays: bufferArrays || {},
            showCompletionHighlight: showCompletionHighlight !== undefined ? showCompletionHighlight : false
        };
        this.lastRenderParams.set(canvasId, params);
        this.dirtyCanvases.add(canvasId);

        if (!this.isLoopRunning) {
            this.startLoop();
        }
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
            window.debugHelper.error('Canvas instance not found:', canvasId);
            return;
        }
        
        const { canvas, ctx } = instance;
        if (!canvas || !ctx) {
            window.debugHelper.error('Canvas not initialized:', canvasId);
            return;
        }
        
        // パラメータ展開
        const { array, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, bufferArrays, showCompletionHighlight } = params;
        
        // 🔍 デバッグ：render() 呼び出し回数をカウント
        if (!this.renderCounts.has(canvasId)) {
            this.renderCounts.set(canvasId, 0);
            this.lastFpsLogs.set(canvasId, Date.now());
        }
        this.renderCounts.set(canvasId, this.renderCounts.get(canvasId) + 1);
        
        const now = Date.now();
        const lastLog = this.lastFpsLogs.get(canvasId);
        const elapsed = (now - lastLog) / 1000;
        
        if (elapsed >= 1.0) {
            const fps = this.renderCounts.get(canvasId) / elapsed;
            window.debugHelper.log(`[JS Canvas] ${canvasId.substring(0, 12)}... JS render() FPS: ${fps.toFixed(1)}`);
            this.renderCounts.set(canvasId, 0);
            this.lastFpsLogs.set(canvasId, now);
        }
        
        const rect = canvas.getBoundingClientRect();
        const width = rect.width;
        const height = rect.height;
        const arrayLength = array.length;
        
        // バッファー配列の数を取得
        const bufferCount = Object.keys(bufferArrays).length;
        
        // 背景をクリア（黒）
        ctx.fillStyle = '#1A1A1A';
        ctx.fillRect(0, 0, width, height);
        
        // 配列が空の場合は何もしない
        if (arrayLength === 0) return;
        
        // バッファー配列が表示されている場合のみ画面を分割
        const showBuffers = bufferCount > 0 && !isSortCompleted;
        const totalSections = showBuffers ? (1 + bufferCount) : 1;
        const sectionHeight = height / totalSections;
        const mainArrayY = showBuffers ? (sectionHeight * bufferCount) : 0; // バッファー表示時は下部、非表示時は画面全体
        
        // バーの幅と隙間を計算
        const minBarWidth = 1.0;
        let gapRatio;
        if (arrayLength <= 256) {
            gapRatio = 0.15;
        } else if (arrayLength <= 1024) {
            gapRatio = 0.10;
        } else {
            gapRatio = 0.05;
        }
        
        const requiredWidth = Math.max(width, arrayLength * minBarWidth / (1.0 - gapRatio));
        const totalBarWidth = requiredWidth / arrayLength;
        const barWidth = totalBarWidth * (1.0 - gapRatio);
        const gap = totalBarWidth * gapRatio;
        
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
        
        // スケール調整（横スクロール対応）
        const scale = Math.min(1.0, width / requiredWidth);
        ctx.save();
        if (scale < 1.0) {
            // 横スクロールが必要な場合は左寄せ
            ctx.scale(scale, 1.0);
        }

        // メイン配列のバーを描画（同色バッチ描画: fillStyle 切り替えを最小化）
        const usableHeight = sectionHeight - 20;
        if (showCompletionHighlight) {
            // 完了ハイライト: 全バーを1色で一括描画
            ctx.fillStyle = this.colors.sorted;
            for (let i = 0; i < arrayLength; i++) {
                const barHeight = (array[i] / maxValue) * usableHeight;
                ctx.fillRect(
                    i * totalBarWidth + (gap / 2),
                    mainArrayY + (sectionHeight - barHeight),
                    barWidth, barHeight
                );
            }
        } else {
            // 通常描画: インデックスを色バケツに振り分けてから色ごとに一括描画
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

            // 描画順: normal → compare → write → read → swap（ハイライトを前面に重ねる）
            const buckets = [
                [normalBucket,  this.colors.normal],
                [compareBucket, this.colors.compare],
                [writeBucket,   this.colors.write],
                [readBucket,    this.colors.read],
                [swapBucket,    this.colors.swap],
            ];

            for (const [indices, color] of buckets) {
                if (indices.length === 0) continue;
                ctx.fillStyle = color;
                for (const i of indices) {
                    const barHeight = (array[i] / maxValue) * usableHeight;
                    ctx.fillRect(
                        i * totalBarWidth + (gap / 2),
                        mainArrayY + (sectionHeight - barHeight),
                        barWidth, barHeight
                    );
                }
            }
        }

        ctx.restore();
        
        // バッファー配列を描画（ソート完了時は非表示）
        if (showBuffers) {
            const bufferIds = Object.keys(bufferArrays).sort((a, b) => parseInt(a) - parseInt(b));
            
            for (let bufferIndex = 0; bufferIndex < bufferIds.length; bufferIndex++) {
                const bufferId = bufferIds[bufferIndex];
                const bufferArray = bufferArrays[bufferId];
                const bufferY = bufferIndex * sectionHeight;
                
                if (!bufferArray || bufferArray.length === 0) continue;
                
                // バッファー配列の最大値（ループで安全に）
                let bufferMaxValue = 0;
                const bufferLength = bufferArray.length;
                for (let i = 0; i < bufferLength; i++) {
                    if (bufferArray[i] > bufferMaxValue) bufferMaxValue = bufferArray[i];
                }
                
                // バッファー配列用のバー幅計算（メイン配列と同じロジック）
                const bufferRequiredWidth = Math.max(width, bufferLength * minBarWidth / (1.0 - gapRatio));
                const bufferTotalBarWidth = bufferRequiredWidth / bufferLength;
                const bufferBarWidth = bufferTotalBarWidth * (1.0 - gapRatio);
                const bufferGap = bufferTotalBarWidth * gapRatio;
                
                // バッファー配列のスケール
                const bufferScale = Math.min(1.0, width / bufferRequiredWidth);
                ctx.save();
                if (bufferScale < 1.0) {
                    ctx.scale(bufferScale, 1.0);
                }
                
                // バッファー配列のバーを描画（単色なので fillStyle は1回）
                const bufferUsableHeight = sectionHeight - 20;
                ctx.fillStyle = '#06B6D4';
                for (let i = 0; i < bufferLength; i++) {
                    const barHeight = (bufferArray[i] / bufferMaxValue) * bufferUsableHeight;
                    ctx.fillRect(
                        i * bufferTotalBarWidth + (bufferGap / 2),
                        bufferY + (sectionHeight - barHeight),
                        bufferBarWidth, barHeight
                    );
                }
                
                ctx.restore();
                
                // バッファーIDラベルを表示
                ctx.fillStyle = '#888';
                ctx.font = '12px monospace';
                ctx.fillText(`Buffer #${bufferId}`, 10, bufferY + 20);
            }
        }
        
        // メイン配列ラベルを表示（バッファーが表示されている場合のみ）
        if (showBuffers) {
            ctx.fillStyle = '#888';
            ctx.font = '12px monospace';
            ctx.fillText('Main Array', 10, mainArrayY + 20);
        }
    },
    
    /**
     * クリーンアップ
     * @param {string} canvasId - 削除するCanvas要素のID（省略時は全削除）
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
                console.log('Canvas instance disposed:', canvasId);
            } else {
                console.warn('Canvas instance not found for disposal:', canvasId);
            }
            
            // FPS計測用のデータ、描画パラメータ、dirty フラグも削除
            this.renderCounts.delete(canvasId);
            this.lastFpsLogs.delete(canvasId);
            this.lastRenderParams.delete(canvasId);
            this.dirtyCanvases.delete(canvasId);
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
            this.renderCounts.clear();
            this.lastFpsLogs.clear();
            this.lastRenderParams.clear();
        }
    }
};

// ウィンドウリサイズ時の処理
window.addEventListener('resize', () => {
    if (window.canvasRenderer.canvas) {
        window.canvasRenderer.resize();
    }
});
