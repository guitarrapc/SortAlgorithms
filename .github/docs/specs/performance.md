# Performance Specification

## Purpose

SortAlgorithms is both an algorithm study library and a performance-oriented implementation. Performance is part of the design contract: hot paths are shaped for the .NET JIT, memory traffic and allocation are controlled, and algorithm variants may deliberately trade code size or predictable-input speed for lower branch-misprediction cost.

Correctness, comparer semantics, observation accuracy, and memory safety take precedence over speed. An optimization is retained only when its applicable input class and trade-offs are understood.

## Measurement Baseline

The optimization decisions, benchmark ratios, and disassembly observations recorded in this specification are based on **.NET 10 with RyuJIT** unless a section states otherwise. They describe verified behavior for that runtime generation, not a promise that an earlier or later .NET runtime, NativeAOT, another JIT, or another CPU will emit identical instructions or preserve the same crossover points. Runtime upgrades require revalidation of claims that depend on generated code.

## Execution Modes

### Unobserved sorting

The ordinary overloads use the value-type `NullContext`. `TContext` remains generic throughout `SortSpan<T, TComparer, TContext>` and algorithm helpers, and hot operations branch on the closed generic type. For a `NullContext` specialization, the JIT can constant-fold the type test and remove callback and statistics paths.

"Zero-cost statistics" means that selecting `NullContext` does not execute observation callbacks or maintain counters. It does not promise identical code size or instruction layout on every .NET/JIT version; generated-code claims are checked with disassembly.

The theoretical basis is closed-generic specialization. `NullContext` is a value type, so the JIT sees a concrete `TContext` instantiation rather than a shared interface object. In that instantiation, `typeof(TContext) != typeof(NullContext)` is a compile-time-constant false condition; dead-code elimination can remove the callback arm, its arguments, and counter-related work. Keeping `TContext` generic through every helper is essential: converting it to `ISortContext` would reintroduce interface dispatch and prevent the same whole-path elimination. Disassembly validation checks the closed `NullContext` path for absence of context callback calls and tracking branches rather than inferring zero cost from source syntax.

### Instrumented sorting

A real `TContext` receives the callbacks defined by [observation.md](observation.md). Instrumentation necessarily performs more work, but the built-in statistics path avoids unnecessary synchronization:

- `StatisticsContext` uses ordinary `++`/`+=`, not `Interlocked.Increment`;
- a statistics instance belongs to one sort, or one thread, and must not be shared by concurrent sorts; and
- swaps and range copies update aggregate counters directly rather than replaying individual callback operations.

The project replaced atomic increments after measuring their lock-prefixed read-modify-write and ordering cost as dominant in instrumented runs. The source note records a 10–20× increment-cost difference for the measured environment; this is motivation, not a portable ratio guarantee.

### Why statistics counters are non-atomic

This choice follows from both the execution model and the generated-code cost.

All sorting implementations in this library execute synchronously on one thread; the repository contains no `Parallel`, `Task`, or `Thread` execution inside an algorithm. A context is caller-owned, and the supported statistics usage is one `StatisticsContext` per sort or per thread. Atomicity therefore provides no correctness benefit for the supported ownership model.

On the analyzed .NET 10 x64 RyuJIT build, `Interlocked.Increment`/`Add` lowered to lock-prefixed atomic read-modify-write operations (`lock xadd` or an equivalent locked instruction). Such an operation must obtain exclusive ownership of the counter's cache line and provides ordering stronger than this counter needs. It also forces a memory RMW on every update, restricting register promotion, combination of adjacent additions, store-buffer freedom, and out-of-order overlap. The analyzed cost was approximately 20 cycles versus an approximately one-cycle ordinary add, with an observed 10–20× per-update difference. Exact instruction selection and cycle counts vary by JIT, CPU, contention, and whether the return value is consumed; the invariant reason for avoiding it is the unnecessary atomic RMW and memory ordering in a single-owner hot path.

One logical `OnSwap` updates three counters: swap count, index-read count by two, and index-write count by two. The former atomic implementation consequently issued three atomic RMW operations per swap. For insertion-style algorithms, where these callbacks execute in the inner loop, synchronization rather than sorting or observation semantics became the dominant measured cost.

The same-run InsertionSort comparison that justified the change recorded:

