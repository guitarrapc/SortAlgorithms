//css_reference ../../src/SortAlgorithm/bin/Debug/net10.0/SortAlgorithm.dll
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine("Testing LibrarySort implementation...");

// Test 1: Simple array
Console.WriteLine("\n=== Test 1: Simple Array ===");
var array1 = new[] { 5, 2, 8, 1, 9, 3 };
Console.WriteLine($"Before: [{string.Join(", ", array1)}]");
try
{
    LibrarySort.Sort(array1.AsSpan());
    Console.WriteLine($"After:  [{string.Join(", ", array1)}]");
    Console.WriteLine($"Sorted: {IsSorted(array1)}");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}

// Test 2: Larger array (>32 to trigger gap-based path)
Console.WriteLine("\n=== Test 2: Larger Array (50 elements) ===");
var array2 = Enumerable.Range(0, 50).Reverse().ToArray();
Console.WriteLine($"Before: [{string.Join(", ", array2.Take(10))}... (reversed 0-49)]");
try
{
    LibrarySort.Sort(array2.AsSpan());
    Console.WriteLine($"After:  [{string.Join(", ", array2.Take(10))}...]");
    Console.WriteLine($"Sorted: {IsSorted(array2)}");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}

// Test 3: Already sorted
Console.WriteLine("\n=== Test 3: Already Sorted ===");
var array3 = new[] { 1, 2, 3, 4, 5 };
Console.WriteLine($"Before: [{string.Join(", ", array3)}]");
try
{
    LibrarySort.Sort(array3.AsSpan());
    Console.WriteLine($"After:  [{string.Join(", ", array3)}]");
    Console.WriteLine($"Sorted: {IsSorted(array3)}");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}

// Test 4: With duplicates
Console.WriteLine("\n=== Test 4: With Duplicates ===");
var array4 = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5 };
Console.WriteLine($"Before: [{string.Join(", ", array4)}]");
try
{
    LibrarySort.Sort(array4.AsSpan());
    Console.WriteLine($"After:  [{string.Join(", ", array4)}]");
    Console.WriteLine($"Sorted: {IsSorted(array4)}");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}

// Test 5: 100 elements random
Console.WriteLine("\n=== Test 5: 100 Random Elements ===");
var array5 = Enumerable.Range(1, 100).OrderBy(_ => Guid.NewGuid()).ToArray();
Console.WriteLine($"Before: [{string.Join(", ", array5.Take(10))}... (100 shuffled elements)]");
try
{
    LibrarySort.Sort(array5.AsSpan());
    Console.WriteLine($"After:  [{string.Join(", ", array5.Take(10))}...]");
    Console.WriteLine($"Sorted: {IsSorted(array5)}");
    Console.WriteLine($"All elements present: {array5.SequenceEqual(Enumerable.Range(1, 100))}");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}

Console.WriteLine("\n=== All Tests Completed ===");

static bool IsSorted<T>(T[] array)
{
    var comparer = Comparer<T>.Default;
    for (int i = 1; i < array.Length; i++)
    {
        if (comparer.Compare(array[i], array[i - 1]) < 0)
            return false;
    }
    return true;
}
