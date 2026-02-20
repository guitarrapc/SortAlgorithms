# ソート実行時間の可視化 仕様書

## 1. 概要

### 1.1 目的

ソートアルゴリズムの可視化において、現在は「ops/frame」による時間制御を行っているが、実際の実行時間とops数は必ずしも比例しない。CPUキャッシュ、メモリアクセスパターン、分岐予測など様々な要因により、ソートアルゴリズムの実際のパフォーマンスは操作数だけでは予測できない。

そのため、**各ソートアルゴリズムが実際にかかった実行時間（実測値）** を画面上に表示し、ユーザーがアルゴリズムの真のパフォーマンスを理解できるようにする。

### 1.2 背景

- **現在の仕様**: ops/frameで再生速度を制御（例: 10 ops/frame = 600 ops/sec at 60 FPS）
- **問題点**: 操作数が同じでも、アルゴリズムによって実行時間は大きく異なる
  - 例: QuickSortとBubbleSortで同じ5,000操作でも、実際の実行時間は10倍以上異なることがある
  - キャッシュミス、分岐予測失敗、メモリアクセスパターンの違いが影響
- **解決策**: 実測値（ミリ秒/マイクロ秒）を計測し、画面に表示する

## 2. 計測方式

### 2.1 実行時間の計測

**計測方法:**
- `System.Diagnostics.Stopwatch` を使用した高精度計測
- ソート実行の開始直前から終了直後までを計測
- 計測対象: 純粋なソートロジックの実行時間（可視化処理は含まない）

**計測タイミング:**
```csharp
var stopwatch = Stopwatch.StartNew();
sortAlgorithm.Sort(span);  // 操作記録を含むソート実行
stopwatch.Stop();
var executionTime = stopwatch.Elapsed;
```

**精度:**
- `Stopwatch` は高精度タイマーを使用（通常はナノ秒精度）
- 表示は **マイクロ秒（μs）またはミリ秒（ms）** 単位
- 非常に高速なソート（< 100μs）でも正確に計測

### 2.2 データモデル

`SortExecutionResult` を拡張して実行時間を保存：

```csharp
public class SortExecutionResult
{
    /// <summary>ソート操作のリスト</summary>
    public List<SortOperation> Operations { get; init; }
    
    /// <summary>実際のソート実行時間（実測値）</summary>
    public TimeSpan ActualExecutionTime { get; init; }
    
    /// <summary>総操作数</summary>
    public int TotalOperations { get; init; }
    
    /// <summary>配列サイズ</summary>
    public int ArraySize { get; init; }
    
    /// <summary>アルゴリズム名</summary>
    public string AlgorithmName { get; init; }
    
    /// <summary>操作数/ミリ秒（パフォーマンス指標）</summary>
    public double OperationsPerMillisecond => TotalOperations / ActualExecutionTime.TotalMilliseconds;
}
```

## 3. 表示方式

### 3.1 表示位置

**統計パネル内に表示:**
- 既存の統計情報パネル（画面左側）に実行時間セクションを追加
- 操作統計の上部または下部に配置
- 常に表示（再生中・停止中問わず）

### 3.2 表示内容

#### 3.2.1 実行時間の表示

```
┌─────────────────────────────────────┐
│ Algorithm: QuickSort                │
│ Array Size: 512                     │
│ Status: Sorting... (45%)            │
├─────────────────────────────────────┤
│ 【Execution Time (Measured)】       │
│   Total Execution: 15.234 ms        │
│   Performance:     341 ops/ms       │
├─────────────────────────────────────┤
│ Operations:                         │
│   - Comparisons:     1,234          │
│   - Swaps:             567          │
│   - Index Reads:     2,345          │
│   - Index Writes:    1,456          │
├─────────────────────────────────────┤
│ Playback Time: 0:15.234 / 0:34.000  │
│ Progress: 2,345 / 5,200 ops (45%)   │
└─────────────────────────────────────┘
```

#### 3.2.2 表示項目の詳細

