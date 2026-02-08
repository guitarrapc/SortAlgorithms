namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// デバッグログの表示を制御するサービス
/// </summary>
public class DebugSettings
{
    private bool _isEnabled = false;

    /// <summary>
    /// デバッグログが有効かどうか
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                OnChanged?.Invoke();
            }
        }
    }

    /// <summary>
    /// 設定変更時に発火するイベント
    /// </summary>
    public event Action? OnChanged;

    /// <summary>
    /// デバッグログを条件付きで出力する
    /// </summary>
    /// <param name="message">ログメッセージ</param>
    public void Log(string message)
    {
        if (_isEnabled)
        {
            Console.WriteLine(message);
        }
    }

    /// <summary>
    /// デバッグログを条件付きで出力する（フォーマット付き）
    /// </summary>
    /// <param name="format">フォーマット文字列</param>
    /// <param name="args">引数</param>
    public void Log(string format, params object[] args)
    {
        if (_isEnabled)
        {
            Console.WriteLine(format, args);
        }
    }
}
