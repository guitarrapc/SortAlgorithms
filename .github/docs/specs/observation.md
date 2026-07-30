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

## Consumer Responsibilities

- Context implementations must tolerate the event volume of the selected algorithm.
- A mutable context is not implicitly safe for concurrent use.
- A context that throws interrupts the sort and may leave the span partially sorted.
- Consumers must not assume that two different algorithms produce identical event sequences for the same input.

## Lessons Learned

Auxiliary-buffer identity is necessary for faithful merge and distribution visualizations; indices alone are ambiguous when the same numeric index exists in multiple buffers. Structured phase and role events are preferable to display strings because presentation belongs to the consumer.

An auxiliary buffer whose element type is an algorithm-private wrapper is reported but not consumable: the value reaching a context is opaque to anything outside the declaring type, so the buffer cannot be rendered even though every operation on it is announced. Slot metadata such as occupancy, tombstones, or bucket tags belongs beside the buffer, not inside its element type. Keeping the buffer's element type equal to the sorted element type also removes the sentinel writes such a wrapper needs, which are otherwise indistinguishable from real element movement in an observation stream.

Comparisons performed directly on a comparer rather than through the observable element accessors disappear from the stream entirely. When an algorithm searches an index or a staging structure, the comparison count a consumer sees can fall to a small fraction of the real one while read and write counts stay plausible, so the omission is not visible from the totals alone.

