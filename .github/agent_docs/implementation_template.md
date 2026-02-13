# Sorting Algorithm Implementation Template

Use this template as a starting point for implementing new sorting algorithms.

```csharp
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Sortings;

/// <summary>
/// {Algorithm Name} sorting algorithm.
/// Time Complexity: O(?) average, O(?) worst
/// Space Complexity: O(?)
/// Stable: Yes/No
/// </summary>
public static class MySort
{
    private const int InsertionSortThreshold = 16;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer (if needed)

    /// <summary>
    /// Sorts the span using {Algorithm Name}.
    /// </summary>
    public static void Sort<T>(Span<T> span)
        => Sort(span, Comparer<T>.Default, NullContext.Default);

    /// <summary>
    /// Sorts the span using {Algorithm Name} with context tracking.
    /// </summary>
    public static void Sort<T>(Span<T> span, ISortContext context)
        => Sort(span, Comparer<T>.Default, context);

    /// <summary>
    /// Sorts the span using {Algorithm Name} with a custom comparer and context tracking.
    /// </summary>
    public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context)
        where TComparer : IComparer<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);

        // Use insertion sort for small arrays
        if (s.Length <= InsertionSortThreshold)
        {
            InsertionSort.Sort(span, comparer, context);
            return;
        }

        SortCore(s, 0, s.Length - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T, TComparer>(SortSpan<T, TComparer> s, int left, int right)
        where TComparer : IComparer<T>
    {
        // Implementation here
        // Use s.Read(), s.Write(), s.Compare(), s.Swap() consistently
    }

    // Additional helper methods as needed
}
```

## Key Points

1. **Three overloads**: Two convenience overloads (no constraint, delegating via `Comparer<T>.Default`) + one main implementation with `TComparer : IComparer<T>`
2. **Generic TComparer pattern**: Main implementation uses `<T, TComparer> where TComparer : IComparer<T>` for zero-alloc devirtualized comparisons
3. **Runtime validation**: Convenience overloads matching `MemoryExtensions.Sort` pattern. `Comparer<T>.Default` performs runtime checks
3. **Early returns**: Check for trivial cases (`Length <= 1`)
4. **Hybrid approach**: Use insertion sort for small subarrays
5. **AggressiveInlining**: Mark hot-path helper methods
6. **SortSpan operations**: Always use `s.Read()`, `s.Write()`, `s.Compare()`, `s.Swap()`
7. **Buffer IDs**: Use constants for buffer identification
8. **XML documentation**: Include time/space complexity and stability
9. **Comparer propagation**: When calling other algorithms' `SortCore`, the comparer propagates automatically through the `SortSpan<T, TComparer>` type
