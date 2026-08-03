# AGENTS.md

SortAlgorithms is a performance-oriented .NET sorting-algorithm library with optional operation observation for statistics and visualization.
Keep changes focused, measurable, and consistent across optimized and observable execution paths.

## Core Principles

- Stay faithful to the textbook algorithm. Each algorithm implements its own definition, keeping the behavior that definition implies — including its bad cases. Bucket sort's O(n²) collapse and quicksort's adversarial inputs are properties to preserve, not defects to patch over. Do not add cutoffs, fallbacks, or hybrid stages that the algorithm's own definition does not contain.
- Optimize within the definition, not around it. An optimization is in scope when it changes only how a step is carried out — a cheaper representation of an auxiliary structure, a removed redundant pass, a parameter the theory leaves free such as a bucket count. It is out of scope when it changes which algorithm is running. When an implementation choice and the algorithm's stated complexity disagree, the implementation is what is wrong.
- Preserve sorting correctness for every supported input pattern, comparer, and context.
- Treat low allocation and hot-path performance as first-class requirements.
- Use `Span<T>` and the generic comparer/context design so the JIT can specialize optimized paths.
- Keep operation observation accurate: reads, writes, comparisons, swaps, copies, phases, roles, and buffer identities must remain meaningful to consumers.
- Prefer straightforward implementations whose algorithmic intent is recognizable.
- Do not add third-party runtime dependencies without an explicit project-level decision.
- Avoid unrelated refactors.

## Project Structure

```text
.github/docs/                 Specifications and implementation references.
src/SortAlgorithm/            Library, algorithms, contexts, and utilities.
src/SortAlgorithm.Benchmark/  BenchmarkDotNet suites.
tests/SortAlgorithm.Tests/    TUnit correctness and behavior tests.
sandbox/                      Exploratory apps and standalone verification files.
scripts/                      Repository maintenance scripts.
```

## Implementation Guidance

- Follow the public contracts in `.github/docs/spec.md` and `.github/docs/specs/`.
- Follow `.github/docs/references/implementation_guidelines.md` when adding or modifying an algorithm.
- Route observable element operations through `SortSpan<T, TComparer, TContext>`; do not silently bypass context callbacks.
- Propagate `TComparer` and `TContext` generically through hot-path helpers to retain JIT specialization.
- Give every auxiliary buffer a stable, documented buffer identifier.
- Return pooled buffers in `finally` blocks and clear them when retaining references would be unsafe.
- Do not use `dotnet-script`; follow `.github/docs/references/sandbox.md` for one-file experiments.
- Use current repository C# conventions: file-scoped namespaces, `var` for evident local types, collection expressions where suitable, and standard .NET naming. Existing code omits `private` on private members and does not prefix fields with `_` or `s_`.
- Describe measured or observable facts in comments. Qualify runtime/JIT effects unless verified, and do not assert complexity or allocation claims without evidence from the implementation.

## Testing And Validation

- Add or update focused TUnit tests for behavior changes.
- Cover empty, single-element, ordered, reverse-ordered, duplicate-heavy, adversarial, and relevant algorithm-specific inputs.
- Verify custom comparers and observable contexts when changing shared infrastructure or public overloads.
- For stable algorithms, verify the relative order of equal keys.
- For classification or branching logic, enumerate true and false equivalence classes and test each class.
- Run the narrowest relevant tests first, then the full suite when practical.
- Benchmark performance-sensitive changes in Release and compare allocation results against a meaningful baseline.

## Documentation

- A class summary describes the algorithm, not this implementation's measurements. Best, average, and worst case state the textbook complexity; they are not rewritten to match what the current code happens to achieve. If the code does not reach the stated complexity, fix the code. Implementation-specific numbers — operation counts, chosen parameters, buffer identifiers — belong beside the theory as clearly labelled implementation notes, and benchmark figures belong in `.github/docs/specs/` or the README rather than in a class summary.
- Performance Characteristics is a fixed schema, not a free list. Exactly these eleven rows, in this order, every one of them present: `Family`, `Stable`, `In-place`, `Best case`, `Average case`, `Worst case`, `Comparisons`, `Swaps`, `Index Reads`, `Index Writes`, `Space`. Pad each label to twelve characters before the colon. A quantity the algorithm never produces is stated as `0` with the reason — merge sort's `Swaps : 0` is a fact about merge sort, not an empty cell. Rows a family needs beyond the eleven (`Rotations`, `Digit Passes`, `Copies`, `Partition`) follow `Space` and obey the same padding, so pick a name that fits. Anything that is not a measure — prose notes, cache behaviour, per-pattern commentary — belongs in the surrounding sections, not in this list. Do not add an `Adaptive` row: whether `Best case` beats `Average case` already says it.
- A row states an order, not the count this implementation happens to reach, and it must cover all the work the algorithm performs — a row that silently omits an algorithm's structural writes is wrong, not merely imprecise. The four operation rows correspond one-to-one with the counters on `StatisticsContext`, so a row can be checked against a run. Their provenance differs and the wording should not blur it: comparisons, swaps, and writes (moves) are classical measures that hold for any implementation, whereas the number of *reads* is not — it depends on how much a given implementation keeps in registers. `Index Reads` therefore means the reads announced through `SortSpan`: well defined by the observation contract and comparable across algorithms here, but not a figure to reconcile against the literature.
- Keep specifications implementation-neutral: document WHAT, WHY, public guarantees, non-goals, and lessons learned.
- Put detailed HOW, code patterns, and command recipes under `.github/docs/references/`, in code comments, or in the implementation.
- After implementation, update related specs to match what was actually built and record newly learned constraints.
- Update README examples when public API behavior changes.