| 項目 | 説明 | フォーマット例 |
|-----|------|--------------|
| **Total Execution** | ソートアルゴリズムの実際の実行時間 | `15.234 ms` |
| **Performance** | 操作数/ミリ秒（ops/ms） | `341 ops/ms` |
| **Playback Time** | 可視化アニメーションの再生時間 | `0:15.234` (分:秒.ミリ秒) |
| **Progress** | 現在の操作数と総操作数 | `2,345 / 5,200 ops (45%)` |

### 3.3 表示フォーマット

#### 3.3.1 実行時間のフォーマット

```csharp
public static string FormatExecutionTime(TimeSpan time)
{
    if (time.TotalMicroseconds < 1)
        return $"{time.TotalNanoseconds:F0} ns";
    else if (time.TotalMicroseconds < 1000)
        return $"{time.TotalMicroseconds:F1} μs";
    else if (time.TotalMilliseconds < 1000)
        return $"{time.TotalMilliseconds:F3} ms";
    else
        return $"{time.TotalSeconds:F3} s";
}
```

**例:**
- `0.000000045 s` → `45 ns`
- `0.000234 s` → `234.0 μs`
- `0.015234 s` → `15.234 ms`
- `1.234567 s` → `1.235 s`

#### 3.3.2 パフォーマンス指標のフォーマット

```csharp
public static string FormatPerformance(double opsPerMs)
{
    if (opsPerMs < 1)
        return $"{(opsPerMs * 1000):F1} ops/s";
    else if (opsPerMs < 1000)
        return $"{opsPerMs:F0} ops/ms";
    else
        return $"{(opsPerMs / 1000):F2} M ops/s";
}
```

**例:**
- `0.5 ops/ms` → `500.0 ops/s`
- `341 ops/ms` → `341 ops/ms`
- `15234 ops/ms` → `15.23 M ops/s`

### 3.4 再生中の表示

#### 3.4.1 線形増加表示（推奨）

実行時間は **再生進捗に応じて0から線形的に増加表示**：

```
┌─────────────────────────────────────┐
│ 【Execution Time (Measured)】       │
│   Current: 6.855 ms / 15.234 ms     │
│   Performance:     341 ops/ms       │
└─────────────────────────────────────┘
```

**計算式:**
```csharp
// 現在の進捗率から実行時間を推定
var progressRatio = (double)currentOperationIndex / totalOperations;
var estimatedCurrentTime = actualExecutionTime * progressRatio;

// 例: 45%再生済み
// 15.234 ms × 0.45 = 6.855 ms
```

**表示の変化:**
- **停止中・完了時**: `Total Execution: 15.234 ms`（確定値）
- **再生中**: `Current: 6.855 ms / 15.234 ms`（進行中）

#### 3.4.2 固定表示（代替案）

シンプルに固定値のみを表示する方式も可能：

```
┌─────────────────────────────────────┐
│ 【Execution Time (Measured)】       │
│   Total Execution: 15.234 ms        │
│   Performance:     341 ops/ms       │
└─────────────────────────────────────┘
```

**推奨**: 線形増加表示の方が、再生中の進捗との対応が直感的。

#### 3.4.3 進捗との関係

**再生中の進捗表示（線形増加方式）:**
- **操作ベースの進捗**: `2,345 / 5,200 ops (45%)`
- **再生時間**: `0:15.234 / 0:34.000`（可視化アニメーションの時間）
- **実測時間（推定）**: `6.855 ms / 15.234 ms`（進捗に応じて線形増加）

**関係性の説明:**
```
実測時間 (15.234 ms)     : ソートアルゴリズムが実際にかかった時間
総操作数 (5,200 ops)      : ソート中に実行された操作の総数
再生時間 (34.000 s)       : 5,200 ops ÷ 10 ops/frame ÷ 60 FPS = 8.67 s
                           （アニメーション用に引き延ばされた時間）

現在の進捗 (45%, 2,345 ops) : 再生位置
推定実行時間 (6.855 ms)   : 15.234 ms × 45% = 6.855 ms（線形推定）
```

## 4. UI/UXデザイン

### 4.1 統計パネルレイアウト

#### 4.1.1 推奨レイアウト（縦配置）

**再生中の表示:**

