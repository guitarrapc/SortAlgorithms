# VisualizationWeb ローディング状態 UI 仕様

## 1. 概要と背景

### 問題

Blazor WebAssembly はブラウザのメインスレッド（UI スレッド）上で動作するシングルスレッド環境である。
C# の処理がそのスレッドを占有している間、ブラウザは画面の再描画もユーザー入力の処理も行えない。
その結果、以下の操作中は **画面が固まったように見え**、ユーザーがボタンを連打したり、
意図しない操作が起きる可能性がある。

### UIがフリーズするタイミング

| タイミング | 発生箇所 | 規模感 |
|-----------|---------|-------|
| ソート処理の実行・記録 | `GenerateAndSort()` → `SortExecutor.ExecuteAndRecord()` (同期版) | n=1024 のバブルソートで数秒 |
| 比較アルゴリズムの追加 | `AddAlgorithmToComparison()` → `ExecuteAndRecordAsync()` | `Task.Yield()` あり、応答性は改善済みだがフィードバックなし |
| 設定変更による全アルゴリズム再実行 | 比較モード内 `needRegeneration` フロー | アルゴリズム数 × 単体時間 |
| 画像ファイルのアップロード・デコード | `HandleFileUpload()` + JSInterop | 大きな画像で数百ms |
| 画像のレンダラー設定（WebGL↔Canvas2D 切替） | `OnRenderSettingsChanged()` + JSInterop | 数十〜数百ms |

### 実現可能性

**すべての要件は実現可能**。実装パターンは以下の通り。

| 要件 | Blazor での実現方法 |
|------|-------------------|
| ローディング表示 | `@if (_isLoading)` で条件レンダリング、CSS アニメーション |
| ボタンのグレーアウト | HTML `disabled` 属性 (`disabled="@_isLoading"`) |
| コントロール全体の無効化 | `<fieldset disabled="...">` でサイドバーをまとめて無効化 |
| メッセージの切り替え | `LoadingState` enum の値に応じてテキストを変える |
| UIスレッド解放 | `await Task.Yield()` の挿入（`ExecuteAndRecordAsync` は実装済み） |
| ローディング状態のトリガー | 非同期操作の前後で `_loadingState` フラグを変更し `StateHasChanged()` を呼ぶ |

> **制約**: `ExecuteAndRecord`（同期版）は `Task.Yield()` を挟めないため、
> `GenerateAndSort()` を `async Task` に変更して `ExecuteAndRecordAsync()` へ移行する必要がある。
> `ExecuteAndRecordAsync` はすでに実装済みで `Task.Yield()` も挿入済みなので、切り替えコストは低い。

---

## 2. ローディング状態の定義

```csharp
/// <summary>
/// UI のローディング状態を表す列挙型。
/// Idle 以外の状態では操作コントロールを無効化し、インジケーターを表示する。
/// </summary>
public enum LoadingState
{
    Idle,             // 通常状態（操作可能）
    Sorting,          // ソート処理の実行・記録中（Generate & Sort）
    AddingAlgorithm,  // 比較モードへのアルゴリズム追加中
    LoadingImage,     // 画像ファイルのアップロード・デコード中
}
```

---

## 3. UIフィードバックの仕様

### 3.1 ローディングオーバーレイ

**表示場所**: `visualization-area`（メインの可視化エリア全体）の上にオーバーレイとして重ねる。

**デザイン**:

```
┌──────────────────────────────┐
│  visualization-area           │
│                               │
│   ╔═════════════════════╗     │
│   ║  ⠋ Sorting...       ║     │  ← 半透明暗オーバーレイ
│   ║                     ║     │     スピナー + メッセージ
│   ╚═════════════════════╝     │
│                               │
└──────────────────────────────┘
```

| プロパティ | 値 |
|-----------|---|
| 背景色 | `rgba(0, 0, 0, 0.55)` |
| position | `absolute; inset: 0` |
| z-index | `100`（キャンバスより前面） |
| スピナー | CSS アニメーション（`@keyframes spin`、border-based） |
| スピナー色 | `#3B82F6`（青、既存のプライマリカラーに合わせる） |
| スピナーサイズ | `40px` |
| フォントサイズ | `1rem`、color: `#e5e7eb` |
| pointer-events | `all`（オーバーレイ下のクリックを遮断） |

**表示メッセージ（状態別）**:

| `LoadingState` | 表示テキスト |
|----------------|-------------|
| `Sorting` | `⏳ Sorting...` |
| `AddingAlgorithm` | `⏳ Adding algorithm...` |
| `LoadingImage` | `⏳ Loading image...` |

### 3.2 サイドバーコントロールの無効化

**方法**: `<fieldset disabled="@(_loadingState != LoadingState.Idle)">` でサイドバー内の
`sidebar-content` 全体を囲む。

- HTML 標準の `fieldset[disabled]` はフォーム要素（`button`、`select`、`input`、`InputFile` 等）を
  子孫も含めてすべて無効化する
- Blazor のイベントハンドラー（`@onclick` 等）は `disabled` 状態のとき発火しない
- ユーザーには視覚的に要素が操作不能（グレーアウト）だと伝わる

**例外**: `fieldset` の外に出すもの（無効化しない）:

- ページタイトル・見出し
- Debug Log トグル（ロード中にもデバッグログは確認したい）

**CSS 補足**: `fieldset` のデフォルトスタイルを上書きしてデザイン崩れを防ぐ。

```css
fieldset.controls-fieldset {
    border: none;
    padding: 0;
    margin: 0;
    min-inline-size: unset;
}
```

### 3.3 ボタンの状態

`fieldset[disabled]` の適用で自動的にグレーアウトされるが、
以下のボタンは追加でテキストを変えてフィードバックを強化する。

| ボタン | 通常テキスト | ローディング中 |
|--------|------------|--------------|
| Generate & Sort | `🎲 Generate & Sort` | `⏳ Sorting...`（テキスト変更 + disabled） |
| Add to Comparison | `➕ Add to Comparison` | `⏳ Adding...`（既存実装を拡張） |

---

## 4. 実装方針

### 4.1 `Index.razor` の変更

#### 追加するフィールド

```csharp
// ローディング状態
private LoadingState _loadingState = LoadingState.Idle;
private bool IsLoading => _loadingState != LoadingState.Idle;

private string LoadingMessage => _loadingState switch
{
    LoadingState.Sorting        => "⏳ Sorting...",
    LoadingState.AddingAlgorithm => "⏳ Adding algorithm...",
    LoadingState.LoadingImage   => "⏳ Loading image...",
    _                          => "",
};
```

#### `GenerateAndSort()` → `async Task GenerateAndSortAsync()`

```
変更前: private void GenerateAndSort()
             → Executor.ExecuteAndRecord(同期) を呼ぶ
             → UIスレッドを完全にブロック

変更後: private async Task GenerateAndSortAsync()
             → _loadingState = LoadingState.Sorting; StateHasChanged();
             → await Task.Yield();  // オーバーレイ表示を確定させる
             → await Executor.ExecuteAndRecordAsync() を呼ぶ
             → _loadingState = LoadingState.Idle; StateHasChanged();
```

#### `AddAlgorithmToComparison()` への追加

既存の `ComparisonMode.IsAddingAlgorithm` フラグとは独立して
`_loadingState = LoadingState.AddingAlgorithm` を設定・解除する。

#### サイドバー HTML 構造の変更

```razor
<div class="sidebar-content">
    <fieldset class="controls-fieldset" disabled="@IsLoading">
        ... (既存のサイドバーコンテンツ)
    </fieldset>
</div>
```

#### ビジュアライゼーションエリアへのオーバーレイ追加

```razor
<div class="visualization-area" style="position: relative;">
    @if (IsLoading)
    {
        <div class="loading-overlay">
            <div class="loading-spinner"></div>
            <span>@LoadingMessage</span>
        </div>
    }
    <div class="visualization-content">
        ... (既存)
    </div>
</div>
```

### 4.2 画像ロード時のフィードバック

画像アップロードは `PictureRowRenderer` / `PictureColumnRenderer` / `PictureBlockRenderer` 内で
行われる。各レンダラーコンポーネントに以下を追加する。

#### 各 PictureXxxRenderer.razor の変更

```
1. private bool _isLoadingImage = false; フィールド追加
2. HandleFileUpload() の先頭で _isLoadingImage = true; StateHasChanged();
3. finally ブロックで _isLoadingImage = false; StateHasChanged();
4. @if (_isLoadingImage) でコンポーネント内オーバーレイを表示
```

または `Index.razor` から `EventCallback OnLoadingStateChanged` を受け取る形にして
`_loadingState` を一元管理する。

> **推奨**: 複雑さを避けるため、まず `Index.razor` に `_loadingState` を一元管理し、
> 画像アップロードは各レンダラーコンポーネント内で独自に `_isLoadingImage` を持つ方式とする。
> 後で統合も可能。

