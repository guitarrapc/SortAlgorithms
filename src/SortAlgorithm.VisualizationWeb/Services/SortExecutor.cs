using SortAlgorithm.Contexts;
using SortAlgorithm.VisualizationWeb.Models;
using System.Buffers;
using System.Diagnostics;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// ソート実行と操作記録を行うサービス
/// </summary>
public class SortExecutor
{
    /// <summary>
    /// 適応的計測の目標合計時間（ms）。
    /// Blazor WASM では Stopwatch が performance.now() を使用し解像度が ~1ms に制限される。
    /// 合計計測時間がこの値を超えるまで繰り返し実行することで安定した平均実行時間を得る。
    /// 高速ソート: 自動的に多数回ループ → 安定、低速ソート: 1回で閾値超過 → 即終了。
    /// </summary>
    private const double MeasurementTargetMs = 50.0;

    /// <summary>
    /// ソートを実行し、すべての操作を記録する
    /// </summary>
    public (List<SortOperation> Operations, StatisticsContext Statistics, TimeSpan ActualExecutionTime) ExecuteAndRecord(ReadOnlySpan<int> sourceArray, AlgorithmMetadata algorithm)
    {
        var operations = new List<SortOperation>();

        // ArrayPoolから配列をレンタル（CompositeContext用作業配列）
        var workArray = ArrayPool<int>.Shared.Rent(sourceArray.Length);

        // 計測専用配列（ArrayPoolで確保し、ループ内で再利用してアロケーションを抑制）
        // Span<int>に変更済みのため、.AsSpan(0, sourceArray.Length)で正確な長さにスライスして渡せる
        var measureArray = ArrayPool<int>.Shared.Rent(sourceArray.Length);
        Span<int> measureSpan = measureArray.AsSpan(0, sourceArray.Length);

        try
        {
            // ウォームアップ（JIT最適化を促進、計測に含めない）
            sourceArray.CopyTo(measureSpan);
            algorithm.SortAction(measureSpan, NullContext.Default);

            // 適応的反復計測:
            // wallClock     → ループ終了判定用（CopyTo 込みの経過時間）
            // sortOnlyTicks → ソート処理のみの累積 tick（CopyTo を除外）
            // 合計計測時間が MeasurementTargetMs を超えるまで繰り返し、
            // 実行回数で割ることで安定した平均実行時間を得る。
            // - 高速ソート（例: 0.01ms/run）→ ~5,000回ループ → 安定した平均値
            // - 低速ソート（例: 100ms/run） → 1回で閾値超過 → 即終了、UX影響なし
            sourceArray.CopyTo(measureSpan);
            var wallClock = Stopwatch.StartNew();
            long sortOnlyTicks = 0L;
            int runs = 0;
            do
            {
                var before = Stopwatch.GetTimestamp();
                algorithm.SortAction(measureSpan, NullContext.Default);
                sortOnlyTicks += Stopwatch.GetTimestamp() - before;
                runs++;
                if (wallClock.Elapsed.TotalMilliseconds < MeasurementTargetMs)
                    sourceArray.CopyTo(measureSpan);
            } while (wallClock.Elapsed.TotalMilliseconds < MeasurementTargetMs);
            wallClock.Stop();

            // ソートのみの平均実行時間（CopyTo のオーバーヘッドを除外）
            var actualExecutionTime = TimeSpan.FromSeconds((double)sortOnlyTicks / Stopwatch.Frequency / runs);

            // ワーク配列を初期状態にリセット（CompositeContext実行用）
            sourceArray.CopyTo(workArray.AsSpan(0, sourceArray.Length));

            // StatisticsContextを作成（正確な統計情報を記録）
            var statisticsContext = new StatisticsContext();
            
            // VisualizationContextを使って操作を記録
            var visualizationContext = new VisualizationContext(
                onCompare: (i, j, result, bufferIdI, bufferIdJ) =>
                {
                    operations.Add(new SortOperation
                    {
                        Type = OperationType.Compare,
                        Index1 = i,
                        Index2 = j,
                        BufferId1 = bufferIdI,
                        BufferId2 = bufferIdJ,
                        CompareResult = result
                    });
                },
                onSwap: (i, j, bufferId) =>
                {
                    operations.Add(new SortOperation
                    {
                        Type = OperationType.Swap,
                        Index1 = i,
                        Index2 = j,
                        BufferId1 = bufferId
                    });
                },
                onIndexRead: (index, bufferId) =>
                {
                    operations.Add(new SortOperation
                    {
                        Type = OperationType.IndexRead,
                        Index1 = index,
                        BufferId1 = bufferId
                    });
                },
                onIndexWrite: (index, bufferId, value) =>
                {
                    operations.Add(new SortOperation
                    {
                        Type = OperationType.IndexWrite,
                        Index1 = index,
                        BufferId1 = bufferId,
                        Value = value as int?
                    });
                },
                onRangeCopy: (sourceIndex, destIndex, length, sourceBufferId, destBufferId, values) =>
                {
                    operations.Add(new SortOperation
                    {
                        Type = OperationType.RangeCopy,
                        Index1 = sourceIndex,
                        Index2 = destIndex,
                        Length = length,
                        BufferId1 = sourceBufferId,
                        BufferId2 = destBufferId,
                        Values = values?.Length > 0
                            ? Array.ConvertAll(values, v => v is int intVal ? intVal : 0)
                            : null
                    });
                }
            );
            
            // CompositeContextを作成して両方のコンテキストを組み合わせる
            var compositeContext = new CompositeContext(statisticsContext, visualizationContext);
            
            // 2回目: CompositeContextで操作・統計を記録（NullContextで計測した実行時間を使用）
            algorithm.SortAction(workArray.AsSpan(0, sourceArray.Length), compositeContext);
            
            
            return (operations, statisticsContext, actualExecutionTime);
        }
        finally
        {
            // ArrayPoolに配列を返却
            ArrayPool<int>.Shared.Return(workArray, clearArray: true);
            ArrayPool<int>.Shared.Return(measureArray, clearArray: true);
        }
    }
}