```
┌─────────────────────────────────────┐
│ Algorithm: QuickSort                │
│ Array Size: 512                     │
│ Status: Sorting... (45%)            │
├─────────────────────────────────────┤
│ ⏱ Execution Time (Measured)         │
│ ───────────────────────────────     │
│   Current: 6.855 ms / 15.234 ms     │
│   Performance:     341 ops/ms       │
│                                     │
│ 📊 Operations                        │
│ ───────────────────────────────     │
│   Comparisons:      1,234           │
│   Swaps:              567           │
│   Index Reads:      2,345           │
│   Index Writes:     1,456           │
│                                     │
│ ⏯ Playback                           │
│ ───────────────────────────────     │
│   Current: 0:15.234 / 0:34.000      │
│   Progress: 2,345 / 5,200 ops       │
│   45%                               │
└─────────────────────────────────────┘
```

**停止中・完了時の表示:**

```
┌─────────────────────────────────────┐
│ Algorithm: QuickSort                │
│ Array Size: 512                     │
│ Status: Completed                   │
├─────────────────────────────────────┤
│ ⏱ Execution Time (Measured)         │
│ ───────────────────────────────     │
│   Total Execution: 15.234 ms        │
│   Performance:     341 ops/ms       │
│                                     │
│ 📊 Operations                        │
│ ───────────────────────────────     │
│   Comparisons:      2,456           │
│   Swaps:            1,234           │
│   Index Reads:      5,200           │
│   Index Writes:     3,890           │
└─────────────────────────────────────┘
```

#### 4.1.2 コンパクトレイアウト（横配置）

**再生中:**
```
┌─────────────────────────────────────┐
│ QuickSort | 512 elements | 45%      │
├─────────────────────────────────────┤
│ ⏱ Exec: 6.855/15.234 ms (341 op/ms) │
│ 📊 Ops: C:1,234 S:567 R:2,345 W:1,456│
│ ⏯ Play: 0:15 / 0:34                 │
└─────────────────────────────────────┘
```

**停止中:**
```
┌─────────────────────────────────────┐
│ QuickSort | 512 elements | Complete │
├─────────────────────────────────────┤
│ ⏱ Execution: 15.234 ms (341 ops/ms) │
│ 📊 Ops: C:2,456 S:1,234 R:5,200 W:3.9K│
└─────────────────────────────────────┘
```

### 4.2 視覚的な強調

#### 4.2.1 アイコン表示

- **⏱ (Stopwatch)**: 実行時間セクション
- **📊 (Bar Chart)**: 操作統計セクション
- **⏯ (Play/Pause)**: 再生コントロールセクション

#### 4.2.2 色分け

| 要素 | 色（ダークモード） | 色（ライトモード） | 説明 |
|-----|------------------|------------------|------|
| 実行時間値 | `#10B981` (緑) | `#059669` (暗緑) | 実測値を強調 |
| パフォーマンス値 | `#3B82F6` (青) | `#2563EB` (暗青) | 効率性を示す |
| 操作数 | `#FFFFFF` (白) | `#1A1A1A` (黒) | 標準テキスト |
| セクション区切り線 | `#374151` (灰) | `#D1D5DB` (明灰) | 区切り |

#### 4.2.3 ツールチップ

各項目にマウスホバーで詳細を表示：

**Total Execution / Currentホバー:**
```
┌────────────────────────────────────┐
│ Actual sorting execution time      │
│ measured using Stopwatch.          │
│                                    │
│ During playback: Estimated based   │
│ on progress (linear interpolation).│
│                                    │
│ This is the real-world performance │
│ of the algorithm, independent of   │
│ visualization speed.               │
└────────────────────────────────────┘
```

**Performanceホバー:**
```
┌────────────────────────────────────┐
│ Operations per millisecond         │
│                                    │
│ Formula: Total Ops / Execution Time│
│ Higher is better.                  │
└────────────────────────────────────┘
```

### 4.3 レスポンシブ対応

#### 4.3.1 デスクトップ（1920x1080以上）
- フル表示（前述の縦配置レイアウト）
- すべての項目を表示

#### 4.3.2 タブレット（768px - 1280px）
- セミコンパクト表示
- アイコンとラベルを併用

#### 4.3.3 モバイル（〜767px）
- コンパクト表示（横配置レイアウト）
- 最小限の情報のみ表示
- 詳細は折りたたみパネル内に配置

