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

    /// <summary>コピーされる値の配列（RangeCopyの場合）</summary>
    public int[]? Values { get; init; }

    /// <summary>
    /// フェーズ種別（Phase 操作の場合）。
    /// param1/param2/param3 は既存の Index1/Index2/Length フィールドで保持する。
    /// </summary>
    public SortAlgorithm.Contexts.SortPhase PhaseKind { get; init; }

    /// <summary>ロールタイプ（RoleAssign 操作の場合、Index1 がインデックス、BufferId1 がバッファーID）</summary>
    public SortAlgorithm.Contexts.RoleType? RoleValue { get; init; }
}
