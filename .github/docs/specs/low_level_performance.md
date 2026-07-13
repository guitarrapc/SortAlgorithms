# Low-Level Performance Contract

## Purpose

SortAlgorithms deliberately uses runtime-sensitive techniques where sorting hot paths benefit from concrete generic types, contiguous memory, and reduced allocation. These techniques are subordinate to correctness, comparer semantics, observation accuracy, and memory safety.

This document states the behavior and constraints the architecture preserves. Current implementation mechanisms and candidates live in [runtime_optimization.md](../references/runtime_optimization.md).

## Required Properties

### Semantic equivalence

Optimized and observable paths produce the same comparer-defined ordering. Release-only fast paths must not change valid-input behavior. Custom comparers always take precedence over natural-order primitive specialization.

### Build-configuration boundary

Debug access through `SortSpan` retains normal span bounds checks to expose invalid indices during development. Release access may use unchecked by-reference arithmetic after the algorithm has established valid indices. The absence of a bounds check is not permission for an algorithm to access outside its span.

### Natural-order specialization

When all of the following hold, the implementation may use direct primitive relational operators:

- execution uses `NullContext`;
- the comparer is the library's natural-order comparer; and
- `T` is one of the explicitly supported primitive types.

The currently supported set is `byte`, `sbyte`, `ushort`, `short`, `uint`, `int`, `ulong`, `long`, `nuint`, `nint`, `float`, `double`, and `Half`.

For floating-point types, direct relational operators treat NaN as unordered, unlike the total ordering exposed by `CompareTo`. Algorithms using the specialized boolean helpers must preserve their documented NaN behavior, including any required NaN handling outside the hot comparison loop.

### Observation boundary

Runtime specialization may eliminate observation work only for `NullContext`. A real context receives the logical callbacks required by [observation.md](observation.md), even when a faster unobserved operation exists.

### Temporary memory

Stack allocation is limited to bounded unmanaged or metadata buffers with a justified stack budget. Larger or generic-element temporary storage uses pooling where appropriate. Pooled arrays are returned on success and failure, and references are cleared when required to avoid retention.

### Uninitialized locals

The assembly skips implicit local initialization. Every stack buffer that is read before every element is overwritten must therefore be initialized explicitly. This is a correctness requirement, not merely an optimization preference.

### Evidence

Claims about eliminated dispatch, bounds checks, instructions, allocation, or speed require evidence appropriate to the claim: source/IL or disassembly for generated-code claims and BenchmarkDotNet results for throughput and allocation claims. Source structure alone describes optimization intent, not guaranteed machine code on every runtime and architecture.

## Portability

The library targets .NET rather than a single CPU ISA. Runtime intrinsics such as `BitOperations` are acceptable because the runtime supplies portable behavior and platform-specific lowering. ISA-specific SIMD is not part of the current architectural guarantee and requires a scalar or portable fallback before adoption.

## Non-goals

- Guaranteed identical machine code across .NET versions, JIT modes, CPUs, or NativeAOT
- A package-wide zero-allocation claim for every algorithm and context
- Removing safety checks from Debug builds
- Bypassing caller-provided comparison semantics
- Adopting a low-level API solely because it is lower level than the BCL equivalent

## Lessons Learned

- Simple forward span loops are often already optimized well; unchecked reference access showed its largest measured benefit on irregular or backward access, so it should remain evidence-driven.
- `SkipLocalsInit` converts an implicit runtime guarantee into a repository-wide audit obligation. Increment-before-initialize histogram buffers are especially easy to miss.
- Generic `T` cannot generally be placed in `stackalloc` storage because it may contain references. Pooling is the practical shared strategy for generic merge buffers.
- A primitive fast path needs an explicit natural-comparer marker. Testing only `typeof(T)` would incorrectly bypass reverse or domain-specific comparers.
- `Span.CopyTo` already expresses overlap-safe bulk movement and is runtime optimized; replacing it with `Unsafe.CopyBlock` has no inherent semantic or performance advantage.