## 5. 実装の考慮事項

### 5.1 計測精度

#### 5.1.1 Stopwatchの精度

```csharp
// Stopwatch の頻度を確認
var frequency = Stopwatch.Frequency;
var isHighResolution = Stopwatch.IsHighResolution;

// 通常、IsHighResolution = true で frequency = 10,000,000 (10 MHz) 以上
// これにより、100 ナノ秒以下の精度で計測可能
```

#### 5.1.2 計測誤差の考慮

- **最小計測時間**: 1マイクロ秒（1μs）以上が望ましい
- **非常に高速なソート**（< 10μs）の場合:
  - 複数回実行して平均を取る（オプション）
  - 「< 10 μs」のように表示（オプション）

#### 5.1.3 Blazor WebAssemblyでの注意点

- WebAssemblyでは、ブラウザのJavaScript APIを経由するため、若干のオーバーヘッドがある
- それでも `performance.now()` などで高精度計測が可能
- .NET 10のBlazorでは `Stopwatch` が `performance.now()` を内部で使用

### 5.2 パフォーマンス指標の解釈

#### 5.2.1 Ops/msの意味

- **定義**: 1ミリ秒あたりに実行された操作数
- **高い値**: キャッシュ効率が良い、分岐予測が成功している
- **低い値**: キャッシュミスが多い、メモリアクセスが遅い

#### 5.2.2 アルゴリズム間の比較

**注意事項（ユーザーへの説明）:**
```
⚠ 注意:
  - 実行時間は配列サイズ、初期順序、実行環境に依存します
  - Ops/msは「効率性」を示しますが、総操作数が少ない方が速い場合もあります
  - 例: QuickSort (500 ops, 1 ms) vs BubbleSort (5,000 ops, 5 ms)
       → QuickSortの方が10倍速いが、Ops/msは同じ
```

### 5.3 データ保存

#### 5.3.1 実行結果のキャッシュ

```csharp
// 同じアルゴリズム+配列サイズの組み合わせは再利用可能
public class ExecutionCache
{
    private Dictionary<string, SortExecutionResult> _cache = new();
    
    public string GetCacheKey(string algorithmName, int arraySize, int seed)
        => $"{algorithmName}_{arraySize}_{seed}";
    
    public void Store(string key, SortExecutionResult result)
        => _cache[key] = result;
    
    public bool TryGet(string key, out SortExecutionResult result)
        => _cache.TryGetValue(key, out result);
}
```

#### 5.3.2 履歴の記録（将来的拡張）

- 過去の実行結果を保存
- アルゴリズムのパフォーマンス推移をグラフ表示
- 異なる環境（PC、ブラウザ）での比較

## 6. 実装例

### 6.1 ソート実行と計測

```csharp
public class SortExecutor
{
    public SortExecutionResult ExecuteSort(
        ISortAlgorithm sortAlgorithm,
        int[] array)
    {
        var operations = new List<SortOperation>();
        var context = new VisualizationContext(
            onCompare: (i, j, result, bufferIdI, bufferIdJ) =>
            {
                operations.Add(new SortOperation
                {
                    Type = OperationType.Compare,
                    Index1 = i,
                    Index2 = j,
                    BufferId1 = bufferIdI,
                    BufferId2 = bufferIdJ
                });
            },
            // ... 他のコールバック
        );
        
        var span = new SortSpan<int>(array, context);
        
        // 実行時間の計測
        var stopwatch = Stopwatch.StartNew();
        sortAlgorithm.Sort(span);
        stopwatch.Stop();
        
        return new SortExecutionResult
        {
            Operations = operations,
            ActualExecutionTime = stopwatch.Elapsed,
            TotalOperations = operations.Count,
            ArraySize = array.Length,
            AlgorithmName = sortAlgorithm.GetType().Name
        };
    }
}
```

### 6.2 統計パネルコンポーネント

