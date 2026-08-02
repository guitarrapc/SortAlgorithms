using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// Distribution generators that shuffle their output must be deterministic when the caller
/// supplies a seeded <see cref="Random"/>. Consumers (e.g. SortVivo's <c>&amp;seed=</c> share URLs)
/// rely on "same seed → same array" for exact reproduction; an internal unseeded
/// <see cref="Random"/> silently breaks that contract.
/// </summary>
public class ArrayPatternsSeededShuffleTests
{
    private static readonly (string Name, Func<int, Random, int[]> Seeded, Func<int, int[]> Unseeded)[] Generators =
    [
        (nameof(ArrayPatterns.GenerateQuadraticDistribution), ArrayPatterns.GenerateQuadraticDistribution, ArrayPatterns.GenerateQuadraticDistribution),
        (nameof(ArrayPatterns.GenerateSquareRootDistribution), ArrayPatterns.GenerateSquareRootDistribution, ArrayPatterns.GenerateSquareRootDistribution),
        (nameof(ArrayPatterns.GenerateCubicDistribution), ArrayPatterns.GenerateCubicDistribution, ArrayPatterns.GenerateCubicDistribution),
        (nameof(ArrayPatterns.GenerateQuinticDistribution), ArrayPatterns.GenerateQuinticDistribution, ArrayPatterns.GenerateQuinticDistribution),
        (nameof(ArrayPatterns.GenerateCubeRootDistribution), ArrayPatterns.GenerateCubeRootDistribution, ArrayPatterns.GenerateCubeRootDistribution),
        (nameof(ArrayPatterns.GenerateFifthRootDistribution), ArrayPatterns.GenerateFifthRootDistribution, ArrayPatterns.GenerateFifthRootDistribution),
        (nameof(ArrayPatterns.GenerateCantorDistribution), ArrayPatterns.GenerateCantorDistribution, ArrayPatterns.GenerateCantorDistribution),
    ];

    [Test]
    public async Task SeededOverloads_AreDeterministicForSameSeed()
    {
        foreach (var (name, seeded, _) in Generators)
        {
            var a = seeded(256, new Random(123));
            var b = seeded(256, new Random(123));
            await Assert.That(a.SequenceEqual(b)).IsTrue().Because($"{name}: same seed must reproduce the same array");

            var c = seeded(256, new Random(456));
            await Assert.That(a.SequenceEqual(c)).IsFalse().Because($"{name}: different seeds should produce different orders");
        }
    }

    [Test]
    public async Task SeededOverloads_PreserveValueDistribution()
    {
        foreach (var (name, seeded, unseeded) in Generators)
        {
            // The shuffle only randomizes order; the multiset of values defines the distribution
            // and must match the parameterless overload exactly.
            var seededSorted = seeded(256, new Random(123)).OrderBy(x => x).ToArray();
            var unseededSorted = unseeded(256).OrderBy(x => x).ToArray();
            await Assert.That(seededSorted.SequenceEqual(unseededSorted)).IsTrue()
                .Because($"{name}: seeding must not change the value distribution");
        }
    }

    [Test]
    public async Task ShuffleArray_SeededOverload_IsDeterministicPermutation()
    {
        var source = Enumerable.Range(0, 256).ToArray();

        var a = ArrayPatterns.ShuffleArray((int[])source.Clone(), new Random(42));
        var b = ArrayPatterns.ShuffleArray((int[])source.Clone(), new Random(42));
        await Assert.That(a.SequenceEqual(b)).IsTrue();
        await Assert.That(a.OrderBy(x => x).SequenceEqual(source)).IsTrue();
    }
}
