using System.Buffers;
using SortAlgorithm.Contexts;
using SortAlgorithm.VisualizationWeb.Models;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// 再生制御とシーク処理を行うサービス（Task.Run高速ループ版）
/// </summary>
public class PlaybackService : IDisposable
{
    private List<SortOperation> _operations = [];
    
    // ArrayPoolで配列を再利用
    private int[] _pooledArray;
    private int _currentArraySize;
    private int[] _initialArray = [];
    private Dictionary<int, int[]> _initialBuffers = new();
    
    // 累積統計（各操作インデックスでの統計値）
    private CumulativeStats[] _cumulativeStats = [];
    
    private const int TARGET_FPS = 60; // ベースフレームレート
    private const int MAX_ARRAY_SIZE = 4096; // 最大配列サイズ
    private const double RENDER_INTERVAL_MS = 16.67; // UI更新間隔（60 FPS）
    
    // Task.Run用のフィールド
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _playbackTask;
    
    // 完了ハイライト用のタイマー
    private CancellationTokenSource? _completionHighlightCts;
    private const int COMPLETION_HIGHLIGHT_DURATION_MS = 2000; // 2秒
    
    // シークのスロットリング用
    private DateTime _lastSeekTime = DateTime.MinValue;
    private const int SEEK_THROTTLE_MS = 16; // 60 FPS
    
    /// <summary>現在の状態</summary>
    public VisualizationState State { get; private set; } = new();
    
    /// <summary>1フレームあたりの操作数（1-1000）</summary>
    public int OperationsPerFrame { get; set; } = 1;
    
    /// <summary>速度倍率（0.1x - 100x）</summary>
    public double SpeedMultiplier { get; set; } = 10.0;
    
    /// <summary>ソート完了時に自動的にリセットするか</summary>
    public bool AutoReset { get; set; } = false;
    
    /// <summary>描画なし超高速モード</summary>
    public bool InstantMode { get; set; } = false;
    
    /// <summary>状態が変更されたときのイベント</summary>
    public event Action? StateChanged;
    
    public PlaybackService()
    {
        // 最大サイズの配列をArrayPoolからレンタル
        _pooledArray = ArrayPool<int>.Shared.Rent(MAX_ARRAY_SIZE);
        _currentArraySize = 0;
    }
    
