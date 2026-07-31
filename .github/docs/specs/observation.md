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

An auxiliary buffer whose element type is an algorithm-private wrapper is reported but not consumable: the value reaching a context is opaque to anything outside the declaring type, so the buffer cannot be rendered even though every operation on it is announced. Slot metadata such as occupancy, tombstones, or bucket tags belongs beside the buffer, not inside its element type. Keeping the buffer's element type equal to the sorted element type also removes the sentinel writes such a wrapper needs, which are otherwise indistinguishable from real element movement in an observation stream.

A single "current phase" slot silently destroys the phase announcements of every hybrid algorithm. The caller announces the handoff, the delegated core immediately announces its own progress, and with one slot the handoff is overwritten before a single observable operation carries it — `SymMergeSort`'s initial-sort announcement was visible for zero operations, and the same held for the insertion-sort and heap-sort fallbacks of the introsort family. Sorting correctness never depended on this, so no result-based test could detect it. Separating scope from detail preserves both without requiring any call site to know whether it is nested.

Phase and role announcements are written by hand and therefore drift out of the coordinate space that element operations use. Element accessors derive their reported index and buffer from the span they were given, so a core that receives a slice reports reads and writes against that slice automatically; a phase or role that passes a raw loop index, or names the main buffer by constant, silently describes a different element. `InsertionSort.SortCore` did both, and because it is the core roughly twenty algorithms delegate to, every bucket `BucketSort` sorted and every scratch block `Glidesort` sorted pointed a consumer at an unrelated position in the input array. Sorting correctness never depended on it, and the stream stayed internally plausible, so only a test that compares role and phase indices against the element operations of the same call detects it. All three delegated cores carried the same defect: `BinaryInsertionSort.SortCore` and `HeapSort.SortCore` merely escaped notice because every caller they have today passes the whole span, which makes the two coordinate spaces agree by coincidence rather than by construction. A phase parameter that carries a count rather than an index — `HeapExtract`'s step and total — must not be shifted, so the rule is per parameter, not per call.

Comparisons performed directly on a comparer rather than through the observable element accessors disappear from the stream entirely. When an algorithm searches an index or a staging structure, the comparison count a consumer sees can fall to a small fraction of the real one while read and write counts stay plausible, so the omission is not visible from the totals alone.

