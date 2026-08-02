using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Utils;

/// <summary>
/// Generates an input that drives <see cref="PDQSort"/> into its worst case, using the killer
/// adversary described on <see cref="KillerAdversaryGenerator"/>.
/// </summary>
/// <remarks>
/// <para>
/// PDQSort is the case the adversary exists for. It counts unbalanced partitions, shuffles
/// elements whenever it finds one, switches to left-leaning partitioning when the pivot repeats,
/// and falls back to heapsort after log2(n) bad partitions - so a fixed layout is recovered from
/// almost immediately. The layout this generator replaced cost PDQSort only ~15% more than random
/// input and never once triggered the fallback.
/// </para>
/// <para><strong>What this achieves:</strong></para>
/// <para>
/// Not O(n^2): PDQSort's heapsort fallback rules that out by construction, and an adversary that
/// claimed otherwise would be reporting a bug rather than a worst case. The result is the worst
/// case PDQSort actually admits - log2(n) bad partitions whose work is thrown away, followed by
/// heapsort over what is left. Measured at n = 100,000 that is ~2.8 n log2 n comparisons against
/// ~1.1 for random input. Generation stays O(n log n) for the same reason the result is not
/// quadratic.
/// </para>
/// <para>
/// The input is derived from the current <see cref="PDQSort"/> code rather than from a
/// transcription of its constants, so it keeps up with changes to the thresholds, the pivot
/// selection, or the shuffle. It is deterministic: no randomness is involved at any point.
/// </para>
/// </remarks>
public static class PdqAdversaryGenerator
{
    /// <summary>
    /// Builds a permutation of [0, <paramref name="length"/>) that is adversarial for <see cref="PDQSort"/>.
    /// </summary>
    /// <param name="length">The number of elements to generate. Must be positive.</param>
    public static int[] Generate(int length)
        => KillerAdversaryGenerator.Generate(length, static (span, comparer) => PDQSort.Sort(span, comparer, NullContext.Default));
}
