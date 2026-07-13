using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Detectors;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using SortAlgorithm.Benchmark;
using System.Reflection;

const string RequiredCpu = "EPYC 7763";

var allowAnyCpu = Array.IndexOf(args, "--allow-any-cpu") >= 0;
args = Array.FindAll(args, static a => a != "--allow-any-cpu");

if (!allowAnyCpu && IsCiEnvironment())
{
    var processorName = CpuDetector.Cpu?.ProcessorName;
    if (processorName is null || !processorName.Contains(RequiredCpu, StringComparison.OrdinalIgnoreCase))
    {
        Console.Error.WriteLine($"Benchmark CI requires CPU containing '{RequiredCpu}'.");
        Console.Error.WriteLine($"Detected: {processorName ?? "(unknown)"}");
        Console.Error.WriteLine("Pass --allow-any-cpu to override.");
        Environment.Exit(1);
    }
}

static bool IsCiEnvironment() =>
    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")) ||
    string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), "true", StringComparison.OrdinalIgnoreCase);

var config = ManualConfig.CreateMinimumViable()
    .AddDiagnoser(MemoryDiagnoser.Default)
    //.AddExporter(DefaultExporters.Plain)
    .AddExporter(MarkdownExporter.Default)
    .AddExporter(MarkdownExporter.GitHub);

// Every job MUST pin InvocationCount to the SortBuffers pool size (with UnrollFactor=1):
// each measured iteration consumes exactly one pre-copied buffer per invocation, keeping
// the Array.Copy restore cost in [IterationSetup] — outside the timed region. A pinned
// invocation count also skips the pilot stage, so per-case time stays bounded.
//
// DOTNET_TieredCompilation=0: with the short fixed warmup, tier-1 background compilation
// can land in the middle of the measured iterations (observed as a sharp 10-15x drop,
// e.g. PDQSort 8192/SingleElementMoved: iterations 1-6 at ~350us, iteration 8 at ~25us),
// producing bimodal rows where Median << Mean. Disabling tiering makes the first
// invocation run fully optimized code, so every iteration measures steady state.
// In Local environment, run the short benchmark.
if (!IsCiEnvironment())
{
    config.AddJob(Job.ShortRun
        .WithInvocationCount(SortBuffers.InvocationsPerIteration)
        .WithUnrollFactor(1)
        .WithEnvironmentVariable("DOTNET_TieredCompilation", "0"));
}
else
{
    // CI: 8 iterations x 64 batched invocations gives far better precision than the old
    // IterationSetup/InvocationCount=1 mode while keeping ~1,400 cases within CI limits.
    config.AddJob(Job.Default
        .WithWarmupCount(2)
        .WithIterationCount(8)
        .WithInvocationCount(SortBuffers.InvocationsPerIteration)
        .WithUnrollFactor(1)
        .WithEnvironmentVariable("DOTNET_TieredCompilation", "0"));
}

if (args.Length == 0)
{
    args = ["--filter", "*Benchmark*"];
}

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args, config);
