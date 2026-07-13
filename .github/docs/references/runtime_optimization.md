# Runtime Optimization Reference

This document describes **how** the current implementation realizes [low_level_performance.md](../specs/low_level_performance.md). It is an implementation reference, not a promise of particular machine code.

## Implemented Mechanisms

| Mechanism | Current implementation | Important constraint |
|---|---|---|
| Span-based storage | Algorithms accept `Span<T>` and use internal `SortSpan<T, TComparer, TContext>` | `SortSpan` is stack-only and synchronous |
| Generic comparer | `TComparer : IComparer<T>` is propagated through algorithm helpers | Do not erase it to `IComparer<T>` in hot paths |
| Generic context | `TContext : ISortContext`; branches test `typeof(TContext) != typeof(NullContext)` | Generated-code elimination depends on runtime specialization |
| Natural comparer marker | `ComparableComparer<T>` implements internal `IComparableComparer` | Primitive paths must remain disabled for custom comparers |
| Primitive comparisons | Four value helpers and four index helpers specialize 13 primitive types | Floating-point NaN semantics require care |
| By-reference Release access | `MemoryMarshal.GetReference` plus `Unsafe.Add(ref, (nint)(uint)index)` | Algorithms must prove indices valid; Debug uses checked span indexing |
| Operation inlining hints | Small `SortSpan`, comparer, context, and utility methods use `AggressiveInlining` | A hint is not an inlining guarantee |
| Stack buffers | Fixed/bounded metadata and counting buffers use `stackalloc` | Stack budget and initialization must be reviewed per site |
| Shared pools | Generic and larger buffers use `ArrayPool<T>.Shared` | Return in `finally`; clear reference-containing arrays as needed |
| Local initialization policy | `globals.cs` applies module-level `SkipLocalsInit` | Buffers read before overwrite call `Clear` or `Fill` explicitly |
| Integer bit operations | Algorithms use `BitOperations.Log2` and `LeadingZeroCount` | Exact lowering is runtime/CPU dependent |
| Cold recursive path | `IntroSortDotnet` uses `NoInlining` on its internal recursion | This is local, not a repository-wide rule |

`SortSpan` also carries `bufferId` and `offset`. Its tracked path reports absolute logical indices, while its `NullContext` path avoids callback work. `CopyTo` uses `Span.CopyTo`; observable copies collect values for the callback, which is intentionally more expensive than the unobserved path.

## Current Primitive Fast Path

The direct boolean comparison path is selected only in Release, with `NullContext`, when the comparer implements `IComparableComparer`. It recognizes:

```text
byte, sbyte, ushort, short, uint, int, ulong, long,
nuint, nint, float, double, Half
```

Other types and all custom comparers fall back to `TComparer.Compare`. Debug always uses the comparer and checked span indexing. Review both build configurations when changing comparison helpers.

## Memory Review Checklist

For every new `stackalloc` site:

1. bound the maximum byte count rather than considering element count alone;
2. identify whether any element is read before it is assigned;
3. call `Clear` or `Fill` when initial values are required under `SkipLocalsInit`;
4. avoid generic `T` unless it is legally unmanaged for the specific API; and
5. add a pooled fallback when the bound can grow with input.

For every pool rental, return from `finally` and use `RuntimeHelpers.IsReferenceOrContainsReferences<T>()` when clearing a generic array is required.

## Techniques Not Currently Adopted As Shared Infrastructure

| Technique | Status and reason |
|---|---|
| `GC.AllocateUninitializedArray<T>` | Not used; pooling covers the principal temporary-buffer cases and uninitialized reads would add risk |
| ISA-specific SIMD (`Vector128/256`, AVX, AdvSimd) | Not used; generic ordering, NaN behavior, cross-architecture fallback, and maintenance cost remain unresolved |
| Portable `Vector<T>` sorting paths | Not used; overlap with runtime-optimized BCL operations and algorithm-specific benefit require evidence |
| `Unsafe.CopyBlock` | Not used; `Span.CopyTo` preserves generic and overlapping-copy semantics and is already optimized by the runtime |
| `CollectionsMarshal.AsSpan` | Not used; public input is span-based rather than list-based |
| `Unsafe.BitCast` | Not used; existing `Unsafe.As` by-reference specialization is already integrated |
| Inline arrays | Not used as shared infrastructure; current fixed buffers use `stackalloc` |

Branch-reduction techniques exist in individual algorithms such as branchless PDQ partitioning and conditional-index calculations. They are algorithm decisions, not a universal requirement; verify generated code and input-pattern trade-offs before generalizing them.

## Historical Measurements

The previous design note recorded local Release measurements for replacing ordinary indexing in `SortSpan` with by-reference access: roughly 1% for sequential scans, 5% for a neighboring compare loop, and 10% for an irregular backward InsertionSort scan at `N=4096`. These results explain the adopted direction but are not a current benchmark baseline because the original hardware, runtime details, raw output, and repeatability data were not retained with the document.

Future measurements should be stored with the benchmark configuration and compared using the policy in [performance.md](../specs/performance.md).
