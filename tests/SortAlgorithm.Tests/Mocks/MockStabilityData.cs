namespace SortAlgorithm.Tests;

public static class MockStabilityData
{
    public static int[] Sorted => _sorted;
    private static int[] _sorted = [1, 1, 1, 2, 2, 3];

    public static int[] Sorted1 => _sorted1;
    private static int[] _sorted1 = [0, 2, 4];

    public static int[] Sorted2 => _sorted2;
    private static int[] _sorted2 = [1, 5];

    public static int[] Sorted3 => _sorted3;
    private static int[] _sorted3 = [3];

    public static IEnumerable<Func<StabilityTestItem[]>> Generate()
    {
        yield return () => new StabilityTestItem[]
        {
            new (1, 0),
            new (2, 1),
            new (1, 2),
            new (3, 3),
            new (1, 4),
            new (2, 5),
        };
    }
}
