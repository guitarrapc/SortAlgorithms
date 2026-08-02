# Validation Contract

## Correctness Evidence

Every algorithm must be tested against the ordering produced by a trusted comparer for representative equivalence classes:

- empty and single-element input;
- random input;
- already sorted and reverse-sorted input;
- repeated and all-equal values;
- negative and positive values where the element type permits them;
- adversarial or structural patterns relevant to the algorithm.

Tests must also demonstrate custom-comparer behavior when overloads or shared comparison infrastructure change. Stable algorithms require separate evidence that equal keys retain their input order.

## Observation Evidence

Changes to `SortSpan`, contexts, copying, auxiliary buffers, phases, or roles must verify both the sorted result and the affected callbacks. Optimized and observable paths are behaviorally equivalent with respect to output ordering, but callback counts and sequences are only compared where a specific contract requires them.

## Performance Evidence

Performance-sensitive changes use BenchmarkDotNet in Release mode. Record the relevant runtime, input sizes and patterns, baseline, throughput result, and allocated bytes. Benchmarks complement correctness tests and never replace them.

## Lessons Learned

Random-only tests do not expose common sorting failures. Duplicate-heavy inputs, presorted inputs, and algorithm-specific adversaries exercise partition boundaries, termination, stability, and worst-case behavior that random samples can miss.

Out-of-bounds accesses can pass every RELEASE test. `SortSpan` indexes the span directly under `#if DEBUG` but uses `Unsafe.Add` without bounds checks otherwise, so a stray read simply returns a neighbouring element and the sorted output can still be correct. Glidesort read one element past a run for years this way: `MergeLeftGap` compared against the first element of the right sub-run without checking that the sub-run was non-empty, and a half-rotated input (two ascending runs, the second entirely below the first) drives the split-point helper to exactly that degenerate result from 128 elements upward. Two habits follow:

- Run the suite in DEBUG as well as RELEASE. The bounds check is the only thing that turns these into a visible failure.
- Guard boundary comparisons on run lengths rather than relying on a caller invariant. In this case both degenerate cases were already handled correctly by the merge loop below the shortcut, so the guard cost nothing.

Structural patterns that make two runs unequal in length belong in the shared standard data set, not in one algorithm's test file. This pattern was absent from `MockStandardData`, so no algorithm was ever checked against it.

An adversary for an adaptive algorithm cannot be written down as a fixed layout. Arrangements drawn to poison one step - the pivot slots of one partition, the run boundaries of one merge - stop being adversarial the moment the algorithm reacts, because the recovery machinery (pattern-defeating shuffles, already-partitioned detection, complexity fallbacks) is precisely what changes the next step. The PDQSort adversary was such a layout and cost PDQSort only ~15% more than random input; deriving it instead by running PDQSort against a comparer that decides values lazily, as each comparison is asked, raised it to ~2.5x and made the heapsort fallback fire. Two consequences worth stating: an adversary generator of this kind is only meaningful for the exact algorithm it was derived against, and it must be validated by what it costs that algorithm, never by how disordered the array looks.

An adversary must not be judged against the wrong bound either. For an algorithm with a complexity fallback, O(n²) is unreachable by construction - an input that reached it would be evidence of a bug in the algorithm, not a better adversary. The bound to assert is the one the algorithm actually admits.

