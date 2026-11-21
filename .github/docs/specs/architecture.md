# Architecture

## Purpose

SortAlgorithms uses one implementation of each algorithm for two execution modes:

1. sorting without observation, with low overhead
2. sorting with operation events for statistics, visualization, or tutorials

Both modes give the same sorted result and use the same comparer semantics. Observation is optional. It must not require a second copy of each algorithm.

## Responsibilities

| Component | Responsibility | Ownership and state |
|---|---|---|
| Static algorithm type | Select and run a named sorting algorithm | Keeps no per-call state after `Sort` returns |
| Caller-owned `Span<T>` | Input and in-place output | The caller owns it |
| `TComparer` | Define the element order | The caller supplies it, or the overload selects it |
| `TContext` / `ISortContext` | Observe logical sorting operations | The caller supplies and owns it |
| `SortSpan<T, TComparer, TContext>` | Connect span access, comparison, buffer identity, absolute offset, and observation | Internal stack-only value that exists only during the sort |
| Auxiliary storage | Support algorithms that are not bufferless | Exists only during the call. The algorithm releases it before it returns. |

Algorithm types are static and keep no mutable sort state. Contexts can be mutable. Concurrent calls are independent when they use different spans and do not share a mutable context that is not thread-safe.

## Execution Modes

### Unobserved mode

The convenience overloads use `NullContext`, a no-op value type. The implementation lets runtime specialization remove observation branches from hot paths. This is the normal mode for benchmarks and production.

### Observed mode

The caller supplies a context type that implements `ISortContext`. The algorithm reports reads, writes, comparisons, swaps, range copies, phases, roles, and buffer identities where they apply. The built-in consumers are `StatisticsContext`, `VisualizationContext`, and `CompositeContext`.

Observed mode gives semantic operation data. It does not promise the same callback sequence across algorithms or releases. See [observation.md](observation.md).

## Public API Shape

Each algorithm is a static class in `SortAlgorithm.Algorithms`. The common overload family has three forms: natural ordering, natural ordering with a context, and a custom comparer with a context. Some algorithms add range, seed, selector, or variant-specific parameters. See [sorting_api.md](sorting_api.md).

The natural-order path requires `T : IComparable<T>` and uses the internal `ComparableComparer<T>`. Primitive-specialized comparisons never bypass a custom comparer.

## Internal Data Model

`SortSpan<T, TComparer, TContext>` is an internal `readonly ref struct`. It holds:

- the current span
- a reference to the first element, for Release fast paths
- the comparer and context values
- a buffer identifier
- an absolute offset for reported indices

A slice keeps its position in the original buffer through its offset. Each auxiliary span gets a different buffer identifier, so a consumer can tell apart equal indices in different buffers.

This type is an internal implementation boundary, not a public extension point. [implementation_guidelines.md](../references/implementation_guidelines.md) describes how to use its methods.

## Design Decisions

### Static algorithms instead of stateful sorter instances

Sorting changes the data of the caller, but it does not need an object that keeps state. Statistics and visualization stay outside the algorithm. This prevents hidden lifetime and reuse rules, and it makes independent concurrent calls simple.

### Generic comparer and context types

`TComparer` and `TContext` stay generic through the whole call chain, so the runtime has concrete types to specialize. If a hot helper takes either type as its interface, interface dispatch returns and the unobserved path is harder to optimize.

### `SortSpan` instead of context-owned mutation

The context observes. It does not own or change the input. `SortSpan` keeps each logical operation and its callback together in one place. The algorithm and the comparer still define the order.

### Structured events instead of display strings

Phases and roles are domain values. Formatting, animation, and localization belong to the consumer, not to the algorithm.

## Non-goals

- `SortSpan` as public API
- Implicit thread safety for a mutable context that concurrent calls share
- The same event sequence for different algorithms
- One auxiliary-storage strategy for every algorithm
- Hiding algorithm-specific properties such as stability or worst-case complexity

## Lessons Learned

- A context design that uses only classes leaves interface dispatch in the path without observation. A value-type `NullContext` with a generic `TContext` separates the two modes better.
- Callbacks with only an index are not sufficient when an algorithm uses temporary buffers. Faithful visualization needs buffer identity and slice offsets.
- A comparison with a cached value needs a marker for an external value, because one or both operands may have no meaningful span index.
- Observation must stay coupled to logical operations. Direct access to the span or the comparer can give correct output with incorrect statistics and visualization.
- A `ref struct` keeps span access scoped, but it cannot cross async, iterator, boxing, or heap-capture boundaries. Sorting stays synchronous by design.
