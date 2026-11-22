# Benchmark Site Reference

How CI benchmark results reach the README and the benchmark site. [specs/benchmark_results.md](../specs/benchmark_results.md) defines the published figures and the reasons for them.

## Flow

```text
Benchmark workflow (workflow_dispatch, benchmark = "*")
  benchmark       BenchmarkDotNet -> *-report-github.md + *-report-full-compressed.json (artifact)
  update-readme   BenchmarkData.cs convert -> run.json
                  UpdateBenchmarkReadme.cs -> README.md + .github/images/benchmark/pareto-{light,dark}.svg
  publish-data    BenchmarkData.cs convert -> benchmark-data:runs/<id>.json
                  BenchmarkData.cs index   -> benchmark-data:index.json, history.json
  deploy-site     pages.yaml: site/ from main + data from benchmark-data -> GitHub Pages

Benchmark Site workflow (pages.yaml) also runs on pushes to main that touch site/.
```

A filtered run (any value other than `*`) updates neither the README nor the data branch. Its results cover only some classes and are not comparable to a full run.

## One-time Setup

1. In the repository settings, set **Pages → Build and deployment → Source** to **GitHub Actions**.
2. Run the Benchmark workflow with all benchmarks. The first `publish-data` creates the orphan branch `benchmark-data` and imports earlier runs from the README history with `import-readme`.

Until `benchmark-data` exists, `pages.yaml` deploys the site without data, and the site shows a "no data" message.

## Running The Benchmark

```powershell
./scripts/run-benchmark.ps1                  # full run on main
./scripts/run-benchmark.ps1 -Wait            # also wait for the README, data, and site jobs
./scripts/run-benchmark.ps1 -RunId <id>      # continue with a run that is already re-running
```

GitHub-hosted runners come with several CPU models. The workflow's `Check runner CPU` step fails within a minute on any model other than the pinned one. The script reads that runner's CPU from the step's annotation and re-runs the failed jobs until the check passes. On 2026-11-22 the pinned CPU came on the 20th attempt, and 14 of the 19 misses were Xeon 6975P-C.

## Scripts

Both scripts are file-based apps. Run them from the repository root.

```powershell
# BenchmarkDotNet JSON -> one run file
dotnet run --file .github/scripts/BenchmarkData.cs -- convert --artifacts BenchmarkDotNet.Artifacts/results --repo . --out out/run.json [--run-url <url>] [--commit <sha>]

# README sections in git history -> one run file per past run
dotnet run --file .github/scripts/BenchmarkData.cs -- import-readme --repo . --out-dir out/data/runs

# runs/*.json -> index.json + history.json
dotnet run --file .github/scripts/BenchmarkData.cs -- index --data out/data

# README summary + SVG charts + full tables
dotnet run --file .github/scripts/UpdateBenchmarkReadme.cs -- BenchmarkDotNet.Artifacts/results README.md <run-url> --run out/run.json --image-dir .github/images/benchmark --site-url https://guitarrapc.github.io/SortAlgorithms/
```

`convert` reads only benchmarks that have `Size` and `Pattern` parameters. It maps each benchmark method to an algorithm type through the `SortAlgorithm.Algorithms.<Type>.` call in the method body. It then reads Family, Stable, In-place, Best case, Average case, and Worst case from that type's Performance Characteristics.

## Benchmark Class Requirements

- `[Params]` on `Size` and on `Pattern` (the six `DataPattern` values used everywhere).
- One method per algorithm that calls `SortAlgorithm.Algorithms.<Type>.Sort(...)` directly, so the converter can find the documentation.
- A `DotnetSort` method that calls `_buffers.Next().AsSpan().Sort()`. It is the cross-class reference.
- Algorithms that are not quadratic should be measured up to 8192, so that they enter the largest league.

## Data Files

`runs/<id>.json` (schema 1). `id` is `yyyyMMddTHHmmZ-<short sha>`. Runs imported from README history use the suffix `readme`.

