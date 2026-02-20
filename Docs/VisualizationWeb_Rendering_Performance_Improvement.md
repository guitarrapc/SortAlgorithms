# VisualizationWeb - 描画パフォーマンス改善仕様書

## 1. 目標

| シナリオ | 現在の上限 | 目標 |
|---------|-----------|------|
| Single Sort (BarChart) | 4096要素で軽快 | **16384要素で60 FPS** |
| Comparison Mode (BarChart) | 1024 × 4ソートまで軽快 | **4096 × 4ソートで60 FPS** |

---

## 2. 現在のアーキテクチャと描画パイプライン

```
PlaybackService (C#)
  ↓ StateChanged event
ComparisonGridItem / Index.razor (C#)
  ↓ InvokeAsync(StateHasChanged)
Blazor 差分検出 & 再レンダリング
  ↓ OnAfterRenderAsync
CanvasChartRenderer.razor (C#)
  ↓ JS.InvokeVoidAsync("canvasRenderer.render", …)  ← JS Interop境界
canvasRenderer.js (JavaScript)
  ↓ Canvas 2D fillRect() × N要素
ブラウザ Canvas 合成 & 画面表示
```

Comparison Mode では上記パイプラインが **Canvasの数だけ並列に走る**（4ソート = 4パイプライン）。

---

## 3. ボトルネック分析

### 3.1 JS Interop のシリアライズコスト（最大ボトルネック）

毎フレーム、C# → JS へ以下のデータを JSON シリアライズ＆デシリアライズして転送している：

| データ | 16384要素時のサイズ(概算) |
|--------|-------------------------|
| `MainArray` (int[]) | 64 KB (16384 × 4B) |
| `CompareIndices` (int[]) | 数十バイト |
| `SwapIndices` (int[]) | 数十バイト |
| `ReadIndices` / `WriteIndices` | 数十バイト |
| `BufferArrays` (Dict → Object) | 最大64 KB × バッファ数 |

**問題点：**
- `JS.InvokeVoidAsync` は内部で `System.Text.Json` によるシリアライズを行う
- **16384 int の配列 → JSON 文字列 "[ 1, 2, 3, … ]"** に変換 → JS 側で再パース
- Comparison Mode 4 Canvas: **256 KB+/フレーム** の JSON シリアライズ・デシリアライズ
- Blazor WASM はシングルスレッドのため、シリアライズ中は UI がブロックされる

**計測ポイント：**
```
[CanvasChartRenderer] RenderCanvas start
  ├─ .ToArray() 呼び出し: HashSet<int> → int[] のコピー
  ├─ JS.InvokeVoidAsync: JSON serialize (C#) → transfer → JSON parse (JS)
  └─ 合計: 1フレームあたり 2-8ms (16384要素時)
```

### 3.2 Canvas 2D の個別 fillRect() 呼び出し

`canvasRenderer.js` の `renderInternal` は要素ごとに `fillRect()` を呼ぶ：

```javascript
for (let i = 0; i < arrayLength; i++) {
    ctx.fillStyle = color;        // 色の切り替え(状態変更)
    ctx.fillRect(x, y, w, h);    // 個別描画
}
```

- 16384要素 = **16384回の fillRect + 色切り替え**
- Canvas 2D は GPU バッチングが限定的で、`fillStyle` 変更のたびにバッチが切れる
- 同色バーをまとめて一度に描画するだけでも大幅改善可能

### 3.3 `Math.max(...array)` のスタックオーバーフローリスク

```javascript
const maxValue = Math.max(...array);
```

- スプレッド演算子は全要素を関数引数として展開する
- **16384要素で問題なし** だが、**65536+** でスタックオーバーフローの可能性
- ループで最大値を求める方法に変更すべき

### 3.4 Blazor 再レンダリングサイクルのオーバーヘッド

```
StateChanged → InvokeAsync(StateHasChanged)
  → Blazor diff → OnAfterRenderAsync → JS Interop
```

- 毎フレーム Blazor のコンポーネントツリー差分検出が走る
- `CanvasChartRenderer` 自体の DOM は `<canvas>` 1つで変化しないが、差分検出のコスト自体が発生
- Comparison Mode では 4 コンポーネント × 差分検出

### 3.5 不要な配列アロケーション

