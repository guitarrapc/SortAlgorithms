namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// 再生状態を表す列挙型
/// </summary>
public enum PlaybackState
{
    /// <summary>停止中（初期状態、リセット後）</summary>
    Stopped,
    
    /// <summary>再生中</summary>
    Playing,
    
    /// <summary>一時停止中</summary>
    Paused
}
