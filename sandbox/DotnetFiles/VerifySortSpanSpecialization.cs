#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:property AllowUnsafeBlocks=true

// Faithfully reproduce SortSpan's comparison paths to verify specialization impact.
// Simulates: struct comparer → AggressiveInlining → CompareTo chain.
//
// Run:  dotnet run sandbox/DotnetFiles/VerifySortSpanSpecialization.cs

using System.Diagnostics;
using System.Runtime.CompilerServices;

const int N = 100_000_000;
var rng = new Random(42);
var data = new int[4096];
for (int i = 0; i < data.Length; i++) data[i] = rng.Next();

// Warmup
Bench_Direct(data, 1_000_000);
Bench_CompareTo(data, 1_000_000);
Bench_StructComparer(data, 1_000_000);

// Actual runs (multiple iterations for stability)
double t1 = 0, t2 = 0, t3 = 0;
long c1 = 0, c2 = 0, c3 = 0;
const int runs = 5;
for (int r = 0; r < runs; r++)
{
    var sw = Stopwatch.StartNew();
    c1 = Bench_Direct(data, N);
    t1 += sw.Elapsed.TotalMilliseconds;

    sw.Restart();
    c2 = Bench_CompareTo(data, N);
    t2 += sw.Elapsed.TotalMilliseconds;

    sw.Restart();
    c3 = Bench_StructComparer(data, N);
    t3 += sw.Elapsed.TotalMilliseconds;
}
t1 /= runs; t2 /= runs; t3 /= runs;

Console.WriteLine($"=== SortSpan comparison verification (N={N:N0}, {runs} runs avg) ===");
Console.WriteLine();
Console.WriteLine($"1. Unsafe.As<T,int> path (specialization)         : {t1,8:F1} ms  count={c1}");
Console.WriteLine($"2. int.CompareTo(int) < 0 (direct, no wrapper)    : {t2,8:F1} ms  count={c2}");
Console.WriteLine($"3. StructComparer.Compare(x,y) < 0 (full chain)   : {t3,8:F1} ms  count={c3}");
Console.WriteLine();
Console.WriteLine($"Ratio 2/1 (CompareTo / specialized)  = {t2 / t1:F3}");
Console.WriteLine($"Ratio 3/1 (full chain / specialized) = {t3 / t1:F3}");
Console.WriteLine();

if (t3 / t1 > 1.10)
    Console.WriteLine("=> CONFIRMED: Type specialization is effective. CompareTo < 0 does NOT fold to x < y.");
else
    Console.WriteLine("=> Specialization makes no measurable difference.");

// === Path 1: Direct comparison (what Unsafe.As specialization produces) ===
[MethodImpl(MethodImplOptions.NoInlining)]
static long Bench_Direct(int[] data, int n)
{
    long count = 0;
    int mask = data.Length - 1;
    for (int i = 0; i < n; i++)
    {
        if (DirectLessThan(data[i & mask], data[(i + 1) & mask]))
            count++;
    }
    return count;
}

// After JIT specialization: Unsafe.As<T,int>(ref a) < Unsafe.As<T,int>(ref b) → a < b
[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool DirectLessThan(int a, int b) => a < b;

// === Path 2: int.CompareTo < 0 (what the fallback produces after inlining) ===
[MethodImpl(MethodImplOptions.NoInlining)]
static long Bench_CompareTo(int[] data, int n)
{
    long count = 0;
    int mask = data.Length - 1;
    for (int i = 0; i < n; i++)
    {
        if (CompareToLessThan(data[i & mask], data[(i + 1) & mask]))
            count++;
    }
    return count;
}

// int.CompareTo(int) → {-1, 0, 1} → < 0
[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool CompareToLessThan(int a, int b) => a.CompareTo(b) < 0;

// === Path 3: Full chain through struct comparer (mirrors SortSpan's actual path) ===
[MethodImpl(MethodImplOptions.NoInlining)]
static long Bench_StructComparer(int[] data, int n)
{
    long count = 0;
    int mask = data.Length - 1;
    var comparer = new MyStructComparer();
    for (int i = 0; i < n; i++)
    {
        if (StructComparerLessThan(data[i & mask], data[(i + 1) & mask], comparer))
            count++;
    }
    return count;
}

// Mirrors: _comparer.Compare(a, b) < 0 where _comparer is struct
[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool StructComparerLessThan<TComparer>(int a, int b, TComparer comparer)
    where TComparer : IComparer<int>
    => comparer.Compare(a, b) < 0;

// Mirrors ComparableComparer<int>: readonly struct, IComparer<int>, AggressiveInlining
readonly struct MyStructComparer : IComparer<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(int x, int y) => x.CompareTo(y);
}
