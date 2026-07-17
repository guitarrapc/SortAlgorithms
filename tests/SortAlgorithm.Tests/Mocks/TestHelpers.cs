namespace SortAlgorithm.Tests;

/// <summary>
/// Deterministic input generators for statistics-oriented tests.
/// A fixed seed keeps operation counts reproducible across runs, while
/// passing multiple seeds still exercises different permutations.
/// </summary>
public static class TestHelpers
{
    public static int[] ShuffledRange(int n, int seed)
    {
        var array = Enumerable.Range(0, n).ToArray();
        new Random(seed).Shuffle(array);
        return array;
    }
}
