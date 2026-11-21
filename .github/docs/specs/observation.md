# Operation Observation

## Purpose

Statistics, visualization, and tutorial consumers use the same sorting implementation through `ISortContext`. Observation describes what the algorithm does. It must not change the sorted result. `NullContext` means no observation.

## Events

An observable algorithm reports these operations to the context:

- comparisons, with the location and buffer identity of both operands
- swaps, reads, writes, and range copies
- the edges of a pointer-based tree, if the algorithm builds one
- structured phases and semantic element roles, if the algorithm provides them

Buffer `0` is the main span of the caller. A positive identifier marks an auxiliary buffer of the sort. `-1` can mark external storage where the context contract allows it. An identifier is a label in one observation stream, not a globally unique handle.

Each reported value and index describes the logical operation at the time of the callback. Do not omit an observable operation to make the statistics smaller.

Callbacks that carry element values are generic in the element type.

- A write has two forms. One form carries the written value. The other form carries no value, for writes whose value is not observable, such as a tree pointer.
- A range copy also has two forms. The second form gives the values as a `ReadOnlySpan<T>` over the source range. This `ReadOnlySpan<T>` is valid only during the call. A context that keeps the values must copy them.

Reported indices are absolute. An element operation on a slice reports the slice offset plus the local index, with the buffer identifier of the slice. A phase or role parameter that names an element uses the same coordinates. Do not shift a parameter that holds a count, such as the step and the total of `HeapExtract`.

A comparison reports the index of each operand that has one. Report `-1` for an operand that is in no buffer. Examples are a cached pivot, a running lane minimum, and the result of a conditional move. No accessor accepts a claim from the caller about the source of a held value. The library cannot check such a claim for an arbitrary element type, so `-1` is the correct report.

## Tree Links

A tree-building algorithm reports each write to a child pointer as a link. The link gives the parent, the child, and the child slot.

- Parent `-1` means that the child became the root.
- Child `-1` means that the slot became empty.

The algorithm also reports the same write as a write without a value, at the same call site. A context that only counts operations therefore does not count links. The library reports links because index operations do not show the shape of a tree. It does not report parent pointers, heights, colors, or priorities. The links already give these values, or the shape does not depend on them.

Links describe only shape:

- The in-order traversal of a reported tree is the sorted output only for a search tree. The sorted sequence is what the algorithm writes to buffer `0`.
- A multiway node uses the same two slots. The left slot of a key names the child before the key. The right slot names the next key in the same node. For the last key, the right slot names the child after the key. An algorithm with a node capacity stores its keys in blocks of that width in its own buffer. It announces the width as a phase parameter. A slot in that buffer holds the value of its last write. Get node identity from the block arithmetic, not from the first write to a slot.
- A forest uses the left-child / right-sibling encoding, which makes a forest a tree. A root link then names the head of that encoding. A node with no incoming link is normal, because a priority queue retires each element that it returns. The root can be `-1` while the structure is a set of subtrees with no root. `PairingHeapSort` does this after it removes its root and before it folds the children of that root back together.

The links that a consumer received describe the current structure. The number of roots, and whether each created node is still in the structure, depend on the algorithm.

## Vector Operations

An algorithm reports a vector instruction lane by lane, in index order:

- A vector load is one read per lane.
- A vector comparison is one comparison per lane, with the result of that lane.
- A lane operand in a register is `-1`.
- A vector store is one write per lane that it writes, with the value that it left there. The algorithm does not write or report a lane that the mask excludes.

Vectors that spill to a stack array between steps are not an element buffer. The algorithm does not report them.

The algorithm selects the lane count at run time from the vector widths that the machine accelerates. An observed stream of the same input can therefore have different reads and comparisons on different machines. The order of lanes that execute together is presentation, not a property of the sort.

## Auxiliary Buffers

- An auxiliary buffer that stores elements follows the same observation contract as the input. A write or a range copy initializes each element before a read uses it.
- A buffer identifier names storage, not a role in that storage. It stays on the same storage for the whole sort, also when source and destination roles alternate. Offsets separate the ranges of one buffer. Do not give a range its own identifier.
- An auxiliary element buffer holds the sorted element type. Keep slot metadata, such as occupancy, tombstones, or bucket tags, next to the buffer, not in a wrapper element type.
- Index arrays, offset buffers, and occupancy bitsets are algorithm metadata, not element storage. If an algorithm reports operations on metadata, it uses a separate identifier, and each write carries the stored index or offset.
- An element copy adds its full length to the read count and to the write count. This includes bootstrap copies, snapshot storage, and staging operations when the algorithm performs them.

## Phase Levels

A phase announcement replaces the previous phase at the same level. There is no exit event. A phase stays current until the next announcement at its level. There are two levels, and a consumer keeps one slot for each level:

- A **scope** phase is a decision of the announcing algorithm. A decision to delegate a subrange to the core of another algorithm is a scope phase.
- A **detail** phase is progress inside a delegated core.

`SortPhase.IsDetailPhase` classifies each phase. A scope announcement clears the detail slot, because the delegated work has ended. A detail announcement does not change the scope slot.

A core that announces detail phases never delegates further. A delegated core that announces scope phases replaces the handoff phase in the scope slot, and its own delegation goes to the detail slot. `StdSort` under `SkaSort` is an example. Two slots therefore describe every sequence that this library emits. An algorithm whose own phases are all detail phases can also run at the top level, without a scope phase.

A hybrid that gives a subrange to another algorithm announces the handoff, with the delegated range, before the delegated core runs. The phase names the real target. For example, `IpnsortSmallSort` exists because the small sort of ipnsort is not an insertion sort, so `HybridToInsertionSort` would be the wrong name.

## Context Implementations

- A new callback in the contract has a no-op default implementation, so existing implementations still compile. A value-type context should override the callback explicitly. A constrained call that falls through to a default interface implementation boxes the receiver. On the `NullContext` path, that would add an allocation.
- A context must handle the event volume of the selected algorithm.
- A mutable context is not safe for concurrent use by default.
- If a context throws, the sort stops and can leave the span partially sorted.

## Consumer Responsibilities

A consumer must allow for these facts:

- Two different algorithms can give different event sequences for the same input.
- An algorithm can change its auxiliary buffers between releases. The buffer set is an implementation choice. The contract guarantees the phases and the span of the caller (buffer `0`), so key on those.
- A handoff phase can stop being current before the delegated range is done. `SkaSort` gives short ranges to `StdSort`. The first scope phase that `StdSort` announces replaces `SkaSortStdSort`.
- A `RadixPass` is not always followed by `DistributionBucket` reports. `RotateLSDRadixSort` and `Radsort` report none, because no bucket has a known contiguous span during their passes.
- Between passes, the buffers do not always hold the elements in their current order. `Radsort` keeps that order in a block permutation. The span becomes ordered only during the final `DistributionWrite` phase.
- A written value does not always stay in place until the algorithm moves it on purpose. The write-both merge and partition steps of `QuadSort`, `FluxSort`, `CrumSort`, and `BlitSort` write both candidates under every context. The algorithm then overwrites the candidate that it did not take, or leaves it unused. A buffer can briefly show a duplicate of an element that is also in another position. The final value of a position is its last write before the step ends.
- A walk of a left-child / right-sibling root list does not always give the current roots. `BinomialHeapSort` relinks its root list one root at a time, so a walk during a merge sees only the relinked prefix. A node is a root exactly when it is not the child of any node. Get root membership from child links.
