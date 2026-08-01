# Operation Observation

## Purpose

The same sorting implementation supports statistics, visualization, and tutorial consumers through `ISortContext`. Observation describes algorithm activity; it must not determine the sorted result.

## Contract

An observable algorithm reports the operations it performs through the supplied context:

- comparisons, including both operand locations and their buffer identities;
- swaps, reads, writes, and range copies;
- structured algorithm phases and semantic element roles when the algorithm provides them.

Buffer `0` identifies the caller's main span. Positive identifiers distinguish auxiliary buffers within an operation. `-1` may identify external storage where the context contract permits it. Identifiers are semantic labels for an observation stream, not globally unique handles.

Values and indices reported to a context describe the algorithm's logical operation at callback time. Implementations must not omit observable operations merely to make statistics look smaller.

Callbacks that carry element values are generic in the element type. Writes come in two forms — one that carries the written value and one that does not, for writes whose value is not observable such as a tree pointer — and range copies likewise, with the values delivered as a `ReadOnlySpan<T>` over the source range. A context that only counts ignores the value and therefore never touches it; a context that needs the value for a known element type tests `typeof(T)` and reinterprets it. The span is valid only for the duration of the call, so a context that retains values must copy them out.

`NullContext` is the no-observation choice. Other contexts may aggregate counts, combine observers, or translate callbacks into visualization state.

## Phase Levels

A phase announcement replaces the previously announced phase at the same level; there is no exit event, and a phase remains current until the next announcement at its level. Announcements occur at two levels, and a consumer must keep one slot per level:

- A **scope** phase is what the announcing algorithm decided to do, including a decision to delegate a subrange to another algorithm's core.
- A **detail** phase is progress reported from inside such a delegated core.

`SortPhase.IsDetailPhase` classifies the two. A scope announcement invalidates the detail slot, because the delegated work it described has ended. A detail announcement leaves the scope slot untouched.

Cores that other algorithms delegate to are leaves: none of them delegates onward to another phase-announcing core. Two levels therefore describe every sequence this library emits, and a consumer never needs an unbounded phase stack. An algorithm whose own phases are classified detail may also run at the top level, where no scope phase accompanies them.

## Consumer Responsibilities

- Context implementations must tolerate the event volume of the selected algorithm.
- A mutable context is not implicitly safe for concurrent use.
- A context that throws interrupts the sort and may leave the span partially sorted.
- Consumers must not assume that two different algorithms produce identical event sequences for the same input.

## Lessons Learned

Auxiliary-buffer identity is necessary for faithful merge and distribution visualizations; indices alone are ambiguous when the same numeric index exists in multiple buffers. Structured phase and role events are preferable to display strings because presentation belongs to the consumer.

An identifier names storage, not a role within it. `BucketSort` gave every bucket one, but a bucket is a range of the temp buffer rather than a separate array, so the same elements were announced under one identity while being distributed and another while being sorted, and the copy back to the input read from a buffer nothing had written. Ranges of one buffer are already unambiguous through their offsets, so the identity added nothing a consumer could use and cost it the ability to follow the elements; the consumer that exists tracked the temp buffer by identifier and therefore saw no per-bucket sorting at all. Splitting a buffer into per-role identifiers is a presentation choice, and presentation belongs to the consumer. The test that pins this compares the set of auxiliary buffers written against the set the final copy reads from: elements must not appear in a buffer nothing reads, nor leave one nothing wrote.

An auxiliary buffer whose element type is an algorithm-private wrapper is reported but not consumable: the value reaching a context is opaque to anything outside the declaring type, so the buffer cannot be rendered even though every operation on it is announced. Slot metadata such as occupancy, tombstones, or bucket tags belongs beside the buffer, not inside its element type. Keeping the buffer's element type equal to the sorted element type also removes the sentinel writes such a wrapper needs, which are otherwise indistinguishable from real element movement in an observation stream.

