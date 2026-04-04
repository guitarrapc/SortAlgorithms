# Run JIT disasm for GenericArraySortHelper to compare int vs user-defined type
$env:DOTNET_TieredCompilation = "0"
$env:DOTNET_JitDisasm = "GenericArraySortHelper*"
$env:DOTNET_JitDisasmSummary = "1"

Write-Host "=== JIT Disasm: GenericArraySortHelper* ===" -ForegroundColor Cyan

$output = dotnet run sandbox/DotnetFiles/InvestigateSortUtils.cs 2>&1
$lines = $output -split "`n"

# Find compiled methods
$methods = $lines | Where-Object { $_ -match "Assembly listing for method" }
Write-Host "`n=== Compiled methods ===" -ForegroundColor Yellow
$methods | ForEach-Object { Write-Host $_ }

# Find call instructions (non-inlined calls indicate bottlenecks)
Write-Host "`n=== Call instructions in sort methods ===" -ForegroundColor Yellow
$inSortMethod = $false
$currentMethod = ""
foreach ($line in $lines) {
    if ($line -match "Assembly listing for method (.+)") {
        $currentMethod = $matches[1]
        $inSortMethod = $currentMethod -match "IntroSort|PickPivot|LessThan|GreaterThan"
    }
    if ($inSortMethod -and $line -match "^\s+[0-9a-fA-F]+\s+call") {
        Write-Host "  [$currentMethod] $($line.Trim())"
    }
}

# Total line count
$totalLines = $lines.Length
Write-Host "`nTotal assembly lines: $totalLines"

$env:DOTNET_TieredCompilation = $null
$env:DOTNET_JitDisasm = $null
$env:DOTNET_JitDisasmSummary = $null