```csharp
// CanvasChartRenderer.razor - 毎フレーム呼ばれる
State.CompareIndices.ToArray(),  // HashSet → 新しい int[] を毎回生成
State.SwapIndices.ToArray(),
State.ReadIndices.ToArray(),
State.WriteIndices.ToArray(),
```

- 60 FPS × 4 Canvas = **240回/秒** の不要なアロケーション
- GC 圧力の増加

---

## 4. 改善戦略（優先度順）

### Phase 1: JS 側自律レンダリング（効果: 大 / 工数: 小）

**概要：** 配列データの転送と描画を分離し、JS 側で `requestAnimationFrame` ループを回す。

**現在：**
```
C# StateChanged (60fps) → JS.InvokeVoidAsync("render", array, ...) 毎回
```

**改善後：**
```
C# StateChanged → JS.InvokeVoidAsync("updateData", array, ...)  ← データ更新のみ
JS requestAnimationFrame loop → 前回と同じデータなら描画スキップ ← 描画は JS が自律
```

**実装：**

```javascript
// canvasRenderer.js に追加
window.canvasRenderer = {
    // ... 既存プロパティ ...
    pendingData: new Map(),    // Canvas ID → 最新データ
    rafId: null,               // requestAnimationFrame ID
    isRunning: false,

    // C# から呼ばれる: データ更新のみ（描画しない）
    updateData: function(canvasId, array, compareIndices, swapIndices,
                         readIndices, writeIndices, isSortCompleted,
                         bufferArrays, showCompletionHighlight) {
        this.pendingData.set(canvasId, {
            array, compareIndices, swapIndices, readIndices, writeIndices,
            isSortCompleted, bufferArrays, showCompletionHighlight,
            dirty: true
        });
        if (!this.isRunning) this.startLoop();
    },

    // rAF 駆動の描画ループ
    startLoop: function() {
        this.isRunning = true;
        const loop = () => {
            let anyDirty = false;
            this.pendingData.forEach((data, canvasId) => {
                if (data.dirty) {
                    this.renderInternal(canvasId, data);
                    data.dirty = false;
                    anyDirty = true;
                }
            });
            if (anyDirty || this.pendingData.size > 0) {
                this.rafId = requestAnimationFrame(loop);
            } else {
                this.isRunning = false;
            }
        };
        this.rafId = requestAnimationFrame(loop);
    },

    stopLoop: function() {
        if (this.rafId) cancelAnimationFrame(this.rafId);
        this.isRunning = false;
    }
};
```

**期待効果：**
- C# からの `JS.InvokeVoidAsync` が描画完了を待たなくなる
- ブラウザの VSync に同期した最適タイミングで描画
- データ転送と描画の非同期化

---

### Phase 2: 同色バッチ描画（効果: 大 / 工数: 小）

**概要：** 同じ色のバーをまとめて描画し、`fillStyle` の切り替え回数を最小化する。

**現在：** 要素ごとに色判定 → fillStyle 設定 → fillRect（最大 16384 回の色切り替え）

**改善後：** 先に全要素を色分類 → 色ごとにまとめて fillRect

```javascript
renderInternal: function(canvasId, params) {
    // ... 省略 ...

    // Phase 2: 同色バッチ描画
    // 各色のバーインデックスを分類
    const buckets = {
        normal: [],
        compare: [],
        swap: [],
        read: [],
        write: [],
        sorted: []
    };

    for (let i = 0; i < arrayLength; i++) {
        if (showCompletionHighlight) {
            buckets.sorted.push(i);
        } else if (swapSet.has(i)) {
            buckets.swap.push(i);
        } else if (compareSet.has(i)) {
            buckets.compare.push(i);
        } else if (writeSet.has(i)) {
            buckets.write.push(i);
        } else if (readSet.has(i)) {
            buckets.read.push(i);
        } else {
            buckets.normal.push(i);
        }
    }

    // 色ごとにまとめて描画（fillStyle 切り替えは最大6回）
    const colorMap = {
        normal: this.colors.normal,
        compare: this.colors.compare,
        swap: this.colors.swap,
        read: this.colors.read,
        write: this.colors.write,
        sorted: this.colors.sorted
    };

    for (const [bucket, indices] of Object.entries(buckets)) {
        if (indices.length === 0) continue;
        ctx.fillStyle = colorMap[bucket];
        for (const i of indices) {
            const value = array[i];
            const barHeight = (value / maxValue) * (sectionHeight - 20);
            const x = i * totalBarWidth + (gap / 2);
            const y = mainArrayY + (sectionHeight - barHeight);
            ctx.fillRect(x, y, barWidth, barHeight);
        }
    }
};
```

