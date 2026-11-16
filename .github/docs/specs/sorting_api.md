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

Equal bucket keys do not imply equal comparer results. When every key is the same,
the explicit-comparer overload still sorts the single bucket stably by that comparer.
`SortBy` can finish after the key scan because equal keys define equal sort positions.

In short: the method name states what defines the order; a parameter that does not define the
order (a bucketing hint) never changes the name.

The 64-bit key width is the abstraction's ceiling by design — wider keys (Int128, BigInteger) and
selectors declaring `KeyBits` outside 1..64 are rejected rather than degraded.

RadixMSD4Sort, RadixMSD10Sort, and AmericanFlagSort process nontrivial buckets by radix
partitioning until the remaining digits are exhausted or the bucket has at most one element.
They do not switch small inputs or child buckets to a comparison sort. Uniform-digit pruning
and early completion for equal mapped keys preserve the radix definition.

SpreadSort is a hybrid. Its comparison stages receive a comparer consistent with the key
mapping: built-in element overloads use the natural comparer, and key-selector overloads use a
comparer over the extracted key.

NaN-prefix preprocessing applies only to the internal natural floating-point comparer. Explicit
comparers and key selectors retain control over every element, including NaN payloads and signed
zeros. Their NaNs are not removed from the range being sorted. See the [F01 correction](../reports/f01-nan-preprocessing-fix-2026-11-15.md).

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

Lesson learned: Boost templates `get_min_count` on its tuning constants and instantiates it twice —
`int_*` for `integer_sort`, `float_*` for `float_sort` — and the difference is not cosmetic.
`float_log_finishing_count = 4` enables a one-pass-completion branch that `int_log_finishing_count
= 31` deliberately disables: when a bin still holds more elements than its remaining key range has
distinct values, one more distribution pass bucket-sorts it outright instead of handing it to
pdqsort. The floating-point path uses the `float_*` constants for this reason.

Whether that branch is reachable depends entirely on key width, which is why measuring it on
`double` alone is misleading. At 32/64-bit key widths `log_divisor` never falls into the branch's
gate (observed 41-53 across narrow, wide, and integer-valued `double` data), so the tuning measures
neutral there. At 16 bits it lands at 3-4, well inside the gate: `Half` is 1.1x to 2.5x faster with
the float constants (65536 elements, narrow range: 1.69ms to 0.68ms). The gain tracks duplicate
density rather than floating point as such — `Half` has few distinct values, so bins stay large.

The same branch was measured on the integer path with `short` (identical 16-bit key width) and came
out neutral, so Boost's choice to disable it for `integer_sort` was left alone. Deviating from the
reference needs its own evidence, and there was none.

Lesson learned: a bin cache sized at `n` is not a safe substitute for Boost's growable
`bin_cache`. Bin counts come from the key *range*, not the element count, so a level whose bins are
mostly empty claims far more slots than it has elements, and `cache_offset + bin_count` can exceed
`n` (reproducible at n=3000). The correct bound is a constant derived from the key width and the
tuning constants — the recursion can only consume `2^max_splits` slots per level and only
`keyBits / 8` levels deep — which also removes an O(n) rental: 33.5 MB per sort at n=8M before,
a fixed ~88 KB pooled buffer after.

## Distribution Auxiliary Structures

A distribution sort's auxiliary structure holds what the sort cannot recover from where an element
sits. How much that is depends on the element type, not on the algorithm, so an algorithm's integer
overload and its key-selector overload may legitimately need different amounts of storage while
remaining the same algorithm. The rule: store only what the position cannot reconstruct.

Pigeonhole sort is where this bites. Its definition says one hole per key value and concatenate the
holes in key order; it says nothing about how a hole holds its contents. For a plain integer, the
hole index already determines the value it holds, and two integers sharing a hole are equal and
therefore indistinguishable — so a hole only has to record how many elements it received.
`PigeonholeSortInteger` stores occupancy and rebuilds each value from its hole index, which leaves
it with no auxiliary element buffer at all: `O(k)` auxiliary space independent of `n`, one read pass
to distribute, one write pass to collect, and every element written exactly once directly into its
final position. `PigeonholeSort.SortBy` cannot do this, because an element there carries payload the
key does not determine, so its holes must hold the elements and its space is `O(n + k)`.

