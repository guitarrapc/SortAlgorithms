// OffscreenCanvas ドットプロット描画 Worker（Phase 4 - DotPlot）
// メインスレッドから描画処理を完全に分離し、独立した rAF ループで描画する
'use strict';

let offscreen = null;
let ctx = null;
let dpr = 1;
let arrays = { main: null, buffers: new Map() };
let renderParams = null;
let isDirty = false;
let isLoopRunning = false;

const colors = {
  compare: '#A855F7',  // 紫
  swap: '#EF4444',     // 赤
  write: '#F97316',    // 橙
  read: '#FBBF24',     // 黄
  sorted: '#10B981'    // 緑 - ソート完了
};

// HSL カラールックアップテーブル（値 → 色文字列）
let colorLUTMax = -1;
let colorLUT = null;

function buildColorLUT(maxValue) {
  if (colorLUTMax === maxValue) return;
  colorLUTMax = maxValue;
  colorLUT = new Array(maxValue + 1);
  for (let v = 0; v <= maxValue; v++) {
    const hue = (v / maxValue) * 360;
    colorLUT[v] = `hsl(${hue}, 70%, 60%)`;
  }
}

// requestAnimationFrame が Worker で利用可能か確認（利用不可の場合は setTimeout でフォールバック）
const _raf = typeof requestAnimationFrame !== 'undefined'
  ? (cb) => requestAnimationFrame(cb)
  : (cb) => setTimeout(cb, 1000 / 60);

self.onmessage = function (e) {
  const msg = e.data;
  switch (msg.type) {
    case 'init': {
      offscreen = msg.canvas; // OffscreenCanvas（Transferable で受け取る）
      dpr = msg.dpr || 1;
      ctx = offscreen.getContext('2d', { alpha: false });
      ctx.scale(dpr, dpr);
      break;
    }
    case 'setArray': {
      arrays.main = new Int32Array(msg.mainArray);
      arrays.buffers.clear();
      if (msg.bufferArrays) {
        for (const [idStr, arr] of Object.entries(msg.bufferArrays)) {
          arrays.buffers.set(parseInt(idStr), new Int32Array(arr));
        }
      }
      renderParams = {
        compareIndices: msg.compareIndices,
        swapIndices: msg.swapIndices,
        readIndices: msg.readIndices,
        writeIndices: msg.writeIndices,
        isSortCompleted: msg.isSortCompleted || false,
        showCompletionHighlight: msg.showCompletionHighlight || false
      };
      scheduleDraw();
      break;
    }
    case 'applyFrame': {
      if (!arrays.main) break;
      // メイン配列に差分を適用（flat: [index, value, ...]）
      if (msg.mainDelta) {
        const delta = msg.mainDelta;
        for (let k = 0; k < delta.length; k += 2) {
          arrays.main[delta[k]] = delta[k + 1];
        }
      }
      // バッファー配列に差分を適用
      if (msg.bufferDeltas) {
        for (const [idStr, delta] of Object.entries(msg.bufferDeltas)) {
          const bid = parseInt(idStr);
          let buf = arrays.buffers.get(bid);
          if (!buf) {
            buf = new Int32Array(arrays.main.length);
            arrays.buffers.set(bid, buf);
          }
          for (let k = 0; k < delta.length; k += 2) {
            buf[delta[k]] = delta[k + 1];
          }
        }
      }
      // ソート完了時はバッファーを解放
      if (msg.isSortCompleted && arrays.buffers.size > 0) {
        arrays.buffers.clear();
      }
      renderParams = {
        compareIndices: msg.compareIndices,
        swapIndices: msg.swapIndices,
        readIndices: msg.readIndices,
        writeIndices: msg.writeIndices,
        isSortCompleted: msg.isSortCompleted || false,
        showCompletionHighlight: msg.showCompletionHighlight || false
      };
      scheduleDraw();
      break;
    }
    case 'resize': {
      if (offscreen) {
        offscreen.width = msg.newWidth;
        offscreen.height = msg.newHeight;
        dpr = msg.dpr || dpr;
        // canvas.width/height の変更でコンテキストがリセットされるため再スケール
        ctx.scale(dpr, dpr);
        if (renderParams && arrays.main) scheduleDraw();
      }
      break;
    }
    case 'dispose': {
      offscreen = null;
      ctx = null;
      arrays = { main: null, buffers: new Map() };
      renderParams = null;
      isDirty = false;
      isLoopRunning = false;
      break;
    }
  }
};

function scheduleDraw() {
  isDirty = true;
  if (!isLoopRunning) startLoop();
}

function startLoop() {
  if (isLoopRunning) return;
  isLoopRunning = true;
  tick();
}

function tick() {
  if (isDirty && offscreen && ctx && arrays.main) {
    isDirty = false;
    draw();
    _raf(tick);
  } else {
    isLoopRunning = false;
  }
}

