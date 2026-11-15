using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Floating-point sorting optimization utilities.
/// </summary>
internal static class FloatingPointUtils
{
    /// <summary>
    /// Moves NaN values to the front when the internal natural floating-point comparer is used.
    /// The relative order of non-NaN elements may change.
    /// </summary>
    /// <remarks>
    /// <para><strong>Behavior:</strong></para>
    /// <list type="bullet">
    /// <item><description>For float, double and Half with the internal natural comparer, moves NaN values to the front</description></item>
    /// <item><description>For other types or comparers, leaves the range untouched and returns begin</description></item>
    /// <item><description>Uses SortSpan so all operations are tracked in statistics</description></item>
    /// </list>
    /// <para><strong>Optimization Techniques:</strong></para>
    /// <list type="bullet">
    /// <item><description><c>typeof(T) == typeof(float)</c> is constant-folded by JIT at compile time</description></item>
    /// <item><description><c>Unsafe.As</c> completely avoids boxing (zero allocation)</description></item>
    /// <item><description>Compiles to type-specialized code for each type</description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">The element type</typeparam>
    /// <typeparam name="TComparer">The comparer type</typeparam>
    /// <typeparam name="TContext">The observation context type</typeparam>
    /// <param name="s">The SortSpan to process</param>
    /// <param name="begin">Starting index (inclusive)</param>
    /// <param name="end">Ending index (exclusive)</param>
    /// <returns>The end of the isolated NaN prefix, or begin when preprocessing does not apply.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MoveNaNsToFront<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Custom orders may place NaNs outside the natural-order prefix.
        if ((typeof(T) != typeof(float) && typeof(T) != typeof(double) && typeof(T) != typeof(Half))
            || s.Comparer is not IComparableComparer)
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
