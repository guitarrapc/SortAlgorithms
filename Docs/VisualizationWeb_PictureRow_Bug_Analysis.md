# PictureRow ComparisonMode バグ分析

## 報告された症状

| 項目 | 内容 |
|------|------|
| モード | ComparisonMode / Picture Row |
| 条件 | Array Size: **2048**、インスタンス数: **6** |
| 症状1 | すべてのキャンバスで、**画像の上半分だけ表示され下半分が真っ暗** |
| 症状2 | **動作のカクツキ**（フレームドロップ） |
| 正常ケース | Array Size: 1024、インスタンス数: 6 → 問題なし |
| レンダラー | WebGL / CPU Worker どちらでも発生 |

---

## バグA：下半分ブラック問題

### 根本原因

`pictureRowRenderWorker.js` の `setImage` ハンドラーと `draw()` の間に**非同期レース条件**がある。

#### 問題コード（`pictureRowRenderWorker.js` 183〜200行目）

```javascript
case 'setImage': {
  const blobOrBuffer = ...;
  if (!blobOrBuffer) break;

  // ❌ createImageBitmap は非同期。完了するまで imageBitmap / imageNumRows は古い値のまま。
  createImageBitmap(blobOrBuffer).then(function (bmp) {
    imageBitmap = bmp;         // ← 非同期完了後にだけ更新される
    imageNumRows = msg.numRows; // ← 非同期完了後にだけ更新される
    if (arrays.main && renderParams) scheduleDraw();
  }).catch(function (err) {
    imageBitmap = null;
    imageNumRows = 0;
    ...
  });
  break;
}
```

#### 問題コード（`draw()` 内 105行目・117行目）

```javascript
// imageNumRows が古い値のままの状態で setArray が届くと、
// rowIdx が imageNumRows 以上の行はすべてスキップされる
if (rowIdx < 0 || rowIdx >= imageNumRows) continue; // ← 下半分がここでスキップされる
```

### バグ発生シーケンス

```
【前提】ユーザーが n=1024 で一度ソートを実行済み
        → Worker 内: imageBitmap=旧bitmap, imageNumRows=1024

【n=2048 に変更してソート実行】

Main Thread (Blazor)           Worker Thread
─────────────────────          ─────────────────────────
setImage(numRows=2048) ──→    createImageBitmap() 開始 (非同期、100〜500ms)
                               imageNumRows はまだ 1024 のまま
setArray(2048要素) ────→      draw() 即座に実行
                               ┌─ imageBitmap != null (古いbitmapが存在)
                               ├─ imageNumRows = 1024 (古い値)
                               ├─ n = 2048
                               │
                               │ ループ i=0..2047:
                               │   rowIdx = array[i] - minVal (0..2047)
                               │   rowIdx >= 1024 → continue ← 下半分スキップ!
                               │
                               └─ 結果: dst y=0..半分のみ描画、残りは #1A1A1A(黒)

applyFrame 毎フレーム ──→     draw() 毎フレーム同じ状態
                               ↑ createImageBitmap が完了するまで継続

(200〜1000ms後)
createImageBitmap 完了        imageBitmap=新bitmap, imageNumRows=2048
                               scheduleDraw() → draw() → ✅ 正常
```

### n=1024 で問題が出ない理由

| ケース | imageNumRows (古い値) | n (新しい値) | rowIdx 範囲 | スキップ |
|--------|----------------------|-------------|------------|---------|
| 1024 → **2048** | 1024 | 2048 | 0..2047 | 1024..2047 がスキップ → **下半分ブラック** |
| (初回) → 1024 | 0 または null | 1024 | 0..1023 | `imageBitmap=null` → バーモード fallback → **問題なし** |
| 1024 → **1024** | 1024 | 1024 | 0..1023 | 全行 < 1024 → **問題なし** |
| 2048 → **1024** | 2048 | 1024 | 0..1023 | 全行 < 2048 → スキップなし (ただし srcRowH が古い値になる別バグあり) |

