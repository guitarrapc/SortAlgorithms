using SortAlgorithm.VisualizationWeb.Models;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// ValueBucket ビジュアライゼーション用トラッカー。
/// Pigeonhole sort / Counting sort / Bucket sort のバケット分配状態を追跡し、各ステップへ付加する。
/// </summary>
sealed class DistributionTracker : IVisualizationTracker
{
    private readonly int _minValue;
    private readonly int _bucketCount;
    private readonly string[] _bucketLabels;
    private readonly List<int>[] _buckets;
    private readonly int[] _shadowTemp;
    private readonly int[] _counts;
    private DistributionPhase _phase;
    private int? _pendingGatherBucket;
    private bool _trackCountingSort;
    private bool _trackBucketSort;
    private int _countPhaseReadCount;
    private bool _hadTempWrite;

    // Decorate() 用キャッシュ
    private DistributionSnapshot? _cachedSnapshot;
    private string? _cachedNarrative;

    internal DistributionTracker(int[] initialArray)
    {
        _minValue = initialArray.Length > 0 ? initialArray.Min() : 0;
        int maxValue = initialArray.Length > 0 ? initialArray.Max() : 0;
        _bucketCount = maxValue > _minValue ? maxValue - _minValue + 1 : 1;
        _bucketLabels = Enumerable.Range(_minValue, _bucketCount).Select(v => v.ToString()).ToArray();
        _buckets = Enumerable.Range(0, _bucketCount).Select(_ => new List<int>()).ToArray();
        _shadowTemp = new int[initialArray.Length];
        _counts = new int[_bucketCount];
        _phase = DistributionPhase.Scatter;
    }

