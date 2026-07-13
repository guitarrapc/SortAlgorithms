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

