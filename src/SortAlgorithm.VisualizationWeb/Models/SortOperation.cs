namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// ソート操作を記録するレコード
/// </summary>
public record SortOperation
{
    /// <summary>操作のタイプ</summary>
    public required OperationType Type { get; init; }
    
    /// <summary>第1インデックス</summary>
    public int Index1 { get; init; }
    
    /// <summary>第2インデックス（Compareやスワップで使用）</summary>
    public int Index2 { get; init; }
    
    /// <summary>第1インデックスのバッファーID（0=メイン配列）</summary>
    public int BufferId1 { get; init; }
    
    /// <summary>第2インデックスのバッファーID（0=メイン配列）</summary>
    public int BufferId2 { get; init; }
    
    /// <summary>範囲コピーの長さ</summary>
    public int Length { get; init; }
    
    /// <summary>比較結果（Compareの場合）</summary>
    public int CompareResult { get; init; }
    
    /// <summary>書き込まれる値（IndexWriteの場合）</summary>
    public int? Value { get; init; }
}