### n=2048 で 6 インスタンスすべてに同時発生する理由

- 6 つのすべての Worker が同じ状態（前回 n=1024）から同じシーケンスを踏む
- `createImageBitmap` が 6 Worker で同時実行 → 各自が独立してデコードするため時間がかかる
- 大きな画像（例: 5MB JPEG）では 1 Worker 200〜500ms × 6 同時 = リソース競合で最大 1〜2 秒
- この間、毎フレームの `applyFrame` → `draw()` がすべて stale な `imageNumRows` で実行される

### Canvas 2D fallback（WebGL OFF 時）に同じバグがない理由

`pictureRowCanvasRenderer.js` の Canvas 2D fallback では `img.complete` で判定する：

```javascript
// 画像ロード完了前は img = null → 条件 false → バーモードへ fallback
this._images.set(canvasId, { img: null, numRows });  // img: null で即座に登録

img.onload = function () {
    self._images.set(canvasId, { img, numRows }); // 完了後に img が入る
};
```

```javascript
// renderInternal 内
if (img && numRows > 0 && img.complete) { // img が null の間は false → fallback
```

Canvas 2D fallback は「img が null の間はバーモード」という自然な安全機構がある。  
**Worker パスにはこの安全機構がない**。

---

## バグB：カクツキ問題

### 根本原因（複数）

#### 原因1：`ExecuteAndRecord` がメインスレッドをブロック

`ComparisonModeService.AddAlgorithm()` 内で `ExecuteAndRecord()` が**同期的**に実行される。

```csharp
// ComparisonModeService.cs
var (operations, statistics, actualExecutionTime) =
    _executor.ExecuteAndRecord(_state.InitialArray, metadata); // ← ブロッキング
```

Blazor WebAssembly は .NET ランタイムが **JS メインスレッド** 上で動作するため、  
`ExecuteAndRecord` がブロックすると **JavaScript イベントループ全体がフリーズ** する。

| Array Size | O(n²) 操作数 | 6アルゴリズム合計 | 推定ブロック時間 |
|-----------|------------|---------------|--------------|
| 1024 | ~1M | ~6M | 0.5〜2 秒 |
| 2048 | ~4M | ~24M | 2〜8 秒 |

#### 原因2：6インスタンス分の JS Interop オーバーヘッド

ComparisonMode では 6 つの `PlaybackService` がそれぞれ独立した RAF ループを持つ。  
毎フレーム、6 インスタンス分の JS Interop が発火する。

```
1フレーム (16ms) あたりの処理:
  ×6 PlaybackService.OnRafTick()
    ×6 JS.InvokeVoidAsync("applyFrame", ...)  ← 各呼び出しが C#→JSON→JS 変換を含む
```

n=2048 では `setArray` の初回送信で 2048 × 4 byte = **8KB の配列** を  
**6 インスタンス分 (48KB)** 全量 JSON シリアライズして JS に渡す。

#### 原因3：Worker への大量メッセージ蓄積

Blazor が `applyFrame` メッセージを Worker よりも速く送信すると、  
Worker のメッセージキューに蓄積され、処理が追いついたときに複数フレームが  
一気に描画され **ガタつき** として現れる。

#### 原因4：メモリ圧力と GC

n=2048 × 6 アルゴリズムのオペレーション記録はヒープを大量消費する。  
Blazor WASM のヒープが圧迫されると **GC Pause** が定期的に発生してフレームドロップする。

| Array Size | 操作記録の推定サイズ (6アルゴリズム) |
|-----------|--------------------------------|
| 1024 | ~30〜100 MB |
| 2048 | ~120〜400 MB |

---

## 影響コード箇所一覧

