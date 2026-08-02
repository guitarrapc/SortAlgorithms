using System.Reflection;
using SortAlgorithm.Algorithms;

namespace SortAlgorithm.Tests;

/// <summary>
/// Ties every algorithm's declared <c>IsStable</c> to the suite that actually measures it.
///
/// <para>
/// Stability used to live only in a doc comment, so nothing connected the claim to the behaviour and nothing
/// connected it to the consumers that republish it. Four algorithms' summaries were wrong — three tree sorts
/// said "No" while their equal keys were provably never reordered, and <see cref="StrandSort"/> said "No" while
/// its own merge loop carried the comment "Stable merge" — and a downstream consumer had eight of the eighty-three
/// values wrong in the other direction. None of it was detectable: a wrong stability claim sorts perfectly.
/// </para>
///
/// <para>
/// The chain that replaces it is: the implementation decides, <see cref="StableSortTestsBase"/> measures,
/// <c>IsStable</c> declares, and a consumer reads the declaration rather than restating it. This test is the
/// link between the second and third steps. The suite an algorithm's tests derive from is the measurement —
/// <see cref="StableSortTestsBase"/> sorts records carrying an original position and requires equal keys to keep
/// it — so requiring the declaration to agree with the choice of base class means a wrong declaration cannot
/// survive alongside a passing suite.
/// </para>
/// </summary>
public class StabilityDeclarationTests
{
    /// <summary>
    /// Algorithms whose test suite cannot observe stability, so the declaration rests on the implementation's
    /// documented placement rule rather than on a measurement.
    /// </summary>
    /// <remarks>
    /// The integer-only entry points take <c>Span&lt;int&gt;</c>, and equal integers are indistinguishable, so no
    /// fixture can tell a stable run from an unstable one. Every algorithm listed here does have a key-selector
    /// overload whose stability was measured by hand when the declaration was written; what is missing is a
    /// standing test, not evidence. Adding one means giving these suites a key-carrying fixture, which is the
    /// natural way to shrink this list.
    /// </remarks>
    private static readonly HashSet<string> NotObservableByItsSuite =
    [
        "AmericanFlagSort", "BucketSortInteger", "CountingSortInteger", "FlashSort",
        "PigeonholeSortInteger", "RadixLSD10Sort", "RadixLSD256Sort", "RadixLSD4Sort",
        "RadixMSD10Sort", "RadixMSD4Sort", "SpreadSort",
        // Standalone suites: their inputs are constrained (power-of-two widths for the networks,
        // a handful of elements for the joke sorts, a key selector for the distribution sorts),
        // so they do not derive from the shared bases at all.
        "BatcherOddEvenMergeSort", "BitonicSort", "BitonicSortNonOptimized", "BogoSort",
        "BucketSort", "CountingSort", "PigeonholeSort", "SlowSort", "StoogeSort",
    ];

    private static IEnumerable<Type> AlgorithmTypes()
        => typeof(BinaryTreeSort).Assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: true, IsSealed: true, IsPublic: true })
            .Where(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(m => m.Name is "Sort" or "SortBy"))
            .OrderBy(t => t.Name, StringComparer.Ordinal);

    private static bool? DeclaredStability(Type algorithm)
        => algorithm.GetProperty("IsStable", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as bool?;

    /// <summary>
    /// Every algorithm declares its stability. A new algorithm that forgets to would otherwise reach a consumer
    /// as a compile error at best, and as a silently missing entry at worst.
    /// </summary>
    [Test]
    public async Task EveryAlgorithmDeclaresItsStability()
    {
        var missing = AlgorithmTypes().Where(t => DeclaredStability(t) is null).Select(t => t.Name).ToList();

        await Assert.That(missing).IsEmpty()
            .Because($"""
                These algorithms have no public static IsStable property, so a consumer has nothing to read and
                has to guess or restate the fact. Add one next to the type's summary:
                {string.Join("\n", missing)}
                """);
    }

    /// <summary>
    /// The declaration must agree with the suite that measures it. Deriving from
    /// <see cref="StableSortTestsBase"/> is an assertion that equal keys keep their order; declaring
    /// <c>IsStable => false</c> alongside it, or the reverse, means one of the two is untrue.
    /// </summary>
    [Test]
    public async Task DeclaredStabilityMatchesTheSuiteThatMeasuresIt()
    {
        var problems = new List<string>();
        var checkedCount = 0;

        foreach (var algorithm in AlgorithmTypes())
        {
            if (NotObservableByItsSuite.Contains(algorithm.Name)) continue;

            var declared = DeclaredStability(algorithm);
            if (declared is null) continue; // reported by the test above

            var suite = typeof(StabilityDeclarationTests).Assembly.GetType($"SortAlgorithm.Tests.{algorithm.Name}Tests");
            if (suite is null)
            {
                problems.Add($"{algorithm.Name}: no {algorithm.Name}Tests, so nothing measures the declaration");
                continue;
            }

            checkedCount++;
            var measuresStability = typeof(StableSortTestsBase).IsAssignableFrom(suite);
            if (measuresStability != declared)
            {
                problems.Add(measuresStability
                    ? $"{algorithm.Name}: declares IsStable => false, but {suite.Name} derives from StableSortTestsBase and that suite passes"
                    : $"{algorithm.Name}: declares IsStable => true, but {suite.Name} does not derive from StableSortTestsBase, so nothing checks it");
            }
        }

        await Assert.That(problems).IsEmpty()
            .Because($"""
                An algorithm's declared stability disagrees with the suite that measures it. Fix whichever is
                wrong — if the algorithm really is stable, derive its tests from StableSortTestsBase so the claim
                is enforced; if it is not, correct IsStable and the type's summary:
                {string.Join("\n", problems)}
                """);

        // A silent drop to zero would make the test vacuous while still passing.
        await Assert.That(checkedCount).IsGreaterThan(50);
    }

    /// <summary>
    /// The exemption list must not outlive its reason. An algorithm listed there whose suite has since gained a
    /// key-carrying fixture is being skipped for no reason, and the list would quietly grow into a place where
    /// unverified claims accumulate.
    /// </summary>
    [Test]
    public async Task ExemptionsAreStillNeeded()
    {
        var stale = new List<string>();

        foreach (var name in NotObservableByItsSuite.OrderBy(x => x, StringComparer.Ordinal))
        {
            var algorithm = AlgorithmTypes().FirstOrDefault(t => t.Name == name);
            if (algorithm is null) { stale.Add($"{name}: no such algorithm any more"); continue; }

            var suite = typeof(StabilityDeclarationTests).Assembly.GetType($"SortAlgorithm.Tests.{name}Tests");
            if (suite is not null && typeof(StableSortTestsBase).IsAssignableFrom(suite))
                stale.Add($"{name}: {suite.Name} now measures stability, so the exemption can go");
        }

        await Assert.That(stale).IsEmpty()
            .Because($"""
                These entries in NotObservableByItsSuite are no longer needed:
                {string.Join("\n", stale)}
                """);
    }
}
