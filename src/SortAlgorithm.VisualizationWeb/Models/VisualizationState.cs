using SortAlgorithm.Contexts;

namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// 各操作時点での累積統計
/// </summary>
internal struct CumulativeStats
{
    public ulong CompareCount;
    public ulong SwapCount;
    public ulong IndexReadCount;
    public ulong IndexWriteCount;
}

/// <summary>
/// 可視化の状態を保持するクラス
/// </summary>
public class VisualizationState
{
    /// <summary>メイン配列</summary>
    public int[] MainArray { get; set; } = [];

    /// <summary>バッファー配列（BufferId -> 配列）</summary>
    public Dictionary<int, int[]> BufferArrays { get; set; } = new();

    /// <summary>比較操作中のインデックス</summary>
    public HashSet<int> CompareIndices { get; set; } = [];

    /// <summary>スワップ操作中のインデックス</summary>
    public HashSet<int> SwapIndices { get; set; } = [];

    /// <summary>読み込み操作中のインデックス</summary>
    public HashSet<int> ReadIndices { get; set; } = [];

    /// <summary>書き込み操作中のインデックス</summary>
    public HashSet<int> WriteIndices { get; set; } = [];

    /// <summary>現在の操作インデックス</summary>
    public int CurrentOperationIndex { get; set; }

    /// <summary>総操作数</summary>
    public int TotalOperations { get; set; }

    /// <summary>可視化モード</summary>
    public VisualizationMode Mode { get; set; } = VisualizationMode.BarChart;

    /// <summary>再生状態</summary>
    public PlaybackState PlaybackState { get; set; } = PlaybackState.Stopped;

    /// <summary>統計情報（StatisticsContextから取得した最終値）</summary>
    public StatisticsContext? Statistics { get; set; }

    /// <summary>累積統計（各操作インデックスでの統計値）</summary>
    internal CumulativeStats[]? CumulativeStats { get; set; }

    /// <summary>比較回数（累積統計または最終値を使用）</summary>
    public ulong CompareCount
    {
        get
        {
            // ソート完了時は最終値（StatisticsContext）を使用
            if (IsSortCompleted && Statistics != null)
                return Statistics.CompareCount;

            // プレイバック中は現在のインデックスに対応する累積値を使用
            if (CumulativeStats != null && CurrentOperationIndex >= 0 && CurrentOperationIndex < CumulativeStats.Length)
                return CumulativeStats[CurrentOperationIndex].CompareCount;

            return 0;
        }
    }

    /// <summary>スワップ回数（累積統計または最終値を使用）</summary>
    public ulong SwapCount
    {
        get
        {
            // ソート完了時は最終値（StatisticsContext）を使用
            if (IsSortCompleted && Statistics != null)
                return Statistics.SwapCount;

            // プレイバック中は現在のインデックスに対応する累積値を使用
            if (CumulativeStats != null && CurrentOperationIndex >= 0 && CurrentOperationIndex < CumulativeStats.Length)
                return CumulativeStats[CurrentOperationIndex].SwapCount;

            return 0;
        }
    }

    /// <summary>読み込み回数（累積統計または最終値を使用）</summary>
    public ulong IndexReadCount
    {
        get
        {
            // ソート完了時は最終値（StatisticsContext）を使用
            if (IsSortCompleted && Statistics != null)
                return Statistics.IndexReadCount;

            // プレイバック中は現在のインデックスに対応する累積値を使用
            if (CumulativeStats != null && CurrentOperationIndex >= 0 && CurrentOperationIndex < CumulativeStats.Length)
                return CumulativeStats[CurrentOperationIndex].IndexReadCount;

            return 0;
        }
    }

    /// <summary>書き込み回数（累積統計または最終値を使用）</summary>
    public ulong IndexWriteCount
    {
        get
        {
            // ソート完了時は最終値（StatisticsContext）を使用
            if (IsSortCompleted && Statistics != null)
                return Statistics.IndexWriteCount;

            // プレイバック中は現在のインデックスに対応する累積値を使用
            if (CumulativeStats != null && CurrentOperationIndex >= 0 && CurrentOperationIndex < CumulativeStats.Length)
                return CumulativeStats[CurrentOperationIndex].IndexWriteCount;

            return 0;
        }
    }

    /// <summary>ソートが完了したかどうか</summary>
    public bool IsSortCompleted { get; set; }

    /// <summary>ソート完了ハイライトを表示するかどうか（2秒間のみ）</summary>
    public bool ShowCompletionHighlight { get; set; }

    /// <summary>ソートの実際の実行時間（Stopwatchで計測した実測値）</summary>
    public TimeSpan ActualExecutionTime { get; set; }

    /// <summary>
    /// 再生進捗に応じた推定実行時間（線形補間）
    /// 再生中は0からActualExecutionTimeへ線形増加、完了時は確定値を返す
    /// </summary>
    public TimeSpan EstimatedCurrentExecutionTime
    {
        get
        {
            if (TotalOperations == 0 || ActualExecutionTime == TimeSpan.Zero)
                return ActualExecutionTime;
            if (IsSortCompleted)
                return ActualExecutionTime;
            var progressRatio = (double)CurrentOperationIndex / TotalOperations;
            return TimeSpan.FromTicks((long)(ActualExecutionTime.Ticks * progressRatio));
        }
    }
}

