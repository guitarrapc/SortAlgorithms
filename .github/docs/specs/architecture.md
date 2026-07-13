# Architecture

## Purpose

SortAlgorithms uses one algorithm implementation for two execution modes:

1. low-overhead sorting without observation; and
2. sorting with operation events for statistics, visualization, or tutorials.

The sorted result and comparer semantics are shared. Observation is an optional concern and must not require a second copy of each algorithm.

## Responsibilities

| Component | Responsibility | Ownership and state |
|---|---|---|
| Static algorithm type | Select and perform a named sorting algorithm | No per-call state retained after `Sort` returns |
| Caller-owned `Span<T>` | Input and in-place output | Owned by the caller |
| `TComparer` | Define element order | Supplied or selected by the overload |
| `TContext` / `ISortContext` | Observe logical sorting operations | Supplied and owned by the caller |
| `SortSpan<T, TComparer, TContext>` | Couple span access, comparison, buffer identity, absolute offset, and observation | Internal stack-only value scoped to the sort |
| Auxiliary storage | Support algorithms that are not bufferless | Scoped to the call and released before return |

Algorithm types are static and retain no mutable sort state. Contexts may be mutable. Concurrent calls are independent when they operate on distinct spans and do not share a non-thread-safe mutable context.

## Execution Modes

### Unobserved mode

Convenience overloads use `NullContext`, a no-op value type. The implementation is structured so runtime specialization can remove observation branches from hot paths. This is the normal benchmark and production mode.

### Observed mode

The caller supplies a context type implementing `ISortContext`. The algorithm reports reads, writes, comparisons, swaps, range copies, phases, roles, and buffer identities as applicable. `StatisticsContext`, `VisualizationContext`, and `CompositeContext` are the built-in consumers.

Observed mode provides semantic operation data, not a promise of identical callback sequences across algorithms or releases. See [observation.md](observation.md).

## Public API Shape

Each algorithm is a static class under `SortAlgorithm.Algorithms`. The common overload family supports natural ordering, natural ordering with a context, and a custom comparer with a context. Some algorithms add range, seed, selector, or variant-specific parameters. See [sorting_api.md](sorting_api.md).

The natural-order path requires `T : IComparable<T>` and uses the internal `ComparableComparer<T>`. A custom comparer is never bypassed by primitive-specialized comparisons.

## Internal Data Model

`SortSpan<T, TComparer, TContext>` is an internal `readonly ref struct`. It represents:

- the current span;
- its first-element reference for Release fast paths;
- the comparer and context values;
- a buffer identifier; and
- an absolute offset used in reported indices.

Slices preserve the logical relationship to the original buffer through offsets. Auxiliary spans receive distinct buffer identifiers so equal numeric indices in different buffers remain distinguishable.

This type is an internal implementation boundary, not a public extension point. Its detailed methods belong in [implementation_guidelines.md](../references/implementation_guidelines.md).

## Design Decisions

### Static algorithms instead of stateful sorter instances

Sorting mutates caller-provided data but does not require an object with retained state. Keeping statistics and visualization outside the algorithm prevents hidden lifetime and reuse semantics and makes concurrent independent calls straightforward.

### Generic comparer and context types

Using `TComparer` and `TContext` throughout the call chain gives the runtime concrete types to specialize. Passing either as its interface inside hot helpers would restore dispatch and make the unobserved path harder to optimize.

### `SortSpan` instead of context-owned mutation

The context observes; it does not own or mutate the input. `SortSpan` centralizes the correspondence between a logical operation and its callback while keeping ordering behavior in the algorithm and comparer.

### Structured events instead of display strings

Phases and roles are domain values. Formatting, animation, and localization belong to consumers rather than algorithms.

## Non-goals

- Exposing `SortSpan` as public API
- Making mutable contexts implicitly safe for shared concurrent use
- Guaranteeing the same event sequence for different algorithms
- Requiring every algorithm to use the same auxiliary-storage strategy
- Hiding algorithm-specific properties such as stability or worst-case complexity

## Lessons Learned

- A class-only context design leaves interface dispatch in the no-observation path. A value-type `NullContext` plus generic `TContext` better represents the desired execution split.
- Index-only callbacks are insufficient once an algorithm uses temporary buffers; buffer identity and slice offsets are required for faithful visualization.
- Comparisons involving cached values need external-value markers because one or both operands may not have a meaningful span index.
- Observation must remain coupled to logical operations. Direct span or comparer access can silently produce correct output but incorrect statistics and visualization.
- A `ref struct` cleanly scopes span access but cannot cross async, iterator, boxing, or heap-capture boundaries; sorting remains synchronous by design.

