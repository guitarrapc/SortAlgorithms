$env:DOTNET_JitDisasm = "PDQSort:Partition*"
$env:DOTNET_TieredCompilation = "0"
$output = & dotnet run -c Release sandbox/DotnetFiles/JitDisasmPDQSort.cs 2>&1
$env:DOTNET_JitDisasm = ""
$env:DOTNET_TieredCompilation = ""

Write-Host "Total lines: $($output.Count)"
Write-Host "`n=== Compiled methods ==="
$output | Where-Object { $_ -match "; Assembly listing" } | ForEach-Object {
    ($_ -replace "SortAlgorithm\.Algorithms\.", "") -replace "\[int,SortAlgorithm[^\]]+\]", "[int,...]"
}
Write-Host "`n=== Non-inlined calls ==="
$output | Where-Object { $_ -match "^\s+call\s" } | ForEach-Object {
    ($_.Trim() -replace "SortAlgorithm\.", "") -replace "\[int,.*?\]", "[int,...]"
} | Sort-Object | Get-Unique
