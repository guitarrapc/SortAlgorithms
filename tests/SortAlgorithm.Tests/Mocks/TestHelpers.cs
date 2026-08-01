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
    /// <summary>
    /// How many of the first <paramref name="digitCount"/> digits (in base <paramref name="radix"/>) of these
    /// values, sign-flipped to radix keys and shifted down by the minimum, the values do not all agree on.
    /// </summary>
    /// <remarks>
    /// This is the number of distribute passes an LSD radix sort has to run: a digit every element shares is
    /// distributed by a stable counting sort back into the order it was already in, so that pass is the
    /// identity. Stated here from the definition rather than from any implementation, so the statistics tests
    /// are asserting the rule and not echoing the code.
    /// </remarks>
    public static int CountNonIdentityDigits(int[] values, int digitCount, ulong radix)
    {
        var keys = Array.ConvertAll(values, v => (ulong)((uint)v ^ 0x8000_0000u));
        var min = keys.Min();

        var executed = 0;
        var divisor = 1UL;
        for (var d = 0; d < digitCount; d++)
        {
            var first = (keys[0] - min) / divisor % radix;
            if (keys.Any(key => (key - min) / divisor % radix != first)) executed++;
            divisor *= radix;
        }
        return executed;
    }

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
