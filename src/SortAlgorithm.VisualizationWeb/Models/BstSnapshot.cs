namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// BinaryTreeSort チュートリアル用の BST スナップショット。
/// 各 TutorialStep に付属し、そのステップ時点の BST 構造を保持する。
/// ノード ID は挿入順（0 = 最初に挿入した要素 = 根）。
/// </summary>
public record BstSnapshot
{
    /// <summary>現在 BST に存在するノード数</summary>
    public int Size { get; init; }

    /// <summary>根ノードの ID。空木の場合 -1</summary>
    public int Root { get; init; } = -1;

    /// <summary>ノード ID → 値。Size 要素のみ有効</summary>
    public int[] Values { get; init; } = [];

    /// <summary>ノード ID → 左子 ID。子なしは -1</summary>
    public int[] Left { get; init; } = [];

    /// <summary>ノード ID → 右子 ID。子なしは -1</summary>
    public int[] Right { get; init; } = [];

    /// <summary>
    /// 直前の挿入で辿ったノード ID のリスト（根 → 挿入位置の親まで）。
    /// ビルドフェーズのみ使用。amber 色でハイライト表示する。
    /// </summary>
    public int[] InsertionPath { get; init; } = [];

    /// <summary>
    /// 直前に挿入されたノードの ID。ビルドフェーズのみ使用（-1 = なし）。
    /// 緑色でハイライト表示する。
    /// </summary>
    public int NewNode { get; init; } = -1;

    /// <summary>
    /// 中順走査で現在アクティブなノードの ID。走査フェーズのみ使用（-1 = なし）。
    /// 橙色でハイライト表示する。
    /// </summary>
    public int ActiveNode { get; init; } = -1;

    /// <summary>true のとき走査フェーズ（全ノード挿入済み、中順で配列に書き戻し中）</summary>
    public bool IsTraversalPhase { get; init; }

    /// <summary>
    /// AVL 木の各ノードの高さ配列（ノード ID → height）。
    /// null のとき通常 BST（高さ非表示）、non-null のとき AVL モード（balance factor を表示）。
    /// </summary>
    public int[]? Heights { get; init; }

    /// <summary>
    /// 直前の挿入で回転に関与したノード ID リスト。AVL のみ使用（他は空配列）。
    /// 紫色でハイライト表示する。
    /// </summary>
    public int[] RotatedNodes { get; init; } = [];
}
