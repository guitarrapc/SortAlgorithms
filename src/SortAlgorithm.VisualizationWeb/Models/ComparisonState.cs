namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// 比較モードの状態を管理
/// </summary>
public class ComparisonState
{
    /// <summary>
    /// 比較中のアルゴリズムリスト（1-9個）
    /// </summary>
    public List<ComparisonInstance> Instances { get; set; } = new();
    
    /// <summary>
    /// 共通の初期配列
    /// </summary>
    public int[] InitialArray { get; set; } = Array.Empty<int>();
    
    /// <summary>
    /// 現在の配列サイズ
    /// </summary>
    public int CurrentArraySize { get; set; }
    
    /// <summary>
    /// 現在の配列パターン
    /// </summary>
    public ArrayPatternMetadata? CurrentPattern { get; set; }
    
    /// <summary>
    /// 比較モードが有効か
    /// </summary>
    public bool IsEnabled { get; set; }
    
    /// <summary>
    /// 最大比較可能数
    /// </summary>
    public const int MaxComparisons = 9;
    
    /// <summary>
    /// グリッド列数を計算
    /// </summary>
    public int GetGridColumns()
    {
        return Instances.Count switch
        {
            1 => 1,
            2 => 2,
            3 => 3,
            4 => 2,
            5 => 3,
            6 => 3,
            7 => 3,
            8 => 3,
            9 => 3,
            _ => 1
        };
    }
    
    /// <summary>
    /// すべてのアルゴリズムが完了したかどうか
    /// </summary>
    public bool AllCompleted => Instances.All(x => x.State.IsSortCompleted);
}
