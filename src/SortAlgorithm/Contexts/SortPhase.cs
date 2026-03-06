namespace SortAlgorithm.Contexts;

/// <summary>
/// アルゴリズムの現在フェーズを表す列挙型。
/// チュートリアル側でフェーズテキストを組み立てるための種別情報を提供する。
/// </summary>
/// <remarks>
/// フェーズごとのパラメータ意味:
/// <list type="table">
///   <listheader><term>Phase</term><description>param1 / param2 / param3</description></listheader>
///   <item><term>BubblePass</term><description>pass（現在パス番号） / totalPasses（総パス数） / boundary（右端位置）</description></item>
///   <item><term>SelectionFindMin</term><description>i（ソート済み境界） / last（末尾インデックス）</description></item>
/// </list>
/// </remarks>
public enum SortPhase
{
    /// <summary>フェーズ未設定（チュートリアルフェーズバーを非表示）</summary>
    None = 0,

    /// <summary>
    /// Bubble Sort のパス。
    /// param1=pass（現在パス番号, 1-based）, param2=totalPasses（総パス数）, param3=boundary（右端位置）
    /// </summary>
    BubblePass,

    /// <summary>
    /// Selection Sort の最小値探索。
    /// param1=i（ソート済み境界インデックス）, param2=last（探索末尾インデックス、inclusive）
    /// </summary>
    SelectionFindMin,
}
