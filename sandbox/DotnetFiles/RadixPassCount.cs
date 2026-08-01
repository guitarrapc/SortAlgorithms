#:project ../../src/SortAlgorithm
#:project ../../src/SortAlgorithm.Benchmark

using SortAlgorithm.Algorithms;
using SortAlgorithm.Benchmark;
using SortAlgorithm.Contexts;

// Counts the actual RadixPass notifications for the benchmark inputs, so the pass-count claim
// is measured rather than derived. Deterministic: no timing involved.
static int Count(Action<PassCounter> sort)
{
    var counter = new PassCounter();
    sort(counter);
    return counter.Passes;
}

foreach (var size in new[] { 1024, 8192 })
{
    var nonNegative = Enumerable.Range(1, size).ToArray();
    var straddling = Enumerable.Range(-(size / 2), size).ToArray();

    foreach (var (label, source) in new[] { ("non-negative [1, n]", nonNegative), ("straddling  [-n/2, n/2)", straddling) })
    {
        var a = source.ToArray();
        var b = source.ToArray();
        var c4 = source.ToArray();
        var c256 = source.ToArray();
        var p4 = Count(c => RadixLSD4SortXorBaseline.Sort(a.AsSpan(), c));
        var p256 = Count(c => RadixLSD256SortXorBaseline.Sort(b.AsSpan(), c));
        var n4 = Count(c => RadixLSD4Sort.Sort(c4.AsSpan(), c));
        var n256 = Count(c => RadixLSD256Sort.Sort(c256.AsSpan(), c));
        var ok = a.SequenceEqual(c4) && b.SequenceEqual(c256) && c4.SequenceEqual(c256);
        Console.WriteLine($"n={size,5} {label}  LSD4 {p4,2} -> {n4,2} passes   LSD256 {p256} -> {n256} passes   sameResult={ok}");
    }
}

sealed class PassCounter : ISortContext
{
    public int Passes { get; private set; }
    public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
    {
        if (phase == SortPhase.RadixPass) Passes++;
    }
    public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
    public void OnSwap(int i, int j, int bufferId) { }
    public void OnIndexRead(int index, int bufferId) { }
    public void OnIndexWrite(int index, int bufferId) { }
    public void OnIndexWrite<T>(int index, int bufferId, T value) { }
    public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
    public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
    public void OnRole(int index, int bufferId, RoleType role) { }
}
