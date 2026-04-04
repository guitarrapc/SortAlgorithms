// Deeper investigation: SortUtils and vectorized sort paths
using System;
using System.Reflection;
using System.Runtime.Intrinsics;

// --- Check SortUtils in detail ---
Console.WriteLine("=== SortUtils details ===");
var coreAssembly = typeof(System.MemoryExtensions).Assembly;

var sortUtils = coreAssembly.GetType("System.Collections.Generic.SortUtils");
if (sortUtils != null)
{
    Console.WriteLine($"Found: {sortUtils.FullName}");
    var methods = sortUtils.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
    foreach (var m in methods)
    {
        var parms = m.GetParameters();
        Console.WriteLine($"  {m.Name}({string.Join(", ", parms.Select(p => $"{p.ParameterType.Name} {p.Name}"))}) returns {m.ReturnType.Name}");
    }
}

// --- Broader search for vectorized sort types ---
Console.WriteLine("\n=== All sort-related types in corlib ===");
var allTypes = coreAssembly.GetTypes();
var sortRelated = allTypes
    .Where(t => t.Name.Contains("Sort", StringComparison.OrdinalIgnoreCase) ||
                t.Name.Contains("Vsort", StringComparison.OrdinalIgnoreCase) ||
                t.Name.Contains("Vector", StringComparison.OrdinalIgnoreCase) && t.Namespace?.StartsWith("System.Collections") == true)
    .OrderBy(t => t.FullName)
    .ToArray();

Console.WriteLine($"Found {sortRelated.Length} types:");
foreach (var t in sortRelated)
{
    Console.WriteLine($"  {t.FullName}");
}

// --- Check GenericArraySortHelper<int> vs GenericArraySortHelper<IntKey> ---
Console.WriteLine("\n=== GenericArraySortHelper<int> methods ===");
var genericHelper = coreAssembly.GetType("System.Collections.Generic.GenericArraySortHelper`1");
if (genericHelper != null)
{
    var intHelper = genericHelper.MakeGenericType(typeof(int));
    var methods = intHelper.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
    foreach (var m in methods.Where(m => m.DeclaringType == intHelper || m.DeclaringType == genericHelper))
    {
        Console.WriteLine($"  {m.Name}");
    }
}

// --- Timing: benchmark all comparison methods ---
Console.WriteLine("\n=== How a.CompareTo(b) compiles for int vs IntKey ===");
// int.CompareTo compiles to:
//   cmp eax, ecx; sete al  (or similar)
// IntKey.CompareTo calls Key.CompareTo which calls int.CompareTo
// Let's measure how fast these are inner-loop

const int N = 100_000_000;
int x = 5, y = 3;

var sw = System.Diagnostics.Stopwatch.StartNew();
int sum1 = 0;
for (int i = 0; i < N; i++)
{
    sum1 += x.CompareTo(y);
}
sw.Stop();
Console.WriteLine($"int.CompareTo x{N}: {sw.Elapsed.TotalMilliseconds:F1}ms sum={sum1}");

// Test indirect path
sw.Restart();
int sum2 = 0;
for (int i = 0; i < N; i++)
{
    sum2 += ((System.IComparable<int>)x).CompareTo(y);  // interface dispatch
}
sw.Stop();
Console.WriteLine($"IComparable<int>.CompareTo x{N}: {sw.Elapsed.TotalMilliseconds:F1}ms sum={sum2}");

// --- Check if MemoryExtensions.Sort<int> uses a different internal implementation ---
Console.WriteLine("\n=== Checking internal path via ArraySortHelper.CreateArraySortHelper ===");
var arraySortHelper = coreAssembly.GetType("System.Collections.Generic.ArraySortHelper`1");
if (arraySortHelper != null)
{
    var createMethod = arraySortHelper.GetMethod("CreateArraySortHelper", BindingFlags.NonPublic | BindingFlags.Static);
    Console.WriteLine($"CreateArraySortHelper exists: {createMethod != null}");

    // Get for int
    var intInstantiation = arraySortHelper.MakeGenericType(typeof(int));
    var defaultField = intInstantiation.GetField("Default", BindingFlags.Public | BindingFlags.Static);
    if (defaultField != null)
    {
        var defaultInstance = defaultField.GetValue(null);
        Console.WriteLine($"ArraySortHelper<int>.Default type: {defaultInstance?.GetType().FullName}");
    }
}

// --- Check if there's a TrySZSort or TrySIMDSort method ---
Console.WriteLine("\n=== Searching for native/SIMD sort entry points ===");
var interestingMethods = allTypes
    .SelectMany(t => {
        try { return t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance); }
        catch { return Array.Empty<MethodInfo>(); }
    })
    .Where(m => m.Name.Contains("SIMD", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Contains("Vectorized", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Contains("TrySZ", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Contains("Native", StringComparison.OrdinalIgnoreCase) && m.Name.Contains("Sort", StringComparison.OrdinalIgnoreCase))
    .Select(m => $"{m.DeclaringType?.Name}.{m.Name}")
    .Distinct()
    .OrderBy(s => s)
    .ToArray();

Console.WriteLine($"Found {interestingMethods.Length} SIMD/Vectorized/TrySZ methods:");
foreach (var m in interestingMethods)
{
    Console.WriteLine($"  {m}");
}
