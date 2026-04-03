#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:property AllowUnsafeBlocks=true
#:project ../../src/SortAlgorithm

// Verify whether the type-specialized IsLessThan produces different codegen
// from the generic fallback (ComparableComparer<int>.Compare < 0).
//
// Run with:   dotnet run sandbox/DotnetFiles/VerifyIsLessThanCodegen.cs
//
// To view JIT disassembly for specific methods, set:
//   $env:DOTNET_JitDisasm = "WithSpecialization|WithoutSpecialization|ComparableOnly"
//   dotnet run sandbox/DotnetFiles/VerifyIsLessThanCodegen.cs

using System.Diagnostics;
using System.Runtime.CompilerServices;
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// === Benchmark Setup ===
const int N = 100_000_000;
var rng = new Random(42);
var a = new int[1024];
var b = new int[1024];
for (int i = 0; i < a.Length; i++) { a[i] = rng.Next(); b[i] = rng.Next(); }

// Warmup
RunWithSpecialization(a, b, 1_000_000);
RunWithoutSpecialization(a, b, 1_000_000);
RunComparableOnly(a, b, 1_000_000);

// Benchmark
var sw = Stopwatch.StartNew();
var count1 = RunWithSpecialization(a, b, N);
var t1 = sw.Elapsed;

sw.Restart();
var count2 = RunWithoutSpecialization(a, b, N);
var t2 = sw.Elapsed;

sw.Restart();
var count3 = RunComparableOnly(a, b, N);
var t3 = sw.Elapsed;

Console.WriteLine($"=== IsLessThan codegen verification (N={N:N0}) ===");
Console.WriteLine($"WithSpecialization     (Unsafe.As path) : {t1.TotalMilliseconds,8:F1} ms  (count={count1})");
Console.WriteLine($"WithoutSpecialization  (Compare < 0)    : {t2.TotalMilliseconds,8:F1} ms  (count={count2})");
Console.WriteLine($"ComparableOnly         (raw CompareTo)  : {t3.TotalMilliseconds,8:F1} ms  (count={count3})");
Console.WriteLine();
Console.WriteLine($"Ratio without/with = {t2.TotalMilliseconds / t1.TotalMilliseconds:F3}");
Console.WriteLine($"Ratio comparable/with = {t3.TotalMilliseconds / t1.TotalMilliseconds:F3}");

if (Math.Abs(t2.TotalMilliseconds / t1.TotalMilliseconds - 1.0) < 0.05)
    Console.WriteLine("\n=> Specialization makes NO measurable difference. JIT already optimizes CompareTo < 0.");
else if (t2.TotalMilliseconds > t1.TotalMilliseconds * 1.05)
    Console.WriteLine("\n=> Specialization IS faster. The JIT does NOT fully optimize CompareTo < 0 in this context.");
else
    Console.WriteLine("\n=> Specialization is slightly slower (noise or harmful).");

// === Method 1: Current code with Unsafe.As specialization ===
[MethodImpl(MethodImplOptions.NoInlining)]
static long RunWithSpecialization(int[] a, int[] b, int n)
{
    long count = 0;
    int mask = a.Length - 1;
    for (int i = 0; i < n; i++)
    {
        // This mirrors the specialized path: Unsafe.As<T, int>(ref a) < Unsafe.As<T, int>(ref b)
        if (WithSpecialization(a[i & mask], b[i & mask]))
            count++;
    }
    return count;
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool WithSpecialization(int x, int y) => x < y;

// === Method 2: Generic comparer fallback (ComparableComparer<int>.Compare < 0) ===
[MethodImpl(MethodImplOptions.NoInlining)]
static long RunWithoutSpecialization(int[] a, int[] b, int n)
{
    long count = 0;
    int mask = a.Length - 1;
    for (int i = 0; i < n; i++)
    {
        if (WithoutSpecialization(a[i & mask], b[i & mask]))
            count++;
    }
    return count;
}

// Simulates the generic comparer fallback: struct comparer → Compare → CompareTo → result < 0
[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool WithoutSpecialization(int x, int y) => StructCompare(x, y) < 0;

// Mirrors ComparableComparer<int>.Compare — struct, AggressiveInlining, null check + CompareTo
[MethodImpl(MethodImplOptions.AggressiveInlining)]
static int StructCompare(int x, int y) => x.CompareTo(y);

// === Method 3: Direct int.CompareTo (no wrapper) ===
[MethodImpl(MethodImplOptions.NoInlining)]
static long RunComparableOnly(int[] a, int[] b, int n)
{
    long count = 0;
    int mask = a.Length - 1;
    for (int i = 0; i < n; i++)
    {
        if (ComparableOnly(a[i & mask], b[i & mask]))
            count++;
    }
    return count;
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool ComparableOnly(int x, int y) => x.CompareTo(y) < 0;
