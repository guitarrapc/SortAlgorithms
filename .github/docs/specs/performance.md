# Performance Specification

This document states the performance guarantees of SortAlgorithm and the evidence a performance claim needs. Benchmark results are published in the [README](../../../README.md#benchmark).

## Unobserved Sorting And `NullContext`

The ordinary overloads sort with `NullContext`, a value type. Selecting it guarantees that no observation callback runs and no counter is maintained. It does not guarantee the same code size or instruction layout on every .NET or JIT version: a claim about generated code is checked against the disassembly of the closed `NullContext` instantiation, not inferred from source.

The guarantee rests on closed-generic specialization, which is why `TContext` and `TComparer` stay generic through `SortSpan<T, TComparer, TContext>` and every helper below it. For the value type `NullContext` the JIT compiles its own instantiation, in which `typeof(TContext) != typeof(NullContext)` is a constant false, so dead-code elimination removes the callback, its arguments and the counter work. A concrete `TComparer` likewise lets a value-type comparer be specialized and called without boxing or interface dispatch. Erasing either to `ISortContext` or `IComparer<T>` in a hot helper brings interface dispatch back and loses both. [architecture.md](architecture.md) records the design decision.

## Statistics Ownership And Counting

A `StatisticsContext` has one owner during a sort. Sorting is synchronous, so its counters use plain increments rather than `Interlocked`; atomic updates would add cost without adding correctness under that ownership. An instance must not be shared by concurrently running sorts, and internal parallelism added to an algorithm must not silently reuse the caller's context.

Swaps and range copies update the aggregate counters directly instead of replaying per-element callbacks. One logical swap counts one swap, two reads and two writes, and a range copy of n elements counts n reads and n writes. A change to the counter implementation must preserve these values.

## Natural-Order Primitive Fast Path

Under natural ordering, `SortSpan` compares 13 primitive types directly: `byte`, `sbyte`, `ushort`, `short`, `uint`, `int`, `ulong`, `long`, `nuint`, `nint`, `float`, `double` and `Half`. The path is taken only when the comparer is the library's internal natural comparer. A caller's comparer is never bypassed, because `typeof(T)` alone would also match a reverse or domain-specific order.

The direct comparison must give the result the comparer gives. For `float`, `double` and `Half`, NaN precedes every number, NaNs compare equal to each other, and signed zeros compare equal. Relational operators alone do not meet this.

## Allocation And Memory Contract

- Hot element-processing loops do not perform per-element heap allocation.
- Bufferless algorithms remain bufferless apart from bounded stack or call metadata documented by the algorithm.
- Algorithms requiring generic auxiliary storage normally use `ArrayPool<T>`; pool misses and pool internals mean this is not a universal zero-allocation guarantee.
- Reference-containing rentals are returned with clearing where retention would be unsafe.
- Module-level `SkipLocalsInit` makes explicit initialization a correctness obligation. Histogram buffers that increment existing values are the highest-risk pattern and must be cleared before use.
- Generic `T` cannot generally be stack-allocated because it may contain references; metadata and generic element buffers use different strategies.

Allocation claims identify the algorithm, overload, element type, input size and pattern, runtime, and benchmark configuration.

## Required Evidence

| Claim | Required evidence |
|---|---|
| Faster/slower or crossover point | BenchmarkDotNet across relevant sizes and patterns |
| Allocation reduction | `MemoryDiagnoser` allocated-byte results |
| Callback or generic branch removed | Disassembly of the closed `NullContext` specialization |
| `setcc`, `cmov`, bounds-check removal, unroll, instruction selection | `DisassemblyDiagnoser` output for the relevant hot method |
| Algorithmic complexity or stability | Implementation reasoning plus focused tests; microbenchmarks are insufficient |

## Vector Code

Vector instructions are not a library-wide strategy. They appear only in the forms below.

- **A vectorized definition or a `*Simd` class.** When an algorithm's definition is itself vectorized, as `SimdQuickSort`'s is, or when a vector form changes which reads and comparisons a scalar algorithm makes, the vector code lives in a class of its own, and the scalar class stays scalar. Such a class runs the same vector code whether observed or not and reports lane operations as [observation.md](observation.md) specifies, so its observed counts can depend on the vector width the machine accelerates.
- **A kernel inside an existing class, under `NullContext` only.** A vector kernel may carry out one step of a scalar algorithm when nothing observes the sort, the key is a primitive type in its natural order, and the hardware supports the instructions. It must produce the same order as the scalar step. Observing contexts always run the scalar step, so their events describe what the unobserved sort did.

Every vector path checks `IsHardwareAccelerated` or the instruction set's `IsSupported`, and where the check fails a non-vector path produces the same result. x86-specific intrinsics are used only behind such a check.

## NetWasm Reporting Helper Inlining

The four observed-comparison reporting helpers in `SortSpan` request aggressive inlining only when compiled with the `NETWASM` symbol, which the NetWasm SDK defines. Standard .NET builds keep the compiler's own inlining heuristics. The condition changes inlining hints only, never comparisons, callback order, buffer identities or bounds checks.

The hints apply only when the library sources are built with the NetWasm SDK; consuming a precompiled ordinary .NET assembly does not apply them. Do not define `NETWASM` for the standard .NET browser runtime to force them. Revalidate the decision when the SDK or runtime is upgraded or the reporting path changes.

## Non-goals

- Identical machine code across .NET versions, JIT modes, architectures, or NativeAOT
- A universal zero-allocation promise for every algorithm and context
- Making `StatisticsContext` safe for concurrent shared mutation
- Removing Debug bounds checks
- Bypassing a caller's comparer
- Replacing a BCL primitive merely because a lower-level API exists
- Requiring branchless execution where the branch predictor is faster
