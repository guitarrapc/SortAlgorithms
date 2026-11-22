# Benchmark Results Specification

This document states what the project publishes from its CI benchmarks, how each published figure is defined, and why. [references/benchmark_site.md](../references/benchmark_site.md) describes the workflow, the file formats, and local preview.

## Where Results Appear

- **README.** The benchmark section starts with a summary: a chart of mean time against run-to-run variation, and the fastest algorithm for each category and input pattern. The full BenchmarkDotNet tables follow in a collapsed block. A full Benchmark workflow run regenerates the section.
- **Benchmark site.** A static site on GitHub Pages reads the published run files. It has five views: Explorer, Leagues, Complexity, Stability, and History.
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
- Every class measures `DotnetSort` (`MemoryExtensions.Sort`) as a shared reference. The reference is never ranked. The site can scale each class's times so that its reference matches the reference in Partition. This corrects drift between classes that ran at different times in the job.
- Overall rankings use **size leagues**. A league contains only the algorithms measured at that size. Small sizes do not separate linear sorts from O(n log n) sorts, so a single ranking at a size every class supports would mislead. O(n²) algorithms appear only in the smaller leagues, because the largest size is impractical for them.
- A class whose algorithms are not quadratic measures the largest size. Tree and the ShellSort variants in Insertion were extended to 8192 for this reason.
- League score: for each pattern, divide the algorithm's time by the fastest time in the league, then take the geometric mean of the six ratios. The worst single pattern is shown next to the score, because a good average can hide an adversarial pattern.

## Complexity View

The site fits k, the slope of log(time) against log(n), for every algorithm and pattern. It classifies k against reference slopes that are fitted over the same sizes. Only n (1.00) and n² (2.00) are independent of the size range; n log n is 1.14 over 256–8192 and 1.16 over 256–1024. Next to k, the view shows the Best, Average, and Worst cases from the algorithm's XML documentation. A pattern that reaches the documented worst case is therefore visible, as the textbook-fidelity rule in [AGENTS.md](../../../AGENTS.md) expects.

k also includes fixed costs and cache effects. For this reason the view shows the slope of each size interval too, not only the fit over all sizes.

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
- Earlier CI runs used three CPU models (EPYC 7763, 9V74, and 9V45). The JSON report names the processor without its clock, while the Markdown header includes the clock. Runs are therefore grouped by model, not by the full processor string.
- `SelectionSortSimd` is Θ(n²), yet in the 2026-11-20 run its interval slopes were 1.30, 1.59, and 1.83 from 256 to 8192. Fixed costs dominate small sizes, so a single fitted k understates the order.
