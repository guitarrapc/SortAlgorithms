# Performance Specification

## Purpose

SortAlgorithms is both an algorithm study library and a performance-oriented implementation. Performance is part of the design contract: hot paths are shaped for the .NET JIT, memory traffic and allocation are controlled, and algorithm variants may deliberately trade code size or predictable-input speed for lower branch-misprediction cost.

Correctness, comparer semantics, observation accuracy, and memory safety take precedence over speed. An optimization is retained only when its applicable input class and trade-offs are understood.

## Runtime Scope

Generated code depends on the runtime, JIT, build configuration, element type, comparer, context, and CPU. Runtime upgrades require revalidation of claims that depend on instruction selection or crossover points. Measurements and implementation decisions are kept in [reports](../reports/README.md), separately from this contract.

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


### Statistics ownership

Sorting is synchronous. A StatisticsContext has one owner during a sort and does not support concurrent mutation. Atomic updates provide no additional correctness under this ownership model. Adding internal parallelism must not silently reuse the current context.

One logical swap updates the swap count and accounts for two reads and two writes. Counter implementation changes must preserve those values. Historical atomic-counter measurements are retained in the [performance evidence record](../reports/performance-evidence-before-f03-2026-11-15.md).

## Implemented Runtime-Level Optimizations

| Area | Adopted design | Why it exists | Constraint |
|---|---|---|---|
| Contiguous input | Algorithms sort `Span<T>` through an internal `readonly ref struct` | Avoid ownership and slicing allocations; expose contiguous storage | `SortSpan` remains synchronous and stack-only |
| Comparer dispatch | `TComparer : IComparer<T>` is propagated as a concrete generic type | Permit value-type comparer specialization and avoid mandatory boxing/interface dispatch | Hot helpers must not erase it to `IComparer<T>` |
| Observation dispatch | `TContext : ISortContext` and value-type `NullContext` propagate through the full call chain | Allow the JIT to remove unobserved callback branches | A real context must receive every logical event |
| Primitive comparisons | Natural ordering specializes 13 primitive types through `typeof(T)` and `Unsafe.As` | Produce boolean comparisons; floating-point predicates include NaN ordering | Enabled only for `IComparableComparer`; custom comparers are never bypassed |
| Release element access | `MemoryMarshal.GetReference` and `Unsafe.Add(ref, (nint)(uint)index)` back `SortSpan` access | Avoid repeated bounds checks and the sign-extension observed with an `int` index | Algorithm logic must establish valid indices |
| Comparer-dependent cursors | In `QuadSort`, `FluxSort` and the sorts under `Algorithms/Partition`, a cursor or a write that only a consistent comparer keeps inside its range is also checked against the range. In the partition sorts, the checks inside loops drop out when `SortSpan.UsesNaturalPrimitiveOrder` holds | A comparer that is not a consistent total order cannot move unchecked access out of the buffer, and the natural primitive order, which cannot trip the checks, does not pay for them | The flag holds only for the internal natural comparer on the primitive set |
| Debug element access | Normal `Span<T>` indexing remains in Debug | Detect out-of-range algorithm defects during development | Debug and Release remain equivalent for valid input |
| Inlining | Small access, comparison, comparer, context, and utility methods use `AggressiveInlining` | Expose constants and generic types to callers and reduce hot call overhead when accepted by the JIT | Inlining is verified, not assumed |
| Local initialization | Module-level `SkipLocalsInit` | Avoid implicit clearing of locals and stack buffers that will be overwritten | Buffers read before overwrite explicitly `Clear` or `Fill` |
| Small temporary storage | Bounded metadata, count, recursion, and offset buffers use `stackalloc` | Avoid heap traffic for fixed or tightly bounded work memory | Review maximum byte count and stack depth per site |
| Large/generic storage | Algorithms rent from `ArrayPool<T>.Shared` | Reuse merge, tree, distribution, and work buffers | Return from `finally`; clear reference-containing arrays when required |
| Bit operations | `BitOperations.Log2` and `LeadingZeroCount` replace manual bit loops | Permit efficient platform lowering | Exact instructions depend on runtime and CPU |
| Bulk movement | `Span.CopyTo` performs overlap-safe range movement | Use the BCL's optimized generic copy rather than hand-written loops | Observable copies also account for logical reads/writes |
| Hot/cold shaping | Small hot helpers use `AggressiveInlining`; `IntroSortDotnet` marks its recursive core `NoInlining` | Balance specialization opportunities against caller code size | Applied locally from measurement or structural need |

