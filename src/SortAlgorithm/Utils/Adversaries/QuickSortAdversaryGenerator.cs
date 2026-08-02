using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Utils;

/// <summary>
/// Generates an input that drives <see cref="QuickSort"/> into its O(n^2) worst case, using the
/// killer adversary described on <see cref="KillerAdversaryGenerator"/>.
/// </summary>
/// <remarks>
/// <para><strong>What it defeats:</strong></para>
/// <para>
/// The adversary is derived against <see cref="QuickSort"/>, which takes the middle element as its
/// pivot. The result is quadratic for the whole middle-pivot family, because they all end up
/// choosing the element the adversary kept undecided: measured at n = 4,000, comparisons reach
/// ~84 n log2 n for <see cref="QuickSort"/>, <see cref="QuickSortMedian3"/> and
/// <see cref="QuickSort3way"/>, against ~1.3-1.5 for random input.
/// </para>
/// <para>
/// Variants that sample differently are not defeated by this input and are not meant to be.
/// <see cref="QuickSortMedian9"/> resists it because the ninther samples nine positions rather
/// than one, and the stable variants pivot on quartile positions instead of the middle. Each of
/// those has its own killer, which is a different array; a single input cannot be worst-case for
/// every pivot rule at once, and one that claims to be is claiming too much.
/// </para>
/// <para><strong>Why it is not a fixed layout:</strong></para>
/// <para>
/// The arrangement this generator replaced was quadratic for <see cref="QuickSort"/> alone, at
/// about a quarter of the achievable comparison count, and cost <see cref="QuickSortMedian3"/> -
/// the variant it was documented as targeting - barely more than random input. It was also beaten
/// on its own target by the ordinary pipe-organ pattern.
/// </para>
/// <para><strong>Cost:</strong></para>
/// <para>
/// Generation runs <see cref="QuickSort"/> once against the adversary, so it costs the quadratic
/// work it is designed to provoke: ~3 ms at n = 2,048 and ~775 ms at n = 32,768. Callers that
/// materialize this pattern repeatedly at large sizes should cache it - it is deterministic, so
/// the result depends on nothing but the length.
/// </para>
/// </remarks>
public static class QuickSortAdversaryGenerator
{
    /// <summary>
    /// Builds a permutation of [0, <paramref name="length"/>) that is adversarial for <see cref="QuickSort"/>.
    /// </summary>
    /// <param name="length">The number of elements to generate. Must be positive.</param>
    public static int[] Generate(int length)
        => KillerAdversaryGenerator.Generate(length, static (span, comparer) => QuickSort.Sort(span, comparer, NullContext.Default));
}
