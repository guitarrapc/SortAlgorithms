namespace SortAlgorithm.Utils;

/// <summary>
/// Runs the sort an adversary is being built against, answering each comparison through
/// <paramref name="comparer"/>. The span holds item identities rather than values.
/// </summary>
public delegate void AdversaryTargetSort(Span<int> span, IComparer<int> comparer);

/// <summary>
/// Builds worst-case inputs with McIlroy's lazy-evaluation ("gas") adversary from
/// "A Killer Adversary for Quicksort" (1999).
/// </summary>
/// <remarks>
/// <para><strong>Why the input cannot simply be written down:</strong></para>
/// <para>
/// A layout drawn to poison one step of a sort stops being adversarial one step later, because
/// the sort reacts to what it sees. Pivot selection reads the data, pattern detection changes
/// what runs next, and recovery machinery such as pattern-defeating shuffles or a complexity
/// fallback exists precisely to undo a bad arrangement. Any fixed layout is therefore adversarial
/// only for whichever step it was drawn against.
/// </para>
/// <para><strong>How the adversary works:</strong></para>
/// <para>
/// The values are decided while the sort runs, not before. Every item starts as "gas" - a value
/// that is not yet committed and compares greater than every committed value. When two gas items
/// are compared, one of them is frozen to the next smallest unused value; the item currently
/// serving as the pivot candidate is kept gaseous as long as possible, so the pivot the sort ends
/// up choosing is always an extreme of its range and the partition is as unbalanced as the
/// implementation allows.
/// </para>
/// <para>
/// The answers stay consistent with a single total order: a gas item is only ever reported as
/// greater than already-frozen items, and each freeze takes a value above every earlier freeze,
/// so nothing said earlier is contradicted later. That is what makes the frozen values a real
/// array - replaying the same sort on it with an ordinary comparer reproduces the comparison
/// sequence exactly.
/// </para>
/// <para><strong>Cost:</strong></para>
/// <para>
/// Generation costs what it makes the target pay, because it is one full run of that target.
/// For a quicksort with no complexity fallback that is quadratic by construction; for a sort that
/// falls back to heapsort it stays O(n log n). This is inherent to the technique: producing an
/// input that forces n^2/4 comparisons requires performing them.
/// </para>
/// <para><strong>Reference:</strong></para>
/// <para>M. D. McIlroy, "A Killer Adversary for Quicksort", Software - Practice and Experience 29(4), 1999.</para>
/// </remarks>
public static class KillerAdversaryGenerator
{
    /// <summary>
    /// Builds a permutation of [0, <paramref name="length"/>) that is adversarial for
    /// <paramref name="target"/>.
    /// </summary>
    /// <param name="length">The number of elements to generate. Must be positive.</param>
    /// <param name="target">
    /// The sort to defeat. It must be deterministic and must reach elements only through the
    /// comparer it is given; a sort that inspects values directly, or that consults a random
    /// source, cannot be replayed and so cannot be killed this way.
    /// </param>
    public static int[] Generate(int length, AdversaryTargetSort target)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
        ArgumentNullException.ThrowIfNull(target);

        var adversary = new GasComparer(length);

        // The elements being sorted are item identities, not values; the adversary decides what
        // value each identity carries as the sort asks about it.
        var items = new int[length];
        for (var i = 0; i < length; i++)
        {
            items[i] = i;
        }

        // Run the algorithm we intend to defeat. Its own decisions are what shape the result.
        target(items.AsSpan(), adversary);

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

        // The item the sort is currently treating as a pivot. Keeping it gaseous is what forces
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

            // Whatever is still gaseous after this comparison is what the sort is carrying around
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