function draw() {
  if (!offscreen || !ctx || !arrays.main || !renderParams) return;

  const {
    compareIndices, swapIndices, readIndices, writeIndices,
    isSortCompleted, showCompletionHighlight
  } = renderParams;
  const array = arrays.main;
  const width = offscreen.width / dpr;
  const height = offscreen.height / dpr;
  const arrayLength = array.length;

  // バッファー配列の数（ソート完了時はゼロ扱い）
  const bufferCount = (isSortCompleted || showCompletionHighlight) ? 0 : arrays.buffers.size;
  const showBuffers = bufferCount > 0;
  const totalSections = showBuffers ? (1 + bufferCount) : 1;
  const sectionHeight = height / totalSections;
  const mainSectionY = showBuffers ? sectionHeight * bufferCount : 0;

  // 背景をクリア（黒）
  ctx.fillStyle = '#1A1A1A';
  ctx.fillRect(0, 0, width, height);

  if (arrayLength === 0) return;

  // 最大値を取得（スプレッド演算子は大配列でスタックオーバーフローのリスクがあるためループで計算）
  let maxValue = 0;
  for (let i = 0; i < arrayLength; i++) {
    if (array[i] > maxValue) maxValue = array[i];
  }
  if (maxValue === 0) maxValue = 1;

  // LUT を構築（最大値が変わったときのみ再構築）
  buildColorLUT(maxValue);

  // ドットサイズを配列サイズに応じて調整
  const dotRadius = arrayLength <= 64 ? 5 : arrayLength <= 256 ? 3 : arrayLength <= 1024 ? 2 : 1;
  const dotSize = dotRadius * 2;
  const usableHeight = sectionHeight - 20; // 上部 20px をラベル用に確保
  const xStep = width / arrayLength;

  // Set を使って高速な存在チェック
  const compareSet = new Set(compareIndices);
  const swapSet = new Set(swapIndices);
  const readSet = new Set(readIndices);
  const writeSet = new Set(writeIndices);

  if (showCompletionHighlight) {
    // 完了ハイライト: 全ドットを1色で一括描画
    ctx.fillStyle = colors.sorted;
    for (let i = 0; i < arrayLength; i++) {
      const x = (i + 0.5) * xStep - dotRadius;
      const y = mainSectionY + usableHeight - (array[i] / maxValue) * usableHeight - dotRadius;
      ctx.fillRect(x, y, dotSize, dotSize);
    }
  } else {
    // インデックスを色バケツに振り分けてから色ごとに一括描画
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
      const y = mainSectionY + usableHeight - (array[i] / maxValue) * usableHeight - dotRadius;
      ctx.fillStyle = colorLUT[array[i]];
      ctx.fillRect(x, y, dotSize, dotSize);
    }

    // ハイライト色をバッチ描画（1色につき1パス）
    const highlightBuckets = [
      [compareBucket, colors.compare],
      [writeBucket, colors.write],
      [readBucket, colors.read],
      [swapBucket, colors.swap],
    ];

    for (const [indices, color] of highlightBuckets) {
      if (indices.length === 0) continue;
      ctx.fillStyle = color;
      for (const i of indices) {
        const x = (i + 0.5) * xStep - dotRadius;
        const y = mainSectionY + usableHeight - (array[i] / maxValue) * usableHeight - dotRadius;
        ctx.fillRect(x, y, dotSize, dotSize);
      }
    }
  }

  // バッファー配列を描画（ソート完了時は非表示）
  if (showBuffers) {
    const sortedBufferIds = [...arrays.buffers.keys()].sort((a, b) => a - b);

    for (let bufferIndex = 0; bufferIndex < sortedBufferIds.length; bufferIndex++) {
      const bufferId = sortedBufferIds[bufferIndex];
      const bufferArray = arrays.buffers.get(bufferId);
      if (!bufferArray || bufferArray.length === 0) continue;

      const bufferSectionY = bufferIndex * sectionHeight;
      const bufferLength = bufferArray.length;

      let bufferMaxValue = 0;
      for (let i = 0; i < bufferLength; i++) {
        if (bufferArray[i] > bufferMaxValue) bufferMaxValue = bufferArray[i];
      }
      if (bufferMaxValue === 0) bufferMaxValue = 1;

      const bufferUsableHeight = sectionHeight - 20;
      const bufferXStep = width / bufferLength;
      const bufferDotRadius = bufferLength <= 64 ? 5 : bufferLength <= 256 ? 3 : bufferLength <= 1024 ? 2 : 1;
      const bufferDotSize = bufferDotRadius * 2;

      ctx.fillStyle = '#06B6D4';
      for (let i = 0; i < bufferLength; i++) {
        const x = (i + 0.5) * bufferXStep - bufferDotRadius;
        const y = bufferSectionY + bufferUsableHeight - (bufferArray[i] / bufferMaxValue) * bufferUsableHeight - bufferDotRadius;
        ctx.fillRect(x, y, bufferDotSize, bufferDotSize);
      }

      ctx.fillStyle = '#888';
      ctx.font = '12px monospace';
      ctx.fillText(`Buffer #${bufferId}`, 10, bufferSectionY + 20);
    }
  }

  // メイン配列ラベル（バッファー表示時のみ）
  if (showBuffers) {
    ctx.fillStyle = '#888';
    ctx.font = '12px monospace';
    ctx.fillText('Main Array', 10, mainSectionY + 20);
  }
}
