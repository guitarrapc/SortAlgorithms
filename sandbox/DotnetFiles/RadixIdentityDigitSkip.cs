#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Digits examined (RadixPass notifications) vs distribute passes actually executed (write bursts),
// for inputs whose low digits are uniform. Deterministic: no timing involved.
static (int Examined, int Executed) Run(Action<SkipCounter> sort)
{
    var counter = new SkipCounter();
    sort(counter);
    return (counter.Examined, counter.Executed);
}

static void Report(string label, int[] source)
{
    var a = source.ToArray();
    var b = source.ToArray();
    var c = source.ToArray();
    var p4 = Run(x => RadixLSD4Sort.Sort(a.AsSpan(), x));
    var p256 = Run(x => RadixLSD256Sort.Sort(b.AsSpan(), x));
    var p10 = Run(x => RadixLSD10Sort.Sort(c.AsSpan(), x));
    var expected = source.OrderBy(v => v).ToArray();
    var ok = a.SequenceEqual(expected) && b.SequenceEqual(expected) && c.SequenceEqual(expected);
    Console.WriteLine($"{label,-34} LSD4 {p4.Executed,2}/{p4.Examined,-2}  LSD256 {p256.Executed}/{p256.Examined}  LSD10 {p10.Executed,2}/{p10.Examined,-2}  sorted={ok}");
}

var rng = new Random(42);
var n = 4096;
int[] Shuffled(Func<int, int> map) => Enumerable.Range(0, n).Select(map).OrderBy(_ => rng.Next()).ToArray();

Console.WriteLine("executed/examined digit passes");
Report("plain [0, n)", Shuffled(x => x));
Report("multiples of 1,000 (decimal-ish)", Shuffled(x => x * 1_000));
Report("multiples of 256 (byte-aligned)", Shuffled(x => x * 256));
Report("multiples of 65,536", Shuffled(x => x * 65_536));
Report("even values only", Shuffled(x => x * 2));
Report("negatives, multiples of 1,000", Shuffled(x => (x - n / 2) * 1_000));

sealed class SkipCounter : ISortContext
{
    public int Examined { get; private set; }
    public int Executed { get; private set; }
    private bool wroteThisPass;

    public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
    {
        if (phase != SortPhase.RadixPass) return;
        Examined++;
        wroteThisPass = false;
    }
    public void OnIndexWrite(int index, int bufferId)
    {
        if (!wroteThisPass) { wroteThisPass = true; Executed++; }
    }
    public void OnIndexWrite<T>(int index, int bufferId, T value) => OnIndexWrite(index, bufferId);
    public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
    public void OnSwap(int i, int j, int bufferId) { }
    public void OnIndexRead(int index, int bufferId) { }
    public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
    public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
    public void OnRole(int index, int bufferId, RoleType role) { }
}
