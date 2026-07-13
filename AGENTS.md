# AGENTS.md

SortAlgorithms is a performance-oriented .NET sorting-algorithm library with optional operation observation for statistics and visualization.
Keep changes focused, measurable, and consistent across optimized and observable execution paths.

## Core Principles

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

- Keep specifications implementation-neutral: document WHAT, WHY, public guarantees, non-goals, and lessons learned.
- Put detailed HOW, code patterns, and command recipes under `.github/docs/references/`, in code comments, or in the implementation.
- After implementation, update related specs to match what was actually built and record newly learned constraints.
- Update README examples when public API behavior changes.
