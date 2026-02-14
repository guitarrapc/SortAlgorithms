namespace SortAlgorithm.Tests;

public enum InputType
{
    Random,
    Reversed,
    Mountain,
    NearlySorted,
    Sorted,
    SameValues,
    Stability,
    AntiQuickSort,

    MixRandom,
    NegativeRandom,
    DictionaryRamdom,

    // Duplicate-heavy patterns (for BlockQuickSort paper benchmarks)
    AllIdentical,
    TwoDistinctValues,
    HalfZeroHalfOne,
    ManyDuplicatesSqrtRange,
    HighlySkewed,

    // Floating-point with NaN patterns
    RandomWithNaN,
    RandomWithHighNaN,
    SortedWithNaN,
    AllNaN,
    RandomNoNaN,

    // IntKey patterns (for JIT optimization verification)
    IntKeyRandom,
}

public interface IInputSample<T> where T : IComparable
{
    InputType InputType { get; set; }
    T[] Samples { get; set; }
    CustomKeyValuePair<T, string>[] DictionarySamples { get; set; }
}

public class InputSample<T> : IInputSample<T> where T : IComparable
{
    public required InputType InputType { get; set; }
    public T[] Samples { get; set; } = [];
    public CustomKeyValuePair<T, string>[] DictionarySamples { get; set; } = [];
}

public readonly struct CustomKeyValuePair<TKey, TValue> : IComparable<CustomKeyValuePair<TKey, TValue>> where TKey : notnull, IComparable
{
    public CustomKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
    public TKey Key { get; }
    public TValue Value { get; }

    public int CompareTo(CustomKeyValuePair<TKey, TValue> other)
    {
        return Key.CompareTo(other.Key);
    }
}
