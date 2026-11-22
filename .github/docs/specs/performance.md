# Performance Specification

This document states the performance guarantees of SortAlgorithm and the evidence that each kind of performance claim needs. The [README](../../../README.md#benchmark) and the benchmark site publish the benchmark results; [benchmark_results.md](benchmark_results.md) defines what they show.

## Unobserved Sorting And `NullContext`

The ordinary overloads sort with `NullContext`, a value type. With `NullContext`, no observation callback runs and no counter changes. `NullContext` does not guarantee the same code size or instruction layout on every .NET or JIT version. Check a claim about generated code against the disassembly of the closed `NullContext` instantiation, not against the source.

This guarantee depends on closed-generic specialization. For this reason, `TContext` and `TComparer` stay generic through `SortSpan<T, TComparer, TContext>` and every helper below it.

- The JIT compiles a separate instantiation for the value type `NullContext`. In that instantiation, `typeof(TContext) != typeof(NullContext)` is a constant `false`. Dead-code elimination then removes the callback, its arguments, and the counter work.
- A concrete `TComparer` lets the JIT specialize a value-type comparer and call it without boxing or interface dispatch.
- If a hot helper takes `ISortContext` or `IComparer<T>` instead, interface dispatch returns and both benefits are lost.

[architecture.md](architecture.md) records the design decision.

## Statistics Ownership And Counting

A `StatisticsContext` has one owner during a sort. Sorting is synchronous, so the counters use plain increments, not `Interlocked`. With one owner, atomic updates add cost and no correctness. Do not share one instance between sorts that run at the same time. An algorithm that adds internal parallelism must not reuse the caller's context silently.

Swaps and range copies update the aggregate counters directly. They do not replay per-element callbacks. One logical swap counts 1 swap, 2 reads, and 2 writes. A range copy of n elements counts n reads and n writes. A change to the counter implementation must keep these values.

## Natural-Order Primitive Fast Path

Under natural ordering, `SortSpan` compares 13 primitive types directly: `byte`, `sbyte`, `ushort`, `short`, `uint`, `int`, `ulong`, `long`, `nuint`, `nint`, `float`, `double`, and `Half`. It uses this path only when the comparer is the internal natural comparer of the library. It never bypasses a caller's comparer, because a `typeof(T)` test alone also matches a reverse or domain-specific order.

The direct comparison must return the same result as the comparer. For `float`, `double`, and `Half`:

- NaN sorts before every number.
- Two NaNs compare equal.
- Negative zero and positive zero compare equal.

Relational operators alone do not give this order.

## Allocation And Memory Contract

- Hot loops that process elements do not allocate on the heap per element.
- A bufferless algorithm stays bufferless. It can use only bounded stack or call metadata that the algorithm documents.
- An algorithm that needs generic auxiliary storage normally rents it from `ArrayPool<T>`. Pool misses and pool internals can allocate, so this is not a zero-allocation guarantee.
- When an algorithm returns a rented array that holds references, it clears the array if keeping the references would be unsafe.
- Module-level `SkipLocalsInit` makes explicit initialization a correctness requirement. A histogram buffer that increments existing values has the highest risk. Clear it before use.
- `stackalloc` cannot allocate a generic `T` in general, because `T` can contain references. Metadata buffers and generic element buffers therefore use different storage.

An allocation claim names the algorithm, overload, element type, input size and pattern, runtime, and benchmark configuration.

## Required Evidence

| Claim | Required evidence |
|---|---|
| Faster/slower or crossover point | BenchmarkDotNet across relevant sizes and patterns |
| Allocation reduction | `MemoryDiagnoser` allocated-byte results |
| Callback or generic branch removed | Disassembly of the closed `NullContext` specialization |
| `setcc`, `cmov`, bounds-check removal, unroll, instruction selection | `DisassemblyDiagnoser` output for the relevant hot method |
| Algorithmic complexity or stability | Implementation reasoning and focused tests. A microbenchmark is not enough. |

## Vector Code

The library uses vector instructions only in these two forms:

- **A class of its own.** Vector code gets its own class in two cases. In the first case, the algorithm has a vectorized definition, as `SimdQuickSort` does. In the second case, the vector form changes which reads and comparisons a scalar algorithm makes. The vector code then goes in a `*Simd` class next to the scalar class, and the scalar class stays scalar. The vector class runs the same vector code with or without an observer. It reports lane operations as [observation.md](observation.md) specifies. Its observed counts can therefore depend on the vector width that the machine accelerates.
- **A kernel inside an existing class.** A vector kernel can do one step of a scalar algorithm only when all of these conditions are true:
  - No observer watches the sort (the context is `NullContext`).
  - The key is a primitive type in its natural order.
  - The hardware supports the instructions.

  The kernel must produce the same order as the scalar step. An observed sort always runs the scalar step, so its events describe what the unobserved sort did.

Every vector path checks `IsHardwareAccelerated` or the `IsSupported` property of the instruction set. When the check fails, a non-vector path produces the same result. Use x86-specific intrinsics only behind such a check.

## NetWasm Reporting Helper Inlining

In `SortSpan`, the four helpers that report observed comparisons request aggressive inlining only when the build defines `NETWASM`. The NetWasm SDK defines this symbol. Standard .NET builds keep the inlining heuristics of the compiler. The symbol changes only inlining hints. It does not change comparisons, callback order, buffer identities, or bounds checks.

The hints apply only when the NetWasm SDK builds the library sources. A precompiled standard .NET assembly does not get them. Do not define `NETWASM` for the standard .NET browser runtime to force the hints. Check this decision again after an SDK or runtime upgrade, and after a change to the reporting path.

## Non-goals

- Identical machine code across .NET versions, JIT modes, architectures, or NativeAOT
- A zero-allocation promise for every algorithm and context
- A `StatisticsContext` that is safe for concurrent shared mutation
- Removal of Debug bounds checks
- Bypassing a caller's comparer
- Replacing a BCL primitive only because a lower-level API exists
- Branchless execution where the branch predictor is faster
