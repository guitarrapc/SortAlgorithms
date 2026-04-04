// Investigation: Why is span.Sort() so fast for int vs IntKey?
// Hypothesis: .NET 9+ has vectorized SIMD sort for primitive types

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

// --- Check intrinsics availability ---
Console.WriteLine("=== .NET Version & SIMD availability ===");
Console.WriteLine($"Runtime: {Environment.Version}");
Console.WriteLine($"Vector128 supported: {System.Runtime.Intrinsics.X86.Sse2.IsSupported}");
Console.WriteLine($"Vector256 supported: {System.Runtime.Intrinsics.X86.Avx2.IsSupported}");
Console.WriteLine($"Vector512 supported: {System.Runtime.Intrinsics.X86.Avx512F.IsSupported}");

// --- Check typeof(T) specialization paths in MemoryExtensions.Sort ---
Console.WriteLine("\n=== MemoryExtensions.Sort internals probe ===");

// Check if there's a TrySZSort or similar intrinsic for int
var memExtType = typeof(System.MemoryExtensions);
var sortMethods = memExtType.GetMethods(BindingFlags.Public | BindingFlags.Static)
    .Where(m => m.Name == "Sort" && m.IsGenericMethodDefinition)
    .ToArray();
Console.WriteLine($"MemoryExtensions.Sort overloads: {sortMethods.Length}");
foreach (var m in sortMethods)
{
    var parms = m.GetParameters();
    Console.WriteLine($"  Sort({string.Join(", ", parms.Select(p => p.ParameterType.Name))})");
}

// Check SpanSortHelper internals
Console.WriteLine("\n=== SpanSortHelper type probe ===");
// The type name may vary but let's search
var types = typeof(System.MemoryExtensions).Assembly.GetTypes()
    .Where(t => t.Name.Contains("Sort") || t.Name.Contains("Vsort") || t.Name.Contains("vsort"))
    .OrderBy(t => t.Name)
    .ToArray();
foreach (var t in types)
{
    Console.WriteLine($"  {t.FullName}");
}

// --- Timing test: int vs IntKey vs int[] (Array.Sort) ---
Console.WriteLine("\n=== Performance comparison ===");

const int Size = 8192;
const int Iterations = 1000;

var rng = new Random(42);
var baseData = Enumerable.Range(0, Size).Select(_ => rng.Next(0, Size)).ToArray();

// int span.Sort()
{
    var elapsed = Stopwatch.StartNew();
    for (int i = 0; i < Iterations; i++)
    {
        var arr = (int[])baseData.Clone();
        arr.AsSpan().Sort();
    }
    elapsed.Stop();
    Console.WriteLine($"int span.Sort()       avg: {elapsed.Elapsed.TotalMicroseconds / Iterations:F1} µs");
}

// int[] Array.Sort()
{
    var elapsed = Stopwatch.StartNew();
    for (int i = 0; i < Iterations; i++)
    {
        var arr = (int[])baseData.Clone();
        Array.Sort(arr);
    }
    elapsed.Stop();
    Console.WriteLine($"int Array.Sort()      avg: {elapsed.Elapsed.TotalMicroseconds / Iterations:F1} µs");
}

// --- Now check int vs IntKey via JIT method inspection ---
Console.WriteLine("\n=== JIT RuntimeHandle probe ===");

// Get the instantiated type handles to see if Sort<int> and Sort<IntKey> share code
var sortIntMethod = sortMethods.FirstOrDefault(m => m.GetParameters().Length == 1 &&
    m.GetParameters()[0].ParameterType.Name.Contains("Span"))
    ?.MakeGenericMethod(typeof(int));

if (sortIntMethod != null)
{
    var handle = sortIntMethod.MethodHandle;
    RuntimeHelpers.PrepareMethod(handle);
    Console.WriteLine($"Sort<int> method handle: 0x{handle.Value:X}");
}

// How the comparison works for int vs IntKey
Console.WriteLine("\n=== Comparison inlining check ===");
int a = 5, b = 3;
var result1 = System.Collections.Generic.Comparer<int>.Default.Compare(a, b);
Console.WriteLine($"Comparer<int>.Default.Compare(5, 3) = {result1}");

// Check if CompareTo on int compiles to simple SUB instruction
// vs IntKey.CompareTo which calls Key.CompareTo()
Console.WriteLine($"int.CompareTo: {a.CompareTo(b)}");

// --- Check VSortRoutines or JIT virtualized sort ---
Console.WriteLine("\n=== Check for vectorized sort availability (type names) ===");
var spanSortTypes = typeof(System.MemoryExtensions).Assembly.GetTypes()
    .Where(t => t.Namespace?.Contains("System") == true &&
               (t.Name.Contains("Sort") || t.Name.Contains("Intro") || t.Name.Contains("Introspective")))
    .OrderBy(t => t.FullName)
    .ToArray();
foreach (var t in spanSortTypes)
{
    var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
        .Where(m => m.Name.Contains("Sort") || m.Name.Contains("Partition"))
        .Select(m => m.Name)
        .Distinct()
        .ToArray();
    if (methods.Length > 0)
        Console.WriteLine($"  {t.FullName}: [{string.Join(", ", methods)}]");
}
