using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

internal static class MergeBuffer
{
    internal static T[] EnsureCapacity<T>([NotNull] ref T[]? buffer, int length, ArrayPool<T> pool)
    {
        if (buffer is not null && buffer.Length >= length) return buffer;

        // A failed rent must leave the current lease owned by the caller's finally block.
        var replacement = pool.Rent(length);
        var previous = buffer;
        buffer = replacement;
        if (previous is not null)
            pool.Return(previous, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        return replacement;
    }
}