**期待効果：**
- `fillStyle` 切り替えが 16384回 → **最大6回** に削減
- Canvas 2D 内部の GPU バッチが効率化
- 特にハイライト要素が少ない通常描画時（99%+ が normal 色）で劇的改善

---

### Phase 3: データ転送の最適化（効果: 大 / 工数: 中）

#### 3a. SharedArrayBuffer / Typed Array による転送

**概要：** JSON シリアライズの代わりに、バイナリデータとして直接転送する。

Blazor WASM ではC#の `byte[]` を `Uint8Array` として JS に渡せる（`IJSUnmarshalledRuntime` / .NET 7+の `IJSStreamReference` を利用）。ただし .NET 10 では `[JSImport]`/`[JSExport]` を使った直接バインディングがより効率的。

```csharp
// C# 側: byte[] として配列をパック
// int[] → ArraySegment<byte> (メモリコピー1回)
var byteArray = MemoryMarshal.AsBytes(state.MainArray.AsSpan()).ToArray();
await JS.InvokeVoidAsync("canvasRenderer.updateDataBinary", _canvasId, byteArray, ...);
```

```javascript
// JS 側: Uint8Array → Int32Array にゼロコピー変換
updateDataBinary: function(canvasId, byteArray, ...) {
    const int32View = new Int32Array(byteArray.buffer,
                                     byteArray.byteOffset,
                                     byteArray.byteLength / 4);
    // int32View を直接使用（コピー不要）
    this.pendingData.set(canvasId, { array: int32View, ... });
}
```

**期待効果：**
- JSON シリアライズ/デシリアライズ完全排除
- 16384 int: JSON "65536 bytes text" → Binary **65536 bytes (raw)**
- パース時間: 数 ms → **ほぼ 0 ms**

#### 3b. 差分転送（Delta Updates）

**概要：** 毎フレーム全配列を送る代わりに、変更のあったインデックスと値のみ送る。

```csharp
// C# 側: 変更追跡
// PlaybackService.ApplyOperation で変更インデックスを記録
private List<(int index, int value)> _changedIndices = new();

// Swap 操作後
_changedIndices.Add((operation.Index1, arr[operation.Index1]));
_changedIndices.Add((operation.Index2, arr[operation.Index2]));
```

```javascript
// JS 側: 差分適用
updateDelta: function(canvasId, changes) {
    const data = this.pendingData.get(canvasId);
    for (const [index, value] of changes) {
        data.array[index] = value;
    }
    data.dirty = true;
}
```

**期待効果：**
- 1フレーム1操作時: 16384 int 転送 → **2-4 int 転送** (99.97%削減)
- OperationsPerFrame=100 でも: 16384 int → **200-400 int**

#### 3c. JS 側に配列コピーを保持

**概要：** 初回に全配列を JS へ転送し、以降は操作コマンド (swap/write) だけを送る。

```javascript
// 初回: 全配列を JS にコピー
setInitialArray: function(canvasId, array) {
    this.arrays.set(canvasId, new Int32Array(array));
}

// 以降: 操作だけ送信
applyOperations: function(canvasId, ops) {
    const arr = this.arrays.get(canvasId);
    for (const op of ops) {
        switch (op.type) {
            case 'swap':
                [arr[op.i], arr[op.j]] = [arr[op.j], arr[op.i]];
                break;
            case 'write':
                arr[op.i] = op.value;
                break;
            case 'rangeCopy':
                arr.set(op.values, op.destIndex);
                break;
        }
    }
    this.pendingData.get(canvasId).dirty = true;
}
```

**期待効果：**
- 配列全体の転送がロード時の1回のみ
- 毎フレーム転送データ量: **数十バイト** (操作コマンドのみ)
- Comparison Mode 4096 × 4: フレームあたり転送 **数百バイト** (現在 64KB × 4)

---

### Phase 4: OffscreenCanvas + Web Worker（効果: 大 / 工数: 大）

**概要：** 描画処理をメインスレッドから Web Worker に移動する。

