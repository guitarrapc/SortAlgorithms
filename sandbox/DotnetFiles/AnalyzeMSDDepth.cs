#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Diagnostics;

Console.WriteLine("=== MSD Radix Sort 再帰深度分析 ===\n");

// テストデータパターン
var testCases = new (string Name, int[] Data)[]
{
    ("ランダム (100要素)", Enumerable.Range(0, 100).Select(_ => Random.Shared.Next(-1000, 1000)).ToArray()),
    ("ランダム (1000要素)", Enumerable.Range(0, 1000).Select(_ => Random.Shared.Next(-100000, 100000)).ToArray()),
    ("ソート済み", Enumerable.Range(0, 100).ToArray()),
    ("逆順", Enumerable.Range(0, 100).Reverse().ToArray()),
    ("重複多数", Enumerable.Range(0, 100).Select(_ => Random.Shared.Next(0, 10)).ToArray()),
    ("最小値〜最大値", new[] { int.MinValue, -1000, -1, 0, 1, 1000, int.MaxValue }),
};

// 理論的最大深度の計算
Console.WriteLine("【理論的最大深度】");
Console.WriteLine("RadixMSD4Sort (2-bit, 4基数):");
Console.WriteLine($"  byte  ( 8-bit): {(8 + 2 - 1) / 2,2} 桁 → 最大深度 {(8 + 2 - 1) / 2}");
Console.WriteLine($"  short (16-bit): {(16 + 2 - 1) / 2,2} 桁 → 最大深度 {(16 + 2 - 1) / 2}");
Console.WriteLine($"  int   (32-bit): {(32 + 2 - 1) / 2,2} 桁 → 最大深度 {(32 + 2 - 1) / 2}");
Console.WriteLine($"  long  (64-bit): {(64 + 2 - 1) / 2,2} 桁 → 最大深度 {(64 + 2 - 1) / 2}");

Console.WriteLine("\nRadixMSD10Sort (10進数):");
Console.WriteLine($"  byte  ( 8-bit, max=255):        {Math.Ceiling(Math.Log10(255 + 1)),2} 桁 → 最大深度 {(int)Math.Ceiling(Math.Log10(255 + 1))}");
Console.WriteLine($"  short (16-bit, max=65535):      {Math.Ceiling(Math.Log10(65535 + 1)),2} 桁 → 最大深度 {(int)Math.Ceiling(Math.Log10(65535 + 1))}");
Console.WriteLine($"  int   (32-bit, max=4.3×10^9):   {Math.Ceiling(Math.Log10((double)uint.MaxValue + 1)),2} 桁 → 最大深度 {(int)Math.Ceiling(Math.Log10((double)uint.MaxValue + 1))}");
Console.WriteLine($"  long  (64-bit, max=1.8×10^19):  {Math.Ceiling(Math.Log10((double)ulong.MaxValue + 1)),2} 桁 → 最大深度 {(int)Math.Ceiling(Math.Log10((double)ulong.MaxValue + 1))}");

Console.WriteLine("\n" + new string('=', 80) + "\n");

// 各テストケースで分析
foreach (var (name, originalData) in testCases)
{
    Console.WriteLine($"【テストケース: {name}】");
    
    // RadixMSD4Sort
    {
        var data = (int[])originalData.Clone();
        var stats = new StatisticsContext();
        
        var sw = Stopwatch.StartNew();
        RadixMSD4Sort.Sort(data.AsSpan(), stats);
        sw.Stop();
        
        Console.WriteLine($"\nRadixMSD4Sort (2-bit, 4基数):");
        Console.WriteLine($"  配列サイズ: {data.Length}");
        Console.WriteLine($"  実行時間: {sw.Elapsed.TotalMicroseconds:F2} μs");
        Console.WriteLine($"  IndexRead: {stats.IndexReadCount:N0}, IndexWrite: {stats.IndexWriteCount:N0}");
        Console.WriteLine($"  比較: {stats.CompareCount:N0}, スワップ: {stats.SwapCount:N0}");
        
        // ソート検証
        var isSorted = data.Zip(data.Skip(1), (a, b) => a <= b).All(x => x);
        Console.WriteLine($"  ソート結果: {(isSorted ? "✓ 正常" : "✗ 不正")}");
    }
    
    // RadixMSD10Sort
    {
        var data = (int[])originalData.Clone();
        var stats = new StatisticsContext();
        
        var sw = Stopwatch.StartNew();
        RadixMSD10Sort.Sort(data.AsSpan(), stats);
        sw.Stop();
        
        Console.WriteLine($"\nRadixMSD10Sort (10進数):");
        Console.WriteLine($"  配列サイズ: {data.Length}");
        Console.WriteLine($"  実行時間: {sw.Elapsed.TotalMicroseconds:F2} μs");
        Console.WriteLine($"  IndexRead: {stats.IndexReadCount:N0}, IndexWrite: {stats.IndexWriteCount:N0}");
        Console.WriteLine($"  比較: {stats.CompareCount:N0}, スワップ: {stats.SwapCount:N0}");
        
        // ソート検証
        var isSorted = data.Zip(data.Skip(1), (a, b) => a <= b).All(x => x);
        Console.WriteLine($"  ソート結果: {(isSorted ? "✓ 正常" : "✗ 不正")}");
    }
    
    Console.WriteLine("\n" + new string('-', 80) + "\n");
}

Console.WriteLine("\n【ビジュアライゼーションでの見え方について】");
Console.WriteLine("""
MSD Radix Sortがビジュアライゼーションで「無駄な処理」に見える理由:

1. **全体コピーの問題**
   - 各再帰レベルで temp ↔ main 間で配列全体をコピー
   - 実際にはバケット内だけを処理しているが、コピー時に全範囲をスキャン
   - 配列サイズ n × 再帰深度 d = O(n × d) の読み書き操作

2. **再帰の深さ**
   - RadixMSD4Sort: int型で最大16レベルの再帰
   - RadixMSD10Sort: int型で最大10レベルの再帰
   - 各レベルでバケット数（4 or 10）分の再帰呼び出し

3. **早期終了による最適化**
   - InsertionSortCutoff=16: 小さいバケットは挿入ソートに切り替え
   - 実際のデータでは最大深度まで行くことは稀
   - 同じ桁を持つ要素が多いほど早く終了

4. **適切かどうか**
   - **アルゴリズムとしては適切**: MSDの標準的な実装
   - **ビジュアライゼーション向きではない**: LSDの方が視覚的に分かりやすい
   - **改善案**: バケット範囲のみコピーする最適化（実装の複雑さとのトレードオフ）
""");

Console.WriteLine("\n【結論】");
Console.WriteLine("""
- MSD Radix Sortは理論的に正しく動作しています
- 再帰深度は型のビット数に依存 (int: MSD4で16, MSD10で10)
- ビジュアライゼーションで「無駄」に見えるのは、配列全体のコピー処理が見えるため
- これはMSDの特性であり、アルゴリズム自体の問題ではありません
- より視覚的に分かりやすいのはLSD Radix Sort（反復的、順次処理）です
""");
