// Canvas 2D ドットプロットレンダラー - 高速散布図描画（複数Canvas対応）

window.dotPlotCanvasRenderer = {
  instances: new Map(),       // Canvas ID -> インスタンスのマップ
  resizeObserver: null,       // ResizeObserver インスタンス
  lastRenderParams: new Map(), // Canvas ID -> 最後の描画パラメータ

  // rAFループ用
  dirtyCanvases: new Set(),  // 再描画が必要なCanvas
  isLoopRunning: false,      // rAFループが実行中かどうか
  rafId: null,               // requestAnimationFrame ID

  // JS 側配列コピー
  arrays: new Map(), // canvasId → { main: Int32Array, buffers: Map<bufferId, Int32Array> }

  // Phase 4: OffscreenCanvas + Worker
  workers: new Map(), // canvasId → { worker: Worker, lastWidth: number, lastHeight: number }

  // キャッシュされた Canvas サイズ（getBoundingClientRect をフレーム毎に呼ばないため）
  cachedSizes: new Map(), // canvasId → { width: number, height: number }

  // 色定義
  colors: {
    compare: '#A855F7',  // 紫
    swap: '#EF4444',     // 赤
    write: '#F97316',    // 橙
    read: '#FBBF24',     // 黄
    sorted: '#10B981'    // 緑 - ソート完了
  },

  // HSL カラールックアップテーブル（Canvas 2D fallback 用）
  _colorLUTMax: -1,
  _colorLUT: null,

  /**
   * Canvas を初期化
   * @param {string} canvasId - Canvas要素のID
   * @param {boolean} useWebGL - WebGL レンダラーを使用するか（false で Canvas 2D Worker）
   */
  initialize: function (canvasId, useWebGL = true) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
      window.debugHelper.error('DotPlot Canvas element not found:', canvasId);
      return false;
    }

    // 既に初期化済みの場合はスキップ（二重初期化防止）
    if (this.workers.has(canvasId) || this.instances.has(canvasId)) {
      window.debugHelper.warn('DotPlot Canvas already initialized:', canvasId);
      return true;
    }

    const dpr = window.devicePixelRatio || 1;
    const rect = canvas.getBoundingClientRect();
    this.cachedSizes.set(canvasId, { width: rect.width, height: rect.height });

    // Phase 4: OffscreenCanvas + Worker パス（Chrome 69+, Firefox 105+, Safari 16.4+）
    if (typeof canvas.transferControlToOffscreen === 'function') {
      canvas.width = rect.width * dpr;
      canvas.height = rect.height * dpr;

      const workerFile = useWebGL ? 'js/dotPlotWebglWorker.js' : 'js/dotPlotRenderWorker.js';
      const offscreen = canvas.transferControlToOffscreen();
      const workerUrl = new URL(workerFile, document.baseURI).href;
      const worker = new Worker(workerUrl);
      worker.postMessage({ type: 'init', canvas: offscreen, dpr }, [offscreen]);

      this.workers.set(canvasId, { worker, lastWidth: canvas.width, lastHeight: canvas.height });
      // ResizeObserver のために canvas 要素を instances に保存（ctx は null）
      this.instances.set(canvasId, { canvas, ctx: null });

      this._ensureResizeObserver();
      this.resizeObserver.observe(canvas);

      window.debugHelper.log('DotPlot Canvas initialized (Worker):', canvasId, rect.width, 'x', rect.height, 'DPR:', dpr, 'WebGL:', useWebGL);
      return true;
    }

    // フォールバック: Canvas 2D パス
    const ctx = canvas.getContext('2d', {
      alpha: false,
      desynchronized: true
    });

    canvas.width = rect.width * dpr;
    canvas.height = rect.height * dpr;
    ctx.scale(dpr, dpr);

    this.instances.set(canvasId, { canvas, ctx });

    this._ensureResizeObserver();
    this.resizeObserver.observe(canvas);

    window.debugHelper.log('DotPlot Canvas initialized (Canvas2D):', canvasId, rect.width, 'x', rect.height, 'DPR:', dpr);
    return true;
  },

  /**
   * ResizeObserver を一度だけ初期化する（内部ヘルパー）
   */
  _ensureResizeObserver: function () {
    if (this.resizeObserver) return;
    this.resizeObserver = new ResizeObserver(entries => {
      for (const entry of entries) {
        const canvas = entry.target;
        const canvasId = canvas.id;
        const instance = this.instances.get(canvasId);

        if (instance) {
          const dpr = window.devicePixelRatio || 1;
          const rect = canvas.getBoundingClientRect();
          const newWidth = rect.width * dpr;
          const newHeight = rect.height * dpr;

          const workerInfo = this.workers.get(canvasId);
          if (workerInfo) {
            // Worker パス: OffscreenCanvas のリサイズを Worker に通知
            if (workerInfo.lastWidth !== newWidth || workerInfo.lastHeight !== newHeight) {
              workerInfo.lastWidth = newWidth;
              workerInfo.lastHeight = newHeight;
              this.cachedSizes.set(canvasId, { width: rect.width, height: rect.height });
              workerInfo.worker.postMessage({ type: 'resize', newWidth, newHeight, dpr });
              window.debugHelper.log('DotPlot Worker canvas resize notified:', canvasId, rect.width, 'x', rect.height);
            }
          } else {
            // Canvas 2D パス: 直接リサイズ
            const { ctx } = instance;
            if (canvas.width !== newWidth || canvas.height !== newHeight) {
              canvas.width = newWidth;
              canvas.height = newHeight;
              ctx.scale(dpr, dpr);
              this.cachedSizes.set(canvasId, { width: rect.width, height: rect.height });

              window.debugHelper.log('DotPlot Canvas auto-resized:', canvasId, rect.width, 'x', rect.height);

              const lastParams = this.lastRenderParams.get(canvasId);
              if (lastParams) {
                requestAnimationFrame(() => {
                  this.renderInternal(canvasId, lastParams);
                });
              }
            }
          }
        }
      }
    });
  },

  /**
   * 新しいソートがロードされたとき（SortVersion 変化時）に C# から呼ばれる。
   */
  setArray: function (canvasId, mainArray, bufferArrays, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight) {
    // Phase 4: Worker パス
    const workerInfo = this.workers.get(canvasId);
    if (workerInfo) {
      workerInfo.worker.postMessage({
        type: 'setArray',
        mainArray,
        bufferArrays,
        compareIndices,
        swapIndices,
        readIndices,
        writeIndices,
        isSortCompleted: isSortCompleted || false,
        showCompletionHighlight: showCompletionHighlight || false
      });
      return;
    }
    // Canvas 2D パス
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
   */
  applyFrame: function (canvasId, mainDelta, bufferDeltas, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight) {
    // Phase 4: Worker パス
    const workerInfo = this.workers.get(canvasId);
    if (workerInfo) {
      workerInfo.worker.postMessage({
        type: 'applyFrame',
        mainDelta,
        bufferDeltas,
        compareIndices,
        swapIndices,
        readIndices,
        writeIndices,
        isSortCompleted: isSortCompleted || false,
        showCompletionHighlight: showCompletionHighlight || false
      });
      return;
    }
    // Canvas 2D パス
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

  _scheduleRender: function (canvasId, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight) {
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
   */
  updateData: function (canvasId, array, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, bufferArrays, showCompletionHighlight) {
    this.setArray(canvasId, array, bufferArrays, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight);
  },

  /**
   * rAF 駆動の描画ループを開始する
   */
  startLoop: function () {
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
   * HSL カラー LUT を構築（最大値が変わったときのみ再構築）
   */
  _buildColorLUT: function (maxValue) {
    if (this._colorLUTMax === maxValue) return;
    this._colorLUTMax = maxValue;
    this._colorLUT = new Array(maxValue + 1);
    for (let v = 0; v <= maxValue; v++) {
      const hue = (v / maxValue) * 360;
      this._colorLUT[v] = `hsl(${hue}, 70%, 60%)`;
    }
  },

  /**
   * 内部描画処理（Canvas 2D fallback 用）
   */
  renderInternal: function (canvasId, params) {
    const instance = this.instances.get(canvasId);
    if (!instance) {
      window.debugHelper.error('DotPlot Canvas instance not found:', canvasId);
      return;
    }

    const { canvas, ctx } = instance;
    if (!canvas || !ctx) {
      window.debugHelper.error('DotPlot Canvas not initialized:', canvasId);
      return;
    }

    const { compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, showCompletionHighlight } = params;

    const entry = this.arrays.get(canvasId);
    if (!entry || !entry.main) return;
    const array = entry.main;

    const size = this.cachedSizes.get(canvasId);
    if (!size) return;
    const width = size.width;
    const height = size.height;
    const arrayLength = array.length;

    const bufferCount = (isSortCompleted || showCompletionHighlight) ? 0 : entry.buffers.size;
    const showBuffers = bufferCount > 0;
    const totalSections = showBuffers ? (1 + bufferCount) : 1;
    const sectionHeight = height / totalSections;
    const mainSectionY = showBuffers ? sectionHeight * bufferCount : 0;

    ctx.fillStyle = '#1A1A1A';
    ctx.fillRect(0, 0, width, height);

    if (arrayLength === 0) return;

    let maxValue = 0;
    for (let i = 0; i < arrayLength; i++) {
      if (array[i] > maxValue) maxValue = array[i];
    }
    if (maxValue === 0) maxValue = 1;

    this._buildColorLUT(maxValue);
    const colorLUT = this._colorLUT;

    const dotRadius = arrayLength <= 64 ? 5 : arrayLength <= 256 ? 3 : arrayLength <= 1024 ? 2 : 1;
    const dotSize = dotRadius * 2;
    const usableH = sectionHeight - 20;
    const xStep = width / arrayLength;

    const compareSet = new Set(compareIndices);
    const swapSet = new Set(swapIndices);
    const readSet = new Set(readIndices);
    const writeSet = new Set(writeIndices);

    if (showCompletionHighlight) {
      ctx.fillStyle = this.colors.sorted;
      for (let i = 0; i < arrayLength; i++) {
        const x = (i + 0.5) * xStep - dotRadius;
        const y = mainSectionY + usableH - (array[i] / maxValue) * usableH - dotRadius;
        ctx.fillRect(x, y, dotSize, dotSize);
      }
    } else {
      const swapBucket = [];
      const compareBucket = [];
      const writeBucket = [];
      const readBucket = [];
      const normalBucket = [];

      for (let i = 0; i < arrayLength; i++) {
        if (swapSet.has(i)) swapBucket.push(i);
        else if (compareSet.has(i)) compareBucket.push(i);
        else if (writeSet.has(i)) writeBucket.push(i);
        else if (readSet.has(i)) readBucket.push(i);
        else normalBucket.push(i);
      }

      // 通常色（HSLグラデーション）: 各要素で fillStyle が変わるため個別描画
      for (const i of normalBucket) {
        const x = (i + 0.5) * xStep - dotRadius;
        const y = mainSectionY + usableH - (array[i] / maxValue) * usableH - dotRadius;
        ctx.fillStyle = colorLUT[array[i]];
        ctx.fillRect(x, y, dotSize, dotSize);
      }

      // ハイライト色をバッチ描画
      const highlightBuckets = [
        [compareBucket, this.colors.compare],
        [writeBucket, this.colors.write],
        [readBucket, this.colors.read],
        [swapBucket, this.colors.swap],
      ];

      for (const [indices, color] of highlightBuckets) {
        if (indices.length === 0) continue;
        ctx.fillStyle = color;
        for (const i of indices) {
          const x = (i + 0.5) * xStep - dotRadius;
          const y = mainSectionY + usableH - (array[i] / maxValue) * usableH - dotRadius;
          ctx.fillRect(x, y, dotSize, dotSize);
        }
      }
    }

    // バッファー配列を描画
    if (showBuffers) {
      const sortedBufferIds = [...entry.buffers.keys()].sort((a, b) => a - b);

      for (let bufferIndex = 0; bufferIndex < sortedBufferIds.length; bufferIndex++) {
        const bufferId = sortedBufferIds[bufferIndex];
        const bufferArray = entry.buffers.get(bufferId);
        if (!bufferArray || bufferArray.length === 0) continue;

        const bufferSectionY = bufferIndex * sectionHeight;
        const bufferLength = bufferArray.length;

        let bufferMaxValue = 0;
        for (let i = 0; i < bufferLength; i++) {
          if (bufferArray[i] > bufferMaxValue) bufferMaxValue = bufferArray[i];
        }
        if (bufferMaxValue === 0) bufferMaxValue = 1;

        const bufferUsableH = sectionHeight - 20;
        const bufferXStep = width / bufferLength;
        const bufferDotRadius = bufferLength <= 64 ? 5 : bufferLength <= 256 ? 3 : bufferLength <= 1024 ? 2 : 1;
        const bufferDotSize = bufferDotRadius * 2;

        ctx.fillStyle = '#06B6D4';
        for (let i = 0; i < bufferLength; i++) {
          const x = (i + 0.5) * bufferXStep - bufferDotRadius;
          const y = bufferSectionY + bufferUsableH - (bufferArray[i] / bufferMaxValue) * bufferUsableH - bufferDotRadius;
          ctx.fillRect(x, y, bufferDotSize, bufferDotSize);
        }

        ctx.fillStyle = '#888';
        ctx.font = '12px monospace';
        ctx.fillText(`Buffer #${bufferId}`, 10, bufferSectionY + 20);
      }
    }

    if (showBuffers) {
      ctx.fillStyle = '#888';
      ctx.font = '12px monospace';
      ctx.fillText('Main Array', 10, mainSectionY + 20);
    }
  },

  /**
   * クリーンアップ
   */
  dispose: function (canvasId) {
    if (canvasId) {
      // Phase 4: Worker を終了
      const workerInfo = this.workers.get(canvasId);
      if (workerInfo) {
        workerInfo.worker.postMessage({ type: 'dispose' });
        workerInfo.worker.terminate();
        this.workers.delete(canvasId);
      }

      const canvas = document.getElementById(canvasId);

      if (canvas && this.resizeObserver) {
        this.resizeObserver.unobserve(canvas);
      }

      const deleted = this.instances.delete(canvasId);
      if (deleted) {
        console.log('DotPlot Canvas instance disposed:', canvasId);
      } else {
        console.warn('DotPlot Canvas instance not found for disposal:', canvasId);
      }

      this.lastRenderParams.delete(canvasId);
      this.dirtyCanvases.delete(canvasId);
      this.arrays.delete(canvasId);
      this.cachedSizes.delete(canvasId);
    } else {
      // Phase 4: すべての Worker を終了
      this.workers.forEach(info => {
        info.worker.postMessage({ type: 'dispose' });
        info.worker.terminate();
      });
      this.workers.clear();

      if (this.rafId) {
        cancelAnimationFrame(this.rafId);
        this.rafId = null;
      }
      this.isLoopRunning = false;
      this.dirtyCanvases.clear();

      if (this.resizeObserver) {
        this.resizeObserver.disconnect();
        this.resizeObserver = null;
      }

      this.instances.clear();
      this.lastRenderParams.clear();
      this.arrays.clear();
      this.cachedSizes.clear();
    }
  }
};
