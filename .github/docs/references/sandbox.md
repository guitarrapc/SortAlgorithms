# Sandbox Reference

Do not use `dotnet-script` or `dotnet script` in this repository.

Place disposable C# experiments in `sandbox/DotnetFiles/`. The .NET SDK supports file-based apps, including project references declared in the file:

```csharp
#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

var values = new[] { 5, 3, 8, 1, 2 };
var statistics = new StatisticsContext();
PowerSort.Sort(values.AsSpan(), statistics);
Console.WriteLine(string.Join(", ", values));
```

Run it from the repository root:

```powershell
dotnet run sandbox/DotnetFiles/YourFile.cs
```

For an existing sandbox project, use its project file explicitly:

```powershell
dotnet run -c Release --project sandbox/YourProject/YourProject.csproj
```

`sandbox/ScandumReferenceTrace/trace.c` prints the expected values of `ScandumReferenceTraceTests` from the C sources of quadsort and fluxsort. Its header lists the build steps.

`sandbox/SimdQuickSortReferenceTrace/trace.cpp` prints the rows of `SimdQuickSortReferenceTraceTests` from Bramas's `sort512.hpp`. It needs a CPU with AVX-512F; its header lists the build steps. Build it in a directory on the WSL file system: a copy of `sort512.hpp` edited in place under `/mnt/c` with `sed -i` came out with NUL bytes and did not compile, while a copy on the WSL file system built cleanly.
