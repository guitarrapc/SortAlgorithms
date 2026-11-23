# Starts the Benchmark workflow and re-runs its benchmark job until the runner has the pinned CPU with AVX-512.
# The workflow's "Check runner CPU" step fails within a minute on any other CPU. This script waits for
# that step, reports the runner's CPU from the step's annotation, and re-runs the failed jobs.
# Requires PowerShell 7.3+ and an authenticated gh. Run it from the repository root.
#
#   ./scripts/run-benchmark.ps1                            # full run ("*") on main
#   ./scripts/run-benchmark.ps1 -Benchmark '*TreeBenchmark*'
#   ./scripts/run-benchmark.ps1 -RunId 45002371533         # continue with an existing run
#   ./scripts/run-benchmark.ps1 -Wait                      # also wait for the whole run
param(
    [string]$Benchmark = '*',
    [long]$RunId = 0,
    [string]$Ref = 'main',
    [int]$MaxAttempts = 30,
    [int]$AttemptTimeoutMinutes = 30,
    [switch]$Wait
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true
$workflow = 'benchmark.yaml'
$jobName = 'benchmark'
$stepName = 'Check runner CPU'

function Get-LatestDispatchId {
    return [long](gh run list --workflow $workflow --event workflow_dispatch --limit 1 --json databaseId --jq '.[0].databaseId // 0')
}

# First annotation message on the job that matches $Pattern, as its first capture group.
function Get-Annotation([long]$JobId, [string]$Pattern) {
    foreach ($message in @(gh api "repos/{owner}/{repo}/check-runs/$JobId/annotations" --jq '.[].message')) {
        if ($message -match $Pattern) { return $Matches[1] }
    }
    return $null
}

if ($RunId -eq 0) {
    $before = Get-LatestDispatchId
    gh workflow run $workflow --ref $Ref -f "benchmark=$Benchmark" -f 'allow-any-cpu=false'
    for ($i = 0; $i -lt 40 -and $RunId -eq 0; $i++) {
        Start-Sleep -Seconds 3
        $latest = Get-LatestDispatchId
        if ($latest -gt $before) { $RunId = $latest }
    }
    if ($RunId -eq 0) { throw 'The dispatched run did not appear in the run list.' }
}
Write-Host "Run: $(gh run view $RunId --json url --jq .url)"

$attempt = [int](gh run view $RunId --json attempt --jq .attempt)
$passed = $false
$misses = [System.Collections.Generic.List[string]]::new()
for ($tries = 1; $tries -le $MaxAttempts; $tries++) {
    # Read the attempt's own jobs: right after a re-run, the run view can still show the previous attempt.
    $deadline = (Get-Date).AddMinutes($AttemptTimeoutMinutes)
    $conclusion = $null
    $job = $null
    while (-not $conclusion) {
        if ((Get-Date) -gt $deadline) { throw "Attempt ${attempt}: the CPU check did not finish within $AttemptTimeoutMinutes minutes." }
        Start-Sleep -Seconds 10
        $job = (gh api "repos/{owner}/{repo}/actions/runs/$RunId/attempts/$attempt/jobs" | ConvertFrom-Json).jobs |
            Where-Object name -eq $jobName | Select-Object -First 1
        if (-not $job) { continue }
        $conclusion = ($job.steps | Where-Object name -eq $stepName | Select-Object -First 1).conclusion
        if (-not $conclusion -and $job.status -eq 'completed') {
            throw "Attempt ${attempt}: the $jobName job ended ($($job.conclusion)) before the CPU check ran."
        }
    }

    if ($conclusion -in 'success', 'skipped') {
        $cpu = Get-Annotation $job.id '^CPU OK: (.+)$'
        Write-Host "Attempt ${attempt}: CPU check $conclusion$(if ($cpu) { " ($cpu)" })."
        $passed = $true
        break
    }

    $cpu = Get-Annotation $job.id "this runner has '([^']*)'"
    if (-not $cpu) { $cpu = 'unknown' }
    $misses.Add($cpu)
    Write-Host "Attempt ${attempt}: wrong CPU ($cpu), re-running."
    # gh run rerun accepts only a completed run; the jobs after benchmark are skipped within seconds.
    while ((gh run view $RunId --json status --jq .status) -ne 'completed') { Start-Sleep -Seconds 5 }
    gh run rerun $RunId --failed
    $attempt++
    while ([int](gh run view $RunId --json attempt --jq .attempt) -lt $attempt) { Start-Sleep -Seconds 3 }
}

if ($misses.Count -gt 0) {
    Write-Host 'Runners with another CPU:'
    $misses | Group-Object | Sort-Object Count -Descending | ForEach-Object { Write-Host ("  {0,3}  {1}" -f $_.Count, $_.Name) }
}
if (-not $passed) { throw "No runner with the required CPU after $MaxAttempts attempts. Continue with -RunId $RunId." }

if ($Wait) {
    gh run watch $RunId --interval 60 --exit-status
}
