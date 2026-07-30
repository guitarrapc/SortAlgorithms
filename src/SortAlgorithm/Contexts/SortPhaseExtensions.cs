namespace SortAlgorithm.Contexts;

/// <summary>
/// Classification helpers for <see cref="SortPhase"/>.
/// </summary>
/// <remarks>
/// <para>
/// Phase announcements form two levels, because a hybrid algorithm hands a subrange to a core that
/// another algorithm owns. The outer algorithm announces what it decided to do (a <em>scope</em>
/// phase such as <see cref="SortPhase.HybridToInsertionSort"/>); the core it delegates to announces
/// its own progress through that subrange (a <em>detail</em> phase such as
/// <see cref="SortPhase.InsertionPass"/>). Both are true at the same time, so a consumer that keeps
/// a single "current phase" slot loses the outer one the moment the inner core starts reporting.
/// </para>
/// <para>
/// Only cores that other algorithms call produce detail phases, and those cores are leaves: none of
/// them delegates to another phase-announcing core. Nesting therefore never exceeds two levels, and
/// two slots — one scope, one detail — describe every sequence this library emits.
/// </para>
/// <para>
/// A phase is classified detail purely by which core emits it, not by how it reads. When a detail
/// phase's own algorithm runs at the top level there is no scope phase to pair it with, and a
/// consumer is expected to render the detail phase alone.
/// </para>
/// </remarks>
public static class SortPhaseExtensions
{
    /// <summary>
    /// Returns true when the phase is announced by a core that other algorithms delegate to,
    /// and therefore describes progress inside a subrange rather than the caller's own decision.
    /// </summary>
    /// <param name="phase">The phase to classify.</param>
    /// <returns>
    /// True for phases emitted by <c>InsertionSort.SortCore</c>, <c>BinaryInsertionSort.SortCore</c>,
    /// and <c>HeapSort.SortCore</c>; false for every other phase, including <see cref="SortPhase.None"/>.
    /// </returns>
    /// <remarks>
    /// Keep this in sync with the set of cores called across algorithm boundaries. A core that starts
    /// announcing a phase without being classified here will overwrite its caller's scope phase, which
    /// is exactly the failure this classification exists to prevent.
    /// </remarks>
    public static bool IsDetailPhase(this SortPhase phase) => phase switch
    {
        // InsertionSort.SortCore — called by ~20 algorithms across the partition, merge and distribution families.
        SortPhase.InsertionPass => true,
        // BinaryInsertionSort.SortCore — called by TimSort to extend short natural runs to minRun.
        SortPhase.BinaryInsertionPass => true,
        // HeapSort.SortCore — the depth-limit fallback of IntroSort, StdSort, PDQSort, PDQSortBranchless and BlockQuickSort.
        SortPhase.HeapBuild => true,
        SortPhase.HeapExtract => true,
        _ => false,
    };

    /// <summary>
    /// Returns true when the phase describes the announcing algorithm's own decision, and belongs in
    /// the scope slot. This is the complement of <see cref="IsDetailPhase"/> for every phase except
    /// <see cref="SortPhase.None"/>, which occupies neither slot.
    /// </summary>
    /// <param name="phase">The phase to classify.</param>
    public static bool IsScopePhase(this SortPhase phase)
        => phase != SortPhase.None && !phase.IsDetailPhase();
}
