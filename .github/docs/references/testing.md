# Testing Reference

The repository uses TUnit with Microsoft.Testing.Platform.

Run the full suite from the repository root:

```powershell
dotnet test
```

Run the test project directly or filter a tree node:

```powershell
dotnet run --project tests/SortAlgorithm.Tests
dotnet run --project tests/SortAlgorithm.Tests -- --treenode-filter "/*/*/PowerSortTests/*"
```

Use the mock data sources under `tests/SortAlgorithm.Tests/Mocks/` rather than duplicating common patterns. A typical correctness test copies the sample, sorts through a `StatisticsContext`, creates the expected order with `Array.Sort`, and compares with `CollectionOrdering.Matching`.

For stable algorithms, sort records containing both a key and original position, then assert both key order and preserved positions for equal keys. For shared infrastructure, test `NullContext`, an observable context, and a custom comparer. See [validation.md](../specs/validation.md) for the required behavioral classes.