The primitive set is `byte`, `sbyte`, `ushort`, `short`, `uint`, `int`, `ulong`, `long`, `nuint`, `nint`, `float`, `double`, and `Half`. Floating-point predicates preserve comparer ordering: NaN precedes numbers, NaNs compare equal, and signed zeros compare equal. Direct relational operators alone do not meet this contract.

InsertionSort prepares the NaN state of a cached key only after a shift is required, and reuses that state for the remaining comparisons. Already-ordered elements retain a short path. Other comparers and observing contexts retain their comparison results and event sequence. The [follow-up measurements](../reports/nan-comparison-optimization-2026-11-14.md) record the evidence; no prescan or different sorting algorithm is introduced.

### Why unchecked by-reference access is selective

RyuJIT already removes many bounds checks from simple forward loops through range analysis. Replacing every span access with `Unsafe` would therefore add risk without a guaranteed benefit. The retained `SortSpan` Release path was selected because one centralized implementation covers the irregular, backward, and runtime-indexed accesses where range proof is weakest, while Debug retains checked indexing.

The validation of unchecked access must cover irregular indices as well as sequential scans. Historical access-loop measurements and address-generation observations are kept in the [performance evidence record](../reports/performance-evidence-before-f03-2026-11-15.md).

The same validation rule applies to offset buffers in `PDQSortBranchless`: their store index is data-dependent, so each unchecked write needs a valid range established by the algorithm.

## Implemented Algorithm-Level Optimizations

### Branchless PDQSort partition

`PDQSortBranchless` is a scalar block-partition implementation, not an unimplemented SIMD proposal. It preserves PDQSort's pivot selection, duplicate handling, pattern detection, heapsort fallback, and tail-recursion elimination while replacing the data-dependent partition branch with two 64-byte offset buffers.

The partition preserves the following structure:

- boolean classification is converted with `Unsafe.As<bool, byte>` and accumulated into offset counts;
- runtime-indexed offset-buffer writes use first-element refs plus `Unsafe.Add` within the buffer bounds;
- classification is manually unrolled eight times; and
- cyclic permutation exchanges misplaced pairs in blocks, reducing branch-predictor pressure on random and duplicate-heavy inputs.

The extra classification bookkeeping makes this a trade-off rather than a universal replacement for branchy PDQSort. Input predictability, size, runtime, and CPU affect the result. Measurements and crossover observations are kept in the [performance evidence record](../reports/performance-evidence-before-f03-2026-11-15.md).

### Branch reduction and guarded loops

The repository also applies branch-sensitive structure where an algorithm justifies it:

