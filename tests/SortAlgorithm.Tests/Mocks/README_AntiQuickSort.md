# AntiQuickSort テストデータ

このディレクトリには、QuickSortの最悪ケースを引き起こすテストデータが含まれています。

## 概要

中央要素をピボットとして選択するQuickSort実装に対して、最悪ケース（O(n²)）の動作を引き起こすデータパターンを提供します。

## 実装されたパターン

### 1. Pipe Organ Pattern（推奨・最も効果的）

```
[0, 1, 2, ..., n/2, n/2-1, ..., 2, 1, 0]
```

**特徴:**
- 山型/ピラミッド形状のデータ
- 中央ピボット選択に対して最も効果的
- n=1000 で約 **252,000 回の比較**（ランダムの19倍）

**理論的背景:**
- 中央要素が常に最大値付近になる
- 極端に不均衡なパーティションが繰り返し発生
- 理論的最悪ケース (n²/2) の約50%に到達

### 2. McIlroy's Adversarial Pattern

McIlroyの論文 ([PDF](https://www.cs.dartmouth.edu/~doug/mdmspe.pdf)) に基づく動的アドバーサリーアルゴリズム。

**特徴:**
- ソート実行中に比較を追跡
- 最悪ケースを引き起こすように値を動的に割り当て
- より汎用的だが、実装依存度が高い

## パフォーマンス測定結果 (n=1000)

| パターン       | 比較回数 | ランダムとの比率 |
|----------------|----------|------------------|
| Random         | 13,164   | 1.0x             |
| Sorted         | 9,009    | 0.7x             |
| Reversed       | 9,016    | 0.7x             |
| Sawtooth       | 12,654   | 1.0x             |
| **PipeOrgan**  | **252,463** | **19.2x**     |
| Interleaved    | 13,238   | 1.0x             |

**理論値:**
- 平均ケース (2n ln n): ~13,816 比較
- 最悪ケース (n²/2): ~500,000 比較

## 使用方法

### テストでの使用

```csharp
[Test]
[MethodDataSource(typeof(MockDataSource), nameof(MockDataSource.AntiQuickSortData))]
public async Task SortTest(IInputSample<int> inputSample)
{
    var stats = new StatisticsContext();
    var array = inputSample.Samples.ToArray();
    
    QuickSort.Sort(array.AsSpan(), stats);
    
    // Pipe Organ パターンで大幅に比較回数が増加することを確認
    // n=1000 で約250,000回の比較が発生
}
```

### デモプログラム

```bash
dotnet run --project sandbox/AntiQuickSortDemo/AntiQuickSortDemo.csproj
```

## 他のパターン（効果が低い）

以下のパターンも `MockQuickSortWorstCaseData.cs` で提供されていますが、
中央ピボット QuickSort に対しては効果が低いことが実証されています：

- **Sawtooth**: `[0, n-1, 1, n-2, 2, n-3, ...]` - ランダムと同等
- **Interleaved**: `[0, n/2, 1, n/2+1, ...]` - ランダムと同等

## なぜPipe Organが効果的なのか

1. **中央ピボットの弱点を突く**
   - 配列の中央要素 = データの最大値付近
   - 一方のパーティションがほぼ空、もう一方がn-1個の要素を持つ
   
2. **再帰的に悪化**
   - 各レベルで同じパターンが繰り返される
   - O(n²) の動作に近づく

3. **実測データ**
   - ランダムデータと比較して **19倍** の比較回数
   - 理論的最悪ケースの **50%** に到達

## 参考文献

- McIlroy, M. D. (1999). "A Killer Adversary for Quicksort"
  - https://www.cs.dartmouth.edu/~doug/mdmspe.pdf
- QuickSort implementation: `src/SortAlgorithm/Algorithms/Partition/QuickSort.cs`
  - Hoare partition scheme
  - Middle element pivot: `(left + right) / 2`
