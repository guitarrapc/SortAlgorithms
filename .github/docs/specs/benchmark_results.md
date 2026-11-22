# Benchmark Results Specification

This document states what the project publishes from its CI benchmarks, how each published figure is defined, and why. [references/benchmark_site.md](../references/benchmark_site.md) describes the workflow, the file formats, and local preview.

## Where Results Appear

- **README.** The benchmark section starts with a summary: a chart of mean time against run-to-run variation, and the fastest algorithm for each category and input pattern. The full BenchmarkDotNet tables follow in a collapsed block. A full Benchmark workflow run regenerates the section.
- **Benchmark site.** A static site on GitHub Pages reads the published run files. It has five views: Explorer, Leagues, Complexity, Stability, and History. On a phone, each view shows its ranking, matrix, or chart in the first screen; explanations are one sentence, with the rest behind a disclosure.
- **SortVivo** ranks algorithms in the browser through WebAssembly. Those numbers depend on the viewer's machine. The CI results are the machine-fixed counterpart.

## Measurement Environment

The workflow runs on a GitHub-hosted runner and fails early unless the CPU model is the pinned one. Absolute times are valid only for that model. The site therefore compares a run only with earlier runs on the same CPU model. All benchmark classes run one after another in the same job.

## Published Figures

| Figure | Definition | Why |
|---|---|---|
| Mean | BenchmarkDotNet mean time for one sort | The primary speed measure |
| Error | Half-width of the 99.9% confidence interval | Shown as error bars. When two intervals overlap, the two results are not distinguished. |
| CV | StdDev / Mean within one run | The stability measure. CV below 2% is stable, below 5% is noisy, and 5% or more is unstable. |
| Allocated | Managed bytes per sort from `MemoryDiagnoser` | Pooled buffers do not appear after warmup. A nonzero value therefore signals a broken allocation guarantee, not the size of an auxiliary buffer. |
| Per-iteration measurements | Every measured iteration, outliers included | Shows bimodal or outlier-driven results that Mean ± StdDev hides |

Error and StdDev are not shown as two independent measures. Error is t·StdDev/√N, and the iteration count fixes N, so Error adds nothing to StdDev as a stability measure.

## Comparing Across Benchmark Classes

- The Ratio column of each class uses that class's own baseline. Never compare Ratio across classes.
- Every class measures `DotnetSort` (`MemoryExtensions.Sort`) as a shared reference. The reference is never ranked. The site can scale each class's times by one factor so that its reference matches the reference in Partition. The factor is the geometric mean of the reference ratios over every size and pattern that both classes measured. This corrects drift between classes that ran at different times in the job. A factor for each size and pattern would instead carry the noise of a single reference measurement into every algorithm of the class.
- Overall rankings use **size leagues**. A league contains only the algorithms measured at that size. Small sizes do not separate linear sorts from O(n log n) sorts, so a single ranking at a size every class supports would mislead.
- Sizes are set per class, not per algorithm. Exchange and Selection, which are mostly quadratic, stop at 1024 and appear only in the smaller leagues. Tree and Insertion measure up to 8192 so that their non-quadratic algorithms enter the largest league. Their quadratic algorithms therefore run at 8192 too.
- Ratios on the site divide by `DotnetSort`, not by the fastest algorithm. The fastest algorithm differs by pattern and by size, so a reader would have to look it up before a ratio means anything. Every .NET reader knows `Span.Sort`, so ×0.5 reads directly as half the time of the standard sort. `DotnetSort` is not ranked. In a league it stays at the top of the list together with the scale while the list scrolls, so that any row can be compared with it; a line marks where it would rank.
- League score: for each pattern, divide the algorithm's time by the time of `DotnetSort` in Partition, then take the geometric mean of the six ratios. All algorithms in a league share each pattern's divisor, so the order is the same as with any other common divisor; only the numbers change. The worst single pattern is shown next to the score, because a good average can hide an adversarial pattern. Where a run has no reference at that size, which happens only in some imported runs, the fastest algorithm for each pattern is the divisor, and the page says so.

## Complexity View

The site fits k, the slope of log(time) against log(n), for every algorithm and pattern. It classifies k against reference slopes that are fitted over the same sizes. Only n (1.00) and n² (2.00) are independent of the size range; n log n is 1.14 over 256–8192 and 1.16 over 256–1024. Next to k, the view shows the Best, Average, and Worst cases from the algorithm's XML documentation. A pattern that reaches the documented worst case is therefore visible, as the textbook-fidelity rule in [AGENTS.md](../../../AGENTS.md) expects. Cells that reach ≈ n² fall into two lists. One list holds cells whose documented worst case is below n²; these need a check of the implementation. The other list holds cells within a documented worst case of n², or within a bound in another variable, such as Θ(n·m) with m distinct keys.