```
メインスレッド:                    Worker スレッド:
  C# PlaybackService               OffscreenCanvas 描画
  ↓ postMessage(操作データ)   →     rAF ループ
  UIフリーズなし                    fillRect() × N
```

**制約：**
- `OffscreenCanvas` は Chrome 69+, Firefox 105+, Safari 16.4+ でサポート
- Blazor WASM から Worker への通信は `postMessage` + `Transferable` で行う
- SharedArrayBuffer を使えばゼロコピーデータ共有も可能
  - ただし `Cross-Origin-Opener-Policy: same-origin` と `Cross-Origin-Embedder-Policy: require-corp` ヘッダーが必要

**実装概要：**

```javascript
// worker.js
self.onmessage = function(e) {
    const { type, canvasId } = e.data;
    if (type === 'init') {
        const canvas = e.data.canvas; // OffscreenCanvas
        const ctx = canvas.getContext('2d', { alpha: false });
        // Worker 内で描画ループ
    }
    if (type === 'update') {
        // 配列データ更新 + dirty フラグ
    }
};

// メインスレッド
const offscreen = canvasElement.transferControlToOffscreen();
worker.postMessage({ type: 'init', canvas: offscreen }, [offscreen]);
```

**期待効果：**
- 描画がメインスレッドを完全にブロックしない
- C# の PlaybackService 処理と描画が並列実行
- Comparison Mode で特に効果的（4 Worker = 4 並列描画）

---

### Phase 5: ImageData ピクセルバッファ直接書き込み（効果: 中 / 工数: 中）

**概要：** `fillRect()` の代わりに `ImageData` のピクセルバッファに直接書き込む。

```javascript
renderViaImageData: function(canvasId, params) {
    const { canvas, ctx } = this.instances.get(canvasId);
    const width = canvas.width;
    const height = canvas.height;

    // ピクセルバッファを取得（または再利用）
    const imageData = ctx.createImageData(width, height);
    const pixels = imageData.data; // Uint8ClampedArray

    // 背景色で塗りつぶし
    for (let i = 0; i < pixels.length; i += 4) {
        pixels[i] = 26; pixels[i+1] = 26; pixels[i+2] = 26; pixels[i+3] = 255;
    }

    // バーを直接ピクセル書き込み
    for (let i = 0; i < arrayLength; i++) {
        const barHeight = ...;
        const x = Math.floor(i * totalBarWidth);
        const barW = Math.max(1, Math.floor(barWidth));
        const yStart = Math.floor(mainArrayY + sectionHeight - barHeight);
        const yEnd = Math.floor(mainArrayY + sectionHeight);

        const [r, g, b] = this.getColorRGB(i, ...);

        for (let py = yStart; py < yEnd; py++) {
            for (let px = x; px < x + barW && px < width; px++) {
                const offset = (py * width + px) * 4;
                pixels[offset] = r;
                pixels[offset + 1] = g;
                pixels[offset + 2] = b;
                pixels[offset + 3] = 255;
            }
        }
    }

    ctx.putImageData(imageData, 0, 0);
}
```

**期待効果：**
- Canvas API 呼び出しオーバーヘッド排除（`fillRect` × N → `putImageData` × 1）
- 16384バーの場合、多くが1-2ピクセル幅なので書き込み量は少ない
- ただし高 DPI ディスプレイではピクセル数が増えるため注意

**注意：**
- 高 DPI (2x) で 1920×1080 Canvas = 3840×2160 = 8.3M pixels × 4 = 33MB/フレーム
- `ImageData` 再利用（`createImageData` を毎フレーム呼ばない）で軽減
- 大画面・高 DPI では Phase 4 (Worker) との組み合わせが効果的

---

### Phase 6: WebGL レンダラー（効果: 最大 / 工数: 大）

**概要：** Canvas 2D を WebGL に置き換え、GPU で直接描画する。

```
C# → JS (操作データのみ) → WebGL Vertex Buffer → GPU 描画
```

**実装アプローチ：**

