namespace SandboxBenchmark;

/// <summary>
/// Benchmark to measure the impact of different InsertionSort thresholds in IntroSort.
/// Tests the hypothesis that primitive types benefit from larger thresholds (30) while
/// reference types or complex types benefit from smaller thresholds (8).
/// 
/// This corresponds to C++ std::introsort's optimization:
/// const difference_type __limit = is_trivially_copy_constructible&lt;value_type&gt;::value &amp;&amp;
///                                 is_trivially_copy_assignable&lt;value_type&gt;::value ? 30 : 6;
/// </summary>
[MemoryDiagnoser]
public class IntroSortThresholdBenchmark
{
    /// <summary>
    /// Array sizes to test.
    /// - 100: Small arrays where threshold matters most
    /// - 1000: Medium arrays where Ninther kicks in
    /// - 10000: Large arrays to measure overall impact
    /// </summary>
    [Params(256, 1024, 2048)]
    public int Size { get; set; }

    /// <summary>
    /// Data patterns to test.
    /// - Random: Typical unsorted data
    /// - Sorted: Best case for nearly-sorted detection
    /// - Reversed: Worst case for some pivot strategies
    /// - NearlySorted: 95% sorted, tests nearly-sorted optimization
    /// </summary>
    [Params(DataPattern.Random, DataPattern.Sorted, DataPattern.Reversed, DataPattern.NearlySorted)]
    public DataPattern Pattern { get; set; }

    // Primitive type tests (int)
    private int[] _intThreshold8 = default!;
    private int[] _intThreshold16 = default!;
    private int[] _intThreshold30 = default!;

    // Reference type tests (string)
    private string[] _stringThreshold8 = default!;
    private string[] _stringThreshold16 = default!;
    private string[] _stringThreshold30 = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        // Setup will be called for each combination of Size and Pattern
        _intThreshold8 = BenchmarkData.GenerateIntArray(Size, Pattern);
        _intThreshold16 = BenchmarkData.GenerateIntArray(Size, Pattern);
        _intThreshold30 = BenchmarkData.GenerateIntArray(Size, Pattern);

        _stringThreshold8 = BenchmarkData.GenerateStringArray(Size, Pattern);
        _stringThreshold16 = BenchmarkData.GenerateStringArray(Size, Pattern);
        _stringThreshold30 = BenchmarkData.GenerateStringArray(Size, Pattern);
    }

    // ==================== Primitive Type (int) Benchmarks ====================

    [Benchmark(Baseline = true)]
    public void Int_Threshold16_Current()
    {
        SortAlgorithm.Algorithms.IntroSort.SortWithCustomThreshold(_intThreshold16.AsSpan(), 16);
    }

    [Benchmark]
    public void Int_Threshold8_Small()
    {
        SortAlgorithm.Algorithms.IntroSort.SortWithCustomThreshold(_intThreshold8.AsSpan(), 8);
    }

    [Benchmark]
    public void Int_Threshold30_Large()
    {
        SortAlgorithm.Algorithms.IntroSort.SortWithCustomThreshold(_intThreshold30.AsSpan(), 30);
    }

    // ==================== Reference Type (string) Benchmarks ====================

    [Benchmark]
    public void String_Threshold16_Current()
    {
        SortAlgorithm.Algorithms.IntroSort.SortWithCustomThreshold(_stringThreshold16.AsSpan(), 16);
    }

    [Benchmark]
    public void String_Threshold8_Small()
    {
        SortAlgorithm.Algorithms.IntroSort.SortWithCustomThreshold(_stringThreshold8.AsSpan(), 8);
    }

    [Benchmark]
    public void String_Threshold30_Large()
    {
        SortAlgorithm.Algorithms.IntroSort.SortWithCustomThreshold(_stringThreshold30.AsSpan(), 30);
    }
}
