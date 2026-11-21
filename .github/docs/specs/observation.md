# Operation Observation

## Purpose

The same sorting implementation supports statistics, visualization, and tutorial consumers through `ISortContext`. Observation describes algorithm activity; it must not determine the sorted result. `NullContext` is the no-observation choice.

## Events

An observable algorithm reports the operations it performs through the supplied context:

- comparisons, including both operand locations and their buffer identities;
- swaps, reads, writes, and range copies;
- the edges of a pointer-based tree, when the algorithm builds one;
- structured algorithm phases and semantic element roles when the algorithm provides them.

Buffer `0` identifies the caller's main span. Positive identifiers distinguish auxiliary buffers within an operation. `-1` may identify external storage where the context contract permits it. Identifiers are semantic labels for an observation stream, not globally unique handles.

Values and indices reported to a context describe the algorithm's logical operation at callback time. Implementations must not omit observable operations merely to make statistics look smaller.

Callbacks that carry element values are generic in the element type. Writes come in two forms, one that carries the written value and one that does not, for writes whose value is not observable such as a tree pointer. Range copies likewise come in two forms, the second delivering the values as a `ReadOnlySpan<T>` over the source range. The span is valid only for the duration of the call, so a context that retains values must copy them out.

Reported indices are absolute: an element operation on a slice reports the slice's offset plus the local index, against the slice's buffer. A phase or role parameter that names an element uses the same coordinates. A parameter that carries a count, such as `HeapExtract`'s step and total, is not shifted.

A comparison reports the index of every operand that has one. An operand held outside any buffer, such as a cached pivot, a running lane minimum, or a value produced by a conditional move, is reported as `-1`. No accessor accepts a caller's claim about where a held value came from, because the claim cannot be checked for an arbitrary element type; `-1` is the accurate answer.

## Tree Links

A tree-building algorithm reports every write to a child pointer as a link: the parent, the child, and the child slot. A parent of `-1` means the child became the root; a child of `-1` means the slot was emptied. The write itself is still reported as a valueless write at the same call site, so a context that only counts operations treats links as free. Links are reported because a tree's shape cannot be recovered from index operations. Parent pointers, heights, colors, and priorities are not reported: they are derivable from the links or irrelevant to the shape.

Links describe shape and nothing else.

- The in-order traversal of a reported tree is the sorted output only for a search tree. The sorted sequence is what the algorithm writes to buffer `0`.
- A multiway node is reported through the same two slots: the left slot of a key names the child before it, and its right slot names the next key of the same node or, for the last key, the child after it. An algorithm whose nodes have a capacity lays its keys out in blocks of that width in its own buffer and announces the width as a phase parameter. A slot of that buffer holds whatever was last written to it, so node identity comes from the block arithmetic, not from the first write to a slot.
- A forest is reported through the left-child / right-sibling encoding, in which a forest is a tree, and a root link then names the head of that encoding. A node with no incoming link is an expected state, because a priority queue retires each element it hands back. The root may be `-1` while the structure is a set of subtrees with none designated, as in `PairingHeapSort` between removing its root and folding that root's children back.

The links a consumer has received describe the structure as it stands. How many roots it has, and whether every created node is still part of it, are properties of the algorithm.

## Vector Operations

A vector instruction is reported lane by lane, in index order: a vector load as one read per lane, and a vector comparison as one comparison per lane carrying that lane's result. A lane operand held in a register is reported as `-1`. A vector store is reported as one write per lane it writes, carrying the value it left there; a lane that a mask leaves out is neither written nor reported. Vectors spilled to a stack array between steps are not an element buffer and are not reported.

The lane count is chosen at run time from the vector widths the machine accelerates, so an observed stream of the same input can differ between machines in its reads and comparisons. The order in which simultaneous lanes are listed is presentation, not a property of the sort.

## Auxiliary Buffers

- Element storage in an auxiliary buffer follows the same observation contract as the input. Writes or range copies initialize its elements before reads consume them.
- A buffer identifier names storage, not a role within it, and stays attached to the same storage for the whole sort, including when source and destination roles alternate. Ranges of one buffer are distinguished by their offsets, not by separate identifiers.
- An auxiliary element buffer holds the sorted element type. Slot metadata such as occupancy, tombstones, or bucket tags lives beside the buffer, not in a wrapper element type.
- Index arrays, offset buffers, and occupancy bitsets are algorithm metadata, not element storage. When an algorithm reports operations on metadata, it uses an identifier of its own, and each write carries the index or offset stored.
- Element copies contribute their full length to read and write counts, including bootstrap copies, snapshot storage, and staging operations when the algorithm performs them.

## Phase Levels

A phase announcement replaces the previously announced phase at the same level; there is no exit event, and a phase remains current until the next announcement at its level. Announcements occur at two levels, and a consumer keeps one slot per level:

- A **scope** phase is what the announcing algorithm decided to do, including a decision to delegate a subrange to another algorithm's core.
- A **detail** phase is progress reported from inside such a delegated core.

`SortPhase.IsDetailPhase` classifies the two. A scope announcement invalidates the detail slot, because the delegated work it described has ended. A detail announcement leaves the scope slot untouched.

A core that announces detail phases never delegates onward. A delegated core that announces scope phases, as `StdSort` does under `SkaSort`, replaces the handoff in the scope slot, and its own delegation reports into the detail slot. Two slots therefore describe every sequence this library emits. An algorithm whose own phases are classified detail may also run at the top level, where no scope phase accompanies them.

A hybrid that hands a subrange to another algorithm announces the handoff, with the range being delegated, before the delegated core runs. The phase names what the target actually is: `IpnsortSmallSort` exists rather than reusing `HybridToInsertionSort` because ipnsort's small sort is not an insertion sort.

## Context Implementations

- A callback added to the contract carries a no-op default implementation so that existing implementations keep compiling. Value-type contexts override it explicitly: a constrained call that falls through to a default interface implementation boxes the receiver, which would put an allocation on the `NullContext` path.
- A context must tolerate the event volume of the selected algorithm.
- A mutable context is not implicitly safe for concurrent use.
- A context that throws interrupts the sort and may leave the span partially sorted.

## Consumer Responsibilities

Consumers must not assume:

- that two different algorithms produce identical event sequences for the same input;
- that an algorithm keeps the same set of auxiliary buffers across releases. The set is an implementation choice; phases and the caller's span (buffer `0`) are what the contract guarantees, and a consumer keys on them;
- that a handoff phase stays current for the whole delegated range. `SkaSort` hands short ranges to `StdSort`, whose scope phases replace `SkaSortStdSort` as soon as StdSort announces one;
- that a `RadixPass` is followed by `DistributionBucket` reports. `RotateLSDRadixSort` and `Radsort` report none, because no bucket occupies a known contiguous span during their passes;
- that the buffers hold the elements in their current order between passes. `Radsort` keeps that order in a block permutation, and the span becomes ordered only during the closing `DistributionWrite` phase;
- that a written value stays where it was written until the algorithm moves it on purpose. The write-both merge and partition steps of `QuadSort`, `FluxSort`, `CrumSort`, and `BlitSort` write both candidates under every context, so a buffer briefly shows a duplicate of an element that is still elsewhere; a position's final value is the last write to it before the step ends;
- that walking a left-child / right-sibling root list gives the current roots at every moment. `BinomialHeapSort` relinks its root list one root at a time, so a walk mid-merge sees only the relinked prefix. A node is a root exactly when it is nobody's child, so a consumer derives membership from child links.
