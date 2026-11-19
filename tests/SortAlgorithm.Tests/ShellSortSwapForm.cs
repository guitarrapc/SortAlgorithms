using SortAlgorithm.Algorithms;

namespace SortAlgorithm.Tests;

/// <summary>
/// Runs the Shell sort gap passes in the form that exchanges neighbours of a chain, and counts their work. The sorts hold the element
/// instead, so they make the same comparisons, write once per exchange the model makes, and once more for each element that moved.
/// </summary>
internal static class ShellSortSwapForm
{
    public readonly record struct Counts(ulong Compares, ulong Exchanges, ulong Moved, ulong Held);

    public static Counts Count(int[] input, ReadOnlySpan<int> gaps)
    {
        var a = (int[])input.Clone();
        var n = a.Length;
        var gapIndex = gaps.Length - 1;
        ShellGapSequences.FindLast(gaps, n, ref gapIndex);

        ulong compares = 0, exchanges = 0, moved = 0, held = 0;
        for (; gapIndex >= 0; gapIndex--)
        {
            var h = gaps[gapIndex];
            for (var i = h; i < n; i++)
            {
                held++;
                var j = i;
                while (j >= h)
                {
                    compares++;
                    if (a[j - h] <= a[j]) break;
                    (a[j - h], a[j]) = (a[j], a[j - h]);
                    exchanges++;
                    j -= h;
                }
                if (j != i) moved++;
            }
        }
        return new Counts(compares, exchanges, moved, held);
    }
}
