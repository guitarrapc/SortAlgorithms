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

## Declared Properties

Every algorithm type exposes `public static bool IsStable`, stating whether it preserves the relative order of elements that compare equal. It is part of the API rather than only of the documentation because consumers act on it: a catalog that labels algorithms, a UI that offers "stable only", or a caller choosing between two algorithms all need the fact in a form they can read. A consumer must read this property rather than restate the value, so that the answer has one source.

The library does not promise stability across algorithms — that is explicitly out of scope — so the property is per algorithm and says nothing about any other. A stable algorithm additionally carries the reasoning in its type summary, because stability is usually a consequence of one specific choice (which side equal keys are sent to, which end of a merge wins a tie) rather than of the algorithm's shape, and the next person to touch that choice needs to know it was load-bearing.

Stability is not derivable from a sorted result, so a wrong value is invisible in every ordering test. `StableSortTestsBase` is what measures it, and `StabilityDeclarationTests` requires the declaration to agree with the suite an algorithm's tests derive from. Where a suite cannot observe stability — integer-only entry points make equal keys indistinguishable — the algorithm is listed as an explicit exemption there rather than silently skipped.

## Radix Key Mapping

The radix-sort family (LSD/MSD radix, American Flag, Spread) is constrained by an order-preserving key mapping rather than by element type: the public `IRadixKeySelector<T>` maps an element to a fixed-width unsigned key (at most 64 bits), and all digit extraction and bucket math operates on that key. Two method names split the API by what defines the order:

- `Sort(...)` — the element itself is the key: integer overloads (`T : IBinaryInteger<T>`) and
  Half/float/double overloads (IEEE 754 total-order transform, all NaN values first, matching
  `IComparable<T>` semantics).
- `SortBy(...)` — an extracted key defines the order: `Func<T, int>` convenience overloads, and
  full-control overloads taking any `struct` `IRadixKeySelector<T>` (user-defined keys up to
  64 bits, JIT-devirtualized). The two names are required because a
  `SortBy<T, TRadixKey>(Span<T>, TRadixKey)` overload named `Sort` would collide with
  `Sort<T, TContext>(Span<T>, TContext)` — C# does not distinguish signatures by constraints.

The same rule applies to the key-selector distribution sorts: `CountingSort.SortBy` / `PigeonholeSort.SortBy` / `BucketSort.SortBy` order strictly by the extracted key (stable, no `IComparable<T>` requirement). The exception that proves the rule is `BucketSort.Sort(span, keySelector, comparer, context)`: there the explicit comparer defines the final order and the key selector is only a bucket-distribution accelerator, so the method keeps the `Sort` name — the order source is visible in the signature. That overload's precondition is that the key is order-consistent with the comparer (`comparer.Compare(x, y) <= 0` implies `key(x) <= key(y)`); an inconsistent hint produces unsorted output.

Equal bucket keys do not imply equal comparer results. When every key is the same, the explicit-comparer overload still sorts the single bucket stably by that comparer. `SortBy` can finish after the key scan because equal keys define equal sort positions.

In short: the method name states what defines the order; a parameter that does not define the order (a bucketing hint) never changes the name.

The 64-bit key width is the abstraction's ceiling by design — wider keys (Int128,  BigInteger) and selectors declaring `KeyBits` outside 1..64 are rejected rather than degraded.
