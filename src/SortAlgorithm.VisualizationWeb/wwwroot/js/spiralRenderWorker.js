// OffscreenCanvas 螺旋描画 Worker（Phase 4 - Spiral）
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

// 螺旋パラメータ
const SPIRAL_TURNS = 3;
const SPIRAL_MIN_RADIUS_RATIO = 0.08;
const SPIRAL_MAX_RADIUS_RATIO = 0.46;

// 螺旋座標 LUT（配列長・キャンバスサイズが変わったときのみ再構築）
let posLUTLength = 0;
let posLUTCanvasW = 0;
let posLUTCanvasH = 0;
let posLUTX = null; // Float32Array
let posLUTY = null; // Float32Array

// HSL カラールックアップテーブル
let colorLUTMax = -1;
let colorLUT = null;

function buildPosLUT(n, width, height) {
  if (posLUTLength === n && posLUTCanvasW === width && posLUTCanvasH === height) return;
  posLUTLength = n;
  posLUTCanvasW = width;
  posLUTCanvasH = height;
  posLUTX = new Float32Array(n);
  posLUTY = new Float32Array(n);
  const cx = width / 2;
  const cy = height / 2;
  const minDim = Math.min(width, height);
  const minR = minDim * SPIRAL_MIN_RADIUS_RATIO;
  const maxR = minDim * SPIRAL_MAX_RADIUS_RATIO;
  const twoPI = 2 * Math.PI;
  for (let i = 0; i < n; i++) {
    const t = n <= 1 ? 0 : i / (n - 1);
    const theta = t * SPIRAL_TURNS * twoPI - Math.PI / 2;
    const r = minR + (maxR - minR) * t;
    posLUTX[i] = cx + r * Math.cos(theta);
    posLUTY[i] = cy + r * Math.sin(theta);
  }
}

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
        // キャンバスサイズ変更で座標 LUT を無効化（次回 draw で再構築）
        posLUTLength = 0;
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

  ctx.fillStyle = '#1A1A1A';
  ctx.fillRect(0, 0, width, height);

  if (arrayLength === 0) return;

  // 最大値を取得（ループで計算、スタックオーバーフロー回避）
  let maxValue = 0;
  for (let i = 0; i < arrayLength; i++) {
    if (array[i] > maxValue) maxValue = array[i];
  }
  if (maxValue === 0) maxValue = 1;

  // LUT を構築（変更時のみ再構築）
  buildColorLUT(maxValue);
  buildPosLUT(arrayLength, width, height);

  // ドットサイズを配列サイズに応じて調整
  const dotRadius = arrayLength <= 64 ? 5 : arrayLength <= 256 ? 3 : arrayLength <= 1024 ? 2 : 1;
  const dotSize = dotRadius * 2;

  // Set を使って高速な存在チェック
  const compareSet = new Set(compareIndices);
  const swapSet = new Set(swapIndices);
  const readSet = new Set(readIndices);
  const writeSet = new Set(writeIndices);

  if (showCompletionHighlight) {
    // 完了ハイライト: 全ドットを1色で一括描画
    ctx.fillStyle = colors.sorted;
    for (let i = 0; i < arrayLength; i++) {
      ctx.fillRect(posLUTX[i] - dotRadius, posLUTY[i] - dotRadius, dotSize, dotSize);
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
      ctx.fillStyle = colorLUT[array[i]];
      ctx.fillRect(posLUTX[i] - dotRadius, posLUTY[i] - dotRadius, dotSize, dotSize);
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
        ctx.fillRect(posLUTX[i] - dotRadius, posLUTY[i] - dotRadius, dotSize, dotSize);
      }
    }
  }

  // バッファー配列をボトムバンドに DotPlot スタイルで描画（ソート完了時は非表示）
  const bufferCount = (isSortCompleted || showCompletionHighlight) ? 0 : arrays.buffers.size;
  if (bufferCount > 0) {
    const sortedBufferIds = [...arrays.buffers.keys()].sort((a, b) => a - b);
    const bufferBandH = Math.min(height * 0.18 / bufferCount, 80);

    for (let bufferIndex = 0; bufferIndex < sortedBufferIds.length; bufferIndex++) {
      const bufferId = sortedBufferIds[bufferIndex];
      const bufferArray = arrays.buffers.get(bufferId);
      if (!bufferArray || bufferArray.length === 0) continue;

      const bufferLength = bufferArray.length;
      let bufferMaxValue = 0;
      for (let i = 0; i < bufferLength; i++) {
        if (bufferArray[i] > bufferMaxValue) bufferMaxValue = bufferArray[i];
      }
      if (bufferMaxValue === 0) bufferMaxValue = 1;

      const bufferSectionY = height - (bufferCount - bufferIndex) * bufferBandH;
      const bufferUsableH = bufferBandH - 16;
      const xStep = width / bufferLength;
      const bufferDotRadius = bufferLength <= 64 ? 3 : bufferLength <= 256 ? 2 : 1;
      const bufferDotSize = bufferDotRadius * 2;

      ctx.fillStyle = '#06B6D4';
      for (let i = 0; i < bufferLength; i++) {
        const x = (i + 0.5) * xStep - bufferDotRadius;
        const y = bufferSectionY + bufferUsableH - (bufferArray[i] / bufferMaxValue) * bufferUsableH - bufferDotRadius;
        ctx.fillRect(x, y, bufferDotSize, bufferDotSize);
      }

      ctx.fillStyle = '#888';
      ctx.font = '12px monospace';
      ctx.fillText(`Buffer #${bufferId}`, 10, bufferSectionY + 14);
    }
  }
}