k also includes fixed costs, cache effects, and branch prediction. For this reason the view shows the slope of each size interval too, not only the fit over all sizes.

## Data Retention

Each full run produces one run file. Run files are kept on the orphan branch `benchmark-data`, so that `main` does not collect data commits. The first publish imports the earlier runs from the README history. Imported runs have summary statistics only, with no per-iteration measurements and no median.

## Non-goals

- Comparing different machines or CPU models
- Statistical tests beyond confidence-interval overlap
- Ranking the reference sort

## Lessons Learned

- In the 2026-11-20 run, `SelectionSort` and `DoubleSelectionSort` were each measured in two classes. The two measurements differed by at most 0.9%, so drift between classes was small in that run. The reference method remains as a guard for runs where drift is larger.
- In that run, high-CV cases were scattered across algorithms and sizes instead of concentrating on particular algorithms. Treat a high CV from one run as noise until the history shows the same CV again.
- At n = 8192 the 99.9% interval is typically below 1% of the mean, so error bars mostly matter at small sizes.
- In the 2026-11-22 run, a single case with a CV above 15% compressed every other point to the bottom of a linear CV axis. The charts use a square-root CV scale.
- Earlier CI runs used three CPU models (EPYC 7763, 9V74, and 9V45), and runs on one model reported clocks from 2.60 to 3.69 GHz. Runs are therefore grouped by model, not by the full processor string. The JSON report names the processor without its clock, so the converter reads the clock from the Markdown report of the same class.
- The 2026-11-22 04:43 run was the first with the reference in every class. The class factors were within 1.0% of 1. Factors for single sizes and patterns ranged up to 19.5%. The largest came from one reference cell in Partition with a CV of 6.6%, and only 4 of 180 differences exceeded the combined confidence intervals. The correction therefore uses one factor per class.
- Measuring Tree and Insertion at 4096 and 8192 lengthened the benchmark job from 17 to 26 minutes.
- `QuickSort` was considered as the site's baseline, because its name is the most familiar. In the 2026-11-22 08:54 run at n = 8192 it took 16.9 times as long as `DotnetSort` on PipeOrgan, where its middle-element pivot degrades toward n². A baseline with a quadratic pattern would shrink every ratio on that pattern and change meaning with n. `IntroSortDotnet`, the library's port of the runtime introsort, scored ×0.999 against `DotnetSort` in the same league, but only Partition measures it, so it cannot correct drift between classes. `DotnetSort` ranked between 45th and 46th of 95 algorithms there, so ratios fall on both sides of ×1.
- In that run, `BitonicSort` on Random input had k = 1.16, near n log n, although its comparison count is exactly (n/4)·lg n·(lg n + 1) at every size, a slope of 1.27. The same sort on Sorted input and `BitonicSortBranchless` on any input had k ≈ 1.26, with a constant time per comparison. Only the data-dependent branch on Random input made the time per comparison fall as n grew. A k below the documented order is therefore not, by itself, evidence that the implementation does less work. Compare `StatisticsContext` counts first.
- In that run, the slope from 4096 to 8192 alone was about 2 on Sorted, Reversed, or PipeOrgan input in eleven cells of seven algorithms whose documented worst case is below n², among them `TernaryHeapSort`, `SmoothSort`, `TournamentSort`, and `IntroSortDotnet`. Their comparison and write counts grew as n log n over the same interval. These patterns give every iteration the same input. Random input, which differs per iteration, showed no such step. The step is therefore consistent with the branch predictor learning the repeated input at the smaller size. A large slope on these patterns alone is therefore a timing effect until the operation counts show otherwise.
- The twelfth such cell, `StdStableSort` on Reversed input, was real work. Its insertion-sort leaves alternate between 64 and 128 elements as the recursion depth changes parity, so comparisons per element alternate between about 34 and 66 at successive powers of two. The order stays n log n, but a single interval can show a slope of 0.1 or 2.0.
- `SelectionSortSimd` is Θ(n²), yet in the 2026-11-20 run its interval slopes were 1.30, 1.59, and 1.83 from 256 to 8192. Fixed costs dominate small sizes, so a single fitted k understates the order.
