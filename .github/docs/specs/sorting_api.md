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

## Radix Key Mapping

The radix-sort family (LSD/MSD radix, American Flag, Spread) is constrained by an order-preserving
key mapping rather than by element type: the public `IRadixKeySelector<T>` maps an element to a
fixed-width unsigned key (at most 64 bits), and all digit extraction and bucket math operates on
that key. Two method names split the API by what defines the order:

- `Sort(...)` — the element itself is the key: integer overloads (`T : IBinaryInteger<T>`) and
  Half/float/double overloads (IEEE 754 total-order transform, all NaN values first, matching
  `IComparable<T>` semantics).
- `SortBy(...)` — an extracted key defines the order: `Func<T, int>` convenience overloads, and
  full-control overloads taking any `struct` `IRadixKeySelector<T>` (user-defined keys up to
  64 bits, JIT-devirtualized). The two names are required because a
  `SortBy<T, TRadixKey>(Span<T>, TRadixKey)` overload named `Sort` would collide with
  `Sort<T, TContext>(Span<T>, TContext)` — C# does not distinguish signatures by constraints.

The same rule applies to the key-selector distribution sorts: `CountingSort.SortBy` /
`PigeonholeSort.SortBy` / `BucketSort.SortBy` order strictly by the extracted key (stable, no
`IComparable<T>` requirement). The exception that proves the rule is
`BucketSort.Sort(span, keySelector, comparer, context)`: there the explicit comparer defines the
final order and the key selector is only a bucket-distribution accelerator, so the method keeps
the `Sort` name — the order source is visible in the signature. That overload's precondition is
that the key is order-consistent with the comparer (`comparer.Compare(x, y) <= 0` implies
`key(x) <= key(y)`); an inconsistent hint produces unsorted output.

In short: the method name states what defines the order; a parameter that does not define the
order (a bucketing hint) never changes the name.

The 64-bit key width is the abstraction's ceiling by design — wider keys (Int128, BigInteger) and
selectors declaring `KeyBits` outside 1..64 are rejected rather than degraded.

Comparison fallbacks inside these algorithms (insertion-sort cutoffs, pdqsort fallback) receive a
comparer from the public overload that must order consistently with the key mapping: built-in
element overloads pass the element's natural comparer, key-selector overloads pass a comparer over
the extracted key.

Lesson learned: routing the fallback comparer through the key mapping unconditionally regressed
SpreadSort's sorted-input early detection by ~67% (key transform on every comparison); passing the
natural comparer for built-in element types recovered baseline performance while the key-selector
path keeps its by-construction consistency.

Lesson learned: "order-consistent with the key" is a stronger requirement than the natural comparer
satisfies for floating point, and violating it is a crash rather than merely unsorted output. A
distribution sort that derives a bucket index as `key - min` needs `min` to be the minimum *by key*;
if the comparer disagrees, the index leaves the bucket array. The natural comparer disagrees twice
over: `SortSpan` specializes it to raw IEEE 754 operators, under which NaN is unordered and so is
never selected as the minimum even though its key is the smallest; and `CompareTo` itself reports
`-0.0` and `+0.0` as equal (because `-0.0 == 0.0`) while their keys differ. Neither shows up in
Debug — `SortSpan`'s debug path routes through the comparer — nor under an observing context, so
this class of defect needs Release tests on the no-context overload specifically.

The resolution that kept both properties: find the extremes by key (correctness), keep the
already-sorted check on the comparer (speed — it retains the primitive specialization and has no
loop-carried dependency), and detect NaN from the extremes rather than with a dedicated pre-pass,
since NaN is the only floating-point value mapping to key 0. Making the whole pass key-based instead
was measurably worse: hoisting the previous key into a local serialized a loop that had previously
pipelined, costing ~3x on already-sorted `double`.

Lesson learned: a bin cache sized at `n` is not a safe substitute for Boost's growable
`bin_cache`. Bin counts come from the key *range*, not the element count, so a level whose bins are
mostly empty claims far more slots than it has elements, and `cache_offset + bin_count` can exceed
`n` (reproducible at n=3000). The correct bound is a constant derived from the key width and the
tuning constants — the recursion can only consume `2^max_splits` slots per level and only
`keyBits / 8` levels deep — which also removes an O(n) rental: 33.5 MB per sort at n=8M before,
a fixed ~88 KB pooled buffer after.

## Stability

Stability is an algorithm property, not a library-wide guarantee. An algorithm documented as stable preserves the original relative order of elements that compare equal. An unstable algorithm may reorder equal elements.

## Failure Behavior

Invalid algorithm-specific arguments fail through normal .NET argument exceptions. Exceptions raised by a comparer or observation context propagate to the caller; the library does not translate them into sorting-specific exceptions.

## Why This Shape

Static, span-based APIs avoid ownership ambiguity and make mutation explicit. Generic comparer and context types allow the runtime to specialize calls without giving up custom ordering or observation.