### 4.3 CSS の追加（`app.css` または `index.css`）

```css
/* ローディングオーバーレイ */
.loading-overlay {
    position: absolute;
    inset: 0;
    z-index: 100;
    background: rgba(0, 0, 0, 0.55);
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 12px;
    color: #e5e7eb;
    font-size: 1rem;
    pointer-events: all;
}

.loading-spinner {
    width: 40px;
    height: 40px;
    border: 4px solid rgba(59, 130, 246, 0.3);
    border-top-color: #3B82F6;
    border-radius: 50%;
    animation: spin 0.8s linear infinite;
}

@keyframes spin {
    to { transform: rotate(360deg); }
}

/* fieldset リセット */
fieldset.controls-fieldset {
    border: none;
    padding: 0;
    margin: 0;
    min-inline-size: unset;
}
```

---

## 5. 実装スコープ（優先度付き）

### Phase 1（高優先度）: ソート処理中のフリーズ解消 + 基本インジケーター

- [ ] `GenerateAndSort()` を `async Task` 化し `ExecuteAndRecordAsync()` を使用
- [ ] `Index.razor` に `LoadingState` enum と `_loadingState` フィールド追加
- [ ] ビジュアライゼーションエリアにローディングオーバーレイ追加
- [ ] `<fieldset disabled>` でサイドバーコントロールを一括無効化
- [ ] CSS にスピナーとオーバーレイのスタイルを追加

### Phase 2（中優先度）: 比較モードのフィードバック改善

- [ ] `AddAlgorithmToComparison()` のローディング状態を `_loadingState` で管理
- [ ] 比較モードの再生成フロー（`needRegeneration`）にもローディング状態を追加

### Phase 3（低優先度）: 画像ロード中のインジケーター

- [ ] `PictureRowRenderer` / `PictureColumnRenderer` / `PictureBlockRenderer` に
  `_isLoadingImage` フラグとオーバーレイを追加
- [ ] ドラッグ＆ドロップでのファイル受信時も同様に対応

---

## 6. 考慮事項・注意点

### Blazor WASM のレンダリングタイミング

- `_loadingState = LoadingState.Sorting; StateHasChanged();` の直後に
  `await Task.Yield();` を挿入しないと、オーバーレイが実際に表示される前に
  重い処理が始まってしまう可能性がある。
- `await Task.Yield()` により制御をブラウザに返すことで、
  StateHasChanged でスケジュールされた再レンダリングが実行される。

### `SeekBar` とのインタラクション

- ローディング中はシークバーの操作も `fieldset[disabled]` で無効化される。
  ソート処理が完了していない状態でシークされると不整合が起きるため、これは望ましい。

### エラー時のクリーンアップ

- `ExecuteAndRecordAsync()` が例外をスローした場合でも
  `_loadingState = LoadingState.Idle` に戻す必要がある。
- `try/finally` パターンで確実にリセットする。

```csharp
_loadingState = LoadingState.Sorting;
StateHasChanged();
await Task.Yield();
try
{
    var result = await Executor.ExecuteAndRecordAsync(array, metadata);
    Playback.LoadOperations(...);
}
catch (Exception ex)
{
    DebugSettings.Log($"Error: {ex.Message}");
}
finally
{
    _loadingState = LoadingState.Idle;
    StateHasChanged();
}
```

### モバイル / 低スペック環境

- `ExecuteAndRecordAsync` の `YieldIntervalMs = 16ms`（約1フレーム）は
  低スペック端末では体感上まだ重いことがある。
- ローディングオーバーレイがある前提では、ユーザーは「処理中」と認識できるため、
  現状の `YieldIntervalMs` 値のままで十分と判断する。

---

## 7. 参考：既存の類似実装

`ComparisonModeControls.razor` の "Add to Comparison" ボタンは
`ComparisonMode.IsAddingAlgorithm` フラグによる無効化がすでに実装されている。

```razor
var isDisabled = ComparisonMode.State.Instances.Count >= ComparisonState.MaxComparisons
              || !ComparisonMode.State.IsEnabled
              || ComparisonMode.IsAddingAlgorithm;
<button disabled="@isDisabled">
    @(ComparisonMode.IsAddingAlgorithm ? "⏳ Adding..." : "➕ Add to Comparison")
</button>
```

この実装パターンを `GenerateAndSort` ボタンやサイドバー全体に拡張するのが今回の仕様の核心。