| ファイル | 行番号 | 内容 |
|---------|-------|------|
| `wwwroot/js/pictureRowRenderWorker.js` | 183〜199 | `setImage` ハンドラー（レース条件の発生源） |
| `wwwroot/js/pictureRowRenderWorker.js` | 105, 117 | `draw()` 内 `rowIdx >= imageNumRows` スキップ条件 |
| `wwwroot/js/pictureRowRenderWorker.js` | 89 | `imageBitmap && imageNumRows > 0` チェック |
| `Services/ComparisonModeService.cs` | `AddAlgorithm()` | `ExecuteAndRecord` 同期ブロック |
| `Components/PictureRowRenderer.razor` | `RenderCanvas()` | setImage → setArray 呼び出し順序 |

---

## 修正方針

### Fix A：レース条件の解消（バグA 対応）

**方針**: `setImage` 受信時に**即座に `imageBitmap = null` にリセット**する。  
`createImageBitmap` が完了するまで `draw()` はバーモード fallback になる。  
（半黒表示よりバー表示のほうが UX として許容される）

**修正対象**: `wwwroot/js/pictureRowRenderWorker.js`

```javascript
// ---- 変更前 ----
case 'setImage': {
  const blobOrBuffer = msg.imageBlob ? msg.imageBlob : ...;
  if (!blobOrBuffer) break;
  createImageBitmap(blobOrBuffer).then(function (bmp) {
    imageBitmap = bmp;
    imageNumRows = msg.numRows;  // ← 非同期完了後にだけ更新（レース条件）
    if (arrays.main && renderParams) scheduleDraw();
  }).catch(...);
  break;
}

// ---- 変更後 ----
case 'setImage': {
  const blobOrBuffer = msg.imageBlob ? msg.imageBlob : ...;
  if (!blobOrBuffer) break;

  // 受信直後に古い bitmap を無効化し、draw() をバーモード fallback にする
  imageBitmap = null;
  imageNumRows = 0;
  if (arrays.main && renderParams) scheduleDraw(); // バーモードでの即時再描画

  const requestId = ++pendingImageRequestId; // リクエスト追跡（後述）
  createImageBitmap(blobOrBuffer).then(function (bmp) {
    if (requestId !== pendingImageRequestId) return; // 古いリクエストは無視
    imageBitmap = bmp;
    imageNumRows = msg.numRows;
    if (arrays.main && renderParams) scheduleDraw();
  }).catch(function (err) {
    if (requestId !== pendingImageRequestId) return;
    imageBitmap = null;
    imageNumRows = 0;
    if (arrays.main && renderParams) scheduleDraw();
  });
  break;
}
```

**追加変数**: ファイル先頭に `let pendingImageRequestId = 0;` を追加。  
`dispose` ハンドラーにも `pendingImageRequestId = 0;` を追加。

**効果**:
- `setImage` 受信 → 即座に `imageBitmap = null` → `draw()` はバーモードへ fallback
- `createImageBitmap` 完了後 → 正しい `imageBitmap` と `imageNumRows` がセットされ画像表示
- 複数回 `setImage` が連続送信された場合も最新のリクエストだけが反映される

---

### Fix B：カクツキ軽減（バグB 対応）

#### B-1: `setArray` の転送最適化（短期）

JS Interop で `int[]` を JSON 経由ではなく `DotNetStreamReference` や  
`Uint8Array` で転送し、デシリアライズコストを削減する。  
（既存の JSInterop バインディングの変更が必要）

現時点では **`OperationsPerFrame` のデフォルト値を比較モードでは小さくする**  
ことで体感を改善できる。

#### B-2: ComparisonMode の画像共有最適化（短期）

現状、6 Worker が各自独立して `createImageBitmap` を呼んでいる。  
同一画像 Blob に対して `createImageBitmap` を **Main Thread で 1 回だけ** 呼び、  
生成した `ImageBitmap` を全 Worker に `postMessage` で転送（Transferable）すれば  
デコード時間が 1/6 になる。

