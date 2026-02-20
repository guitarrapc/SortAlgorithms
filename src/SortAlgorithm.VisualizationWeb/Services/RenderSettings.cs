namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// レンダラー設定を管理するサービス
/// </summary>
public class RenderSettings
{
    private bool _useWebGL = true;

    /// <summary>
    /// WebGL レンダラーを使用するかどうか。
    /// false の場合は Canvas 2D Worker にフォールバックする。
    /// </summary>
    public bool UseWebGL
    {
        get => _useWebGL;
        set
        {
            if (_useWebGL != value)
            {
                _useWebGL = value;
                OnChanged?.Invoke();
            }
        }
    }

    /// <summary>
    /// 設定変更時に発火するイベント
    /// </summary>
    public event Action? OnChanged;
}
