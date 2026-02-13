# SortAlgorithmLab - Project Instructions

## What is this project?

This is a C# sorting algorithm laboratory for educational and performance analysis purposes. It implements various sorting algorithms with comprehensive statistics tracking and visualization support.

**Tech Stack:** C# (.NET 10+), xUnit, BenchmarkDotNet

## Project Structure

- `src/SortAlgorithm/` - Core sorting algorithms and interfaces
  - `Algorithms/` - Sorting algorithm implementations
  - `Contexts/` - Statistics and visualization contexts
- `tests/SortAlgorithm.Tests/` - Unit tests for all algorithms
- `sandbox/` - Benchmark and experimental code
- `.github/agent_docs/` - Detailed implementation guidelines

## How to Work on This Project

### Running Tests

```shell
dotnet test
```

### Running Benchmarks

```shell
cd sandbox/SandboxBenchmark
dotnet run -c Release
```

### Building the Project

```shell
dotnet build
```

### Run Some Script

**IMPORTANT:** Never use `dotnet script` or `dotnet-script` command. This project does NOT use dotnet-script.

If you need to create a .cs file to verify something, you can create it in the `sandbox/DotnetFiles/` folder and run it.

See `dotnet run` details here: https://github.com/dotnet/sdk/blob/main/documentation/general/dotnet-run-file.md

- For a standalone C# file (without .csproj):

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

var stats = new StatisticsContext();
PowerSort.Sort<int>([ 5, 3, 8, 1, 2 ], stats);

Console.WriteLine("Sorted array with PowerSort.");
Console.WriteLine($"Compares: {stats.CompareCount}, Swaps: {stats.SwapCount}, IndexReads: {stats.IndexReadCount}, IndexWrites: {stats.IndexWriteCount}");
```

```shell
# Create a single .cs file and run it directly
dotnet run dotnet run sandbox/DotnetFiles/YourCsFile.cs
```

- For a project folder with .csproj:

```shell
cd sandbox/YourProjectFolder
dotnet run -c Release
# Or specify the project file:
dotnet run -c Release --project YourProjectName.csproj
```


## Important Guidelines

When implementing or reviewing sorting algorithms, refer to these detailed guides:

- **[Architecture](.github/agent_docs/architecture.md)** - Understand the Context + SortSpan pattern
- **[Performance Requirements](.github/agent_docs/performance_requirements.md)** - Zero-allocation, aggressive inlining, and memory management
- **[SortSpan Usage](.github/agent_docs/sortspan_usage.md)** - How to use SortSpan for all operations
- **[Implementation Template](.github/agent_docs/implementation_template.md)** - Template for new algorithms
- **[Coding Style](.github/agent_docs/coding_style.md)** - C# style conventions for this project
- **[Testing Guidelines](.github/agent_docs/testing_guidelines.md)** - Writing/Run effective unit tests

**Key Rule:** Always use `SortSpan<T, TComparer>` methods (`Read`, `Write`, `Compare`, `Swap`, `CopyTo`) instead of direct array access. This ensures accurate statistics tracking. All algorithms use the generic `TComparer : IComparer<T>` pattern for zero-allocation devirtualized comparisons, with convenience overloads that delegate via `new ComparableComparer<T>()`. This follows the same pattern as `MemoryExtensions.Sort` in dotnet/runtime - runtime validation instead of compile-time constraints.

## Progressive Disclosure

Before implementing a new sorting algorithm or making significant changes:

1. Read the relevant documentation files in `.github/agent_docs/`
2. Review existing similar implementations in `src/SortAlgorithm/Algorithms/`
3. Check corresponding tests in `tests/SortAlgorithm.Tests/`

Ask which documentation files you need if you're unsure what to read.