```javascript
// Main thread: pictureRowCanvasRenderer.js setImage 内
// 1回だけ createImageBitmap を呼ぶ
createImageBitmap(cached.blob).then(bitmap => {
  // 全 Worker に同一 ImageBitmap を送信（transfer ではなく structuredClone）
  for (const [cid, workerInfo] of this.workers) {
    if (this.pendingSetImageCanvases.has(cid)) {
      workerInfo.worker.postMessage(
        { type: 'setImageBitmap', bitmap, numRows },
        [bitmap] // Transferable: 最初の Worker だけ転送可能
      );
    }
  }
});
```

> **注意**: `ImageBitmap` は Transferable だが **1 つの所有者にしか転送できない**。  
> 6 Worker に同一ビットマップを送るには `structuredClone` または各 Worker 用に個別生成が必要。  
> 代替案: Main Thread の OffscreenCanvas で画像を 1 度描画して各 Worker に行データを送る。

#### B-3: `ExecuteAndRecord` の非同期化（中長期）

`ComparisonModeService.AddAlgorithm()` 内の `ExecuteAndRecord` を  
`Task.Yield()` を挟みながら分割実行することで UI スレッドのブロックを解消する。

```csharp
// 現状（ブロッキング）
var (operations, statistics, actualExecutionTime) =
    _executor.ExecuteAndRecord(_state.InitialArray, metadata);

// 改善案: バックグラウンドで実行（Blazor WASM は単一スレッドだが yield で UI を解放）
await Task.Run(async () => {
    // 1000 操作ごとに yield して UI スレッドを開放
    ...
});
```

> **注意**: Blazor WebAssembly はメインスレッドのみのため `Task.Run` は  
> 並列実行ではないが、`await Task.Yield()` での yield は可能。

#### B-4: 比較モードの `applyFrame` バッチ化（中長期）

現状の 6 インスタンス独立 RAF ループを、**1 つの RAF ループで 6 インスタンスを  
まとめて処理**するアーキテクチャに変更することで JS Interop 呼び出し回数を削減できる。

---

## 修正優先順位

| 優先度 | Fix | 難易度 | 効果 |
|--------|-----|--------|------|
| **P0 (必須)** | Fix A: `setImage` レース条件解消 | 低 | バグA(下半分ブラック)が解消 |
| **P1 (推奨)** | Fix B-2: 画像デコードの共有化 | 中 | Worker 起動時のカクツキ軽減 |
| **P2 (任意)** | Fix B-3: `ExecuteAndRecord` 非同期化 | 高 | アルゴリズム追加時のフリーズ解消 |
| **P3 (任意)** | Fix B-4: `applyFrame` バッチ化 | 高 | 再生中の JS Interop 負荷削減 |

---

## 修正後の期待される動作

### Array Size 2048 × 6インスタンス

- ソート開始直後: **バーモード**で表示（画像デコード中）
- `createImageBitmap` 完了後 (~200ms): **画像モード**に切り替わり全行が正常表示
- ソートアニメーション中: **全行が正常に更新される**（下半分ブラックなし）

### 注意事項

- Fix A 適用後は、ソート開始から 100〜500ms の間はバーが表示される
- これは「画像が消えた」ではなく正常な Loading 挙動
- 必要なら Loading インジケーターを追加することも検討できる

---

## 関連ファイル

| ファイル | 役割 |
|---------|------|
| `wwwroot/js/pictureRowRenderWorker.js` | OffscreenCanvas Worker（バグAの発生箇所） |
| `wwwroot/js/pictureRowCanvasRenderer.js` | Main Thread / Canvas 2D fallback（バグAなし） |
| `Components/PictureRowRenderer.razor` | Blazor コンポーネント（setImage/setArray 呼び出し） |
| `Services/ComparisonModeService.cs` | 比較モード管理（バグBの発生箇所） |
| `Services/PlaybackService.cs` | 再生制御・RAF ループ |
