using SortAlgorithm.Contexts;

namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// アルゴリズムのメタデータ
/// </summary>
public record AlgorithmMetadata
{
    /// <summary>アルゴリズムの表示名</summary>
    public required string Name { get; init; }
    
    /// <summary>アルゴリズムのカテゴリ</summary>
    public required string Category { get; init; }
    
    /// <summary>時間計算量（平均）</summary>
    public required string TimeComplexity { get; init; }
    
    /// <summary>最大要素数</summary>
    public required int MaxElements { get; init; }
    
    /// <summary>推奨要素数</summary>
    public required int RecommendedSize { get; init; }
    
    /// <summary>ソート実行デリゲート</summary>
    public required Action<int[], ISortContext> SortAction { get; init; }
    
    /// <summary>説明</summary>
    public string Description { get; init; } = string.Empty;
}
