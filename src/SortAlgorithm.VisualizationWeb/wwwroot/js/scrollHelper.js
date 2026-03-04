// スクロールヘルパー - タブレット・スマホでビジュアライゼーションエリアへスクロール

window.scrollHelper = {
  /**
   * ビジュアライゼーションエリアへスムーズスクロール（タブレット・スマホのみ）
   * PCレイアウト（1280px以上）ではサイドバーが並列表示のためスキップ
   */
  scrollToVisualization: function () {
    if (window.innerWidth >= 1280) return;
    var el = document.getElementById('visualization-area');
    if (!el) return;
    el.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
};
