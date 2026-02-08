namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// 配列生成パターンのメタデータ
/// </summary>
public record ArrayPatternMetadata
{
    /// <summary>パターンの表示名</summary>
    public required string Name { get; init; }
    
    /// <summary>パターンのカテゴリ</summary>
    public required string Category { get; init; }
    
    /// <summary>配列生成デリゲート</summary>
    public required Func<int, Random, int[]> Generator { get; init; }
    
    /// <summary>説明</summary>
    public string Description { get; init; } = string.Empty;
}
