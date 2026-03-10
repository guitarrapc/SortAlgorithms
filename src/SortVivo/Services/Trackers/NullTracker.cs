using SortVivo.Models;

namespace SortVivo.Services;

/// <summary>
/// TutorialVisualizationHint.None 用のトラッカー。すべての操作を何もせずに通過させる。
/// </summary>
sealed class NullTracker : IVisualizationTracker
{
    public static readonly NullTracker Instance = new();

    private NullTracker() { }

    public void Process(SortOperation op, int[] mainArray, Dictionary<int, int[]> buffers) { }

    public TutorialStep Decorate(TutorialStep step) => step;

    public void PostStep() { }
}
