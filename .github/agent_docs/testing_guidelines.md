# Testing Guidelines

## Test Framework

This project uses **TUnit** as the testing framework. All tests are located in `tests/SortAlgorithm.Tests/`.

## Running Tests

### Run All Tests

```shell
dotnet test
```

or

```shell
cd tests/SortAlgorithm.Tests
dotnet run
```

### Run Specific Tests with Filter

TUnit uses a tree-node filter format:

```shell
dotnet run --treenode-filter /<Assembly>/<Namespace>/<Class name>/<Test name>
```

**Examples:**

Run all tests in a specific test class `PowerSortTests`:

```shell
dotnet run --treenode-filter /*/*/PowerSortTests/*
```

Run a specific test method:

```shell
dotnet run --treenode-filter /*/*/PowerSortTests/SortResultOrderTest
```

## Test Structure

### Standard Test Class Pattern

Each sorting algorithm should have a corresponding test class named `{AlgorithmName}Tests.cs`:

```csharp
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class PowerSortTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        PowerSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }
}
```

### Mock Data Sources

Available mock data sources are located in `tests/SortAlgorithm.Tests/Mocks/`:

## Required Test Cases

Every sorting algorithm must include the following test cases:

### 1. Sort Correctness Test (`SortResultOrderTest`)

Test with various input patterns using `[MethodDataSource]`:

```csharp
[Test]
[MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
[MethodDataSource(typeof(MockNegativePositiveRandomData), nameof(MockNegativePositiveRandomData.Generate))]
[MethodDataSource(typeof(MockNegativeRandomData), nameof(MockNegativeRandomData.Generate))]
[MethodDataSource(typeof(MockReversedData), nameof(MockReversedData.Generate))]
[MethodDataSource(typeof(MockMountainData), nameof(MockMountainData.Generate))]
[MethodDataSource(typeof(MockNearlySortedData), nameof(MockNearlySortedData.Generate))]
[MethodDataSource(typeof(MockSameValuesData), nameof(MockSameValuesData.Generate))]
[MethodDataSource(typeof(MockAntiQuickSortData), nameof(MockAntiQuickSortData.Generate))]
[MethodDataSource(typeof(MockQuickSortWorstCaseData), nameof(MockQuickSortWorstCaseData.Generate))]
[MethodDataSource(typeof(MockAllIdenticalData), nameof(MockAllIdenticalData.Generate))]
[MethodDataSource(typeof(MockTwoDistinctValuesData), nameof(MockTwoDistinctValuesData.Generate))]
[MethodDataSource(typeof(MockHalfZeroHalfOneData), nameof(MockHalfZeroHalfOneData.Generate))]
[MethodDataSource(typeof(MockManyDuplicatesSqrtRangeData), nameof(MockManyDuplicatesSqrtRangeData.Generate))]
[MethodDataSource(typeof(MockHighlySkewedData), nameof(MockHighlySkewedData.Generate))]
public async Task SortResultOrderTest(IInputSample<int> inputSample)
{
    var stats = new StatisticsContext();
    var array = inputSample.Samples.ToArray();

    YourSort.Sort(array.AsSpan(), stats);

    // Check is sorted
    Array.Sort(inputSample.Samples);
    await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
}
```

### 2. Stability Test (for stable sorts only)

If the algorithm is **stable**, include stability tests:

```csharp
[Test]
[MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
public async Task StabilityTest(StabilityTestItem[] items)
{
    var stats = new StatisticsContext();

    YourSort.Sort(items.AsSpan(), stats);

    // Verify sorting correctness
    await Assert.That(items.Select(x => x.Value).ToArray())
        .IsEquivalentTo(MockStabilityData.Sorted, CollectionOrdering.Matching);

    // Verify stability: for each group of equal values, original order is preserved
    var value1Indices = items.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
    await Assert.That(value1Indices)
        .IsEquivalentTo(MockStabilityData.Sorted1, CollectionOrdering.Matching);
}

[Test]
[MethodDataSource(typeof(MockStabilityWithIdData), nameof(MockStabilityWithIdData.Generate))]
public async Task StabilityTestWithComplex(StabilityTestItemWithId[] items)
{
    var stats = new StatisticsContext();

    YourSort.Sort(items.AsSpan(), stats);

    for (var i = 0; i < items.Length; i++)
    {
        await Assert.That(items[i].Key).IsEqualTo(MockStabilityWithIdData.Sorted[i].Key);
        await Assert.That(items[i].Id).IsEqualTo(MockStabilityWithIdData.Sorted[i].Id);
    }
}

[Test]
[MethodDataSource(typeof(MockStabilityAllEqualsData), nameof(MockStabilityAllEqualsData.Generate))]
public async Task StabilityTestWithAllEqual(StabilityTestItem[] items)
{
    var stats = new StatisticsContext();

    YourSort.Sort(items.AsSpan(), stats);

    // All values should be 1
    foreach (var item in items)
        await Assert.That(item.Value).IsEqualTo(1);

    // Original order should be preserved
    await Assert.That(items.Select(x => x.OriginalIndex).ToArray())
        .IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
}
```

## Assertions

### Common Assertion Patterns

**Check sorted order:**
```csharp
await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
```

**Check equality:**
```csharp
await Assert.That(value).IsEqualTo(expected);
```

**Check statistics (if needed):**
```csharp
await Assert.That(stats.CompareCount).IsGreaterThan(0UL);
await Assert.That(stats.SwapCount).IsGreaterThan(0UL);
```

## DEBUG-Only Tests

Some tests may only run in DEBUG mode to test statistics tracking:

```csharp
#if DEBUG
[Test]
public async Task StatisticsTrackingTest()
{
    // Statistics tracking tests...
}
#endif
```

## Best Practices

1. **Always use `StatisticsContext`**: Even if you don't verify statistics, create a context to ensure the algorithm works correctly with tracking.
2. **Test with multiple data patterns**: Use `[MethodDataSource]` to test with various input patterns in a single test method.
3. **Use async Task**: All test methods should return `async Task` and use `await Assert.That(...)`.
4. **Copy input for verification**: When verifying sort correctness, copy the original input and sort it with `Array.Sort` for comparison.
5. **Test edge cases**: Include tests for empty arrays, single elements, and duplicate-heavy patterns.
6. **Verify stability properly**: For stable sort algorithms, always include stability tests using `StabilityTestItem` types.
7. **Follow naming conventions**:
   - Test class: `{AlgorithmName}Tests`
   - Test method: Descriptive name ending with `Test`
   - Use `SortResultOrderTest` for basic correctness
   - Use `StabilityTest` for stability verification

## Example: Complete Test Class

```csharp
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class YourSortTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    [MethodDataSource(typeof(MockReversedData), nameof(MockReversedData.Generate))]
    [MethodDataSource(typeof(MockNearlySortedData), nameof(MockNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockAllIdenticalData), nameof(MockAllIdenticalData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        YourSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        var stats = new StatisticsContext();

        YourSort.Sort(items.AsSpan(), stats);

        await Assert.That(items.Select(x => x.Value).ToArray())
            .IsEquivalentTo(MockStabilityData.Sorted, CollectionOrdering.Matching);

        var value1Indices = items.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        await Assert.That(value1Indices)
            .IsEquivalentTo(MockStabilityData.Sorted1, CollectionOrdering.Matching);
    }
}
```
