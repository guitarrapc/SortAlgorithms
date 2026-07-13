using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
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
    // x 500ms IterationTime + pilot) takes 10-30s per case — far too slow for ~1,400
    // cases. 8 iterations x 64ms keeps invocation batching (the precision win) while
    // bringing per-case time back to roughly the pre-GlobalSetup level.
    config.AddJob(Job.Default
        .WithWarmupCount(2)
        .WithIterationCount(8)
        .WithIterationTime(TimeInterval.FromMilliseconds(64)));
}

if (args.Length == 0)
{
    args = ["--filter", "*Benchmark*"];
}

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args, config);