- `BlockQuickSort` classifies elements into block index buffers. Unconditional offset stores are confined to the `NullContext` path, because an observing context must not be told about index-buffer writes the algorithm did not logically perform;
- `Glidesort` contains branchless small sorting-network stages; its bidirectional stable partition uses a write-both kernel (unconditional stores to both candidate slots, the reference implementation's technique) and an AVX-512 compress kernel under `NullContext`, because RyuJIT compiles conditional-destination writes to data-dependent branches rather than cmov;
- `QuadSort` and `FluxSort` port quadsort's and fluxsort's branchless merge and partition steps in the reference's gcc form, which writes both candidates and advances the cursors arithmetically. The clang form selects the value to write, and RyuJIT compiled that selection inside the merge loop to a data-dependent branch (verified by disassembly), which made random input measurably slower. Unlike Glidesort the write-both form runs under every context, so observed counts describe the code the unobserved path runs;
- `InsertionSort` and `StdSort` use guarded/unguarded inner-loop variants where a sentinel or established boundary removes repeated boundary work;
- `StdSort` uses a natural-order primitive-type gate for 64-element bitset partitions and small conditional-move networks. Bitsets are local values. The gate does not depend on `TContext`, so observed statistics describe the same algorithm the fast path runs; classification state never touches an observable buffer; and
- PDQ variants use partial insertion-sort exits, already-partitioned detection, duplicate blocks, pattern-defeating shuffles, and heapsort fallback to avoid expensive work on favorable or adversarial inputs.

Branchless source syntax alone is not sufficient evidence. A ternary may still compile to a branch, and branch removal may increase instruction count or code size. The project validates relevant generated code and benchmarks multiple input patterns.

### Loop, recursion, and small-sort shaping

- Selected classification and copy loops are manually unrolled only when the JIT did not produce the desired shape and measurement justified it.
- Several quicksort and merge implementations use tail-recursion elimination or bounded explicit `stackalloc` work stacks to reduce call-stack growth and keep hot iteration local.
- Hybrid algorithms switch to insertion sort or sorting networks below algorithm-specific thresholds. Thresholds are local implementation decisions and do not define a package-wide constant.
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
- generates each of an iteration's input buffers from its own seed, so no invocation of a seeded pattern sorts an input another invocation has already sorted (patterns that do not depend on a seed, such as sorted or pipe organ, are the same in every buffer by definition);
- pins invocation count to the prepared buffer count and uses `UnrollFactor=1`;
- records explicit tiering settings so runtime transitions do not silently change the measurement conditions; and
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

One local exception: `QuadSort` holds the 32-element stack array that quadsort's definition merges through (`VAR swap[32]` in the reference) in an `[InlineArray(32)]` struct of `T`, because `stackalloc` cannot hold an arbitrary generic `T`. The runtime tracks reference fields of an inline array, so the buffer is safe for reference-containing `T`; tests cover strings and records. It is the algorithm's own fixed-size stack storage, not shared infrastructure, and other algorithms do not reuse it.

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
- Fresh input per benchmark invocation matters. Re-sorting one array trains predictors and can hide the advantage of branchless partitioning.
- A fresh copy of the same input is not fresh input. Until 2026-11-18 the harness copied one source array into every buffer. The predictor learned it across an iteration's 64 invocations, so branchy sorts on random data looked several times faster up to 4,096 elements (`PDQSort`: 29 µs at 4,096 and 202 µs at 8,192; with a seed per buffer, 108 µs and 234 µs). Watch for a jump between adjacent sizes far larger than n log n.
- Seed-independent patterns keep that bias, and pipe organ keeps it by choice. Measured on 2026-11-18 at 1,024 and 8,192 elements, branchy sorts ran faster on it than with an untrained predictor: `PDQSort` 1.7–2.3×, `PDQSortBranchless` 1.3–1.7×, `Span.Sort` 1.1–1.3×. Shifting the peak by ±32 removed most of the bias, but it also made `Span.Sort` 10–18% faster with an untrained predictor. The centered peak is what makes pipe organ a median-of-three worst case.
- Bounds-check removal matters most for irregular or runtime-indexed access. Simple forward loops are often already optimized by the JIT.
- Manual unrolling is justified only after disassembly shows the JIT did not unroll and benchmarks show an input-relevant benefit.
- Branchless classification helps unpredictable data but can lose on small or predictable inputs because extra bookkeeping remains.
- `SkipLocalsInit` turns an implicit runtime guarantee into a repository-wide audit obligation; increment-before-initialize buffers are easy to miss.
- A primitive fast path needs a natural-comparer marker. `typeof(T)` alone would silently violate reverse or domain-specific ordering.
- Generated code is a property of the closed generic instantiation, build configuration, JIT, and CPU; documentation describes the measured context.
- Preparing an invariant floating-point key can remove repeated NaN checks, but the measured element types and calling context determine whether it helps. Shared fallbacks need caller measurements as well as direct sort measurements. See the [2026-11-14 optimization record](../reports/prepared-floating-key-optimization-2026-11-14.md).
- Generic comparison policies can retain a common source loop. Helper boundaries and all shared callers still require correctness and performance validation.
- Type conditions that specialize to constants can still change inlining and code layout. Validate the generated code and all affected callers.
- Floating-point and integer measurements do not cover wider value types. Validate large structs separately before sharing a hot path.
- Before concluding two implementations have equivalent performance, include inputs that exercise their different work and memory traffic.
- Storing what a position already determines is a cost, not a safeguard. See the distribution auxiliary structure rule in `sorting_api.md`.
- A value select that RyuJIT turns into `cmov` outside a loop can still become a branch inside one. QuadSort's merge step was written as `dst = gt ? r : l`; outside the loop it compiled to `cmov`, inside the merge loop to `test`/`je`, and random input paid for the mispredictions (QuadSort at n = 100,000 random `int` went from 4.6 ms to 1.9 ms once the step wrote both candidates). Check the loop body, not the first unrolled step.
- A port that reproduces a reference's comparison counts exactly on identical inputs is the strongest fidelity evidence available for an adaptive sort, whose branching depends on the data. QuadSort and FluxSort were checked that way against the C sources, with the reference's address-derived sample offset replaced by the port's deterministic one. `ScandumReferenceTraceTests` keeps the check in the suite and compares the order of the compared pairs as well as their number, because sorted output cannot reveal a changed threshold or a skipped route.
- A port inherits the reference's trust in the comparer, but not the cost of breaking it. quadsort's cross merge and seven-element parity swap rely on earlier comparisons to keep their cursors inside the runs. So do the partition scans of the quicksort family and the unguarded insertion sorts that run on ranges with a sentinel before them, and the stable partitions that compare each element twice rely on the second pass repeating the first. With a comparer that is not a consistent total order, unchecked Release access ended the process with `AccessViolationException` in `PDQSort` and `IntroSort`, read outside the range in seven other partition sorts, copied memory from outside the span into `QuadSort`'s output, corrupted the heap through `StableQuickSort`'s scratch buffer, and left `BidirectionalStableQuickSort` partitioning without progress. These sorts now check those cursors and writes and throw `InvalidOperationException`. The checks compare indices and never elements, so comparison order is unchanged.
- Matching counts do not prove a partition holds the right elements. `BidirectionalStableQuickSort`'s second pass can fill its gap with exactly as many elements as the first pass skipped and still pick other ones, so a broken comparer can repeat some elements and lose others. Only recording the first pass's answers would prevent it, which the algorithm does not do, so the sort documents the loss instead. `StableQuickSort` writes every element exactly once in its second pass and keeps its elements.
- A cursor check inside a scan loop costs a compare and a branch for every element, and unrolling cannot remove it from the short scans of random input. PDQSort was timed under tiered compilation with the old and new code sorting the same inputs in one process (2026-11-19, 1,000 to 1,000,000 elements, median of per-input time ratios). With checks for every comparer, natural `int` sorts were 5–6% slower on random input and 9–15% slower on pipe-organ input. The natural order of a primitive type cannot trip the checks, so `SortSpan.UsesNaturalPrimitiveOrder` lets the JIT drop them there. Natural `int` and `double` sorts then stayed within the noise of an A/A control, in which two identical copies of the old code differed by up to 13% on sorted `double`. A custom struct comparer on `int` keeps the checks and was 3–5% slower on random input and 8–20% slower on pipe-organ input. IntroSort's new check is in its insertion sort, and it stayed within 5% with that comparer. The same timing of the nine other partition sorts that gained checks (1,000 to 100,000 elements, pipe-organ input only up to 10,000) kept natural `int` sorts between 0.83 and 1.07 times the old time, within that A/A noise. With the custom struct comparer, random input was up to 5% slower. Long scans cost the most: `QuickSort` on pipe-organ input took up to 1.31 times the old time and `IntroSortDotnet` on sorted input up to 1.27 times.
- With `[MethodImpl(MethodImplOptions.NoInlining)]` on the throw helper, the JIT kept the helper's call inside PDQSort's scan loop. The loop then took two branches per element and saved more registers. With `DOTNET_TieredCompilation=0`, long scans took up to 1.38 times the old time. A plain `[DoesNotReturn]` helper let the JIT move the call to cold code after the method, which brought the worst case to 1.13 times.
- A guard cell catches a cursor that leaves a sliced span only on the high side. `SortSpan` widens the index as unsigned, so index -1 addresses about four billion elements past the start and faults before any guard cell is read. The safety tests therefore sort a range inside a larger span through the range overloads, which keeps guard cells on both sides within the span.
- Recursion depth can match a reference while stack use does not, because a generic frame grows with the element. `FluxSort` recurses as deep as fluxsort, but tier-1 inlining put copies of 1 KiB elements into its partition frame, tens of kilobytes per level, and an input built to skew every partition overflowed the default stack at 1,000,000 elements with a valid comparer. Keeping element work out of the recursive method brought that frame to about 1 KB or less.

## Compiler-Specific Reporting Helper Inlining

The four observed-comparison reporting helpers in `SortSpan` request aggressive inlining only when compiled with `NETWASM`. NetWasm SDK 0.13.0 supplies that symbol automatically. Standard .NET builds retain the compiler's existing inlining heuristics. The condition changes method implementation hints, not comparisons, callback ordering, buffer identities, or bounds checks.

Compiler-specific measurements and rejected alternatives are retained in the [performance evidence record](../reports/performance-evidence-before-f03-2026-11-15.md). They do not promise the same behavior on another runtime or compiler.

A consumer must build the library sources with the NetWasm SDK to obtain these hints; consuming an ordinary precompiled .NET assembly does not retroactively apply the condition. Do not define `NETWASM` for the standard .NET browser runtime merely to force these hints. Revalidate this compiler-specific decision when upgrading the runtime or changing the call/recording path.
