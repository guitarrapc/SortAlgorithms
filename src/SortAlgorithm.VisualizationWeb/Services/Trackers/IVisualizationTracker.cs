using SortAlgorithm.VisualizationWeb.Models;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// ビジュアライゼーションヒントごとに SortOperation を処理し、
/// TutorialStep にヒント固有のフィールドとナラティブを付加するトラッカー。
/// </summary>
interface IVisualizationTracker
{
    /// <summary>
    /// ApplyOperation の前に呼び出す。内部状態とスナップショット・ナラティブキャッシュを更新する。
    /// </summary>
    void Process(SortOperation op, int[] mainArray, Dictionary<int, int[]> buffers);

    /// <summary>
    /// Phase 操作を受け取り、内部状態を更新する。
    /// TutorialStepBuilder が OperationType.Phase 操作を検出したときに呼び出す。
    /// デフォルト実装は何もしない。
    /// </summary>
    void ProcessPhase(SortAlgorithm.Contexts.SortPhase phase, int p1, int p2, int p3) { }

    /// <summary>
    /// ベース TutorialStep が構築された後に呼び出す。
    /// ヒント固有フィールドを付加し、必要に応じて Narrative を上書きした新しい step を返す。
    /// </summary>
    TutorialStep Decorate(TutorialStep step);

    /// <summary>
    /// step をリストに追加した後に呼び出す。後処理（LSD バケットクリアなど）に使用する。
    /// </summary>
    void PostStep();
}
