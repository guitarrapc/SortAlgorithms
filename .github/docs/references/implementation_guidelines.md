# Implementation Guidelines

This reference describes **how** contributors implement the contracts in [spec.md](../spec.md). It is not a public API specification.

## Standard Shape

Use a static algorithm type and propagate all specialization parameters through internal helpers:

```csharp
public static void Sort<T>(Span<T> span) where T : IComparable<T>
    => Sort(span, new ComparableComparer<T>(), NullContext.Default);

public static void Sort<T, TContext>(Span<T> span, TContext context)
    where T : IComparable<T>
    where TContext : ISortContext
    => Sort(span, new ComparableComparer<T>(), context);

public static void Sort<T, TComparer, TContext>(
    Span<T> span, TComparer comparer, TContext context)
    where TComparer : IComparer<T>
    where TContext : ISortContext
{
    var values = new SortSpan<T, TComparer, TContext>(span, context, comparer, 0);
    // Algorithm implementation.
}
```

Use existing algorithms as the source of truth when an algorithm needs range, key-selector, seeded, or variant overloads.

## Observable Operations

- Use `SortSpan.Read`, `Write`, `Compare`, `Swap`, and `CopyTo` for logical element operations.
- Compare cached values through `SortSpan` overloads rather than calling the comparer directly.
- Wrap every auxiliary span in a `SortSpan` with the same comparer and context.
- Use buffer `0` for the input and stable positive IDs for auxiliary buffers.
- Propagate `TComparer` and `TContext` through private methods. Converting either to its interface loses specialization.
- Use structured `SortPhase` and `RoleType` values; presentation text belongs outside the algorithm.

Direct span access is appropriate only for storage that is not part of the observable sorting model, and the reason should be evident from nearby code or comments.

## Temporary Storage

Prefer no auxiliary storage when the algorithm permits it. For required storage, choose stack or pooled memory according to element safety, input size, and stack budget; do not apply a universal element-count threshold without considering `sizeof(T)` and reference-containing types. Return rentals in `finally`, and clear returned regions when references must not be retained.

## Hot Paths

- Keep loops simple and cache repeatedly used values.
- Use aggressive inlining only for small, measured hot helpers.
- Avoid LINQ, closures, iterator state machines, boxing, exception-driven control flow, and per-element allocation in hot paths.
- Use a small-sort fallback only when benchmarks justify the threshold for that algorithm.

## Documentation Checklist

Document the algorithm's ordering, stability, average/worst time complexity, auxiliary-space complexity, special preconditions, and relevant source or paper. After implementation, update the related spec with any newly discovered constraint or failed assumption.

