using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Detectors;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using SortAlgorithm.Benchmark;
using System.Reflection;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

const string RequiredCpu = "EPYC 9V74";

if (args.Contains("--audit-verify"))
{
    SortAuditVerification.Run();
    return;
}

var audit = args.Contains("--audit");
args = Array.FindAll(args, static a => a != "--audit");

var allowAnyCpu = Array.IndexOf(args, "--allow-any-cpu") >= 0;
args = Array.FindAll(args, static a => a != "--allow-any-cpu");

if (!allowAnyCpu && IsCiEnvironment())
{
    // Some runners of the required model have no AVX-512; the SIMD sorts then fall back to 256-bit vectors.
    var processorName = CpuDetector.Cpu?.ProcessorName;
    var avx512 = Avx512F.IsSupported && Vector512.IsHardwareAccelerated;
    if (processorName is null || !processorName.Contains(RequiredCpu, StringComparison.OrdinalIgnoreCase) || !avx512)
    {
        Console.Error.WriteLine($"Benchmark CI requires CPU containing '{RequiredCpu}' with AVX-512.");
        Console.Error.WriteLine($"Detected: {processorName ?? "(unknown)"}{(avx512 ? "" : " (no AVX-512)")}");
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

if (audit)
{
    config.AddExporter(JsonExporter.Full);
    config.AddJob(Job.Default
        .WithWarmupCount(3)
        .WithIterationCount(15)
        .WithInvocationCount(SortBuffers.InvocationsPerIteration)
        .WithUnrollFactor(1)
        .WithEnvironmentVariable("DOTNET_TieredCompilation", "0"));
}
else
{
    config.AddFilter(new SimpleFilter(static b => !b.Descriptor.Type.Name.StartsWith("SortAudit", StringComparison.Ordinal)));
    // Per-iteration measurements feed the benchmark site (.github/scripts/BenchmarkData.cs).
    config.AddExporter(JsonExporter.FullCompressed);
}

// Every job MUST pin InvocationCount to the SortBuffers pool size (with UnrollFactor=1):
// each measured iteration consumes exactly one pre-copied buffer per invocation, keeping
// the Array.Copy restore cost in [IterationSetup] — outside the timed region. A pinned
// invocation count also skips the pilot stage, so per-case time stays bounded.
// ScalingBenchmark overrides the count with [InvocationCount(SortBuffers.LargeInvocationsPerIteration)]
// and builds its pool with the same count.
//
// DOTNET_TieredCompilation=0: with the short fixed warmup, tier-1 background compilation
// can land in the middle of the measured iterations (observed as a sharp drop partway
// through the iterations, e.g. PDQSort 8192/SingleElementMoved), producing bimodal rows where Median << Mean. Disabling tiering makes the first
// invocation run fully optimized code, so every iteration measures steady state.
// In Local environment, run the short benchmark.
if (!audit && !IsCiEnvironment())
{
    config.AddJob(Job.ShortRun
        .WithInvocationCount(SortBuffers.InvocationsPerIteration)
        .WithUnrollFactor(1)
        .WithEnvironmentVariable("DOTNET_TieredCompilation", "0"))
        .WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(Perfolizer.Horology.TimeUnit.Nanosecond));
}
else if (!audit)
{
    // CI: 8 iterations x 64 batched invocations gives far better precision than the old
    // IterationSetup/InvocationCount=1 mode while keeping ~1,400 cases within CI limits.
    config.AddJob(Job.Default
        .WithWarmupCount(2)
        .WithIterationCount(8)
        .WithInvocationCount(SortBuffers.InvocationsPerIteration)
        .WithUnrollFactor(1)
        .WithEnvironmentVariable("DOTNET_TieredCompilation", "0"))
        .WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(Perfolizer.Horology.TimeUnit.Nanosecond));
}

if (args.Length == 0)
{
    args = ["--filter", audit ? "*SortAudit*Benchmark*" : "*Benchmark*"];
}

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args, config);
