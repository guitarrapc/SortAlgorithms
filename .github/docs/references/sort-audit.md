# Reproducing the sorting audit

The audit uses the existing benchmark project. Measured methods call each algorithm directly with `NullContext`; verification delegates run outside the timed region.

```powershell
dotnet run -c Release --project src/SortAlgorithm.Benchmark -- --audit-verify
dotnet run -c Release --project src/SortAlgorithm.Benchmark -- --audit --filter '*SortAudit*Benchmark*' --artifacts output/sort-audit/baseline
./scripts/summarize-sort-audit.ps1
dotnet run -c Release --project tests/SortAlgorithm.Tests
```

`--audit` selects three warmups, fifteen measurement iterations, sixty-four fresh buffers per iteration, and disabled tiering. Do not override invocation count or unroll factor: each invocation consumes one restored buffer. Array restoration is in iteration setup. Each method returns the middle element of its output. Setup checks the selected workload against natural ordering before timing.

The separate verification command checks every registered public sorting type, rejects missing registrations, verifies supported inputs inside sentinel-protected slices, and checks documented rejection of non-power-of-two Bitonic inputs and excessive integer counting/pigeonhole ranges. BogoSort and BozoSort are limited to seven elements, SlowSort to thirty-two, and StoogeSort to sixty-four. These limits bound the audit, not the algorithms.

Run benchmarks without concurrent builds or test runs, and inspect the identical-code canary before accepting a small delta. The broad matrix is a screening run; shortlisted changes require repeated, same-run comparisons and disassembly.
