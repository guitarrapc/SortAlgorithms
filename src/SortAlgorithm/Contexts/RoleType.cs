namespace SortAlgorithm.Contexts;

/// <summary>
/// チュートリアル用のロールタイプ。
/// アルゴリズムの意味的な役割をマーブルのバッジとして表示するために使用する。
/// </summary>
public enum RoleType
{
    /// <summary>ロールなし（クリア）</summary>
    None,

    /// <summary>Quick Sort のピボット要素</summary>
    Pivot,

    /// <summary>Selection Sort などの最小値候補</summary>
    CurrentMin,

    /// <summary>Bubble Sort などの最大値候補</summary>
    CurrentMax,

    /// <summary>2ポインタ系アルゴリズムの左端ポインタ</summary>
    LeftPointer,

    /// <summary>2ポインタ系アルゴリズムの右端ポインタ</summary>
    RightPointer,

    /// <summary>木ソートなどで入力配列から今まさに挿入対象として取り出した要素</summary>
    Inserting,

    /// <summary>Ford-Johnson ペア化フェーズでペア内の大きい方の要素 (a_i)</summary>
    FjLarger,

    /// <summary>Ford-Johnson ペア化フェーズでペア内の小さい方の要素 (b_i)</summary>
    FjSmaller,

    /// <summary>Ford-Johnson の奇数余り要素（ペアを持たない 1 要素）</summary>
    FjStraggler,
}
