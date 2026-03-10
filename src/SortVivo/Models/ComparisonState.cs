namespace SortVivo.Models;

/// <summary>
/// 比較モードの状態を管理
/// </summary>
public class ComparisonState
{
    /// <summary>
    /// 比較中のアルゴリズムリスト（1-6個）
    /// </summary>
    public List<ComparisonInstance> Instances { get; set; } = new();

    /// <summary>
    /// 共通の初期配列
    /// </summary>
    public int[] InitialArray { get; set; } = Array.Empty<int>();

    /// <summary>
    /// 現在の配列サイズ
    /// </summary>
    public int CurrentArraySize { get; set; }

    /// <summary>
    /// 現在の配列パターン
    /// </summary>
    public ArrayPatternMetadata? CurrentPattern { get; set; }

    /// <summary>
    /// 現在処理中のアルゴリズム名（AddAndGenerate中）
    /// </summary>
    public string? ProcessingAlgorithmName { get; set; }

    /// <summary>
    /// 最大比較可能数
    /// </summary>
    public const int MaxComparisons = 6;

    /// <summary>
    /// 比較モードでの distinct アルゴリズム数に応じた最大配列サイズ。
    /// 各 SortOperation はヒープ上のオブジェクトとして記録されるため、
    /// アルゴリズム数が増えると総メモリ消費が線形に増加し WASM の OOM を引き起こす。
    /// - N=1:   制限なし (int.MaxValue)
    /// - N=2:   4096
    /// - N≥3:   2048
    /// </summary>
    public static int MaxComparisonElements(int distinctAlgorithmCount) => distinctAlgorithmCount switch
    {
        <= 1 => int.MaxValue,
        <= 2 => 4096,
        _ => 2048,
    };

    /// <summary>
    /// 既存インスタンス一覧と追加予定のアルゴリズム名から最大配列サイズを返す。
    /// 同一アルゴリズムを追加する場合は distinct 数が増えないため制限が緩和される。
    /// </summary>
    public static int MaxComparisonElements(List<ComparisonInstance> instances, string addingAlgorithmName)
    {
        var distinctCount = instances.Select(i => i.AlgorithmName).Distinct().Count();
        if (!instances.Any(i => i.AlgorithmName == addingAlgorithmName))
            distinctCount++;
        return MaxComparisonElements(distinctCount);
    }

    /// <summary>
    /// すべてのアルゴリズムが完了したかどうか
    /// </summary>
    public bool AllCompleted => Instances.All(x => x.State.IsSortCompleted);
}
