namespace SortAlgorithm.Tests;

// Helper struct for stability testing
public readonly record struct StabilityTestItem(int Value, int OriginalIndex) : IComparable<StabilityTestItem>
{
    // Only compare by Value, ignore OriginalIndex
    // This allows testing if equal values maintain their original order
    public int CompareTo(StabilityTestItem other)
    {
        return Value.CompareTo(other.Value);
    }
}

// Helper struct for complex stability testing
public readonly record struct StabilityTestItemWithId(int Key, string Id) : IComparable<StabilityTestItemWithId>
{
    // Only compare by Key, ignore Id
    // This allows testing if equal keys maintain their original order
    public int CompareTo(StabilityTestItemWithId other)
    {
        return Key.CompareTo(other.Key);
    }
}
