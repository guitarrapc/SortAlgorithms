using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;

namespace SortAlgorithm.Tests;

public class BucketSortConstantKeyTests
{
    [Test]
    [Arguments(int.MinValue)]
    [Arguments(0)]
    [Arguments(int.MaxValue)]
    public void ConstantBucketStillUsesTheComparer(int key)
    {
        foreach (var n in new[] { 0, 1, 2, 3, 31, 33, 1009 })
        foreach (var pattern in new[] { "sorted", "reverse", "duplicates", "random", "equal" })
        foreach (var descending in new[] { false, true })
        {
            var random = new Random(42);
            var items = Enumerable.Range(0, n).Select(i => new Item(pattern switch
            {
                "sorted" => i, "reverse" => n - i, "duplicates" => (n - i) % 4,
                "equal" => 1, _ => random.Next(-20, 21)
            }, i)).ToArray();
            var comparer = new ItemComparer(descending);
            Verify(items, key, comparer, NullContext.Default);
            Verify(items, key, comparer, new StatisticsContext());
            IComparer<Item> boxed = comparer;
            Verify(items, key, boxed, NullContext.Default);
            Verify(items, key, boxed, new StatisticsContext());
            var referenceComparer = Comparer<Item>.Create(comparer.Compare);
            Verify(items, key, referenceComparer, NullContext.Default);
            Verify(items, key, referenceComparer, new StatisticsContext());
        }
    }

    [Test]
    public void ConstantBucketPreservesFloatingPointBitsAndEqualElementOrder()
    {
        VerifyFloating<Half>();
        VerifyFloating<float>();
        VerifyFloating<double>();
    }

    static void VerifyFloating<T>() where T : unmanaged, IFloatingPointIeee754<T>
    {
        foreach (var n in new[] { 0, 1, 2, 3, 31, 1009 })
        foreach (var allNaN in new[] { false, true })
        foreach (var reverse in new[] { false, true })
        {
            var input = Enumerable.Range(0, n).Select(i => allNaN || i % 3 == 0
                ? PreparedFloatingSortCorrectnessTests.NaN<T>(i % 4)
                : i % 3 == 1 ? -T.Zero : T.Zero).ToArray();
            var comparer = reverse ? Comparer<T>.Create((x, y) => y.CompareTo(x)) : Comparer<T>.Default;
            var expected = input.OrderBy(x => x, comparer).Select(PreparedFloatingSortCorrectnessTests.Bits).ToArray();
            var plain = input.ToArray();
            var observed = input.ToArray();
            BucketSort.Sort(plain.AsSpan(), _ => 0, comparer, NullContext.Default);
            BucketSort.Sort(observed.AsSpan(), _ => 0, comparer, new StatisticsContext());
            if (!plain.Select(PreparedFloatingSortCorrectnessTests.Bits).SequenceEqual(expected)
                || !observed.Select(PreparedFloatingSortCorrectnessTests.Bits).SequenceEqual(expected))
                throw new InvalidOperationException("Constant bucket changed floating-point order, stability or bits.");
        }
    }

    static void Verify<TComparer, TContext>(Item[] input, int key, TComparer comparer, TContext context)
        where TComparer : IComparer<Item>
        where TContext : ISortContext
    {
        var expected = input.OrderBy(x => x, comparer).ToArray();
        var sentinel = new Item(987654, -1);
        var actual = Enumerable.Repeat(sentinel, input.Length + 4).ToArray();
        input.CopyTo(actual, 2);
        var keyCalls = 0;
        BucketSort.Sort(actual.AsSpan(2, input.Length), _ => { keyCalls++; return key; }, comparer, context);
        if (!actual.AsSpan(2, input.Length).SequenceEqual(expected))
            throw new InvalidOperationException("Constant bucket ignored order or stability.");
        if (actual[0] != sentinel || actual[1] != sentinel || actual[^2] != sentinel || actual[^1] != sentinel)
            throw new InvalidOperationException("A sort changed a sentinel.");
        if (keyCalls != (input.Length <= 1 ? 0 : input.Length))
            throw new InvalidOperationException("Key extraction was repeated during insertion.");
    }

    [Test]
    public void ConstantSortByRemainsAReadOnlyKeyScan()
    {
        foreach (var n in new[] { 0, 1, 3, 1009 })
        {
            var items = Enumerable.Range(0, n).Select(i => new Item(n - i, i)).ToArray();
            var original = items.ToArray();
            var stats = new StatisticsContext();
            var calls = 0;
            BucketSort.SortBy(items.AsSpan(), _ => { calls++; return 7; }, stats);
            if (!items.SequenceEqual(original) || calls != (n <= 1 ? 0 : n)
                || stats.IndexReadCount != (ulong)calls || stats.CompareCount != 0 || stats.IndexWriteCount != 0
                || stats.SwapCount != 0)
                throw new InvalidOperationException("Equal-key SortBy changed its contract.");
            BucketSort.SortBy(items.AsSpan(), _ => 7);
            if (!items.SequenceEqual(original)) throw new InvalidOperationException("SortBy lost stability.");
        }
    }

    [Test]
    public void ConstantBucketObservesDistributionInsertionAndCopy()
    {
        int[] actual = [3, 1, 2, 1];
        var expected = actual.ToArray();
        var events = new List<string>();
        var expectedEvents = new List<string>();
        BucketSort.Sort(actual.AsSpan(), _ => 0, Comparer<int>.Default, Observe(events));
        var context = Observe(expectedEvents);
        var main = new SortSpan<int, Comparer<int>, VisualizationContext>(expected, context, Comparer<int>.Default, 0);
        var temp = new SortSpan<int, Comparer<int>, VisualizationContext>(new int[expected.Length], context, Comparer<int>.Default, 1);
        for (var i = 0; i < main.Length; i++) main.Read(i);
        context.OnPhase(SortPhase.DistributionCount);
        context.OnPhase(SortPhase.DistributionAccumulate);
        context.OnPhase(SortPhase.DistributionWrite);
        for (var i = 0; i < main.Length; i++) temp.Write(i, main.Read(i));
        InsertionSort.SortCore(temp, 0, temp.Length);
        temp.CopyTo(0, main, 0, main.Length);
        if (!actual.SequenceEqual(expected) || !events.SequenceEqual(expectedEvents))
            throw new InvalidOperationException("Single-bucket operations were not accurately observed.");
    }

    static VisualizationContext Observe(List<string> events) => new(
        onCompare: (i, j, r, bi, bj) => events.Add($"c:{i}:{j}:{r}:{bi}:{bj}"),
        onIndexRead: (i, b) => events.Add($"r:{i}:{b}"),
        onSwap: (i, j, b) => events.Add($"s:{i}:{j}:{b}"),
        onIndexWrite: (i, b, v) => events.Add($"w:{i}:{b}:{v}"),
        onRangeCopy: (i, j, n, bi, bj, v) => events.Add($"x:{i}:{j}:{n}:{bi}:{bj}"),
        onPhase: (p, a, b, c) => events.Add($"p:{p}:{a}:{b}:{c}"),
        onRole: (i, b, r) => events.Add($"role:{i}:{b}:{r}"),
        onLink: (p, c, b, s) => events.Add($"link:{p}:{c}:{b}:{s}"));

    public sealed record Item(int Value, int OriginalIndex);

    readonly struct ItemComparer(bool descending) : IComparer<Item>
    {
        public int Compare(Item? x, Item? y) => descending
            ? y!.Value.CompareTo(x!.Value) : x!.Value.CompareTo(y!.Value);
    }
}
