using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using System.Reflection;

// --verify: run the PDQSort variant correctness gate without benchmarking, then exit.
if (Array.IndexOf(args, "--verify") >= 0)
{
    SortAlgorithm.Benchmark.PDQVariants.VerifyAllVariantsOnce();
    Console.WriteLine("Correctness gate PASSED: all PDQSort variants match Array.Sort.");
    return;
}

// --micro: micro-optimization loop mode. Meaningful statistics (3 warmups / 15 iterations)
// plus JIT disassembly export. The default ShortRun (3 iterations, InvocationCount=1) is
// too noisy for accept/refute decisions at the μs scale.
var micro = Array.IndexOf(args, "--micro") >= 0;
args = Array.FindAll(args, static a => a != "--micro");

ManualConfig config;
if (micro)
{
    config = ManualConfig.CreateMinimumViable()
        .AddDiagnoser(MemoryDiagnoser.Default)
        .AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(
            maxDepth: 3,
            printSource: true,
            exportGithubMarkdown: true,
            exportCombinedDisassemblyReport: true)))
        .AddExporter(MarkdownExporter.GitHub)
        .AddJob(Job.Default.WithWarmupCount(3).WithIterationCount(15));
}
else
{
    config = ManualConfig.CreateMinimumViable()
        .AddDiagnoser(MemoryDiagnoser.Default)
        //.AddExporter(DefaultExporters.Plain)
        .AddExporter(MarkdownExporter.Default);

    // In Local environment, run the short benchmark.
    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")))
    {
        config.AddJob(Job.ShortRun);
    }
}


BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args, config);
