namespace SortVivo.Models;

/// <summary>
/// Patience Sort パイル表示用スナップショット。
/// ディール（配り）フェーズとマージフェーズのパイル状態を保持する。
/// </summary>
public record PatienceSnapshot
{
    /// <summary>true = k-way マージフェーズ、false = ディール（配り）フェーズ</summary>
    public bool IsMergePhase { get; init; }

    /// <summary>
    /// 各パイルの値列。インデックス 0 = 底、最後 = トップ（スタック上端）。
    /// </summary>
    public int[][] PileValues { get; init; } = [];

    /// <summary>現在操作対象のパイルインデックス（-1 = なし）</summary>
    public int ActivePileIndex { get; init; } = -1;

    /// <summary>現在ディール中または抽出中の要素の値（-1 = なし）</summary>
    public int ActiveValue { get; init; } = -1;
}
