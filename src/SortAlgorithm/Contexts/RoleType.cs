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
}
