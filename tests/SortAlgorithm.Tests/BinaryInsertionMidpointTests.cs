using SortAlgorithm.Algorithms;

namespace SortAlgorithm.Tests;

public class BinaryInsertionMidpointTests
{
    [Test]
    public void MidpointCoversNonnegativeIndexBoundaries()
    {
        int[] boundaries = [0, 1, 2, 3, 7, 31, 1009,
            int.MaxValue / 2 - 1, int.MaxValue / 2, int.MaxValue / 2 + 1,
            int.MaxValue - 2, int.MaxValue - 1, int.MaxValue];

        foreach (var left in boundaries)
        foreach (var right in boundaries)
        {
            Verify(left, right);
        }
    }

    [Test]
    public void MidpointMatchesWideArithmeticForRandomIndices()
    {
        var random = new Random(2026);
        for (var i = 0; i < 65_536; i++)
        {
            var left = (int)random.NextInt64((long)int.MaxValue + 1);
            var right = (int)random.NextInt64((long)int.MaxValue + 1);
            Verify(left, right);
        }
    }

    static void Verify(int left, int right)
    {
        var expected = (int)(((long)left + right) / 2);
        var actual = BinaryInsertionSort.GetMidpoint(left, right);
        if (actual != expected)
            throw new InvalidOperationException($"Midpoint({left}, {right}) was {actual}, expected {expected}.");
    }
}