```javascript
// バーを四角形（2つの三角形）としてインスタンス描画
// 各バーの属性: position(x), height, color

// 頂点シェーダー
const vsSource = `
    attribute vec2 a_position;    // バーの四隅
    attribute float a_barIndex;   // バーのインデックス
    attribute float a_barHeight;  // バーの高さ（正規化）
    attribute vec3 a_color;       // バーの色

    uniform vec2 u_resolution;

    void main() {
        // バーの位置とサイズを計算
        float barWidth = 2.0 / float(u_arrayLength);
        float x = -1.0 + a_barIndex * barWidth + a_position.x * barWidth;
        float y = -1.0 + a_position.y * a_barHeight * 2.0;
        gl_Position = vec4(x, y, 0.0, 1.0);
    }
`;
```

**期待効果：**
- 16384バー → **1回の drawArraysInstanced 呼び出し** で描画
- GPU の並列処理で描画時間が **0.1ms 以下**
- Comparison Mode 4096 × 4 = 16384バー でも余裕
- 65536+ 要素にもスケール可能

**注意：**
- WebGL のセットアップコードが複雑
- WebGL コンテキスト数の制限（ブラウザごとに8-16程度）
  - Comparison Mode 9 Canvas で制限に達する可能性
  - 対策: 単一 Canvas に複数ビューポートで描画

---

## 5. C# 側の改善

### 5a. HashSet.ToArray() の排除

```csharp
// 現在: 毎フレーム新しい配列を生成
State.CompareIndices.ToArray()  // GC 圧力

// 改善: 再利用可能なバッファを使用
private int[] _compareBuffer = new int[64];

private int[] GetIndicesArray(HashSet<int> set) {
    if (set.Count > _compareBuffer.Length)
        _compareBuffer = new int[set.Count * 2];
    set.CopyTo(_compareBuffer);
    return _compareBuffer; // 注: Length ≠ Count なので Count も渡す
}
```

または Phase 3c（JS側にコピー保持）を採用すれば、ハイライトインデックスのみの転送でよい。

### 5b. ShouldRender() オーバーライド

```csharp
// CanvasChartRenderer.razor
protected override bool ShouldRender()
{
    // Canvas は JS 側で描画するため、Blazor の DOM 差分は不要
    // ただし、State の有無が変わった場合（canvas 要素の表示/非表示）のみ再レンダリング
    var shouldRender = _previousHasData != (State?.MainArray.Length > 0);
    _previousHasData = State?.MainArray.Length > 0;
    return shouldRender || !_isInitialized;
}
```

これにより **Blazor の差分検出コスト自体を排除** できる。JS 側の描画は `OnAfterRenderAsync` に依存せず、Phase 1 の自律レンダリングで行う。

### 5c. JS Interop 呼び出し頻度制御

```csharp
// 描画の間引き: 前回の JS Interop から一定時間経過していなければスキップ
private DateTime _lastJsCall = DateTime.MinValue;
private const double MIN_JS_CALL_INTERVAL_MS = 16.0; // 60 FPS 上限

private async Task RenderCanvas()
{
    var now = DateTime.UtcNow;
    if ((now - _lastJsCall).TotalMilliseconds < MIN_JS_CALL_INTERVAL_MS)
        return; // スキップ
    _lastJsCall = now;

    await JS.InvokeVoidAsync("canvasRenderer.updateData", ...);
}
```

---

## 6. 実装ロードマップ

### Phase 1 + 2: 即時効果（推定工数: 1-2日）

| 改善 | 変更ファイル | 期待効果 |
|------|-------------|---------|
| JS 自律 rAF ループ | `canvasRenderer.js`, `circularCanvasRenderer.js` | 描画タイミング最適化 |
| 同色バッチ描画 | `canvasRenderer.js` | fillStyle 切り替え 16384→6回 |
| `Math.max(...array)` 修正 | `canvasRenderer.js`, `circularCanvasRenderer.js` | スタックオーバーフロー防止 |
| `ShouldRender()` 追加 | `CanvasChartRenderer.razor`, `CircularRenderer.razor` | Blazor 差分排除 |

**期待される改善：**
- Single Sort 16384 要素: 30-40 FPS → **55-60 FPS**
- Comparison Mode 2048 × 4: カクつき → **スムーズ**

### Phase 3: データ転送最適化（推定工数: 2-3日）

| 改善 | 変更ファイル | 期待効果 |
|------|-------------|---------|
| JS 側配列保持 + 操作コマンド転送 | `canvasRenderer.js`, `CanvasChartRenderer.razor`, `PlaybackService.cs` | 転送量 99% 削減 |
| HashSet.ToArray() 排除 | `CanvasChartRenderer.razor`, `CircularRenderer.razor` | GC 圧力削減 |
| JS Interop 頻度制御 | `CanvasChartRenderer.razor` | 不要な Interop 排除 |

