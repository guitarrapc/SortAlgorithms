# SortAlgorithms Specification

User-facing specification entry point for the **SortAlgorithm** library. Detailed contracts are split by behavior under [specs/](specs/). Implementation guidance and command recipes live under [references/](references/).

## Motivation

SortAlgorithms provides a broad set of sorting algorithms for comparison, education, visualization, and performance study without forcing those uses into separate implementations. A caller can use the low-overhead path for ordinary sorting or supply an observation context to inspect the same algorithm's operations.

The library favors explicit generic contracts because sorting performance depends on comparison dispatch, temporary storage, and observation overhead. These concerns must not change the sorted result.

## Implemented Scope

| Area | Contract |
|---|---|
| Component boundaries and execution modes | [Architecture](specs/architecture.md) |
| In-place sorting over `Span<T>` | [Sorting API](specs/sorting_api.md) |
| Natural-order and custom-comparer overloads | [Sorting API](specs/sorting_api.md) |
| Statistics and visualization callbacks | [Observation](specs/observation.md) |
| Allocation and performance expectations | [Performance](specs/performance.md) |
| Low-level runtime optimization boundaries | [Low-level performance](specs/low_level_performance.md) |
| Correctness and stability expectations | [Validation](specs/validation.md) |

The implemented algorithm catalog is maintained in [README.md](../../README.md#implemented-sort-algorithm).

## Out Of Scope

- A single automatic policy that selects the best algorithm for arbitrary data
- A universal stability guarantee across all algorithms
- A universal worst-case time or auxiliary-space guarantee across all algorithms
- Thread safety for a span or mutable context shared by concurrent sort operations
- Persisting or rendering visualization events in the core library

## Related Documents

- [specs/sorting_api.md](specs/sorting_api.md) — common sorting contract and overload families
- [specs/architecture.md](specs/architecture.md) — responsibilities, execution modes, and design decisions
- [specs/observation.md](specs/observation.md) — context events and buffer identity
- [specs/performance.md](specs/performance.md) — performance and allocation guarantees
- [specs/low_level_performance.md](specs/low_level_performance.md) — runtime-sensitive optimization contract
- [specs/validation.md](specs/validation.md) — required behavioral evidence
- [references/implementation_guidelines.md](references/implementation_guidelines.md) — contributor implementation patterns
- [references/runtime_optimization.md](references/runtime_optimization.md) — current low-level mechanisms and candidate techniques
- [references/testing.md](references/testing.md) — test commands and test-authoring recipes
- [references/sandbox.md](references/sandbox.md) — standalone experiment workflow
