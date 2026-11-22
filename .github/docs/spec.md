# SortAlgorithms Specification

This is the entry point to the specification of the **SortAlgorithm** library. [specs/](specs/) holds the detailed contracts, one file for each behavior. [references/](references/) holds implementation guidance and command recipes.

## Motivation

SortAlgorithms provides many sorting algorithms for comparison, education, visualization, and performance study. All of these uses share one implementation of each algorithm. A caller can use the low-overhead path for ordinary sorting, or supply an observation context to inspect the operations of the same algorithm.

The library prefers explicit generic contracts, because sorting performance depends on comparison dispatch, temporary storage, and observation overhead. These concerns must not change the sorted result.

## Implemented Scope

| Area | Contract |
|---|---|
| Component boundaries and execution modes | [Architecture](specs/architecture.md) |
| In-place sorting over `Span<T>` | [Sorting API](specs/sorting_api.md) |
| Natural-order and custom-comparer overloads | [Sorting API](specs/sorting_api.md) |
| Statistics and visualization callbacks | [Observation](specs/observation.md) |
| Performance guarantees, allocation, and required evidence | [Performance](specs/performance.md) |
| Published CI benchmark figures, size leagues, and the benchmark site | [Benchmark Results](specs/benchmark_results.md) |
| Correctness and stability expectations | [Validation](specs/validation.md) |

[README.md](../../README.md#implemented-sort-algorithm) lists the implemented algorithms.

## Out Of Scope

- One automatic policy that selects the best algorithm for any data
- A stability guarantee that covers all algorithms
- A worst-case time or auxiliary-space guarantee that covers all algorithms
- Thread safety for a span or a mutable context that concurrent sort operations share
- Storage or rendering of visualization events in the core library

## Related Documents

- [specs/sorting_api.md](specs/sorting_api.md): common sorting contract and overload families
- [specs/architecture.md](specs/architecture.md): responsibilities, execution modes, and design decisions
- [specs/observation.md](specs/observation.md): context events and buffer identity
- [specs/performance.md](specs/performance.md): performance guarantees, required evidence, and vector-code policy
- [specs/validation.md](specs/validation.md): required behavioral evidence
- [specs/benchmark_results.md](specs/benchmark_results.md): what the README and the benchmark site publish, and how each figure is defined
- [references/benchmark_site.md](references/benchmark_site.md): benchmark data flow, file formats, and local preview
- [references/implementation_guidelines.md](references/implementation_guidelines.md): implementation patterns for contributors
- [references/sandbox.md](references/sandbox.md): workflow for standalone experiments
