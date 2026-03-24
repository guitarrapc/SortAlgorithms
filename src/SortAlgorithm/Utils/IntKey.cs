namespace SortAlgorithm.Utils;

/// <summary>
/// JIT最適化の検証用の構造体
/// int の代わりにユーザー定義型を使うことで、JIT による極端な最適化を抑制する
/// <br/>
/// Struct for verifying JIT optimization
/// Using user-defined type instead of int to suppress extreme JIT optimizations
/// </summary>
public readonly struct IntKey : IComparable<IntKey>, IComparable, IEquatable<IntKey>
{
    /// <summary>
    /// 比較用のキー値
    /// </summary>
    public int Key { get; }

    public IntKey(int key)
    {
        Key = key;
    }

    public int CompareTo(IntKey other)
    {
        return Key.CompareTo(other.Key);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null)
            return 1;
        if (obj is IntKey other)
            return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(IntKey)}");
    }

    public bool Equals(IntKey other)
    {
        return Key == other.Key;
    }

    public override bool Equals(object? obj)
    {
        return obj is IntKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public override string ToString()
    {
        return Key.ToString();
    }

    public static bool operator ==(IntKey left, IntKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IntKey left, IntKey right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(IntKey left, IntKey right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(IntKey left, IntKey right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(IntKey left, IntKey right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(IntKey left, IntKey right)
    {
        return left.CompareTo(right) >= 0;
    }
}
