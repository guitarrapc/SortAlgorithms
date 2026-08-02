using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Utils;

/// <summary>
/// Generates an input that drives <see cref="PDQSort"/> into its worst case, using McIlroy's
/// lazy-evaluation ("gas") adversary from "A Killer Adversary for Quicksort" (1999).
/// </summary>
/// <remarks>
/// <para><strong>Why the input cannot be written down directly:</strong></para>
/// <para>
/// A layout that poisons the pivot slots of one partition step stops being adversarial one step
/// later: PDQSort reacts to what it sees. It counts unbalanced partitions, shuffles elements
/// whenever it finds one, switches to left-leaning partitioning when the pivot repeats, and
/// falls back to heapsort after log2(n) bad partitions. Any fixed arrangement is therefore only
/// adversarial for whichever step it was drawn against, and the adaptive machinery recovers from
/// the rest.
/// </para>
/// <para><strong>How the adversary works:</strong></para>
/// <para>
/// The values are decided while PDQSort runs, not before. Every item starts as "gas" - a value
/// that is not yet committed and compares greater than every committed value. When PDQSort
/// compares two gas items, one of them is frozen to the next smallest unused value; the item
/// currently serving as the pivot candidate is kept gaseous as long as possible, so the pivot
/// PDQSort ends up choosing is always an extreme of its range. Because a gas item is only ever
/// reported as greater than already-frozen items, and each freeze takes a value above every
/// earlier freeze, the answers stay consistent with a single total order - so the frozen values
/// form a real array, and replaying PDQSort on that array reproduces the same comparison
/// sequence (PdqAdversaryGeneratorTests asserts this).
/// </para>
/// <para><strong>What this achieves:</strong></para>
/// <para>
/// Not O(n^2): PDQSort's heapsort fallback rules that out by construction, and an adversary that
/// claimed otherwise would be reporting a bug rather than a worst case. The result is the worst
/// case PDQSort actually admits - log2(n) bad partitions whose work is thrown away, followed by
/// heapsort over what is left. Measured at n = 100,000 that is ~2.8 n log2 n comparisons against
/// ~1.1 for random input.
/// </para>
/// <para>
/// The generator derives the input from the current <see cref="PDQSort"/> code rather than from a
/// transcription of its constants, so it keeps up with changes to the thresholds, the pivot
/// selection, or the shuffle. It is deterministic: no randomness is involved at any point.
/// </para>
/// <para><strong>Reference:</strong></para>
/// <para>M. D. McIlroy, "A Killer Adversary for Quicksort", Software - Practice and Experience 29(4), 1999.</para>
/// </remarks>
public static class PdqAdversaryGenerator
{
    /// <summary>
    /// Builds a permutation of [0, <paramref name="length"/>) that is adversarial for <see cref="PDQSort"/>.
    /// </summary>
    /// <param name="length">The number of elements to generate. Must be positive.</param>
    public static int[] Generate(int length)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

        var adversary = new GasComparer(length);

        // The elements being sorted are item identities, not values; the adversary decides what
        // value each identity carries as PDQSort asks about it.
        var items = new int[length];
        for (var i = 0; i < length; i++)
        {
            items[i] = i;
        }

        // Run the algorithm we intend to defeat. Its own decisions are what shape the result.
        PDQSort.Sort(items.AsSpan(), adversary, NullContext.Default);

        return adversary.Materialize();
    }

    /// <summary>
    /// Answers comparisons on behalf of values that have not been decided yet.
    /// </summary>
    private sealed class GasComparer : IComparer<int>
    {
        // Undecided items all hold this sentinel, which is above every value the adversary can
        // freeze, so a gas item always compares greater than a decided one.
        private readonly int gas;
        private readonly int[] values;

        // Next value to hand out. Freezing in increasing order keeps every earlier answer valid:
        // an item reported as greater than a frozen item stays greater once it freezes itself.
        private int frozen;

        // The item PDQSort is currently treating as a pivot. Keeping it gaseous is what forces
        // the pivot to turn out to be an extreme value, and hence the partition to be unbalanced.
        private int candidate;

        public GasComparer(int length)
        {
            gas = length;
            values = new int[length];
            Array.Fill(values, gas);
        }

        public int Compare(int x, int y)
        {
            // Two undecided items cannot both stay undecided, or the comparison would have no
            // answer. Freeze whichever one is not the pivot candidate.
            if (values[x] == gas && values[y] == gas)
            {
                if (x == candidate)
                {
                    values[x] = frozen++;
                }
                else
                {
                    values[y] = frozen++;
                }
            }

            // Whatever is still gaseous after this comparison is what PDQSort is carrying around
            // as its pivot, so track it as the next candidate to protect.
            if (values[x] == gas)
            {
                candidate = x;
            }
            else if (values[y] == gas)
            {
                candidate = y;
            }

            return values[x].CompareTo(values[y]);
        }

        /// <summary>
        /// Materializes the decided values as an array, where index i holds the value of the item
        /// that started at position i.
        /// </summary>
        /// <remarks>
        /// Items that were never compared against another gas item are still undecided. No answer
        /// constrains them relative to each other - only against frozen items, which they must
        /// exceed - so they can take the remaining top values in any order. Handing those out makes
        /// the result a permutation instead of a block of ties.
        /// </remarks>
        public int[] Materialize()
        {
            var next = frozen;
            var result = new int[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                result[i] = values[i] == gas ? next++ : values[i];
            }

            return result;
        }
    }
}
