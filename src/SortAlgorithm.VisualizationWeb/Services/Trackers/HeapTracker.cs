using SortAlgorithm.VisualizationWeb.Models;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// HeapTree / TernaryHeapTree / WeakHeapTree ビジュアライゼーション用トラッカー。
/// ヒープ境界と WeakHeap の reverse bit 配列を追跡し、各ステップへ付加する。
/// </summary>
sealed class HeapTracker : IVisualizationTracker
{
    private readonly bool _isWeak;
    private int _heapBoundary;
    private bool _heapBuildDone;
    private int? _pendingRootValue;
    private readonly bool[] _reverseBits;

    // Decorate() 用キャッシュ
    private int _cachedBoundary;
    private bool[]? _cachedReverseBits;

    internal HeapTracker(TutorialVisualizationHint hint, int arrayLength)
    {
        _isWeak = hint == TutorialVisualizationHint.WeakHeapTree;
        _heapBoundary = arrayLength;
        _cachedBoundary = arrayLength;
        _reverseBits = _isWeak ? new bool[arrayLength] : [];
        _cachedReverseBits = _isWeak ? (bool[])_reverseBits.Clone() : null;
    }

    public void Process(SortOperation op, int[] mainArray, Dictionary<int, int[]> buffers)
    {
        if (op.BufferId1 != 0) return;

        switch (op.Type)
        {
            case OperationType.Swap:
            {
                if (!_heapBuildDone && (op.Index1 == 0 || op.Index2 == 0))
                    _heapBuildDone = true;

                int rootIdx = Math.Min(op.Index1, op.Index2);
                int lastIdx = Math.Max(op.Index1, op.Index2);

                if (_heapBuildDone && rootIdx == 0 && lastIdx == _heapBoundary - 1)
                {
                    // 抽出 Swap: root ↔ last ヒープ要素 — reverse bit は反転しない
                    _heapBoundary--;
                }
                else if (_isWeak)
                {
                    // Merge Swap: a[lastIdx] > a[rootIdx] → FlipBit(lastIdx)
                    _reverseBits[lastIdx] = !_reverseBits[lastIdx];
                }
                break;
            }
            case OperationType.IndexRead when op.Index1 == 0:
                if (!_heapBuildDone) _heapBuildDone = true;
                _pendingRootValue = mainArray[0];
                break;

            case OperationType.IndexWrite when _heapBuildDone
                && _pendingRootValue.HasValue
                && op.Value == _pendingRootValue
                && op.Index1 == _heapBoundary - 1:
                _heapBoundary--;
                _pendingRootValue = null;
                break;
        }

        _cachedBoundary = _heapBoundary;
        if (_isWeak)
            _cachedReverseBits = (bool[])_reverseBits.Clone();
    }

    public TutorialStep Decorate(TutorialStep step)
    {
        step = step with { HeapBoundary = _cachedBoundary };
        if (_isWeak)
            step = step with { WeakHeapReverseBits = _cachedReverseBits };
        return step;
    }

    public void PostStep() { }
}
