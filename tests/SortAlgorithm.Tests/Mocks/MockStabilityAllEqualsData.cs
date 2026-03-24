namespace SortAlgorithm.Tests;

public static class MockStabilityAllEqualsData
{
    public static int[] Sorted => _sorted;
    private static int[] _sorted = [0, 1, 2, 3, 4];

    public static IEnumerable<Func<StabilityTestItem[]>> Generate()
    {
        yield return () => new StabilityTestItem[]
        {
            new (1, 0),
            new (1, 1),
            new (1, 2),
            new (1, 3),
            new (1, 4),
        };
    }
}