| Field | Meaning |
|---|---|
| `source` | `ci` or `readme` |
| `timestamp`, `commit`, `runUrl` | When the run was converted, the benchmarked commit (null for imports), and the workflow run |
| `reference` | Name of the reference method (`DotnetSort`) |
| `environment` | `cpu` (processor and clock; `convert` takes the clock from the class's `*-report-github.md`), `cores`, `os`, `runtime`, `sdk`, `benchmarkDotNet` |
| `algorithms.<method>` | `type`, `file`, `family`, `stable`, `inPlace`, `best`, `average`, `worst`; `reference: true` for the reference |
| `cases[]` | `category`, `method`, `size`, `pattern`, `mean`, `median`, `stdDev`, `error`, `n`, `allocated`, `measurements[]` (ns per sort for each WorkloadActual iteration) |

`index.json` lists runs in time order with `id`, `source`, `timestamp`, `commit`, `runUrl`, `cpu`, `file`, and `cases`. `latest` names the newest run. `history.json` maps `category|method|size|pattern` to rows of `[runIndex, mean, error, stdDev]`. `runIndex` refers to `index.json` `runs`. `history.json` grows with every run, by about 50 KB per run at the current case count.

## Site Code

`site/` is plain HTML and ES modules with no build step. d3 7.9.0 loads from cdnjs with an SRI hash.

| File | Role |
|---|---|
| `index.html`, `assets/style.css` | Shell, header, run picker, tabs; color tokens for light and dark |
| `assets/app.js` | Hash router (`#view?param=…`) and run loading |
| `assets/lib.js` | Data access, formatting, CPU-model grouping, anchor correction, layout helpers (`intro`, `chartWidth`, `narrow`), chart helpers |
| `assets/views/*.js` | One module per view; each exports `render(root, context)` |

Views keep their state in the hash, so a view can be linked with its filters.

Phones break at 600px (`narrow()` in `lib.js` matches the CSS), and tablets at 900px, where tables switch to short dates and CPU models and Explorer rows become cards.

Compare keeps its choice in the URL: `a` lists the eight color slots in order, with an empty entry for a free slot, so that removing an algorithm does not repaint the others; `a=-` means no algorithm. `y` selects the y value and `ys=one` the shared y axis. Complexity links to `#compare?add=<category>|<method>`, which adds that algorithm to the default set. Six tabs need about 357px at the phone tab size, so screens up to 360px get tighter tabs.

Tables that end a view (Explorer, History) sit in a `.tbox` that grows with the table, so the page scrolls instead of a nested box, and their header sticks below the tabs. A box with `overflow: auto` would become the scroll container and stop that header from sticking to the page, so `.tbox` uses `overflow: clip`, which clips without scrolling. Its table must therefore fit at every width; check that at 320, 601, and 901px. Only the Complexity matrix keeps a fixed-height scroll box, because its detail panel sits below it and must stay near the cell that selects it. A `.panel` sizes its column `minmax(0, 1fr)` so that a wide child, such as a select as wide as its longest option, cannot widen the panel past the screen. In mobile emulation the layout viewport grows with an overflowing page, so `scrollWidth > innerWidth` does not detect the overflow; compare element edges with the emulated width instead. Charts are drawn at the pixel width of their container (`chartWidth`), so a view that draws an SVG registers its drawing function with `context.onResize`; `app.js` calls it when the content width changes. Explorer and Leagues are HTML and reflow with CSS alone. On phones a League row keeps a fixed-width right column, because each row is its own grid: an `auto` column sized to the row's text would give every row, and the axis row, a different bar width. A sticky header inside a view uses `top: var(--tabs-h)`; `app.js` sets that variable to the height of the sticky tabs whenever it renders them, because a `ResizeObserver` reports only on the next rendered frame. Tooltips also open on tap, because touch screens have no hover.

## Local Preview

The site fetches JSON, so opening `index.html` from the file system does not work. Build a data directory and serve `site/` with `data/` beside it:

```powershell
dotnet run -c Release --project src/SortAlgorithm.Benchmark/SortAlgorithm.Benchmark.csproj -- --filter "*TreeBenchmark*"
dotnet run --file .github/scripts/BenchmarkData.cs -- import-readme --repo . --out-dir site/data/runs
dotnet run --file .github/scripts/BenchmarkData.cs -- convert --artifacts BenchmarkDotNet.Artifacts/results --repo . --out site/data/runs/local.json
dotnet run --file .github/scripts/BenchmarkData.cs -- index --data site/data
```

Then serve `site/` with any static file server. `site/data/` is ignored by git.
