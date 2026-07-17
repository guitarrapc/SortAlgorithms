using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

/// <summary>
/// Expected value of an operation counter after sorting a specific input.
/// </summary>
public enum CountExpectation
{
    /// <summary>No assertion on this counter.</summary>
    Any,
    /// <summary>The counter must be exactly zero.</summary>
    Zero,
    /// <summary>The counter must be non-zero.</summary>
    NonZero,
}

/// <summary>
/// Assertion helper shared by the sort test base classes.
/// </summary>
public static class StatAssert
{
    public static async Task Count(ulong actual, CountExpectation expectation, string name)
    {
        switch (expectation)
        {
            case CountExpectation.Zero:
                await Assert.That(actual).IsEqualTo(0UL).Because($"{name} should be 0");
                break;
            case CountExpectation.NonZero:
                await Assert.That(actual).IsNotEqualTo(0UL).Because($"{name} should be non-zero");
                break;
        }
    }
}

/// <summary>
/// Standard test suite shared by every comparison sort. Derive from this class
/// (or <see cref="StableSortTestsBase"/> for stable sorts), add
/// <c>[InheritsTests]</c>, and implement <see cref="Sort{T, TContext}"/>.
/// Algorithm-specific tests (theoretical operation counts, range overloads,
/// special code paths) stay in the derived class.
/// </summary>
public abstract class SortTestsBase
{
    /// <summary>Invokes the sort under test.</summary>
    protected abstract void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext;

    /// <summary>Inputs larger than this are skipped in the data-driven tests. Override for slow algorithms.</summary>
    protected virtual int MaxOrderTestSize => int.MaxValue;

    /// <summary>Expected IndexWriteCount after sorting already-sorted input.</summary>
    protected virtual CountExpectation SortedInputWrites => CountExpectation.Any;

    /// <summary>Expected SwapCount after sorting already-sorted input.</summary>
    protected virtual CountExpectation SortedInputSwaps => CountExpectation.Any;

    [Test]
    [MethodDataSource(typeof(MockStandardData), nameof(MockStandardData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        Skip.When(inputSample.Samples.Length > MaxOrderTestSize, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        Skip.When(inputSample.Samples.Length > MaxOrderTestSize, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        Skip.When(inputSample.Samples.Length > MaxOrderTestSize, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        Skip.When(inputSample.Samples.Length > MaxOrderTestSize, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockIntKeyRandomData), nameof(MockIntKeyRandomData.Generate))]
    public async Task SortIntStructResultOrderTest(IInputSample<Utils.IntKey> inputSample)
    {
        Skip.When(inputSample.Samples.Length > MaxOrderTestSize, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        Sort(empty.AsSpan(), stats);
    }

    [Test]
    public async Task EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        Sort(single.AsSpan(), stats);

        await Assert.That(single[0]).IsEqualTo(42);
    }

    [Test]
    public async Task EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        Sort(twoSorted.AsSpan(), stats);

        await Assert.That(twoSorted).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        Sort(twoReversed.AsSpan(), stats);

        await Assert.That(twoReversed).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        Sort(three.AsSpan(), stats);

        await Assert.That(three).IsEquivalentTo([1, 2, 3], CollectionOrdering.Matching);
    }

    [Test]
    public async Task StringSortTest()
    {
        var stats = new StatisticsContext();
        var strings = new[] { "zebra", "apple", "mango", "banana", "cherry" };
        Sort(strings.AsSpan(), stats);

        await Assert.That(strings).IsEquivalentTo(["apple", "banana", "cherry", "mango", "zebra"], CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        Skip.When(inputSample.Samples.Length > MaxOrderTestSize, "Skip large inputs for statistics test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await StatAssert.Count(stats.IndexWriteCount, SortedInputWrites, "IndexWriteCount");
        await StatAssert.Count(stats.SwapCount, SortedInputSwaps, "SwapCount");
    }
}