A single "current phase" slot silently destroys the phase announcements of every hybrid algorithm. The caller announces the handoff, the delegated core immediately announces its own progress, and with one slot the handoff is overwritten before a single observable operation carries it — `SymMergeSort`'s initial-sort announcement was visible for zero operations, and the same held for the insertion-sort and heap-sort fallbacks of the introsort family. Sorting correctness never depended on this, so no result-based test could detect it. Separating scope from detail preserves both without requiring any call site to know whether it is nested.

Phase and role announcements are written by hand and therefore drift out of the coordinate space that element operations use. Element accessors derive their reported index and buffer from the span they were given, so a core that receives a slice reports reads and writes against that slice automatically; a phase or role that passes a raw loop index, or names the main buffer by constant, silently describes a different element. `InsertionSort.SortCore` did both, and because it is the core roughly twenty algorithms delegate to, every bucket `BucketSort` sorted and every scratch block `Glidesort` sorted pointed a consumer at an unrelated position in the input array. Sorting correctness never depended on it, and the stream stayed internally plausible, so only a test that compares role and phase indices against the element operations of the same call detects it. All three delegated cores carried the same defect: `BinaryInsertionSort.SortCore` and `HeapSort.SortCore` merely escaped notice because every caller they have today passes the whole span, which makes the two coordinate spaces agree by coincidence rather than by construction. A phase parameter that carries a count rather than an index — `HeapExtract`'s step and total — must not be shifted, so the rule is per parameter, not per call.

Typing an observation callback's value as `object` costs one allocation per observed element, and the cost is invisible from the sorted result. Writes were reported as `object?` and range copies as `object?[]`, so every observing pass boxed each written element and materialized an array per copy: 12–19 MiB for a single pass over 32768 elements, paid twice per visualization because the preflight and the recording pass each run one. Nothing about the sort changed, and no correctness test could see it. Keeping the value generic removes the cost entirely without narrowing the contract — a context that ignores the value now allocates nothing at all. The convenience `VisualizationContext` still exposes `object?` callbacks because a caller that does not know the element type needs them, but it only pays when a callback is actually attached, and a consumer recording millions of operations should implement `ISortContext` directly instead.

A comparison whose operands are both reported as "nowhere" cannot be placed on any array, and an algorithm that holds an operand in a local reaches for exactly that form. The value-based accessors were the only ones available, so an insertion sort's shift test, a partition's pivot test and a merge's cursor test all announced a comparison naming neither side, while the reads immediately before them carried the very indices that were being discarded — the totals stayed right and only the meaning was lost. Accessors that take the index for whichever operand has one fix the common case, and where the caller needs the value afterwards the accessor can perform the read and hand it back, which keeps the read count honest and leaves no room for the location to be wrong.

What remains is algorithms that read once and compare several times — a dual-pivot scan testing one element against two pivots, a branchless network writing back a conditionally selected value. Locating those comparisons requires either reading again, which announces work the algorithm deliberately avoids, or an accessor that takes the caller's word for which index a value came from. The second was tried and reverted: the claim cannot be checked for an arbitrary element type, because a write that replaces the element with an equal-ordering one is indistinguishable from no write at all, and it recreates the same unverifiable-index failure that made hand-written role coordinates drift. Reporting `-1` is then the accurate answer rather than a gap to close, and it is the only honest answer for a value a conditional move produced, which is in no buffer at all. Driving the count of such comparisons to zero is the wrong target; it buys a metric by adding a defect class.

Comparisons performed directly on a comparer rather than through the observable element accessors disappear from the stream entirely. When an algorithm searches an index or a staging structure, the comparison count a consumer sees can fall to a small fraction of the real one while read and write counts stay plausible, so the omission is not visible from the totals alone. The same happens to reads and writes when a private helper takes a raw span instead of a `SortSpan`: `BucketSortInteger` sorted every bucket through one, so the whole per-bucket phase — the part a distribution visualization exists to show — produced no events, and the auxiliary buffer looked like a box that is filled and then read back. Tests can be written so that they pass either way, because expectations derived from an implementation that already omits the operations simply encode the omission; expressing a count as a relation between two observed quantities, rather than as a constant, is what makes the gap visible.