    public void Process(SortOperation op, int[] mainArray, Dictionary<int, int[]> buffers)
    {
        int distActiveBucket = -1;
        int distActiveElement = -1;

        // === Counting sort 検出 ===
        // パターン: n × Read(main) (Count) → n × (Read(main) + Write(temp)) (Place) → RangeCopy
        if (op.Type == OperationType.IndexRead && op.BufferId1 == 0 && !_trackCountingSort && !_trackBucketSort)
        {
            if (_countPhaseReadCount == 0 && !_hadTempWrite)
            {
                _trackCountingSort = true;
                _phase = DistributionPhase.Count;
            }
        }

        if (_trackCountingSort && _phase == DistributionPhase.Count)
        {
            if (op.Type == OperationType.IndexRead && op.BufferId1 == 0)
            {
                int v = mainArray[op.Index1];
                int bIdx = v - _minValue;
                if ((uint)bIdx < (uint)_bucketCount)
                {
                    _counts[bIdx]++;
                    distActiveBucket = bIdx;
                    _countPhaseReadCount++;
                }
            }
            else if (op.Type == OperationType.IndexWrite && op.BufferId1 == 1)
            {
                _phase = DistributionPhase.Place;
                _countPhaseReadCount = 0;
            }
        }

        if (_trackCountingSort && _phase == DistributionPhase.Place)
        {
            if (op.Type == OperationType.IndexWrite && op.BufferId1 == 1 && op.Value.HasValue)
            {
                int v = op.Value.Value;
                int bIdx = v - _minValue;
                if ((uint)bIdx < (uint)_bucketCount)
                {
                    _buckets[bIdx].Add(v);
                    if (op.Index1 < _shadowTemp.Length)
                        _shadowTemp[op.Index1] = v;
                    distActiveBucket = bIdx;
                    distActiveElement = _buckets[bIdx].Count - 1;
                }
            }
            else if (op.Type == OperationType.RangeCopy && op.BufferId1 == 1 && op.BufferId2 == 0)
            {
                _phase = DistributionPhase.Gather;
                distActiveBucket = -1;
            }
        }

        // === Bucket sort 検出 ===
        // Scatter と Pigeonhole を区別: Write(temp) が出現したら Bucket sort 確定
        if (!_trackCountingSort && op.Type == OperationType.IndexWrite && op.BufferId1 == 1 && op.Value.HasValue)
        {
            if (!_trackBucketSort && _countPhaseReadCount == 0)
                _trackBucketSort = true;

            int v = op.Value.Value;
            int bIdx = v - _minValue;
            if ((uint)bIdx < (uint)_bucketCount)
            {
                _buckets[bIdx].Add(v);
                if (op.Index1 < _shadowTemp.Length)
                    _shadowTemp[op.Index1] = v;
                distActiveBucket = bIdx;
                distActiveElement = _buckets[bIdx].Count - 1;
                _phase = DistributionPhase.Scatter;
            }
        }

        // Pigeonhole Gather: Read(temp) + Write(main)
        if (!_trackCountingSort && !_trackBucketSort)
        {
            if (op.Type == OperationType.IndexRead && op.BufferId1 == 1)
            {
                if (op.Index1 >= 0 && op.Index1 < _shadowTemp.Length)
                {
                    int v = _shadowTemp[op.Index1];
                    int bIdx = v - _minValue;
                    if ((uint)bIdx < (uint)_bucketCount)
                    {
                        distActiveBucket = bIdx;
                        distActiveElement = _buckets[bIdx].Count - 1;
                        _pendingGatherBucket = bIdx;
                        _phase = DistributionPhase.Gather;
                    }
                }
            }
            else if (op.Type == OperationType.IndexWrite && op.BufferId1 == 0 && _pendingGatherBucket.HasValue)
            {
                int bIdx = _pendingGatherBucket.Value;
                if (_buckets[bIdx].Count > 0)
                    _buckets[bIdx].RemoveAt(_buckets[bIdx].Count - 1);
                distActiveBucket = bIdx;
                distActiveElement = -1;
                _pendingGatherBucket = null;
                _phase = DistributionPhase.Gather;
            }
        }

        // Bucket sort Gather: RangeCopy(temp→main)
        if (_trackBucketSort && op.Type == OperationType.RangeCopy && op.BufferId1 == 1 && op.BufferId2 == 0)
        {
            _phase = DistributionPhase.Gather;
            distActiveBucket = -1;
        }

        _cachedSnapshot = new DistributionSnapshot
        {
            BucketCount = _bucketCount,
            BucketLabels = _bucketLabels,
            Buckets = _buckets.Select(b => b.ToArray()).ToArray(),
            Phase = _phase,
            ActiveBucketIndex = distActiveBucket,
            ActiveElementInBucket = distActiveElement,
            Counts = _trackCountingSort ? (int[])_counts.Clone() : null,
        };

        // ナラティブ上書き
        int mainReadValue = op.Index1 >= 0 && op.Index1 < mainArray.Length ? mainArray[op.Index1] : 0;
        if (_trackCountingSort)
        {
            _cachedNarrative = (op.Type, _phase) switch
            {
                (OperationType.IndexRead, DistributionPhase.Count)
                    => $"Count value {mainReadValue} — increment bucket [{_bucketLabels[distActiveBucket]}]",
                (OperationType.IndexWrite, DistributionPhase.Place) when op.Value.HasValue && distActiveBucket >= 0
                    => $"Place value {op.Value.Value} into position {op.Index1} from bucket [{_bucketLabels[distActiveBucket]}]",
                (OperationType.RangeCopy, _)
                    => "Gather all values back to main array — sorting complete",
                _ => null,
            };
        }
        else if (_trackBucketSort)
        {
            _cachedNarrative = (op.Type, op.BufferId1) switch
            {
                (OperationType.IndexRead, 0)
                    => $"Read value {mainReadValue} from index {op.Index1}",
                (OperationType.IndexWrite, 1) when op.Value.HasValue && distActiveBucket >= 0
                    => $"Scatter value {op.Value.Value} into bucket [{_bucketLabels[distActiveBucket]}]",
                (OperationType.RangeCopy, _)
                    => "Gather sorted buckets back to main array",
                _ => null,
            };
        }
        else // Pigeonhole
        {
            _cachedNarrative = (op.Type, op.BufferId1) switch
            {
                (OperationType.IndexRead, 0) when _phase == DistributionPhase.Scatter
                    => $"Read value {mainReadValue} from index {op.Index1}",
                (OperationType.IndexWrite, 1) when op.Value.HasValue && distActiveBucket >= 0
                    => $"Scatter value {op.Value.Value} into bucket [{_bucketLabels[distActiveBucket]}]",
                (OperationType.IndexRead, 1) when distActiveBucket >= 0 && op.Index1 < _shadowTemp.Length
                    => $"Pick up value {_shadowTemp[op.Index1]} from bucket [{_bucketLabels[distActiveBucket]}]",
                (OperationType.IndexWrite, 0) when _phase == DistributionPhase.Gather && distActiveBucket >= 0
                    => $"Place value {op.Value!.Value} from bucket [{_bucketLabels[distActiveBucket]}] to index {op.Index1}",
                _ => null,
            };
        }

        // _hadTempWrite は現在の op を処理した後に更新する（次回の検出判定に使用）
        if (op.Type == OperationType.IndexWrite && op.BufferId1 == 1)
            _hadTempWrite = true;
    }

    public TutorialStep Decorate(TutorialStep step)
    {
        if (_cachedSnapshot == null) return step;
        return step with
        {
            Distribution = _cachedSnapshot,
            Narrative = _cachedNarrative ?? step.Narrative,
        };
    }

    public void PostStep() { }
}
