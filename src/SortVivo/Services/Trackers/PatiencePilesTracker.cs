using SortAlgorithm.Contexts;
using SortVivo.Models;

namespace SortVivo.Services;

/// <summary>
/// Patience Sort パイル表示用トラッカー。
/// ディールフェーズでは Compare 操作から現在配置中の要素を追跡し、
/// マージフェーズでは IndexRead からパイルトップの抽出を追跡する。
///
/// PatienceSort のパイル構築は SortContext 操作を出さないため、
/// コンストラクタで同一アルゴリズムをオフラインシミュレーションして
/// 各要素の配置先パイルを事前計算する。
/// </summary>
sealed class PatiencePilesTracker : IVisualizationTracker
{
    private readonly int[] _initialArray;
    private readonly int _n;

    // 事前計算: 要素インデックス → 配置先パイルインデックス
    private readonly int[] _elementPileAssignment;

    // ライブパイル状態（スタック: トップ = Peek(), 底 = 最後にプッシュされた要素）
    // 積み順: 古い要素が下、新しい要素（値が小さい or 等しい）が上
    private readonly List<Stack<int>> _pileLiveStacks = [];

    // 現在のフェーズ
    private SortPhase _currentPhase = SortPhase.None;

    // ディールフェーズ追跡
    private int _currentDealElement = -1; // 現在 Compare 中の要素インデックス (i)
    private int _lastPlacedElement = -1;  // 最後にスタックに積んだ要素インデックス

    // スナップショット用
    private int _activePile = -1;
    private int _activeValue = -1;

    internal PatiencePilesTracker(int[] initialArray)
    {
        _initialArray = initialArray;
        _n = initialArray.Length;
        _elementPileAssignment = new int[_n];

        // ── オフラインシミュレーション: 各要素の配置先を事前計算 ──────────────
        var simPileTops = new List<int>(); // 各パイルのトップ要素インデックス
        for (int i = 0; i < _n; i++)
        {
            int value = initialArray[i];
            int lo = 0, hi = simPileTops.Count;
            while (lo < hi)
            {
                int mid = lo + (hi - lo) / 2;
                // PatienceSort と同じ判定: top < value → go right
                if (initialArray[simPileTops[mid]] < value)
                    lo = mid + 1;
                else
                    hi = mid;
            }
            if (lo == simPileTops.Count)
                simPileTops.Add(i);
            else
                simPileTops[lo] = i;
            _elementPileAssignment[i] = lo;
        }
    }

    public void ProcessPhase(SortPhase phase, int p1, int p2, int p3)
    {
        if (phase == SortPhase.PatienceSortDeal)
        {
            _currentPhase = SortPhase.PatienceSortDeal;
            _pileLiveStacks.Clear();
            _currentDealElement = -1;
            _lastPlacedElement = -1;
            _activePile = -1;
            _activeValue = -1;
        }
        else if (phase == SortPhase.PatienceSortMerge)
        {
            // 未配置の末尾要素を全て積む
            FlushUnplaced(_n - 1);
            _currentPhase = SortPhase.PatienceSortMerge;
            _activePile = -1;
            _activeValue = -1;
        }
    }

    public void Process(SortOperation op, int[] mainArray, Dictionary<int, int[]> buffers)
    {
        if (_currentPhase == SortPhase.PatienceSortDeal
            && op.Type == OperationType.Compare
            && op.BufferId1 == 0 && op.BufferId2 == 0)
        {
            int elementIdx = op.Index2; // i (配置中の要素)

            if (elementIdx != _currentDealElement)
            {
                // 新しい要素に切り替わった → 前要素を含む未配置分を全て積む
                if (_currentDealElement >= 0)
                    FlushUnplaced(_currentDealElement);
                _currentDealElement = elementIdx;
            }

            // 現在考慮中の要素をハイライト（まだパイルには乗っていない）
            _activeValue = _initialArray[elementIdx];
            _activePile = -1;
        }
        else if (_currentPhase == SortPhase.PatienceSortMerge
            && op.Type == OperationType.IndexRead
            && op.BufferId1 == 0)
        {
            // s.Read(topIdx): パイルトップを抽出
            int topIdx = op.Index1;
            int targetPile = _elementPileAssignment[topIdx];
            if ((uint)targetPile < (uint)_pileLiveStacks.Count
                && _pileLiveStacks[targetPile].Count > 0
                && _pileLiveStacks[targetPile].Peek() == topIdx)
            {
                _pileLiveStacks[targetPile].Pop();
                _activePile = targetPile;
                _activeValue = _initialArray[topIdx];
            }
        }
    }

    public TutorialStep Decorate(TutorialStep step)
    {
        // 各パイルを底→トップ順（index 0 = 底、last = トップ）に変換
        var pileValues = _pileLiveStacks
            .Select(stack => stack.Reverse().Select(idx => _initialArray[idx]).ToArray())
            .ToArray();

        var snapshot = new PatienceSnapshot
        {
            IsMergePhase = _currentPhase == SortPhase.PatienceSortMerge,
            PileValues = pileValues,
            ActivePileIndex = _activePile,
            ActiveValue = _activeValue,
        };
        return step with { Patience = snapshot };
    }

    public void PostStep() { }

    // ─── ヘルパー ──────────────────────────────────────────────────────────

    /// <summary>
    /// 要素 0..<paramref name="upToInclusive"/> のうち未配置の要素を全てスタックへ積む。
    /// </summary>
    private void FlushUnplaced(int upToInclusive)
    {
        for (int k = _lastPlacedElement + 1; k <= upToInclusive && k < _n; k++)
        {
            int pileIdx = _elementPileAssignment[k];
            // パイルが足りなければ追加
            while (_pileLiveStacks.Count <= pileIdx)
                _pileLiveStacks.Add(new Stack<int>());
            _pileLiveStacks[pileIdx].Push(k);
        }
        _lastPlacedElement = Math.Min(upToInclusive, _n - 1);
        // 最後に積んだ要素を activeとして表示
        if (upToInclusive >= 0 && upToInclusive < _n)
        {
            _activePile = _elementPileAssignment[upToInclusive];
            _activeValue = _initialArray[upToInclusive];
        }
    }
}
