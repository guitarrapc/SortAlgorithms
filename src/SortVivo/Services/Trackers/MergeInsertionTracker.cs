using SortAlgorithm.Contexts;
using SortVivo.Models;

namespace SortVivo.Services;

/// <summary>
/// MergeInsertionSort（Ford-Johnson）チュートリアル用トラッカー。
/// <br/>
/// アルゴリズム本体から削除された FjSmaller / FjLarger / FjStraggler のロール管理を
/// トラッカー側でハンドリングする。
/// ペアリングフェーズの Compare 操作から大小を判定してロールを設定し、
/// InsertPend フェーズ開始時にロールをクリアする。
/// </summary>
sealed class MergeInsertionTracker : IVisualizationTracker
{
    private readonly int _n;
    private bool _isTopLevelPairing;
    private SortPhase _currentPhase;
    private readonly Dictionary<int, RoleType> _trackerRoles = new();

    internal MergeInsertionTracker(int n) => _n = n;

    public void ProcessPhase(SortPhase phase, int p1, int p2, int p3)
    {
        _currentPhase = phase;
        switch (phase)
        {
            case SortPhase.MergeInsertionPairing:
                _isTopLevelPairing = p2 == _n / 2 - 1;
                break;
            case SortPhase.MergeInsertionSortLarger:
                if (_isTopLevelPairing)
                {
                    _isTopLevelPairing = false;
                    if (_n % 2 == 1)
                        _trackerRoles[_n - 1] = RoleType.FjStraggler;
                }
                break;
            case SortPhase.MergeInsertionInsertPend:
                _trackerRoles.Clear();
                break;
        }
    }

    public void Process(SortOperation op, int[] mainArray, Dictionary<int, int[]> buffers)
    {
        if (!_isTopLevelPairing) return;
        if (_currentPhase != SortPhase.MergeInsertionPairing) return;
        if (op.Type != OperationType.Compare || op.BufferId1 != 0 || op.BufferId2 != 0) return;

        if (op.CompareResult <= 0)
        {
            _trackerRoles[op.Index1] = RoleType.FjSmaller;
            _trackerRoles[op.Index2] = RoleType.FjLarger;
        }
        else
        {
            _trackerRoles[op.Index2] = RoleType.FjSmaller;
            _trackerRoles[op.Index1] = RoleType.FjLarger;
        }
    }

    public TutorialStep Decorate(TutorialStep step)
    {
        if (_trackerRoles.Count == 0) return step;
        var merged = new Dictionary<int, RoleType>(step.Roles);
        foreach (var (idx, role) in _trackerRoles)
            merged[idx] = role;
        return step with { Roles = merged };
    }

    public void PostStep() { }
}