| Case | Atomic counters | Plain `++`/`+=` | Improvement |
|---|---:|---:|---:|
| Statistics, `n=256` | 73.3 µs | 10.5 µs | 7.0× |
| Statistics, `n=1024` | 1,352 µs | 177 µs | 7.6× |

Relative to `NullContext`, instrumented overhead fell from 10–14× to 2.5–2.7×. The result attributes approximately 87% of the former instrumented time to atomic-counter cost in that workload, consistent with the generated-code hypothesis. Counter values remained identical for single-threaded execution, and the recorded validation run completed 6,711 tests with zero failures without changing statistics expectations.

This decision would need revisiting only if shared concurrent mutation became a supported `StatisticsContext` contract. Adding internal parallelism to an algorithm must not silently reuse the current non-atomic context.

## Implemented Runtime-Level Optimizations

| Area | Adopted design | Why it exists | Constraint |
|---|---|---|---|
| Contiguous input | Algorithms sort `Span<T>` through an internal `readonly ref struct` | Avoid ownership and slicing allocations; expose contiguous storage | `SortSpan` remains synchronous and stack-only |
| Comparer dispatch | `TComparer : IComparer<T>` is propagated as a concrete generic type | Permit value-type comparer specialization and avoid mandatory boxing/interface dispatch | Hot helpers must not erase it to `IComparer<T>` |
| Observation dispatch | `TContext : ISortContext` and value-type `NullContext` propagate through the full call chain | Allow the JIT to remove unobserved callback branches | A real context must receive every logical event |
| Primitive comparisons | Natural ordering specializes 13 primitive types through `typeof(T)` and `Unsafe.As` | Produce direct boolean comparisons rather than `CompareTo` followed by an integer-result test | Enabled only for `IComparableComparer`; custom comparers are never bypassed |
| Release element access | `MemoryMarshal.GetReference` and `Unsafe.Add(ref, (nint)(uint)index)` back `SortSpan` access | Avoid repeated bounds checks and the sign-extension observed with an `int` index | Algorithm logic must establish valid indices |
| Debug element access | Normal `Span<T>` indexing remains in Debug | Detect out-of-range algorithm defects during development | Debug and Release remain equivalent for valid input |
| Inlining | Small access, comparison, comparer, context, and utility methods use `AggressiveInlining` | Expose constants and generic types to callers and reduce hot call overhead when accepted by the JIT | Inlining is verified, not assumed |
| Local initialization | Module-level `SkipLocalsInit` | Avoid implicit clearing of locals and stack buffers that will be overwritten | Buffers read before overwrite explicitly `Clear` or `Fill` |
| Small temporary storage | Bounded metadata, count, recursion, and offset buffers use `stackalloc` | Avoid heap traffic for fixed or tightly bounded work memory | Review maximum byte count and stack depth per site |
| Large/generic storage | Algorithms rent from `ArrayPool<T>.Shared` | Reuse merge, tree, distribution, and work buffers | Return from `finally`; clear reference-containing arrays when required |
| Bit operations | `BitOperations.Log2` and `LeadingZeroCount` replace manual bit loops | Permit efficient platform lowering | Exact instructions depend on runtime and CPU |
| Bulk movement | `Span.CopyTo` performs overlap-safe range movement | Use the BCL's optimized generic copy rather than hand-written loops | Observable copies also account for logical reads/writes |
| Hot/cold shaping | Small hot helpers use `AggressiveInlining`; `IntroSortDotnet` marks its recursive core `NoInlining` | Balance specialization opportunities against caller code size | Applied locally from measurement or structural need |

The primitive set is `byte`, `sbyte`, `ushort`, `short`, `uint`, `int`, `ulong`, `long`, `nuint`, `nint`, `float`, `double`, and `Half`. Direct floating-point relational operators treat NaN as unordered, unlike `CompareTo`; algorithms using these helpers preserve their documented NaN handling.

### Why unchecked by-reference access is selective

RyuJIT already removes many bounds checks from simple forward loops through range analysis. Replacing every span access with `Unsafe` would therefore add risk without a guaranteed benefit. The retained `SortSpan` Release path was selected because one centralized implementation covers the irregular, backward, and runtime-indexed accesses where range proof is weakest, while Debug retains checked indexing.

