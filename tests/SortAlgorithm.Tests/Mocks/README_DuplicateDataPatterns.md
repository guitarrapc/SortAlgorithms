# Duplicate-Heavy Data Patterns for BlockQuickSort Testing

This document describes the duplicate-heavy test data patterns based on the BlockQuickSort paper benchmarks (Edelkamp & Weiß, 2016).

## Overview

These mock data classes provide standardized test datasets for sorting algorithms to evaluate their performance and correctness with arrays containing many duplicate elements. These patterns are particularly important for BlockQuickSort, as the paper specifically benchmarks these scenarios.

## Test Data Classes

### 1. MockAllIdenticalData
**InputType:** `AllIdentical`

**Description:** Arrays where all elements have the same value.

**Purpose:** Tests the worst-case scenario for partitioning algorithms where no meaningful partition can be made.

**Sizes:** 100, 500, 1000, 10000

**Paper Reference:** "constant array" benchmark

**Example:**
```
[42, 42, 42, 42, ..., 42]
```

---

### 2. MockTwoDistinctValuesData
**InputType:** `TwoDistinctValues`

**Description:** Arrays with only two distinct values (0 and 1) distributed randomly.

**Purpose:** Tests binary partitioning behavior and verifies the algorithm handles the simplest duplicate scenario efficiently.

**Sizes:** 100, 500, 1000, 5000, 10000

**Paper Reference:** "random 0-1 values" benchmark

**Example:**
```
[0, 1, 0, 0, 1, 1, 0, 1, ...]
```

---

### 3. MockHalfZeroHalfOneData
**InputType:** `HalfZeroHalfOne`

**Description:** Arrays split into two halves - first half all 0s, second half all 1s (pre-sorted).

**Purpose:** Tests performance on already-sorted binary data.

**Sizes:** 100, 256 (block boundary), 1000, 10000

**Paper Reference:** "A[i] = 0 for i < n/2 and A[i] = 1 otherwise" benchmark

**Example:**
```
[0, 0, 0, ..., 0, 1, 1, 1, ..., 1]
         ↑ n/2
```

---

### 4. MockManyDuplicatesSqrtRangeData
**InputType:** `ManyDuplicatesSqrtRange`

**Description:** Arrays with random values in the range [0, √n), creating approximately √n duplicates of each value.

**Purpose:** Tests partitioning with moderate duplicate density. As n grows, duplicates become more common.

**Sizes:** 100 (range 0-9), 500 (range 0-21), 1000 (range 0-31), 10000 (range 0-99), 25000 (range 0-157)

**Paper Reference:** "random values between 0 and √n" benchmark

**Example (n=100):**
```
[3, 7, 1, 3, 9, 2, 7, 1, ...] // values in [0, 10)
```

**Note:** The 25000-element array triggers BlockQuickSort's median-of-√n pivot selection strategy.

---

### 5. MockHighlySkewedData
**InputType:** `HighlySkewed`

**Description:** Arrays where approximately 90% of elements have the same value (1), and the remaining 10% are random.

**Purpose:** Tests extreme skewness where pivot selection may consistently encounter duplicates.

**Sizes:** 100, 500, 1000, 10000

**Paper Reference:** Inspired by duplicate-check scenarios mentioned in the paper

**Example:**
```
[1, 1, 1, 1, 437, 1, 1, 1, 1, 892, 1, 1, ...]
       ↑ ~90% are value 1
```

---

## Integration with Tests

These mock classes are used in the `SortResultOrderTest` theory in BlockQuickSortTests:

```csharp
[Test]
[MethodDataSource(typeof(MockDataSource), nameof(MockDataSource.AllIdenticalData))]
[MethodDataSource(typeof(MockDataSource), nameof(MockDataSource.TwoDistinctValuesData))]
[MethodDataSource(typeof(MockDataSource), nameof(MockDataSource.HalfZeroHalfOneData))]
[MethodDataSource(typeof(MockDataSource), nameof(MockDataSource.ManyDuplicatesSqrtRangeData))]
[MethodDataSource(typeof(MockDataSource), nameof(MockDataSource.HighlySkewedData))]
public async Task SortResultOrderTest(IInputSample<int> inputSample)
{
    // Test implementation
}
```

## Total Test Cases

Each mock class provides multiple size variations:
- MockAllIdenticalData: 4 test cases
- MockTwoDistinctValuesData: 5 test cases
- MockHalfZeroHalfOneData: 4 test cases
- MockManyDuplicatesSqrtRangeData: 5 test cases
- MockHighlySkewedData: 4 test cases

**Total: 22 duplicate-heavy test cases**

## Usage in Other Sorting Algorithms

These mock data classes are designed to be reusable across all sorting algorithm tests. To use them in another algorithm's tests:

```csharp
[Test]
[MethodDataSource(typeof(MockDataSource), nameof(MockDataSource.AllIdenticalData))]
[MethodDataSource(typeof(MockDataSource), nameof(MockDataSource.TwoDistinctValuesData))]
// ... other mock data classes
public async Task YourSortAlgorithmTest(IInputSample<int> inputSample)
{
    var array = inputSample.Samples.ToArray();
    YourSortAlgorithm.Sort(array.AsSpan());
    
    // Verify sorted
    await Assert.That(IsSorted(array)).IsTrue();
}
```

## Paper Reference

Edelkamp, S., & Weiß, A. (2016). BlockQuicksort: How Branch Mispredictions don't affect Quicksort. 
arXiv:1604.06697. https://arxiv.org/abs/1604.06697

The paper specifically benchmarks sorting algorithms on these patterns to evaluate:
- Cache efficiency with duplicates
- Branch prediction performance
- Partitioning quality when elements are not unique
- Adaptive pivot selection effectiveness

## Design Principles

1. **Deterministic:** All mock classes use fixed random seeds (42) for reproducibility
2. **Comprehensive:** Cover small to very large arrays (100 to 25000 elements)
3. **Boundary-aware:** Include sizes at block boundaries (e.g., 256 = 2×128)
4. **Theory-compatible:** Implement `IEnumerable<object[]>` for xUnit Theory support
5. **Documented:** Each class has XML documentation explaining its purpose
