# Performance Requirements

All sorting algorithms must be implemented with **maximum attention to performance and memory efficiency**.

## Core Requirements

### 1. Zero Allocations

- Never allocate arrays or collections during sorting
- Use `Span<T>` for all array operations
- Use `stackalloc` for small temporary buffers (≤ 128 elements)
- Use `ArrayPool<T>.Shared` for large temporary buffers (> 128 elements)
- **NEVER** use `new T[]` or `new List<T>` for internal buffers

**Example:**

```csharp
// ✅ For small buffers - use stackalloc (no heap allocation)
if (span.Length <= 128)
{
    Span<T> tempBuffer = stackalloc T[span.Length];
    var temp = new SortSpan<T, TComparer>(tempBuffer, context, comparer, BUFFER_TEMP);
    // ...sorting logic...
}
// ✅ For large buffers - use ArrayPool (reusable, no GC pressure)
else
{
    var rentedArray = ArrayPool<T>.Shared.Rent(span.Length);
    try
    {
        var tempBuffer = rentedArray.AsSpan(0, span.Length);
        var temp = new SortSpan<T, TComparer>(tempBuffer, context, comparer, BUFFER_TEMP);
        // ...sorting logic...
    }
    finally
    {
        ArrayPool<T>.Shared.Return(rentedArray);
    }
}
```

### 2. Generic TComparer for Devirtualized Comparisons

- Use `<T, TComparer> where TComparer : IComparer<T>` on the main implementation method
- When `TComparer` is a struct (e.g., `Comparer<T>.Default`), the JIT devirtualizes and inlines `Compare()` calls
- Convenience overloads delegate via `Comparer<T>.Default`
- Never use `IComparer<T>` as a parameter type directly (use the generic `TComparer` pattern instead)

### 3. Aggressive Inlining

- Mark hot-path methods with `[MethodImpl(MethodImplOptions.AggressiveInlining)]`
- Especially for methods called frequently in loops (comparisons, swaps, etc.)

### 4. Loop Optimization

- Cache frequently accessed values outside loops
- Use `for` loops with indices instead of `foreach`
- Minimize redundant comparisons
- Avoid repeated property access or method calls

### 5. Hybrid Approaches

- Use insertion sort for small subarrays (typically < 16-32 elements)
- Switch algorithms based on data characteristics when beneficial
- Define threshold constants clearly (e.g., `InsertionSortThreshold = 16`)

## Verification

- Test with BenchmarkDotNet to measure performance
- Verify zero allocations in Release builds
- Ensure Context overhead is minimal with `NullContext.Default`
- Compare against standard library implementations where applicable