The .NET 10 local comparison at `n=4096` recorded roughly 1% improvement for a sequential scan, 5% for an adjacent-element comparison loop, and 10% for InsertionSort's irregular backward scan. This gradient matched the theory: the simpler the index progression, the more likely the original check was already eliminated. The `(nint)(uint)index` address form was retained after .NET 10 RyuJIT disassembly showed that it avoided the repeated sign-extension associated with the measured `int` address calculation. These figures explain the design choice but are not portable performance guarantees.

The same evidence rule applies to offset buffers in `PDQSortBranchless`: their store index is data-dependent, so the measured JIT could not prove it in range, making the unchecked first-element-ref path materially different from ordinary sequential indexing.

## Implemented Algorithm-Level Optimizations

### Branchless PDQSort partition

`PDQSortBranchless` is a scalar block-partition implementation, not an unimplemented SIMD proposal. It preserves PDQSort's pivot selection, duplicate handling, pattern detection, heapsort fallback, and tail-recursion elimination while replacing the data-dependent partition branch with two 64-byte offset buffers.

The implementation uses techniques selected from BenchmarkDotNet and `DisassemblyDiagnoser` results:

- boolean classification is converted with `Unsafe.As<bool, byte>` so .NET 10 RyuJIT emits a flag-producing comparison (`setcc`) and addition rather than a data-dependent classification branch;
- runtime-indexed offset-buffer writes use first-element refs plus `Unsafe.Add`, because checked span indexing retained a bounds check in the measured loop and cost 5–15%;
- classification is manually unrolled eight times because the measured .NET 10 RyuJIT did not perform the desired unroll; the retained version improved the many-duplicate case by about 13%; and
- cyclic permutation exchanges misplaced pairs in blocks, reducing branch-predictor pressure on random and duplicate-heavy inputs.

This is a trade-off rather than a universal replacement for branchy PDQSort. The retained .NET 10 / Ryzen 9 7950X3D measurements showed branchless PDQSort faster for sufficiently large random and low-cardinality inputs, but slower at `n=256` and on reverse/pipe-organ inputs where branches are predictable. The crossover was approximately `n=512–1024` in that environment.

### Branch reduction and guarded loops

The repository also applies branch-sensitive structure where an algorithm justifies it:

- `BlockQuickSort` classifies elements into block index buffers to amortize unpredictable partition decisions;
- `Glidesort` contains branchless small sorting-network stages and conditional-index merge/partition paths;
- `InsertionSort` and `StdSort` use guarded/unguarded inner-loop variants where a sentinel or established boundary removes repeated boundary work; and
- PDQ variants use partial insertion-sort exits, already-partitioned detection, duplicate blocks, pattern-defeating shuffles, and heapsort fallback to avoid expensive work on favorable or adversarial inputs.

Branchless source syntax alone is not sufficient evidence. A ternary may still compile to a branch, and branch removal may increase instruction count or code size. The project validates relevant generated code and benchmarks multiple input patterns.

### Loop, recursion, and small-sort shaping

- Selected classification and copy loops are manually unrolled only when the JIT did not produce the desired shape and measurement justified it.
- Several quicksort and merge implementations use tail-recursion elimination or bounded explicit `stackalloc` work stacks to reduce call-stack growth and keep hot iteration local.
- Hybrid algorithms switch to insertion sort or sorting networks below algorithm-specific thresholds. Thresholds are benchmark decisions, not a package-wide constant; `IntroSort`, for example, records a measured threshold of 30.
- Cached pivot and element values reduce repeated observable reads and memory traffic.
- Distribution algorithms prefer shifts and masks for power-of-two radices and use compact count/offset buffers.

## Allocation And Memory Contract

- Hot element-processing loops do not perform per-element heap allocation.
- Bufferless algorithms remain bufferless apart from bounded stack or call metadata documented by the algorithm.
- Algorithms requiring generic auxiliary storage normally use `ArrayPool<T>`; pool misses and pool internals mean this is not a universal zero-allocation guarantee.
- Reference-containing rentals are returned with clearing where retention would be unsafe.
- `SkipLocalsInit` makes explicit initialization a correctness obligation. Histogram buffers that increment existing values are the highest-risk pattern and must be cleared before use.
- Generic `T` cannot generally be stack-allocated because it may contain references; metadata and generic element buffers use different strategies.