**期待される改善：**
- Comparison Mode 4096 × 4: **55-60 FPS**
- JS Interop あたりの転送量: 64KB → **数百バイト**

### Phase 4: Worker 並列化（推定工数: 3-5日）

| 改善 | 変更ファイル | 期待効果 |
|------|-------------|---------|
| OffscreenCanvas + Worker | 新規: `renderWorker.js`, 変更: `canvasRenderer.js` | メインスレッド解放 |
| SharedArrayBuffer | CORS ヘッダー設定、Worker コード | ゼロコピーデータ共有 |

**期待される改善：**
- Single Sort 16384 要素: 描画がメインスレッドに **影響ゼロ**
- Comparison Mode 4096 × 4: 各 Worker が独立描画

### Phase 5-6: 将来的な拡張（推定工数: 5-10日）

| 改善 | 条件 |
|------|------|
| ImageData ピクセル直書き | Phase 4 との組み合わせで効果的 |
| WebGL レンダラー | 65536+ 要素対応、最高性能が必要な場合 |

---

## 7. 各 Phase の効果予測サマリ

```
                    Single 16384       Comparison 4096×4
現在:               ~30 FPS (重い)     ~15 FPS (カクつき)
Phase 1+2:          ~55 FPS            ~35 FPS
Phase 3:            ~60 FPS            ~55 FPS
Phase 4:            60 FPS (余裕)      60 FPS (余裕)
Phase 5+6:          60 FPS (65536+可)  60 FPS (8192×4可)
```

---

## 8. 検証方法

### FPS 計測

既存のデバッグインフラを活用：

```javascript
// canvasRenderer.js 内の FPS ログ（既存）
window.debugHelper.log(`[JS Canvas] ${canvasId} JS render() FPS: ${fps.toFixed(1)}`);
```

### テストシナリオ

| # | シナリオ | 要素数 | Canvas数 | 合格基準 |
|---|---------|--------|----------|---------|
| 1 | Single BarChart | 16384 | 1 | 55+ FPS |
| 2 | Single Circular | 16384 | 1 | 55+ FPS |
| 3 | Comparison BarChart | 4096 | 4 | 50+ FPS |
| 4 | Comparison BarChart | 2048 | 9 | 45+ FPS |
| 5 | Single BarChart + Seek | 16384 | 1 | シーク応答 < 100ms |

### Chrome DevTools による計測

1. **Performance タブ**: フレーム時間、Long Task の検出
2. **Memory タブ**: GC 頻度の監視
3. **Console**: 既存の FPS ログで確認

---

## 9. リスクと制約

| リスク | 影響 | 対策 |
|--------|------|------|
| OffscreenCanvas 非対応ブラウザ | Phase 4 使用不可 | Feature detection + fallback |
| SharedArrayBuffer の CORS 要件 | ホスティング設定変更必要 | GitHub Pages は対応可能 |
| WebGL コンテキスト数制限 | Comparison 9 Canvas で問題 | 単一 Canvas 複数ビューポート |
| Blazor WASM シングルスレッド | C# 側処理がブロック | Phase 4 で描画を Worker に移動 |
| `[JSImport]`/`[JSExport]` の成熟度 | .NET 10 での安定性 | `IJSRuntime` fallback |

---

## 10. 結論

**最もコスパの高い改善は Phase 1-3**（推定工数 3-5 日）で、目標の **16384 Single Sort 60 FPS** と **4096 × 4 Comparison 55+ FPS** を達成できる見込み。

Phase 4 以降は、さらなるスケーラビリティ（65536+ 要素、8192 × 9 Comparison）が必要になった場合に検討する。

**推奨実装順序：**
1. Phase 2 (同色バッチ描画) — 既存コードの小変更で即座に効果
2. Phase 1 (JS 自律レンダリング) — Blazor-JS 間の非同期化
3. Phase 3c (JS 側配列保持) — 転送量の劇的削減
4. Phase 5b (ShouldRender) — Blazor 差分コスト排除
5. 計測してボトルネックが残る場合に Phase 4 以降を検討