    /// <summary>
    /// ソート操作をロードする
    /// </summary>
    public void LoadOperations(ReadOnlySpan<int> initialArray, List<SortOperation> operations, StatisticsContext statistics)
    {
        Stop();
        _operations = operations;
        _currentArraySize = initialArray.Length;
        
        // プールされた配列の必要な部分だけを使用
        initialArray.CopyTo(_pooledArray.AsSpan(0, _currentArraySize));
        _initialArray = _pooledArray.AsSpan(0, _currentArraySize).ToArray(); // 初期状態のコピーを保持
        _initialBuffers.Clear();
        
        // 累積統計を計算（StatisticsContextの計算ロジックを使用）
        _cumulativeStats = new CumulativeStats[operations.Count + 1]; // +1は初期状態用
        ulong cumulativeCompares = 0;
        ulong cumulativeSwaps = 0;
        ulong cumulativeReads = 0;
        ulong cumulativeWrites = 0;
        
        for (int i = 0; i < operations.Count; i++)
        {
            var op = operations[i];
            
            // StatisticsContextと同じロジックで累積統計を計算
            switch (op.Type)
            {
                case OperationType.Compare:
                    cumulativeCompares++;
                    break;
                    
                case OperationType.Swap:
                    if (op.BufferId1 >= 0) // StatisticsContextと同じ条件
                    {
                        cumulativeSwaps++;
                        cumulativeReads += 2;  // Swap = 2 reads
                        cumulativeWrites += 2; // Swap = 2 writes
                    }
                    break;
                    
                case OperationType.IndexRead:
                    if (op.BufferId1 >= 0)
                    {
                        cumulativeReads++;
                    }
                    break;
                    
                case OperationType.IndexWrite:
                    if (op.BufferId1 >= 0)
                    {
                        cumulativeWrites++;
                    }
                    break;
                    
                case OperationType.RangeCopy:
                    if (op.BufferId1 >= 0)
                    {
                        cumulativeReads += (ulong)op.Length;
                    }
                    if (op.BufferId2 >= 0)
                    {
                        cumulativeWrites += (ulong)op.Length;
                    }
                    break;
            }
            
            // この操作後の累積統計を保存（インデックスi+1に保存）
            _cumulativeStats[i + 1] = new CumulativeStats
            {
                CompareCount = cumulativeCompares,
                SwapCount = cumulativeSwaps,
                IndexReadCount = cumulativeReads,
                IndexWriteCount = cumulativeWrites
            };
        }
        
        // 現在のVisualizationModeを保持
        var currentMode = State.Mode;
        
        State = new VisualizationState
        {
            MainArray = _pooledArray.AsSpan(0, _currentArraySize).ToArray(), // 現在の状態用のコピー
            TotalOperations = operations.Count,
            CurrentOperationIndex = 0,
            PlaybackState = PlaybackState.Stopped,
            Mode = currentMode, // モードを引き継ぐ
            IsSortCompleted = false, // 明示的にfalseに設定
            ShowCompletionHighlight = false, // ハイライト表示もfalse
            Statistics = statistics, // StatisticsContextを設定（最終値として保持）
            CumulativeStats = _cumulativeStats // 累積統計配列を設定
        };
        
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 再生開始
    /// </summary>
    public void Play()
    {
        if (State.PlaybackState == PlaybackState.Playing) return;
        
        State.PlaybackState = PlaybackState.Playing;
        
        // 既存のタスクをキャンセル
        _cancellationTokenSource?.Cancel();
        
        // 新しいキャンセルトークンを作成
        _cancellationTokenSource = new CancellationTokenSource();
        
        // 描画なしモードの場合は即座に完了
        if (InstantMode)
        {
            PlayInstant();
            return;
        }
        
        // バックグラウンドで高速ループを開始
        _playbackTask = Task.Run(() => PlaybackLoopAsync(_cancellationTokenSource.Token));
        
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 描画なし超高速実行
    /// </summary>
    private async void PlayInstant()
    {
        // UI更新を完全スキップして全操作を処理
        while (State.CurrentOperationIndex < _operations.Count)
        {
            var operation = _operations[State.CurrentOperationIndex];
            ApplyOperation(operation, applyToArray: true, updateStats: true);
            State.CurrentOperationIndex++;
        }
        
        // 完了
        ClearHighlights(); // ソート完了時にハイライトをクリア
        State.IsSortCompleted = true; // ソート完了フラグを設定
        State.ShowCompletionHighlight = true; // ハイライト表示を開始
        State.PlaybackState = PlaybackState.Paused;
        
        // 最終状態を描画（緑色ハイライト表示）
        StateChanged?.Invoke();
        
        // AutoResetがONの場合は、少し待ってからリセット
        if (AutoReset)
        {
            await Task.Delay(500); // 500ms緑色を表示
            Stop();
        }
        else
        {
            // 完了ハイライトを2秒後にクリア
            ScheduleCompletionHighlightClear();
        }
    }
    
    /// <summary>
    /// 一時停止
    /// </summary>
    public void Pause()
    {
        if (State.PlaybackState != PlaybackState.Playing) return;
        
        State.PlaybackState = PlaybackState.Paused;
        _cancellationTokenSource?.Cancel();
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 停止してリセット
    /// </summary>
    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _completionHighlightCts?.Cancel(); // 完了ハイライトタイマーもキャンセル
        State.CurrentOperationIndex = 0;
        
        // プールされた配列を再利用
        if (_currentArraySize > 0)
        {
            _initialArray.AsSpan().CopyTo(_pooledArray.AsSpan(0, _currentArraySize));
            State.MainArray = _pooledArray.AsSpan(0, _currentArraySize).ToArray();
        }
        
        State.BufferArrays.Clear();
        State.PlaybackState = PlaybackState.Stopped;
        State.IsSortCompleted = false; // リセット時に完了フラグをクリア
        State.ShowCompletionHighlight = false; // ハイライト表示もクリア
        ClearHighlights();
        ResetStatistics();
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 高速再生ループ（SpinWait高精度版）
    /// </summary>
    private async Task PlaybackLoopAsync(CancellationToken cancellationToken)
    {
        var lastRenderTime = DateTime.UtcNow;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var nextFrameTime = 0.0;
        
        try
        {
            while (!cancellationToken.IsCancellationRequested && State.CurrentOperationIndex < _operations.Count)
            {
                // フレーム間隔を計算（ミリ秒）
                var frameInterval = 1000.0 / (TARGET_FPS * SpeedMultiplier);
                
                // 次のフレーム時刻まで待機
                var currentTime = sw.Elapsed.TotalMilliseconds;
                if (currentTime < nextFrameTime)
                {
                    // 高精度待機: SpinWait
                    var spinWait = new SpinWait();
                    while (sw.Elapsed.TotalMilliseconds < nextFrameTime && !cancellationToken.IsCancellationRequested)
                    {
                        spinWait.SpinOnce(); // CPUビジーウェイト
                    }
                }
                
                nextFrameTime = sw.Elapsed.TotalMilliseconds + frameInterval;
                
                // 操作を処理
                ClearHighlights();
                
                int opsToProcess = Math.Min(OperationsPerFrame, _operations.Count - State.CurrentOperationIndex);
                for (int i = 0; i < opsToProcess && State.CurrentOperationIndex < _operations.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    
                    var operation = _operations[State.CurrentOperationIndex];
                    ApplyOperation(operation, applyToArray: true, updateStats: true);
                    State.CurrentOperationIndex++;
                }
                
                // ハイライト更新
                if (State.CurrentOperationIndex > 0 && State.CurrentOperationIndex < _operations.Count)
                {
                    var lastOperation = _operations[State.CurrentOperationIndex - 1];
                    ApplyOperation(lastOperation, applyToArray: false, updateStats: false);
                }
                
                // UI更新（60 FPS制限）
                var now = DateTime.UtcNow;
                var renderElapsed = (now - lastRenderTime).TotalMilliseconds;
                
                if (renderElapsed >= RENDER_INTERVAL_MS)
                {
                    lastRenderTime = now;
                    StateChanged?.Invoke();
                    await Task.Yield(); // UIスレッドに処理を譲る
                }
            }
            
            // 完了処理
            if (State.CurrentOperationIndex >= _operations.Count)
            {
                ClearHighlights(); // ソート完了時にハイライトをクリア
                State.BufferArrays.Clear(); // 🔧 バッファー配列をクリア（表示を消す）
                State.IsSortCompleted = true; // ソート完了フラグを設定
                State.ShowCompletionHighlight = true; // ハイライト表示を開始
                State.PlaybackState = PlaybackState.Paused;
                
                // 緑色ハイライトを表示
                StateChanged?.Invoke();
                
                // AutoResetがONの場合は、少し待ってからリセット
                if (AutoReset)
                {
                    await Task.Delay(500); // 500ms緑色を表示
                    Stop();
                }
                else
                {
                    // 完了ハイライトを2秒後にクリア
                    ScheduleCompletionHighlightClear();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセル時は何もしない
        }
    }
    
    /// <summary>
    /// 再生/一時停止を切り替え
    /// </summary>
    public void TogglePlayPause()
    {
        if (State.PlaybackState == PlaybackState.Playing)
        {
            Pause();
        }
        else
        {
            Play();
        }
    }
    
    /// <summary>
    /// 指定位置にシーク（インクリメンタル方式で高速化）
    /// </summary>
    public void SeekTo(int operationIndex, bool throttle = false)
    {
        if (operationIndex < 0 || operationIndex > _operations.Count)
            return;
        
        // スロットリング: 連続シーク時は一定間隔でのみ処理
        if (throttle)
        {
            var now = DateTime.UtcNow;
            var elapsed = (now - _lastSeekTime).TotalMilliseconds;
            if (elapsed < SEEK_THROTTLE_MS)
            {
                return; // スキップ
            }
            _lastSeekTime = now;
        }
        
        var currentIndex = State.CurrentOperationIndex;
        var targetIndex = operationIndex;
        
        // 現在位置と目的位置が近い場合は、インクリメンタルシーク
        var distance = Math.Abs(targetIndex - currentIndex);
        var replayThreshold = Math.Min(1000, _operations.Count / 4); // 閾値: 1000操作 or 全体の25%
        
        if (distance < replayThreshold && currentIndex <= targetIndex)
        {
            // 前方シーク: 現在位置から目的位置まで進める（高速）
            SeekForward(currentIndex, targetIndex);
        }
        else
        {
            // 後方シークまたは距離が遠い場合: 初期状態からリプレイ
            SeekFromBeginning(targetIndex);
        }
        
        State.CurrentOperationIndex = targetIndex;
        
        // ソート完了状態を更新
        State.IsSortCompleted = (operationIndex >= _operations.Count);
        State.ShowCompletionHighlight = State.IsSortCompleted;
        
        // 現在の操作をハイライト（完了時はハイライトなし）
        ClearHighlights();
        if (targetIndex < _operations.Count)
        {
            ApplyOperation(_operations[targetIndex], applyToArray: false, updateStats: false);
        }
        
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 前方シーク: 現在位置から目的位置まで進める
    /// </summary>
    private void SeekForward(int fromIndex, int toIndex)
    {
        for (int i = fromIndex; i < toIndex && i < _operations.Count; i++)
        {
            ApplyOperation(_operations[i], applyToArray: true, updateStats: true);
        }
    }
    
    /// <summary>
    /// 初期状態からリプレイ
    /// </summary>
    private void SeekFromBeginning(int targetIndex)
    {
        // 初期状態から指定位置まで操作を適用
        State.MainArray = [.. _initialArray];
        State.BufferArrays.Clear();
        ResetStatistics();
        
        for (int i = 0; i < targetIndex && i < _operations.Count; i++)
        {
            ApplyOperation(_operations[i], applyToArray: true, updateStats: true);
        }
    }
    
    private void ApplyOperation(SortOperation operation, bool applyToArray, bool updateStats)
    {
        switch (operation.Type)
        {
            case OperationType.Compare:
                State.CompareIndices.Add(operation.Index1);
                State.CompareIndices.Add(operation.Index2);
                break;
                
            case OperationType.Swap:
                State.SwapIndices.Add(operation.Index1);
                State.SwapIndices.Add(operation.Index2);
                if (applyToArray)
                {
                    var arr = GetArray(operation.BufferId1).AsSpan();
                    (arr[operation.Index1], arr[operation.Index2]) = (arr[operation.Index2], arr[operation.Index1]);
                }
                break;
                
            case OperationType.IndexRead:
                State.ReadIndices.Add(operation.Index1);
                break;
                
            case OperationType.IndexWrite:
                State.WriteIndices.Add(operation.Index1);
                if (applyToArray && operation.Value.HasValue)
                {
                    var arr = GetArray(operation.BufferId1).AsSpan();
                    if (operation.Index1 >= 0 && operation.Index1 < arr.Length)
                    {
                        arr[operation.Index1] = operation.Value.Value;
                    }
                }
                break;
                
            case OperationType.RangeCopy:
                // ハイライト表示: sourceとdestinationの範囲をハイライト
                for (int i = 0; i < operation.Length; i++)
                {
                    if (operation.Index1 >= 0)
                    {
                        State.ReadIndices.Add(operation.Index1 + i);
                    }
                    if (operation.Index2 >= 0)
                    {
                        State.WriteIndices.Add(operation.Index2 + i);
                    }
                }
                
                if (applyToArray)
                {
                    var sourceArr = GetArray(operation.BufferId1);
                    var destArr = GetArray(operation.BufferId2);
                    
                    var sourceSpan = sourceArr.AsSpan();
                    var destSpan = destArr.AsSpan();
                    
                    if (operation.Index1 >= 0 && operation.Index2 >= 0 && 
                        operation.Length > 0 &&
                        operation.Index1 + operation.Length <= sourceSpan.Length &&
                        operation.Index2 + operation.Length <= destSpan.Length)
                    {
                        sourceSpan.Slice(operation.Index1, operation.Length)
                            .CopyTo(destSpan.Slice(operation.Index2, operation.Length));
                    }
                }
                break;
        }
    }
    
    private int[] GetArray(int bufferId)
    {
        if (bufferId == 0) return State.MainArray;
        
        // バッファー配列が存在しない場合のみ作成
        if (!State.BufferArrays.ContainsKey(bufferId))
        {
            State.BufferArrays[bufferId] = new int[State.MainArray.Length];
        }
        return State.BufferArrays[bufferId];
    }
    
    
    /// <summary>
    /// フレームを進める（ComparisonModeService用の公開メソッド）
    /// </summary>
    public void AdvanceFrame(int opsToProcess)
    {
        if (State.CurrentOperationIndex >= _operations.Count)
            return;
        
        ClearHighlights();
        
        int actualOps = Math.Min(opsToProcess, _operations.Count - State.CurrentOperationIndex);
        for (int i = 0; i < actualOps && State.CurrentOperationIndex < _operations.Count; i++)
        {
            var operation = _operations[State.CurrentOperationIndex];
            ApplyOperation(operation, applyToArray: true, updateStats: true);
            State.CurrentOperationIndex++;
        }
        
        // ハイライト更新（最後の操作）
        if (State.CurrentOperationIndex > 0 && State.CurrentOperationIndex < _operations.Count)
        {
            var lastOperation = _operations[State.CurrentOperationIndex - 1];
            ApplyOperation(lastOperation, applyToArray: false, updateStats: false);
        }
        
        // StateChangedは呼ばない（ComparisonModeServiceが統一的に呼ぶ）
    }
    
    private void ClearHighlights()
    {
        State.CompareIndices.Clear();
        State.SwapIndices.Clear();
        State.ReadIndices.Clear();
        State.WriteIndices.Clear();
    }
    
    private void ResetStatistics()
    {
        // StatisticsContextがある場合は何もしない（イミュータブル）
    }
    
    /// <summary>
    /// 完了ハイライトを指定時間後にクリア
    /// </summary>
    private async void ScheduleCompletionHighlightClear()
    {
        // 既存のタイマーをキャンセル
        _completionHighlightCts?.Cancel();
        _completionHighlightCts = new CancellationTokenSource();
        
        try
        {
            await Task.Delay(COMPLETION_HIGHLIGHT_DURATION_MS, _completionHighlightCts.Token);
            
            // ハイライト表示だけをクリア（IsSortCompletedは維持）
            State.ShowCompletionHighlight = false;
            StateChanged?.Invoke();
        }
        catch (TaskCanceledException)
        {
            // キャンセルされた場合は何もしない
        }
    }
    
    public void Dispose()
    {
        // 再生中のタスクをキャンセル
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        
        // 完了ハイライトタイマーをキャンセル
        _completionHighlightCts?.Cancel();
        _completionHighlightCts?.Dispose();
        
        // タスクの完了を待機（最大1秒）
        _playbackTask?.Wait(TimeSpan.FromSeconds(1));
        
        // 累積統計配列をクリア（メモリリーク防止）
        _cumulativeStats = [];
        _operations.Clear();
        _initialBuffers.Clear();
        
        // ArrayPoolに配列を返却
        if (_pooledArray != null)
        {
            ArrayPool<int>.Shared.Return(_pooledArray, clearArray: true);
        }
    }
}
