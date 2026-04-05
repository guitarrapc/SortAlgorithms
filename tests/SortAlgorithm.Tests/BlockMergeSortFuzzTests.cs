using SortAlgorithm.Algorithms;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

/// <summary>
/// ランダムシードを用いたファジングテスト。
/// テスト検出時にシードが確定するため、失敗したテストはシードをコードに固定することで再現できます。
/// <br/>
/// Fuzz tests using random seeds. Seeds are fixed at test-discovery time,
/// so a failing run can be reproduced by hardcoding the seed shown in the test display name.
/// </summary>
public class BlockMergeSortFuzzTests
{
    /// <summary>
    /// Seed・サイズ・パターンを保持するテストケース。
    /// ToString() がテスト表示名になるため失敗時にシードが見えます。
    /// </summary>
    public record FuzzCase(int Seed, int Size, string Pattern)
    {
        public override string ToString() => $"pattern={Pattern}, size={Size}, seed={Seed}";
    }

    /// <summary>
    /// テスト検出時に一度だけ評価され、ランダムシードを確定させます。
    /// サイズは BlockMergeSort の各フェーズ境界を網羅するよう選定しています。
    /// <list type="bullet">
    ///   <item>0–9 : Phase 1 ソートネットワーク境界</item>
    ///   <item>511–513 : CacheSize (512) 境界</item>
    ///   <item>1023–1027 : BlockMergeLevel が初めて起動するサイズ境界</item>
    ///   <item>2048, 4096 : ブロックマージの複数ラウンドを含む大きいサイズ</item>
    /// </list>
    /// </summary>
    public static IEnumerable<Func<FuzzCase>> FuzzCases()
    {
        int[] sizes =
        [
            // Phase 1 sorting-network boundary
            1, 2, 3, 4, 5, 7, 8, 9,
            // Cache-based merge boundary (CacheSize = 512)
            511, 512, 513,
            // BlockMergeLevel activation boundary (~iterator.Length() >= 512)
            1023, 1024, 1025, 1027,
            // Multiple block-merge rounds
            2048, 4096,
        ];

        // Patterns chosen to stress different BlockMergeSort internal paths:
        //   allSame        : leaves buffer1 empty → tests the MergeInPlace fallback
        //   twoValues      : extreme duplicates  → stresses buffer extraction
        //   highDuplicates : values in [0, √size) → partial buffer extraction
        //   random         : general case
        //   nearlySorted   : exercises early-exit "already in order" checks
        //   reversed       : exercises the "reverse order: rotate" fast path
        string[] patterns = ["allSame", "twoValues", "highDuplicates", "random", "nearlySorted", "reversed"];

        foreach (var size in sizes)
        {
            foreach (var pattern in patterns)
            {
                var seed = Random.Shared.Next();
                yield return () => new FuzzCase(seed, size, pattern);
            }
        }
    }

    [Test]
    [MethodDataSource(nameof(FuzzCases))]
    public async Task FuzzSortInt(FuzzCase fuzz)
    {
        var array = BuildArray(fuzz.Seed, fuzz.Size, fuzz.Pattern);
        var expected = array.ToArray();
        Array.Sort(expected);

        BlockMergeSort.Sort(array.AsSpan());

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    static int[] BuildArray(int seed, int size, string pattern)
    {
        var rng = new Random(seed);
        return pattern switch
        {
            "random"         => Enumerable.Range(0, size).Select(_ => rng.Next()).ToArray(),
            "allSame"        => Enumerable.Repeat(42, size).ToArray(),
            "twoValues"      => Enumerable.Range(0, size).Select(_ => rng.Next(0, 2)).ToArray(),
            "highDuplicates" => Enumerable.Range(0, size).Select(_ => rng.Next(0, Math.Max(2, (int)Math.Sqrt(size)))).ToArray(),
            "nearlySorted"   => BuildNearlySorted(rng, size),
            "reversed"       => Enumerable.Range(0, size).Reverse().ToArray(),
            _ => throw new ArgumentOutOfRangeException(nameof(pattern), pattern, null),
        };
    }

    static int[] BuildNearlySorted(Random rng, int size)
    {
        var a = Enumerable.Range(0, size).ToArray();
        // Randomly swap ~5% of positions
        var swaps = Math.Max(1, size / 20);
        for (var i = 0; i < swaps; i++)
        {
            var x = rng.Next(size);
            var y = rng.Next(size);
            (a[x], a[y]) = (a[y], a[x]);
        }
        return a;
    }
}
