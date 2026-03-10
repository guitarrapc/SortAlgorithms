namespace SortVivo.Models;

/// <summary>
/// ソート操作のタイプを表す列挙型
/// </summary>
public enum OperationType
{
    /// <summary>2つの要素を比較</summary>
    Compare,

    /// <summary>2つの要素を入れ替え</summary>
    Swap,

    /// <summary>インデックスから要素を読み込み</summary>
    IndexRead,

    /// <summary>インデックスに要素を書き込み</summary>
    IndexWrite,

    /// <summary>範囲コピー（連続した要素をまとめてコピー）</summary>
    RangeCopy,

    /// <summary>アルゴリズムのフェーズ通知（チュートリアル用メタデータ）</summary>
    Phase,

    /// <summary>配列要素への役割付与（チュートリアル用メタデータ）</summary>
    RoleAssign,
}