```razor
@* StatisticsPanel.razor *@

<div class="statistics-panel">
    <div class="section">
        <h3>⏱ Execution Time (Measured)</h3>
        @if (IsPlaying && CurrentOperationIndex < TotalOperations)
        {
            @* 再生中: 線形増加表示 *@
            <div class="metric-row">
                <span class="label">Current:</span>
                <span class="value execution-time">
                    @FormatExecutionTime(EstimatedCurrentTime) / @FormatExecutionTime(Result.ActualExecutionTime)
                </span>
            </div>
        }
        else
        {
            @* 停止中・完了時: 確定値表示 *@
            <div class="metric-row">
                <span class="label">Total Execution:</span>
                <span class="value execution-time">@FormatExecutionTime(Result.ActualExecutionTime)</span>
            </div>
        }
        <div class="metric-row">
            <span class="label">Performance:</span>
            <span class="value performance">@FormatPerformance(Result.OperationsPerMillisecond)</span>
        </div>
    </div>
    
    <div class="section">
        <h3>📊 Operations</h3>
        <div class="metric-row">
            <span class="label">Comparisons:</span>
            <span class="value">@Statistics.ComparisonCount.ToString("N0")</span>
        </div>
        <!-- ... 他の操作統計 -->
    </div>
</div>

@code {
    [Parameter]
    public SortExecutionResult Result { get; set; }
    
    [Parameter]
    public SortStatistics Statistics { get; set; }
    
    [Parameter]
    public bool IsPlaying { get; set; }
    
    [Parameter]
    public int CurrentOperationIndex { get; set; }
    
    [Parameter]
    public int TotalOperations { get; set; }
    
    // 再生中の推定実行時間を計算
    private TimeSpan EstimatedCurrentTime
    {
        get
        {
            if (TotalOperations == 0) return TimeSpan.Zero;
            var progressRatio = (double)CurrentOperationIndex / TotalOperations;
            return TimeSpan.FromTicks((long)(Result.ActualExecutionTime.Ticks * progressRatio));
        }
    }
    
    private string FormatExecutionTime(TimeSpan time)
    {
        if (time.TotalMicroseconds < 1)
            return $"{time.TotalNanoseconds:F0} ns";
        else if (time.TotalMicroseconds < 1000)
            return $"{time.TotalMicroseconds:F1} μs";
        else if (time.TotalMilliseconds < 1000)
            return $"{time.TotalMilliseconds:F3} ms";
        else
            return $"{time.TotalSeconds:F3} s";
    }
    
    private string FormatPerformance(double opsPerMs)
    {
        if (opsPerMs < 1)
            return $"{(opsPerMs * 1000):F1} ops/s";
        else if (opsPerMs < 1000)
            return $"{opsPerMs:F0} ops/ms";
        else
            return $"{(opsPerMs / 1000):F2} M ops/s";
    }
}
```

### 6.3 スタイリング

```css
/* StatisticsPanel.razor.css */

.statistics-panel {
    background: #1A1A1A;
    color: #FFFFFF;
    padding: 1rem;
    border-radius: 0.5rem;
}

.section {
    margin-bottom: 1.5rem;
}

.section h3 {
    font-size: 0.875rem;
    font-weight: 600;
    margin-bottom: 0.5rem;
    color: #9CA3AF;
}

.metric-row {
    display: flex;
    justify-content: space-between;
    padding: 0.25rem 0;
}

.metric-row .label {
    color: #D1D5DB;
}

.metric-row .value {
    font-weight: 600;
}

.metric-row .value.execution-time {
    color: #10B981; /* 緑 - 実測値を強調 */
}

.metric-row .value.performance {
    color: #3B82F6; /* 青 - パフォーマンス指標 */
}
```

## 7. テスト戦略

### 7.1 単体テスト

#### 7.1.1 計測精度のテスト

```csharp
[Fact]
public void ExecutionTime_ShouldBeMeasuredAccurately()
{
    // Arrange
    var algorithm = new BubbleSort();
    var array = Enumerable.Range(0, 100).Reverse().ToArray();
    var executor = new SortExecutor();
    
    // Act
    var result = executor.ExecuteSort(algorithm, array);
    
    // Assert
    Assert.True(result.ActualExecutionTime.TotalMilliseconds > 0);
    Assert.True(result.ActualExecutionTime.TotalSeconds < 1); // 合理的な範囲
}
```

#### 7.1.2 フォーマット関数のテスト

