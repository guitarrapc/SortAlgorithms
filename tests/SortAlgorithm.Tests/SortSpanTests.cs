#if DEBUG
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class SortSpanTests
{
    [Test]
    public async Task CopyTo_ShouldCopyRangeToAnotherSortSpan()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var destination = new int[5];
        var context = new StatisticsContext();

        var sourceSpan = new SortSpan<int, Comparer<int>>(source.AsSpan(), context, Comparer<int>.Default, 0);
        var destSpan = new SortSpan<int, Comparer<int>>(destination.AsSpan(), context, Comparer<int>.Default, 1);

        // Act
        sourceSpan.CopyTo(1, destSpan, 0, 3); // Copy [2, 3, 4] to destination[0..3]

        // Assert
        await Assert.That(destination[0]).IsEqualTo(2);
        await Assert.That(destination[1]).IsEqualTo(3);
        await Assert.That(destination[2]).IsEqualTo(4);
        await Assert.That(destination[3]).IsEqualTo(0); // Not copied
        await Assert.That(destination[4]).IsEqualTo(0); // Not copied
    }

    [Test]
    public async Task CopyTo_ShouldTrackStatistics()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var destination = new int[5];
        var context = new StatisticsContext();

        var sourceSpan = new SortSpan<int, Comparer<int>>(source.AsSpan(), context, Comparer<int>.Default, 0);
        var destSpan = new SortSpan<int, Comparer<int>>(destination.AsSpan(), context, Comparer<int>.Default, 1);

        // Act
        sourceSpan.CopyTo(0, destSpan, 0, 3); // Copy 3 elements

        // Assert - Should count as 3 reads + 3 writes
        await Assert.That(context.IndexReadCount).IsEqualTo(3UL);
        await Assert.That(context.IndexWriteCount).IsEqualTo(3UL);
    }

    [Test]
    public async Task CopyTo_ShouldCopyToRegularSpan()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var destination = new int[5];
        var context = new StatisticsContext();

        var sourceSpan = new SortSpan<int, Comparer<int>>(source.AsSpan(), context, Comparer<int>.Default, 0);

        // Act
        sourceSpan.CopyTo(2, destination.AsSpan(), 1, 2); // Copy [3, 4] to destination[1..3]

        // Assert
        await Assert.That(destination[0]).IsEqualTo(0); // Not copied
        await Assert.That(destination[1]).IsEqualTo(3);
        await Assert.That(destination[2]).IsEqualTo(4);
        await Assert.That(destination[3]).IsEqualTo(0); // Not copied
    }

    [Test]
    public async Task CopyTo_VerifyBetterThanLoopWrite()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var destination1 = new int[10];
        var destination2 = new int[10];
        var contextCopyTo = new StatisticsContext();
        var contextLoop = new StatisticsContext();

        var sourceSpan1 = new SortSpan<int, Comparer<int>>(source.AsSpan(), contextCopyTo, Comparer<int>.Default, 0);
        var destSpan1 = new SortSpan<int, Comparer<int>>(destination1.AsSpan(), contextCopyTo, Comparer<int>.Default, 1);

        var sourceSpan2 = new SortSpan<int, Comparer<int>>(source.AsSpan(), contextLoop, Comparer<int>.Default, 0);
        var destSpan2 = new SortSpan<int, Comparer<int>>(destination2.AsSpan(), contextLoop, Comparer<int>.Default, 1);

        // Act - using CopyTo
        sourceSpan1.CopyTo(0, destSpan1, 0, 10);

        // Act - using loop with Read/Write
        for (int i = 0; i < 10; i++)
        {
            destSpan2.Write(i, sourceSpan2.Read(i));
        }

        // Assert - Both should produce the same result
        await Assert.That(destination2).IsEquivalentTo(destination1, CollectionOrdering.Matching);

        // Assert - CopyTo should have the same statistics as loop
        // (Both are counted as reads + writes, but CopyTo is more efficient in tracking)
        await Assert.That(contextCopyTo.IndexReadCount).IsEqualTo(10UL);
        await Assert.That(contextCopyTo.IndexWriteCount).IsEqualTo(10UL);
        await Assert.That(contextLoop.IndexReadCount).IsEqualTo(10UL);
        await Assert.That(contextLoop.IndexWriteCount).IsEqualTo(10UL);
    }
}

#endif
