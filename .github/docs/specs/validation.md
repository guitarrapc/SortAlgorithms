# Validation Contract

## Correctness Evidence

Test every algorithm against the order that a trusted comparer produces. Cover these equivalence classes:

- empty and single-element input
- random input
- sorted and reverse-sorted input
- repeated and all-equal values
- negative and positive values, where the element type allows them
- adversarial or structural patterns that are relevant to the algorithm

When overloads or shared comparison infrastructure change, tests must also show the behavior with a custom comparer. A stable algorithm needs separate evidence that equal keys keep their input order.

Run the ordering and stability checks with both `StatisticsContext` and `NullContext`. Floating-point cases must include NaN as the left operand, NaN as the right operand, NaN in both operands, infinities, and signed zeros. A pass on the observable path does not validate the `NullContext` path, because the JIT specializes that path separately.

## Observation Evidence

A change to `SortSpan`, contexts, copying, auxiliary buffers, phases, or roles must check both the sorted result and the affected callbacks. The optimized path and the observable path give the same output order. Tests compare callback counts and sequences only where a specific contract requires them.

## Performance Evidence

Measure a performance-sensitive change with BenchmarkDotNet in Release mode. Record the runtime, the input sizes and patterns, the baseline, the throughput result, and the allocated bytes. Benchmarks add to correctness tests. They never replace them.

## Lessons Learned

Tests with only random input miss common sorting failures. Duplicate-heavy input, presorted input, and algorithm-specific adversaries test partition boundaries, termination, stability, and worst-case behavior. Random samples can miss these cases.

An out-of-bounds access can pass every Release test. In Debug builds, `SortSpan` indexes the span directly. In other builds, it uses `Unsafe.Add` without bounds checks. A stray read then returns a neighboring element, and the sorted output can still be correct.

Glidesort read one element past a run for years in this way. `MergeLeftGap` compared against the first element of the right sub-run, and it did not check that the sub-run had elements. A half-rotated input is two ascending runs, with the second run entirely below the first. From 128 elements upward, this input drives the split-point helper to exactly that empty sub-run. Two habits follow:

- Run the suite in Debug as well as in Release. Only the bounds check makes these defects visible.
- Guard a boundary comparison with the run lengths. Do not rely on an invariant of the caller. In this case, the merge loop below the shortcut already handled both empty cases correctly, so the guard cost nothing.

Put structural patterns that make two runs unequal in length in the shared standard data set, not in the test file of one algorithm. `MockStandardData` did not contain this pattern, so no test ever checked an algorithm against it.

A fixed layout cannot be an adversary for an adaptive algorithm. A layout can attack one step, such as the pivot slots of one partition or the run boundaries of one merge. It stops working as soon as the algorithm reacts. The recovery mechanisms (pattern-defeating shuffles, already-partitioned detection, complexity fallbacks) change the next step.

The PDQSort adversary was such a layout, and it cost PDQSort only about 15% more than random input. Its replacement comes from a run of PDQSort against a comparer that decides each value lazily, when a comparison asks for it. The replacement raised the cost to about 2.5x random input and triggered the heapsort fallback. Two consequences follow:

- An adversary generator of this kind is meaningful only for the exact algorithm that it ran against.
- Validate the generator by its cost to that algorithm, never by how disordered the array looks.

Judge an adversary against the correct bound. For an algorithm with a complexity fallback, O(n²) is unreachable by construction. An input that reached O(n²) would show a bug in the algorithm, not a better adversary. Assert the bound that the algorithm actually allows.

Name an adversary after the pivot rule that it defeats, not after the algorithm family. The documentation said that the QuickSort adversary targeted median-of-3. In fact, it hurt only the middle-pivot variant. Even there, it reached only about a quarter of the possible comparison count. That is less than the decorative pipe-organ pattern that was already in the set. Two checks follow, and both belong in the tests, not in the prose:

- Measure the adversary against every variant in the family.
- Assert both halves of the claim: the intended targets go quadratic, *and* the variants that it does not defeat stay near random.

One array cannot be the worst case for every pivot rule at the same time. An adversary that seems to hurt every variant measures something other than its claim.

An adversary can be worse than no adversary, and only a cost comparison detects this. The TimSort pattern came from run-length sequences that make a merge tree unbalanced. It measured **0.7x** of random input, so it gave TimSort an input that was easier than random. Its tests asserted that the array was a valid permutation and that TimSort could sort it. Both assertions stayed true the whole time.

The mechanism applies in general. A run longer than the minimum exists only because the data was already in order there. Run-length skew therefore has a price: the long runs skip run-building work. For TimSort, the skew gains less than that skipped work, so the trade loses. **When an adversary targets one stage of an algorithm, check what it gives to the other stages.** Assert the total cost, not the cost of one stage.

Not every algorithm has a weakness worth a large factor. On random input, the comparison count of TimSort is already within a few percent of the information-theoretic minimum, and its merge cost is bounded. The possible margin is therefore about 1.2–1.5x, not the 50x or more that a quicksort adversary reaches. An adversary specification should state the margin that the algorithm actually allows. A target of "much worse than random" invites a construction that is silently wrong, instead of one that is honestly modest.

An adversary costs as much to derive as it makes the target pay, because the generator runs the target once in full. For a quicksort with no fallback, that cost is quadratic. This cost is unavoidable: to produce an input that forces n²/4 comparisons, the generator must perform them. Two habits follow:

- A consumer that builds such a pattern many times should cache it. The pattern is deterministic, so only the length matters. The array is mutable, so the cache must return a copy.
- A test data set built from such a pattern must hold the array, not the call.
