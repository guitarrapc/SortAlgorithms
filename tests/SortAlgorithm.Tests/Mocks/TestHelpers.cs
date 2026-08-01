namespace SortAlgorithm.Tests;

/// <summary>
/// Deterministic input generators for statistics-oriented tests.
/// A fixed seed keeps operation counts reproducible across runs, while
/// passing multiple seeds still exercises different permutations.
/// </summary>
public static class TestHelpers
{
    public static int[] ShuffledRange(int n, int seed)
    {
        var array = Enumerable.Range(0, n).ToArray();
        new Random(seed).Shuffle(array);
        return array;
    }

    /// <summary>
    /// A shuffled permutation of [0, n) with every element multiplied by <paramref name="stride"/>,
    /// so the key range is (n - 1) * stride while the element count stays n.
    /// </summary>
    /// <remarks>
    /// Radix statistics tests need this to choose how many digit passes an input costs. The dense
    /// [0, n) ranges used elsewhere keep n &lt;= 100 within one or two passes for every radix here,
    /// which leaves the per-pass accounting — and the buffer parity that decides whether a final
    /// copy happens — resting on a single pass. Widening the range without growing n separates the
    /// pass count from the element count, so the two can be asserted independently.
    /// </remarks>
    public static int[] ShuffledRangeScaled(int n, int seed, int stride)
    {
        var array = ShuffledRange(n, seed);
        for (var i = 0; i < array.Length; i++)
        {
            array[i] *= stride;
        }
        return array;
    }
}