This is what separates pigeonhole sort from counting sort once both are specialized to integers.
Counting sort's defining step is turning counts into cumulative offsets and placing each element at
a computed index; removing it would leave something that is no longer counting sort, so
`CountingSortInteger` keeps the prefix sum, the second read pass, and the `O(n)` output buffer.
Pigeonhole sort never computes a position, so nothing has to be kept. The naming boundary between
the two is not drawn identically across the literature; this library draws it at the prefix sum.

The consequence is measurable rather than notional. Before the occupancy representation,
`PigeonholeSortInteger` held its holes as index linked lists and ran at 2.5x `CountingSortInteger`
on duplicate-heavy input, where a hole's chain grows with `n` and collection becomes a dependent
load chain; afterwards it runs at 0.70-0.89x across every benchmarked pattern. The earlier form was
not an optimization opportunity that had been missed but the opposite — work spent preserving
intra-hole order that no caller can observe.

Stability is unaffected and, for the integer overloads, vacuous: equal integers are
indistinguishable, so no relative order among them is observable in the first place.

## Stability

Stability is an algorithm property, not a library-wide guarantee. An algorithm documented as stable preserves the original relative order of elements that compare equal. An unstable algorithm may reorder equal elements.

## Failure Behavior

Invalid algorithm-specific arguments fail through normal .NET argument exceptions. Exceptions raised by a comparer or observation context propagate to the caller; the library does not translate them into sorting-specific exceptions.

## Why This Shape

Static, span-based APIs avoid ownership ambiguity and make mutation explicit. Generic comparer and context types allow the runtime to specialize calls without giving up custom ordering or observation.


## Algorithm Identity

QuickSort3way uses its Bentley–McIlroy three-way partition for every range with more than one
element. It does not replace small partitions with insertion sort.

LibrarySort starts with one support element and inserts in doubling rounds. Support elements
are redistributed between rounds, with the active gapped prefix capped by the allocated
(1+ε)n storage. An insertion shifts toward a nearby gap rather than triggering early
redistribution. Equal keys retain their input order. The expected O(n log n) bound assumes
random arrival order; the implementation does not shuffle the input, and adversarial arrivals
may require quadratic work.

FlashSort retains its classification, permutation, and insertion-finishing stages. Its
insertion finish is part of the algorithm; a small nonconstant input does not bypass the
classification and permutation stages.
## Large Index Ranges

BlockQuickSort, BitonicSort, BatcherOddEvenMergeSort, and CombSort do not impose a smaller
input-size limit solely to avoid overflow in midpoint, stage, group, or gap calculations.
Their algorithm-specific preconditions still apply, including BitonicSort's power-of-two
length requirement. Allocation limits and the work required to sort a large input are
separate from whether its indices can be represented correctly.

## Shell Gap Sequences

The named Shell sorts use their defined gap sequence in descending order, selecting gaps
no greater than half the requested range length and ending at 1. The tables cover every
selectable gap for an `int` length; they do not introduce an arbitrary smaller maximum.
All five algorithms remain unstable, swap-based gapped insertion sorts.

- Knuth uses `h[0] = 1`, `h[k+1] = 3h[k] + 1`.
- Sedgewick uses `9(2^k - 2^(k/2)) + 1` for even k and
  `8*2^k - 6*2^((k+1)/2) + 1` for odd k, starting at k = 0.
- Tokuda retains the library's floor variant,
  `floor((4/5)*((9/4)^(k+1) - 1))`, starting at k = 0.
- Ciura retains the prefix `1, 4, 10, 23, 57, 132, 301, 701, 1750`, then extends it with
  `h[next] = floor(9h/4)`.
- Lee uses `ceil((gamma^k - 1)/(gamma - 1))`, k >= 1, with the fixed decimal
  `gamma = 2.243609061420001`. This decimal is treated exactly when defining the table;
  runtime floating-point rounding does not select the gaps.

## Merge Scratch Ownership

TimSort and PowerSort retain ownership of their current pooled scratch array until a
replacement has been rented successfully. A failed rent propagates the exception while
leaving the current lease available for the enclosing cleanup. Successful growth may
temporarily hold both arrays. Arrays whose elements contain references are cleared when
returned. No transactional guarantee is made for the input after a sorting exception.