Allocation claims identify the algorithm, overload, element type, input size and pattern, runtime, and benchmark configuration.

## Benchmark And Disassembly Policy

Performance changes are evaluated in Release with BenchmarkDotNet. The repository harness:

- prepares fresh input buffers outside the measured region so a benchmark does not repeatedly sort an already-sorted array;
- pins invocation count to the prepared buffer count and uses `UnrollFactor=1`;
- disables tiered compilation for the short fixed job because tier transitions previously produced bimodal 10–15× iteration changes; and
- uses a fixed CI CPU model unless explicitly overridden, improving comparability across runs.

| Claim | Required evidence |
|---|---|
| Faster/slower or crossover point | BenchmarkDotNet across relevant sizes and patterns |
| Allocation reduction | `MemoryDiagnoser` allocated-byte results |
| Callback or generic branch removed | Disassembly of the closed `NullContext` specialization |
| `setcc`, `cmov`, bounds-check removal, unroll, instruction selection | `DisassemblyDiagnoser` output for the relevant hot method |
| Algorithmic complexity or stability | Implementation reasoning plus focused tests; microbenchmarks are insufficient |

A performance-sensitive change is compared with a meaningful baseline. Regressions require correction or an explicit documented trade-off. Benchmark noise and source-level intuition alone are not evidence.

### Evidence chain for retaining an optimization

Performance decisions follow the same reasoning chain used for the statistics counters and branchless PDQ partition:

1. state the expected bottleneck in CPU/JIT terms;
2. inspect closed-generic disassembly to confirm the expected costly or improved code shape;
3. compare before and after in the same benchmark run across relevant sizes and input classes;
4. verify that the result matches the hypothesis rather than an unrelated setup artifact;
5. run correctness, comparer, context, and allocation validation appropriate to the change; and
6. record the conditions where the choice wins, loses, and remains valid.

An optimization without this chain is a candidate, not an established project decision.

## Not Currently Adopted As Shared Infrastructure

The following are not repository-wide strategies: ISA-specific SIMD (`Vector128/256`, AVX, AdvSimd), portable `Vector<T>` sorting paths, `GC.AllocateUninitializedArray<T>`, `Unsafe.CopyBlock`, `CollectionsMarshal.AsSpan`, `Unsafe.BitCast`, and inline-array buffers. They remain outside the performance contract until correctness, portability, maintenance cost, generated code, and benchmarks justify adoption.

## Non-goals

- Identical machine code across .NET versions, JIT modes, architectures, or NativeAOT
- A universal zero-allocation promise for every algorithm and context
- Making `StatisticsContext` safe for concurrent shared mutation
- Removing Debug bounds checks
- Bypassing a caller's comparer
- Replacing a BCL primitive merely because a lower-level API exists
- Requiring branchless execution where the branch predictor is faster

## Lessons Learned

- Generic `TContext` is essential: an interface-typed context prevents the unobserved path from specializing cleanly.
- Atomic statistics counters made observation disproportionately expensive; per-sort ownership enabled plain increments without weakening the intended usage model.
- The atomic-counter result was not merely a benchmark correlation: three locked RMW operations per swap explained the observed 7.0–7.6× instrumented speedup, while unchanged counts and 6,711 passing tests established semantic equivalence for the supported single-threaded model.
- Fresh input per benchmark invocation matters. Re-sorting one array trains predictors and can hide the advantage of branchless partitioning.
- Bounds-check removal matters most for irregular or runtime-indexed access. Simple forward loops are often already optimized by the JIT.
- Manual unrolling is justified only after disassembly shows the JIT did not unroll and benchmarks show an input-relevant benefit.
- Branchless classification helps unpredictable data but can lose on small or predictable inputs because extra bookkeeping remains.
- `SkipLocalsInit` turns an implicit runtime guarantee into a repository-wide audit obligation; increment-before-initialize buffers are easy to miss.
- A primitive fast path needs a natural-comparer marker. `typeof(T)` alone would silently violate reverse or domain-specific ordering.
- Generated code is a property of the closed generic instantiation, build configuration, JIT, and CPU; documentation describes the measured context.
