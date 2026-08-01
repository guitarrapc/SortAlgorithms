#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Measures how much work the MSD radix sorts actually do, so claims about wasted passes are counted
// rather than reasoned about. Deterministic: no timing involved.

static (int passes, long reads, long writes, long copied) Run(Action<Probe> sort)
{
    var p = new Probe();
    sort(p);
    return (p.Passes, p.Reads, p.Writes, p.Copied);
}

static void Report(string label, int n, (int passes, long reads, long writes, long copied) r, bool ok)
{
    Console.WriteLine($"  {label,-18} passes={r.passes,3}  reads={r.reads,10:N0} ({r.reads / (double)n,6:F1}n)  writes={r.writes,10:N0} ({r.writes / (double)n,5:F1}n)  copied={r.copied,10:N0} ({r.copied / (double)n,5:F1}n)  sorted={ok}");
}

var rng = new Random(42);

foreach (var n in new[] { 1024, 8192 })
{
    var cases = new (string Label, int[] Data)[]
    {
        ("small 0..999",        Enumerable.Range(0, n).Select(_ => rng.Next(0, 1000)).ToArray()),
        ("straddling -500..499",Enumerable.Range(0, n).Select(_ => rng.Next(-500, 500)).ToArray()),
        ("full int range",      Enumerable.Range(0, n).Select(_ => rng.Next(int.MinValue, int.MaxValue)).ToArray()),
        ("all identical",       Enumerable.Repeat(7, n).ToArray()),
    };

    foreach (var (label, data) in cases)
    {
        Console.WriteLine($"n={n} {label}");
        var expected = data.OrderBy(x => x).ToArray();

        var a = data.ToArray();
        var b = data.ToArray();
        var c = data.ToArray();
        Report("RadixMSD4Sort", n, Run(p => RadixMSD4Sort.Sort(a.AsSpan(), p)), a.SequenceEqual(expected));
        Report("RadixMSD10Sort", n, Run(p => RadixMSD10Sort.Sort(b.AsSpan(), p)), b.SequenceEqual(expected));
        Report("AmericanFlagSort", n, Run(p => AmericanFlagSort.Sort(c.AsSpan(), p)), c.SequenceEqual(expected));
        Console.WriteLine();
    }
}

// -0.0 / +0.0: the digit passes order by the IEEE total-order key (-0 before +0), the insertion-sort
// cutoff orders by ComparableComparer (which treats them equal). Show which one a given length gets.
foreach (var n in new[] { 8, 64 })
{
    var d = new double[n];
    for (var i = 0; i < n; i++) d[i] = i % 2 == 0 ? 0.0 : -0.0;
    RadixMSD4Sort.Sort(d.AsSpan());
    var firstIsNegativeZero = double.IsNegative(d[0]);
    var allNegativeZeroFirst = d.TakeWhile(double.IsNegative).Count() == n / 2;
    Console.WriteLine($"double[{n}] alternating +0/-0 -> RadixMSD4Sort: first is -0 = {firstIsNegativeZero}, all -0 grouped first = {allNegativeZeroFirst}");
}

sealed class Probe : ISortContext
{
    public int Passes { get; private set; }
    public long Reads { get; private set; }
    public long Writes { get; private set; }
    public long Copied { get; private set; }

    public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
    {
        if (phase == SortPhase.RadixPass) Passes++;
    }
    public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
    public void OnSwap(int i, int j, int bufferId) { Reads += 2; Writes += 2; }
    public void OnIndexRead(int index, int bufferId) => Reads++;
    public void OnIndexWrite(int index, int bufferId) => Writes++;
    public void OnIndexWrite<T>(int index, int bufferId, T value) => Writes++;
    public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) => Copied += length;
    public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) => Copied += length;
    public void OnRole(int index, int bufferId, RoleType role) { }
    public void OnLink(int parentIndex, int childIndex, int bufferId, LinkSide side) { }
}
