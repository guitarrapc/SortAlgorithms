# Sorting API

## Purpose

Each algorithm has its own static type in `SortAlgorithm.Algorithms`. This design keeps the choice of algorithm explicit. It also lets an algorithm with different properties or options state its own contract honestly.

## Common Contract

- Sorting puts the supplied `Span<T>` in comparer order, in place.
- An empty or single-element span is valid input and stays unchanged.
- The result contains the same elements as the input. An algorithm does not intentionally add, remove, or replace values.
- Natural-order overloads require `T : IComparable<T>`.
- Custom-order overloads accept a generic `TComparer : IComparer<T>`.
- Observable overloads accept a generic `TContext : ISortContext`.
- Algorithm-specific preconditions and options are part of the API and documentation of that algorithm. They are not library-wide behavior.

Conceptually, the common overload family is:

```csharp
Sort<T>(Span<T> span) where T : IComparable<T>
Sort<T, TContext>(Span<T> span, TContext context)
Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
```

Some algorithms add range, key-selector, seed, or variant overloads. These overloads must keep the same ordering and observation semantics for the elements that they sort.

## Declared Properties

Every algorithm type exposes `public static bool IsStable`. It states whether the algorithm keeps the relative order of elements that compare equal. It is part of the API, not only of the documentation, because consumers act on it. Examples are a catalog that labels algorithms, a UI that offers "stable only", and a caller that chooses between two algorithms. A consumer must read this property and not copy its value, so that the answer has one source.

The library does not promise stability for all algorithms. That is out of scope. The property describes one algorithm only. A stable algorithm also gives the reason in its type summary. Stability usually comes from one specific choice, not from the shape of the algorithm. Examples are the side that receives equal keys and the end of a merge that wins a tie. The next person who changes that choice must know that stability depends on it.

A sorted result does not show stability, so no ordering test can detect a wrong value. `StableSortTestsBase` measures stability. `StabilityDeclarationTests` requires the declaration to match the test suite that the tests of the algorithm derive from. Some suites cannot observe stability, because integer-only entry points make equal keys identical. `StabilityDeclarationTests` lists those algorithms as explicit exemptions and does not skip them silently.

## Radix Key Mapping

The radix-sort family (LSD and MSD radix, American Flag, Spread) depends on an order-preserving key mapping, not on the element type. The public `IRadixKeySelector<T>` maps an element to a fixed-width unsigned key of at most 64 bits. All digit extraction and bucket math use that key. Two method names divide the API by what defines the order:

- `Sort(...)`: the element itself is the key. This name covers the integer overloads (`T : IBinaryInteger<T>`) and the `Half`, `float`, and `double` overloads. The floating-point overloads use the IEEE 754 total-order transform and put all NaN values first, as `IComparable<T>` does.
- `SortBy(...)`: an extracted key defines the order. This name covers the `Func<T, int>` convenience overloads and the full-control overloads that take any `struct` `IRadixKeySelector<T>`. A full-control selector can define a key of up to 64 bits, and the JIT devirtualizes it.

Both names are necessary. An overload `SortBy<T, TRadixKey>(Span<T>, TRadixKey)` with the name `Sort` would collide with `Sort<T, TContext>(Span<T>, TContext)`, because C# does not distinguish signatures by constraints.

The key-selector distribution sorts follow the same rule. `CountingSort.SortBy`, `PigeonholeSort.SortBy`, and `BucketSort.SortBy` order only by the extracted key. They are stable and do not require `IComparable<T>`.

`BucketSort.Sort(span, keySelector, comparer, context)` confirms the rule. In this overload the explicit comparer defines the final order, and the key selector only speeds up the distribution into buckets. The method therefore keeps the name `Sort`, and the signature shows the source of the order. The overload requires a key that is consistent with the comparer: `comparer.Compare(x, y) <= 0` implies `key(x) <= key(y)`. An inconsistent key gives unsorted output.

Equal bucket keys do not mean equal comparer results. When all keys are equal, the explicit-comparer overload still sorts the single bucket stably with that comparer. `SortBy` can stop after the key scan, because equal keys define equal sort positions.

Summary: the method name states what defines the order. A parameter that does not define the order, such as a bucketing hint, never changes the name.

The 64-bit key width is the upper limit of the abstraction by design. The library rejects wider keys (`Int128`, `BigInteger`) and selectors that declare `KeyBits` outside 1..64. It does not degrade them.
