param(
    [string]$Artifacts = 'output/sort-audit/baseline/results',
    [string]$Destination = '.github/docs/reports/sort-audit-2026-11-14-matrix.md'
)

$ErrorActionPreference = 'Stop'
$culture = [System.Globalization.CultureInfo]::InvariantCulture
$reports = @(Get-ChildItem -LiteralPath $Artifacts -Filter '*-report-full.json' | ForEach-Object {
    Get-Content -LiteralPath $_.FullName -Raw | ConvertFrom-Json
})
$rows = @($reports | ForEach-Object { $_.Benchmarks })
if ($rows.Count -eq 0 -or @($rows | Where-Object { $null -eq $_.Statistics }).Count -ne 0) {
    throw 'Missing measurements; the matrix must not present failed cases as timings.'
}

function Format-Time($row) {
    return ($row.Statistics.Mean / 1000).ToString('F3', $culture)
}

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# NullContext audit: complete timing matrix')
$lines.Add('')
$lines.Add('Measured on 2026-11-14. See [scope and findings](sort-audit-2026-11-14.md).')
$lines.Add('')
$lines.Add('Times are mean microseconds per sort. Every cell is **small / large**. Allocations are bytes per sort, also small / large. All six patterns are measured at both sizes. A dagger marks an implementation with at least one scenario whose standard deviation exceeds 10% of its mean; use the raw confidence intervals before drawing small-delta conclusions.')
$lines.Add('')
$lines.Add('| Implementation | Sizes | Random | One moved | Sorted | Reversed | Pipe organ | Duplicates | B/op |')
$lines.Add('|---|---|---:|---:|---:|---:|---:|---:|---:|')
foreach ($group in ($rows | Group-Object Method | Sort-Object Name)) {
    $sizes = @($group.Group | ForEach-Object { if ($_.Parameters -match 'Size=(\d+)') { [int]$Matches[1] } } | Sort-Object -Unique)
    if ($sizes.Count -ne 2 -or $group.Count -ne 12) { throw "Incomplete matrix for $($group.Name)" }
    $cells = foreach ($pattern in @('Random', 'SingleElementMoved', 'Sorted', 'Reversed', 'PipeOrgan', 'ManyDuplicates')) {
        $pair = foreach ($size in $sizes) {
            $row = @($group.Group | Where-Object { $_.Parameters -eq "Size=$size&Pattern=$pattern" })
            if ($row.Count -ne 1) { throw "Missing or duplicate scenario: $($group.Name)/$size/$pattern" }
            Format-Time $row[0]
        }
        $pair -join ' / '
    }
    $allocation = foreach ($size in $sizes) {
        $values = @($group.Group | Where-Object { $_.Parameters -match "^Size=$size&" } | ForEach-Object { $_.Memory.BytesAllocatedPerOperation } | Sort-Object -Unique)
        if ($values.Count -eq 1) { "$($values[0])" } else { "$($values[0])–$($values[-1])" }
    }
    $noisy = @($group.Group | Where-Object { $_.Statistics.StandardDeviation -gt $_.Statistics.Mean * .1 }).Count -gt 0
    $label = $group.Name + $(if ($noisy) { ' †' } else { '' })
    $lines.Add('| ' + $label + ' | ' + ($sizes -join ' / ') + ' | ' + ($cells -join ' | ') + ' | ' + ($allocation -join ' / ') + ' |')
}
$lines.Add('')
$lines.Add("Coverage: $(@($rows | Group-Object Method).Count) implementations, $($rows.Count) successful BenchmarkDotNet scenarios.")
Set-Content -LiteralPath $Destination -Value $lines
