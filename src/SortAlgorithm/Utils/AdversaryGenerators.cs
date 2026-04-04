namespace SortAlgorithm.Utils;

public static class TimsortAdversaryGenerator
{
    /// <summary>
    /// Generate an input that is hostile to TimSort:
    /// 1. natural run lengths follow an R_tim-like drag pattern
    /// 2. adjacent runs are value-interleaved, making merges expensive
    /// </summary>
    public static int[] Generate(int size, int minRun = 32)
    {
        if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
        if (minRun <= 0) throw new ArgumentOutOfRangeException(nameof(minRun));
        if (size == 0) return Array.Empty<int>();
        if (size == 1) return new[] { 0 };

        var runs = GenerateScaledRtimRuns(size, minRun);
        return MaterializeInterleavedNaturalRuns(runs);
    }

    /// <summary>
    /// Build run lengths whose sum is exactly size.
    /// Base pattern is R_tim(k), scaled by minRun.
    /// The remainder is merged into the last run to avoid tiny tail artifacts.
    /// </summary>
    private static int[] GenerateScaledRtimRuns(int size, int minRun)
    {
        int k = Math.Max(1, size / minRun);
        var baseRuns = Rtim(k);

        var runs = new List<int>(baseRuns.Count);
        foreach (var x in baseRuns)
            runs.Add(checked(x * minRun));

        int covered = runs.Sum();
        int rem = size - covered;

        if (rem < 0)
        {
            // In practice this should not happen because Sum(Rtim(k)) = k.
            // Kept defensively.
            throw new InvalidOperationException("Rtim scaling exceeded target size.");
        }

        if (rem > 0)
        {
            if (runs.Count == 0)
            {
                runs.Add(rem);
            }
            else
            {
                // Fold the tail into the last run rather than appending a tiny run.
                // Tiny tail runs are often too easy for practical TimSort.
                runs[^1] += rem;
            }
        }

        return runs.ToArray();
    }

    /// <summary>
    /// Buss & Knop style recursive run-count structure.
    /// Sum(Rtim(n)) = n.
    /// </summary>
    private static List<int> Rtim(int n)
    {
        if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n));
        if (n <= 3) return new List<int> { n };

        int nPrime = n / 2;
        var left = Rtim(nPrime);
        var right = Rtim(nPrime - 1);

        var result = new List<int>(left.Count + right.Count + 1);
        result.AddRange(left);
        result.AddRange(right);
        result.Add((n % 2 == 0) ? 1 : 2);
        return result;
    }

    /// <summary>
    /// Materialize runs so that:
    /// - each run is strictly increasing
    /// - each run boundary is descending, so natural runs stay separated
    /// - values from neighboring runs strongly interleave in final sorted order
    ///
    /// Construction:
    /// run i gets values:
    ///   perm(i), perm(i)+R, perm(i)+2R, ...
    /// where R = number of runs.
    ///
    /// This makes merge order between adjacent runs comparison-heavy
    /// instead of trivial "all-left then all-right".
    /// </summary>
    private static int[] MaterializeInterleavedNaturalRuns(int[] runLengths)
    {
        if (runLengths.Length == 0) return Array.Empty<int>();

        int total = 0;
        foreach (int len in runLengths)
        {
            if (len <= 0) throw new ArgumentException("run length must be positive", nameof(runLengths));
            total += len;
        }

        int runCount = runLengths.Length;
        var result = new int[total];

        // Use a permutation that keeps neighboring runs "close" in sorted order.
        // Identity is already decent; bit-reversal can also work, but identity is simple and effective.
        int[] perm = Enumerable.Range(0, runCount).ToArray();

        int index = 0;
        for (int run = 0; run < runCount; run++)
        {
            int len = runLengths[run];
            int baseRank = perm[run];

            for (int j = 0; j < len; j++)
            {
                result[index + j] = baseRank + j * runCount;
            }

            index += len;
        }

        // Sanity:
        // - inside each run: strictly increasing because +runCount each step
        // - boundary between run i and i+1:
        //     last_i = perm[i] + (len_i - 1) * runCount
        //     first_{i+1} = perm[i+1]
        //   Since len_i >= minRun-sized in practice, boundary is descending.
        //
        // If you ever use very tiny runs, add a fix-up pass here.
        return result;
    }

    /// <summary>
    /// Optional helper: extract natural ascending runs as TimSort-style preprocessing would.
    /// Useful for verifying the generator.
    /// </summary>
    public static int[] DetectAscendingRunLengths(ReadOnlySpan<int> a)
    {
        if (a.Length == 0) return Array.Empty<int>();

        var runs = new List<int>();
        int i = 0;

        while (i < a.Length)
        {
            int start = i;
            i++;

            if (i == a.Length)
            {
                runs.Add(1);
                break;
            }

            // Ascending
            while (i < a.Length && a[i - 1] <= a[i])
                i++;

            runs.Add(i - start);
        }

        return runs.ToArray();
    }
}
