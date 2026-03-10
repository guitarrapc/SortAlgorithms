// OffscreenCanvas 円形描画 Worker（Phase 4 - Circular）
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
  swap: '#EF4444',  // 赤
  write: '#F97316',  // 橙
  read: '#FBBF24',  // 黄
  sorted: '#10B981'   // 緑 - ソート完了
};

// 三角関数ルックアップテーブル（メイン配列）
let lutLength = 0;
let cosLUT = null;
let sinLUT = null;
// 三角関数ルックアップテーブル（バッファー配列）
let bufferLutLength = 0;
let bufferCosLUT = null;
let bufferSinLUT = null;
// HSL カラールックアップテーブル
let colorLUTMax = -1;
let colorLUT = null;

function buildTrigLUT(arrayLength) {
  if (lutLength === arrayLength) return;
  lutLength = arrayLength;
  const angleStep = (2 * Math.PI) / arrayLength;
  cosLUT = new Float64Array(arrayLength);
  sinLUT = new Float64Array(arrayLength);
  for (let i = 0; i < arrayLength; i++) {
    const angle = i * angleStep - Math.PI / 2;
    cosLUT[i] = Math.cos(angle);
    sinLUT[i] = Math.sin(angle);
  }
}

function buildBufferTrigLUT(length) {
  if (bufferLutLength === length) return;
  bufferLutLength = length;
  const angleStep = (2 * Math.PI) / length;
  bufferCosLUT = new Float64Array(length);
  bufferSinLUT = new Float64Array(length);
  for (let i = 0; i < length; i++) {
    const angle = i * angleStep - Math.PI / 2;
    bufferCosLUT[i] = Math.cos(angle);
    bufferSinLUT[i] = Math.sin(angle);
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

function valueToHSL(value, maxValue) {
  const hue = (value / maxValue) * 360;
  return `hsl(${hue}, 70%, 60%)`;
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

  // 背景をクリア（黒）
  ctx.fillStyle = '#1A1A1A';
  ctx.fillRect(0, 0, width, height);

  if (arrayLength === 0) return;

  // 円の中心と半径を計算
  const centerX = width / 2;
  const centerY = height / 2;
  const maxRadius = Math.min(width, height) * 0.45; // 90%の直径を使用（余白考慮）
  const minRadius = maxRadius * 0.2;                // 内側の空白（ドーナツ型）

  let mainMinRadius, mainMaxRadius;
  let ringWidth = 0;

  if (showBuffers) {
    // バッファーがある場合: リング分割
    const totalRings = 1 + bufferCount;
    ringWidth = (maxRadius * 0.8) / totalRings;
    mainMinRadius = minRadius;
    mainMaxRadius = minRadius + ringWidth;
  } else {
    // バッファーがない場合: メイン配列が外側まで広がる
    mainMinRadius = minRadius;
    mainMaxRadius = maxRadius;
  }

  // 最大値を取得（ループで計算、スタックオーバーフロー回避）
  let maxValue = 0;
  for (let i = 0; i < arrayLength; i++) {
    if (array[i] > maxValue) maxValue = array[i];
  }

  // Set で高速な存在チェック
  const compareSet = new Set(compareIndices);
  const swapSet = new Set(swapIndices);
  const readSet = new Set(readIndices);
  const writeSet = new Set(writeIndices);

  // LUT を構築（配列サイズ・最大値が変わったときのみ再構築）
  buildTrigLUT(arrayLength);
  buildColorLUT(maxValue);

  // 線の太さを配列サイズに応じて事前に1回計算
  const lineWidth = arrayLength <= 64 ? 3 : arrayLength <= 256 ? 2 : arrayLength <= 1024 ? 1.5 : 1;

  // メイン配列の線を描画（同色バッチ描画: strokeStyle 変更と stroke 呼び出しを最小化）
  ctx.lineWidth = lineWidth;
  if (showCompletionHighlight) {
    // 完了ハイライト: 全線を1色・1パスで一括描画
    ctx.strokeStyle = colors.sorted;
    ctx.beginPath();
    for (let i = 0; i < arrayLength; i++) {
      const radius = mainMinRadius + (array[i] / maxValue) * (mainMaxRadius - mainMinRadius);
      const ci = cosLUT[i], si = sinLUT[i];
      ctx.moveTo(centerX + ci * mainMinRadius, centerY + si * mainMinRadius);
      ctx.lineTo(centerX + ci * radius, centerY + si * radius);
    }
    ctx.stroke();
  } else {
    // インデックスを色バケツに振り分け
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

    // 通常色（HSLグラデーション）を描画 - 各要素で strokeStyle が異なるため個別描画
    for (const i of normalBucket) {
      const radius = mainMinRadius + (array[i] / maxValue) * (mainMaxRadius - mainMinRadius);
      const ci = cosLUT[i], si = sinLUT[i];
      ctx.strokeStyle = colorLUT[array[i]];
      ctx.beginPath();
      ctx.moveTo(centerX + ci * mainMinRadius, centerY + si * mainMinRadius);
      ctx.lineTo(centerX + ci * radius, centerY + si * radius);
      ctx.stroke();
    }

    // ハイライト色をバッチ描画（1色につき1パス・1回の stroke）
    const highlightBuckets = [
      [compareBucket, colors.compare],
      [writeBucket, colors.write],
      [readBucket, colors.read],
      [swapBucket, colors.swap],
    ];

    for (const [indices, color] of highlightBuckets) {
      if (indices.length === 0) continue;
      ctx.strokeStyle = color;
      ctx.beginPath();
      for (const i of indices) {
        const radius = mainMinRadius + (array[i] / maxValue) * (mainMaxRadius - mainMinRadius);
        const ci = cosLUT[i], si = sinLUT[i];
        ctx.moveTo(centerX + ci * mainMinRadius, centerY + si * mainMinRadius);
        ctx.lineTo(centerX + ci * radius, centerY + si * radius);
      }
      ctx.stroke();
    }
  }

  // バッファー配列を同心円リングとして描画（ソート完了時は非表示）
  if (showBuffers) {
    const sortedBufferIds = [...arrays.buffers.keys()].sort((a, b) => a - b);

    for (let bufferIndex = 0; bufferIndex < sortedBufferIds.length; bufferIndex++) {
      const bufferId = sortedBufferIds[bufferIndex];
      const bufferArray = arrays.buffers.get(bufferId);

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

      buildBufferTrigLUT(bufferLength);
      const bufferLineWidth = bufferLength <= 64 ? 3 : bufferLength <= 256 ? 2 : bufferLength <= 1024 ? 1.5 : 1;

      // バッファー配列の線を描画（単色なので1パス・1回の stroke で一括）
      ctx.strokeStyle = '#06B6D4';
      ctx.lineWidth = bufferLineWidth;
      ctx.beginPath();
      for (let i = 0; i < bufferLength; i++) {
        const radius = bufferMinRadius + (bufferArray[i] / bufferMaxValue) * (bufferMaxRadius - bufferMinRadius);
        const ci = bufferCosLUT[i], si = bufferSinLUT[i];
        ctx.moveTo(centerX + ci * bufferMinRadius, centerY + si * bufferMinRadius);
        ctx.lineTo(centerX + ci * radius, centerY + si * radius);
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

  // 中心円を描画（視覚的なアクセント）
  ctx.fillStyle = '#2A2A2A';
  ctx.beginPath();
  ctx.arc(centerX, centerY, minRadius, 0, 2 * Math.PI);
  ctx.fill();
}
