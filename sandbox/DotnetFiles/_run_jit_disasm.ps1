$env:DOTNET_JitDisasm = "PDQSort*"
$env:DOTNET_TieredCompilation = "0"
$output = & dotnet run -c Release sandbox/DotnetFiles/JitDisasmPDQSort.cs 2>&1
$env:DOTNET_JitDisasm = ""
$env:DOTNET_TieredCompilation = ""

$output | Out-File -FilePath sandbox/DotnetFiles/_jit_pdqsort_full.txt -Encoding utf8
Write-Host "Total lines: $(($output).Count)"

# Methods with own JIT compilation
Write-Host "`n=== Compiled methods ==="
$output | Where-Object { $_ -match "; Assembly listing" } | ForEach-Object {
    ($_ -replace "SortAlgorithm\.Algorithms\.", "") -replace "\[int,SortAlgorithm[^\]]+\]", "[int,...]"
}

# Remaining call instructions (not inlined)
Write-Host "`n=== Non-inlined calls ==="
$output | Where-Object { $_ -match "^\s+call\s" } | ForEach-Object {
    ($_.Trim() -replace "SortAlgorithm\.Algorithms\.", "") -replace "\[int,SortAlgorithm[^\]]*\]", "[int,...]"
} | Sort-Object | Get-Unique
