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
