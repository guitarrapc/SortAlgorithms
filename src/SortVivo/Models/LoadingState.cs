namespace SortVivo.Models;

/// <summary>
/// UI のローディング状態を表す列挙型。
/// Idle 以外の状態では操作コントロールを無効化し、インジケーターを表示する。
/// </summary>
public enum LoadingState
{
    /// <summary>通常状態（操作可能）</summary>
    Idle,

    /// <summary>ソート処理の実行・記録中（Generate &amp; Sort）</summary>
    Sorting,

    /// <summary>比較モードへのアルゴリズム追加中</summary>
    AddingAlgorithm,

    /// <summary>画像ファイルのアップロード・デコード中</summary>
    LoadingImage,
}
