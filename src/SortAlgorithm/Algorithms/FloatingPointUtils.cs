using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Floating-point sorting optimization utilities.
/// </summary>
internal static class FloatingPointUtils
{
    /// <summary>
    /// For floating-point types, moves NaN values to the front of the array.
    /// Non-NaN elements maintain their relative order (stable).
    /// </summary>
    /// <remarks>
    /// <para><strong>Behavior:</strong></para>
    /// <list type="bullet">
    /// <item><description>For floating-point types (float, double, Half), detects and moves NaN values to the front</description></item>
    /// <item><description>For non-floating-point types, does nothing and returns begin as-is (JIT optimizes this away)</description></item>
    /// <item><description>Uses SortSpan so all operations are tracked in statistics</description></item>
    /// </list>
    /// <para><strong>Optimization Techniques:</strong></para>
    /// <list type="bullet">
    /// <item><description><c>typeof(T) == typeof(float)</c> is constant-folded by JIT at compile time</description></item>
    /// <item><description><c>Unsafe.As</c> completely avoids boxing (zero allocation)</description></item>
    /// <item><description>Compiles to type-specialized code for each type</description></item>
    /// </list>
    /// <para><strong>Reference Implementation:</strong></para>
    /// <para>Same approach as NaN preprocessing in dotnet/runtime's ArraySortHelper.cs.</para>
    /// </remarks>
    /// <typeparam name="T">The element type</typeparam>
    /// <typeparam name="TComparer">The comparer type</typeparam>
    /// <param name="s">The SortSpan to process</param>
    /// <param name="begin">Starting index (inclusive)</param>
    /// <param name="end">Ending index (exclusive)</param>
    /// <returns>Starting index of non-NaN elements. Returns begin as-is for non-floating-point types.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MoveNaNsToFront<T, TComparer>(SortSpan<T, TComparer> s, int begin, int end)
        where TComparer : IComparer<T>
    {
        // Do nothing for non-floating-point types (JIT optimizes typeof check)
        if (typeof(T) != typeof(float) && typeof(T) != typeof(double) && typeof(T) != typeof(Half))
        {
            return begin;
        }

        int nanIndex = begin;

        // float case
        if (typeof(T) == typeof(float))
        {
            for (int i = begin; i < end; i++)
            {
                var value = s.Read(i);
                // Unsafe.As avoids boxing (zero-cost abstraction)
                if (float.IsNaN(Unsafe.As<T, float>(ref value)))
                {
                    if (i != nanIndex)
                    {
                        s.Swap(nanIndex, i);  // Tracked in statistics
                    }
                    nanIndex++;
                }
            }
            return nanIndex;
        }

        // double case
        if (typeof(T) == typeof(double))
        {
            for (int i = begin; i < end; i++)
            {
                var value = s.Read(i);
                if (double.IsNaN(Unsafe.As<T, double>(ref value)))
                {
                    if (i != nanIndex)
                    {
                        s.Swap(nanIndex, i);
                    }
                    nanIndex++;
                }
            }
            return nanIndex;
        }

        // Half case
        if (typeof(T) == typeof(Half))
        {
            for (int i = begin; i < end; i++)
            {
                var value = s.Read(i);
                if (Half.IsNaN(Unsafe.As<T, Half>(ref value)))
                {
                    if (i != nanIndex)
                    {
                        s.Swap(nanIndex, i);
                    }
                    nanIndex++;
                }
            }
            return nanIndex;
        }

        // Unreachable (JIT removes this)
        return begin;
    }
}
