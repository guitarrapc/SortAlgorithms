# Sorting API

## Purpose

Every algorithm exposes its own static type in `SortAlgorithm.Algorithms`. This keeps algorithm choice explicit and allows algorithms with distinct properties or options to remain honest about their contracts.

## Common Contract

- Sorting mutates the supplied `Span<T>` into comparer order.
- Empty and single-element spans are valid inputs and remain unchanged.
- The result contains the same elements as the input; algorithms do not intentionally add, remove, or replace values.
- Natural-order overloads require `T : IComparable<T>`.
- Custom-order overloads accept a generic `TComparer : IComparer<T>`.
- Observable overloads accept a generic `TContext : ISortContext`.
- Algorithm-specific preconditions and options, when present, are part of that algorithm's API and documentation rather than universal library behavior.

The common overload family is conceptually:

```csharp
Sort<T>(Span<T> span) where T : IComparable<T>
Sort<T, TContext>(Span<T> span, TContext context)
Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
```

Some algorithms provide additional range, key-selector, seed, or variant overloads. Those extensions must preserve the same ordering and observation semantics for the elements they cover.

## Stability

Stability is an algorithm property, not a library-wide guarantee. An algorithm documented as stable preserves the original relative order of elements that compare equal. An unstable algorithm may reorder equal elements.

## Failure Behavior

Invalid algorithm-specific arguments fail through normal .NET argument exceptions. Exceptions raised by a comparer or observation context propagate to the caller; the library does not translate them into sorting-specific exceptions.

## Why This Shape

Static, span-based APIs avoid ownership ambiguity and make mutation explicit. Generic comparer and context types allow the runtime to specialize calls without giving up custom ordering or observation.