```csharp
[Theory]
[InlineData(0.000000045, "45 ns")]
[InlineData(0.000234, "234.0 μs")]
[InlineData(0.015234, "15.234 ms")]
[InlineData(1.234567, "1.235 s")]
public void FormatExecutionTime_ShouldReturnCorrectFormat(double seconds, string expected)
{
    // Arrange
    var time = TimeSpan.FromSeconds(seconds);
    
    // Act
    var result = StatisticsPanel.FormatExecutionTime(time);
    
    // Assert
    Assert.Equal(expected, result);
}
```

### 7.2 統合テスト

#### 7.2.1 複数アルゴリズムでの計測

```csharp
[Fact]
public void ExecutionTime_ShouldVaryByAlgorithm()
{
    // Arrange
    var algorithms = new ISortAlgorithm[]
    {
        new BubbleSort(),
        new QuickSort(),
        new MergeSort()
    };
    var array = Enumerable.Range(0, 256).Reverse().ToArray();
    var executor = new SortExecutor();
    
    // Act
    var results = algorithms.Select(alg => executor.ExecuteSort(alg, array.ToArray())).ToList();
    
    // Assert
    Assert.True(results[0].ActualExecutionTime > results[1].ActualExecutionTime); // Bubble > Quick
    Assert.All(results, r => Assert.True(r.ActualExecutionTime.TotalMilliseconds > 0));
}
```

### 7.3 UIテスト

#### 7.3.1 表示確認

- 統計パネルに実行時間が表示されることを確認
- **再生中**: `Current: X.XXX ms / Y.YYY ms` 形式で表示
- **停止中・完了時**: `Total Execution: Y.YYY ms` 形式で表示
- フォーマットが正しいことを確認（ms/μs/ns）
- レスポンシブレイアウトで適切に表示されることを確認

#### 7.3.2 線形増加の確認

- 再生中、実行時間が進捗に応じて線形的に増加することを確認
- シークバーで位置を変更した際、実行時間も即座に更新されることを確認
- 一時停止時、実行時間が現在の推定値で固定されることを確認
- 再生完了時、`Total Execution` 表示に切り替わることを確認

#### 7.3.3 ツールチップ確認

- ホバー時にツールチップが表示されることを確認
- ツールチップの内容が正確であることを確認
- 再生中は「線形補間による推定値」である旨が表示されることを確認

## 8. 将来的な拡張

### 8.1 実行時間の詳細分析

- **段階別計測**: 初期化、ソート本体、後処理の時間を個別に計測
- **CPU使用率**: ソート実行中のCPU使用率を表示
- **メモリ使用量**: 割り当てられたメモリ量を表示

### 8.2 履歴とグラフ表示

- **実行履歴**: 過去の実行結果を保存
- **パフォーマンスグラフ**: 配列サイズと実行時間の関係をグラフ化
- **比較グラフ**: 複数アルゴリズムの実行時間を棒グラフで比較

### 8.3 統計的分析

- **複数回実行**: 同じ条件で複数回実行して平均・標準偏差を計算
- **信頼区間**: 実行時間の信頼区間を表示
- **パフォーマンスプロファイル**: ホットスポット分析（どの操作に時間がかかっているか）

### 8.4 エクスポート機能

- **CSV出力**: 実行結果をCSVファイルでエクスポート
- **レポート生成**: 実行時間とパフォーマンスのレポートをHTMLで生成
- **共有機能**: 実行結果をURLで共有

## 9. 参考資料

### 9.1 計測関連

- [Stopwatch Class (Microsoft Docs)](https://learn.microsoft.com/dotnet/api/system.diagnostics.stopwatch)
- [High-Resolution Timing in .NET](https://learn.microsoft.com/dotnet/standard/datetime/high-resolution-timing)
- [Performance.now() (MDN)](https://developer.mozilla.org/docs/Web/API/Performance/now)

### 9.2 可視化関連

- [Data Visualization Best Practices](https://www.tableau.com/learn/articles/data-visualization-tips)
- [Material Design Guidelines - Metrics](https://material.io/design/layout/metrics-keylines.html)

---

**Document Version**: 1.0
**Last Updated**: 2025-01-XX
**Author**: SortAlgorithmLab Team
**Related Documents**: 
- `VisualizationWeb.md` - メイン仕様書
