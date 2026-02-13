# Sorting Algorithm Architecture

## Design Pattern: Class-based Context + SortSpan

Sorting algorithms follow a consistent architecture:

- **Static methods** - Sort algorithms are implemented as static methods (stateless)
- **ISortContext** - Handles observation (statistics, visualization) via callback interface
- **SortSpan<T, TComparer>** - ref struct that wraps `Span<T>` + `ISortContext` + `TComparer` for clean API
- **IComparer<T> / TComparer** - Generic comparer pattern for zero-alloc devirtualized comparisons

```
┌─────────────────────────────────────────────────────────────┐
│  BubbleSort.Sort<T>(span)              // No constraint     │
│  BubbleSort.Sort<T>(span, context)     // No constraint     │
│  BubbleSort.Sort<T,TComparer>(span, comparer, context)      │
│  ─────────────────────────────────────────────────────────  │
│  • Static methods (no instance required)                    │
│  • Stateless (pure functions)                               │
│  • Context handles statistics/visualization                 │
│  • TComparer : IComparer<T> for zero-alloc comparisons      │
└─────────────────────────────────────────────────────────────┘
```

## Public API Pattern

Each algorithm exposes **convenience overloads** (no constraint) that delegate to the main implementation via `Comparer<T>.Default`. This matches the `MemoryExtensions.Sort` pattern from dotnet/runtime - runtime validation instead of compile-time constraints:

```csharp
// Convenience: no context
public static void Sort<T>(Span<T> span)
    => Sort(span, Comparer<T>.Default, NullContext.Default);

// Convenience: with context
public static void Sort<T>(Span<T> span, ISortContext context)
    => Sort(span, Comparer<T>.Default, context);

// Main implementation: generic TComparer for zero-alloc devirtualization
public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context)
    where TComparer : IComparer<T>
{
    var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);
    SortCore(s, 0, span.Length);
}
```

All internal/private methods use `<T, TComparer>` type parameters with `where TComparer : IComparer<T>`.

## File Locations

- Algorithm implementations: [src/SortAlgorithm/Sortings/](../../src/SortAlgorithm/Sortings/)
- Core interfaces: [src/SortAlgorithm/](../../src/SortAlgorithm/)
- Unit tests: [tests/SortAlgorithm.Tests/](../../tests/SortAlgorithm.Tests/)
- Benchmark code: [sandbox/SandboxBenchmark/](../../sandbox/SandboxBenchmark/)

## Context Types

| Context | Purpose | Overhead |
|---------|---------|----------|
| `NullContext.Default` | No statistics (production) | Minimal (empty methods) |
| `StatisticsContext` | Collect operation counts | Small (Interlocked.Increment) |
| `VisualizationContext` | Animation/rendering callbacks | Medium (callback invocation) |
| `CompositeSortContext` | Combine multiple contexts | Medium-Large |
