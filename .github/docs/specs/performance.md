# Performance And Allocation

## Goal

The ordinary sorting path is intended for performance-sensitive use, while observable paths deliberately trade additional work for insight. Both paths must remain correct and use the same ordering semantics.

## Guarantees

- The no-observation path uses `NullContext` and generic specialization so observation callbacks can be removed or inlined by the JIT.
- Comparison dispatch remains generic on `TComparer`; callers can use value-type comparers without mandatory interface boxing.
- Algorithms that require temporary storage bound that storage according to their documented algorithmic space complexity.
- Implementations avoid per-element heap allocation in hot loops.
- Pooled storage, when used, is returned even when sorting fails.

"Zero allocation" is not a universal package guarantee: some algorithms inherently require auxiliary storage, pool misses are runtime-dependent, and caller-provided comparers or contexts may allocate. Allocation claims must therefore name the algorithm, overload, input class, runtime configuration, and benchmark evidence.

## Regression Policy

A performance-sensitive change must be compared with a meaningful baseline in Release mode. Regressions in throughput or allocation require either correction or an explicit documented trade-off. Benchmark noise alone is not evidence of a regression; results should include enough context to reproduce the comparison.

## Why These Constraints Exist

Comparer interface dispatch, observation callbacks, and temporary collections can dominate simple sorting loops. Generic specialization and span-oriented storage keep those costs visible and allow callers to choose observation only when needed.

