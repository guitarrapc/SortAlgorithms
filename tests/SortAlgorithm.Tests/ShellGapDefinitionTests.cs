using System.Numerics;
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class ShellGapDefinitionTests
{
    [Test]
    [Arguments("Knuth")]
    [Arguments("Sedgewick")]
    [Arguments("Tokuda")]
    [Arguments("Ciura")]
    [Arguments("Lee")]
    public void TablesCoverTheSupportedLengthDomain(string name)
    {
        var gaps = Gaps(name);
        for (var i = 0; i < gaps.Length; i++)
            if (gaps[i] != GapAt(name, i) || gaps[i] < 1 || gaps[i] > int.MaxValue / 2 || i > 0 && gaps[i] <= gaps[i - 1])
                throw new InvalidOperationException($"Invalid gap: {name}/{i}.");
        if (gaps[0] != 1 || GapAt(name, gaps.Length) <= int.MaxValue / 2)
            throw new InvalidOperationException($"Incomplete gap table: {name}.");
    }

    [Test]
    [Arguments("Knuth")]
    [Arguments("Sedgewick")]
    [Arguments("Tokuda")]
    [Arguments("Ciura")]
    [Arguments("Lee")]
    public void SelectionMatchesEveryGapBoundary(string name)
    {
        var gaps = Gaps(name);
        var lengths = new List<int> { 0, 1, 2, 3, 16, 17, 31, 33, 65, 17857, 17858, 89683, 89684, int.MaxValue };
        foreach (var gap in gaps)
        {
            lengths.Add(2 * gap - 1);
            lengths.Add(2 * gap);
            lengths.Add(2 * gap + 1);
        }
        var random = new Random(42);
        for (var i = 0; i < 128; i++) lengths.Add((int)random.NextInt64(0, (long)int.MaxValue + 1));
        foreach (var length in lengths)
        {
            var expected = -1;
            while (GapAt(name, expected + 1) <= (BigInteger)length / 2) expected++;
            var actual = gaps.Length - 1;
            ShellGapSequences.FindLast(gaps, length, ref actual);
            if (actual != expected) throw new InvalidOperationException($"Wrong selected gap: {name}/{length}.");
        }
    }

    [Test]
    [Arguments("Knuth")]
    [Arguments("Sedgewick")]
    [Arguments("Tokuda")]
    [Arguments("Ciura")]
    [Arguments("Lee")]
    public void OrderedRangesAnnounceTheDefinedPasses(string name)
    {
        foreach (var length in name == "Ciura" ? new[] { 6, 33, 20003, 89684 } : new[] { 6, 33, 17858, 20003 })
        foreach (var reverse in new[] { false, true })
        {
            const int first = 3;
            var values = Enumerable.Repeat(-987654321, length + 6).ToArray();
            for (var i = 0; i < length; i++) values[first + i] = reverse ? length - i : i;
            var before = values.ToArray();
            var phases = new List<(int, int, int)>();
            long compares = 0, reads = 0;
            var context = new VisualizationContext(
                onCompare: (i, j, _, bi, bj) => { CheckIndex(i, bi); CheckIndex(j, bj); compares++; },
                onIndexRead: (i, b) => { CheckIndex(i, b); reads++; },
                onSwap: (_, _, _) => throw new InvalidOperationException("Ordered input was swapped."),
                onIndexWrite: (_, _, _) => throw new InvalidOperationException("Ordered input was written."),
                onPhase: (phase, gap, pass, total) =>
                {
                    if (phase != SortPhase.ShellGapPass) throw new InvalidOperationException("Unexpected algorithm phase.");
                    phases.Add((gap, pass, total));
                });
            var comparer = reverse ? Comparer<int>.Create((a, b) => b.CompareTo(a)) : Comparer<int>.Default;
            Run(name, values.AsSpan(), first, first + length, comparer, context);
            var expected = new List<int>();
            for (var i = 0; GapAt(name, i) <= (BigInteger)length / 2; i++) expected.Add((int)GapAt(name, i));
            expected.Reverse();
            if (!phases.SequenceEqual(expected.Select((gap, i) => (gap, i + 1, expected.Count)))
                || compares != expected.Sum(gap => (long)length - gap) || reads != 2 * compares || !values.SequenceEqual(before))
                throw new InvalidOperationException($"Passes, observations or range contents changed: {name}/{length}/{reverse}.");
            void CheckIndex(int index, int buffer)
            {
                if (buffer != 0 || index < first || index >= first + length)
                    throw new InvalidOperationException("An observation escaped the requested range.");
            }
        }
    }

    internal static ReadOnlySpan<int> Gaps(string name) => name switch
    {
        "Knuth" => ShellGapSequences.Knuth,
        "Sedgewick" => ShellGapSequences.Sedgewick,
        "Tokuda" => ShellGapSequences.Tokuda,
        "Ciura" => ShellGapSequences.Ciura,
        "Lee" => ShellGapSequences.Lee,
        _ => throw new ArgumentOutOfRangeException(nameof(name))
    };

    static BigInteger GapAt(string name, int index)
    {
        switch (name)
        {
            case "Knuth":
                BigInteger knuth = 1;
                for (var i = 0; i < index; i++) knuth = 3 * knuth + 1;
                return knuth;
            case "Sedgewick":
                var exponent = index / 2;
                return index % 2 == 0
                    ? 9 * BigInteger.Pow(4, exponent) - 9 * BigInteger.Pow(2, exponent) + 1
                    : BigInteger.Pow(4, exponent + 2) - 3 * BigInteger.Pow(2, exponent + 2) + 1;
            case "Tokuda":
                return 4 * (BigInteger.Pow(9, index + 1) - BigInteger.Pow(4, index + 1)) / (5 * BigInteger.Pow(4, index + 1));
            case "Ciura":
                int[] prefix = [1, 4, 10, 23, 57, 132, 301, 701, 1750];
                if (index < prefix.Length) return prefix[index];
                BigInteger ciura = prefix[^1];
                for (var i = prefix.Length; i <= index; i++) ciura = ciura * 9 / 4;
                return ciura;
            case "Lee":
                BigInteger numerator = 1, denominator = 1;
                var a = new BigInteger(2243609061420001L);
                var b = new BigInteger(1000000000000000L);
                for (var i = 0; i < index; i++)
                {
                    numerator = numerator * a + denominator * b;
                    denominator *= b;
                }
                return (numerator + denominator - 1) / denominator;
            default: throw new ArgumentOutOfRangeException(nameof(name));
        }
    }

    internal static void Run<TContext>(string name, Span<int> values, int first, int last, IComparer<int> comparer, TContext context) where TContext : ISortContext
    {
        switch (name)
        {
            case "Knuth": ShellSortKnuth1973.Sort(values, first, last, comparer, context); break;
            case "Sedgewick": ShellSortSedgewick1986.Sort(values, first, last, comparer, context); break;
            case "Tokuda": ShellSortTokuda1992.Sort(values, first, last, comparer, context); break;
            case "Ciura": ShellSortCiura2001.Sort(values, first, last, comparer, context); break;
            case "Lee": ShellSortLee2021.Sort(values, first, last, comparer, context); break;
            default: throw new ArgumentOutOfRangeException(nameof(name));
        }
    }
}
