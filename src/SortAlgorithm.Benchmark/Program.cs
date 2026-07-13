using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Horology;
using System.Reflection;

var config = ManualConfig.CreateMinimumViable()
    .AddDiagnoser(MemoryDiagnoser.Default)
    //.AddExporter(DefaultExporters.Plain)
    .AddExporter(MarkdownExporter.Default);

// In Local environment, run the short benchmark.
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")))
{
    config.AddJob(Job.ShortRun);
}
else
{
    // CI: bound the per-case cost. Without IterationSetup, BenchmarkDotNet batches
    // thousands of invocations per iteration, so the default pipeline (>=15 iterations
    // x 500ms IterationTime + pilot + one process launch per case) takes 10-30s per
    // case — far too slow for ~1,400 cases.
    // - InProcessEmitToolchain removes the per-case process launch (~1s each), the
    //   dominant fixed cost. CI reports are relative comparisons, so losing process
    //   isolation is an acceptable trade-off here.
    // - 6 iterations x 48ms keeps invocation batching (the precision win over the old
    //   IterationSetup/InvocationCount=1 mode) at a fraction of the default budget.
    // InProcessEmitToolchain may cause side-effects.
    // - Sometimes showing 1-4B illusionary allocations per case, but the relative comparison is still valid.
    // - No Process isolation may affect other benchmarks, it means outliers may be more common.
    config.AddJob(Job.Default
        .WithToolchain(InProcessEmitToolchain.Instance)
        .WithWarmupCount(1)
        .WithIterationCount(6)
        .WithIterationTime(TimeInterval.FromMilliseconds(48)));
}

if (args.Length == 0)
{
    args = ["--filter", "*Benchmark*"];
}

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args, config);
