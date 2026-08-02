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

Name an adversary after the pivot rule it defeats, not after the algorithm family. The QuickSort adversary was documented as targeting median-of-3 while in fact only hurting the middle-pivot variant, and even there it reached about a quarter of the achievable comparison count - less than the decorative pipe-organ pattern already in the set. Two checks follow from this, and both belong in the tests rather than the prose: measure the adversary against every variant in the family, and assert both halves of the claim - that the intended targets go quadratic *and* that the variants it does not defeat stay near random. A single array cannot be worst-case for every pivot rule at once, so an adversary that appears to hurt everything is measuring something other than what it claims.

An adversary can be worse than no adversary at all, and only a cost comparison catches it. The TimSort pattern was built from the run-length sequences that make a merge tree lopsided, and it measured **0.7x** of random input - it was handing TimSort an input strictly easier than random. Its tests asserted the array was a valid permutation and that TimSort could sort it, and both stayed true the whole time. The mechanism is worth stating because it generalizes: a run longer than the minimum exists only because the data was already ordered there, so run-length skew is bought with the run-building cost it skips, and for TimSort that trade loses. **Whenever an adversary targets one stage of an algorithm, check what it hands back at the other stages** - and assert the total, not the stage.

Not every algorithm has a lever worth a large factor. TimSort's comparison count on random input is already within a few percent of the information-theoretic minimum and its merge cost is bounded, so the achievable margin is ~1.2-1.5x, not the 50x-plus a quicksort adversary reaches. An adversary spec should state the margin the algorithm actually admits; a target of "much worse than random" invites a construction that is quietly wrong instead of one that is honestly modest.

Deriving an adversary costs what it makes the target pay, because generation is one full run of that target. For a quicksort with no fallback that is quadratic - unavoidable, since producing an input that forces n²/4 comparisons requires performing them. Two habits follow: consumers that materialize such a pattern repeatedly should cache it (it is deterministic, so only the length matters, but the array is mutable and must be handed out as a copy), and test data sets built from one must hold the array rather than the call.

