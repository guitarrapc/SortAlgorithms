// visualizationOverlay.js
// タッチデバイス（スマホ・タブレット）でグラフをタップしたとき、
// YouTube 風に半透明アイコンを一瞬表示してフェードアウトさせる。
// デスクトップ（マウス）では CSS :hover::after が担当するため、
// タッチイベントが使える環境でのみ動作する。

(function () {
    if (!('ontouchstart' in window)) return;

    var SELECTOR =
        '.bar-chart-container,' +
        '.circular-chart-container,' +
        '.disparity-chords-container,' +
        '.dot-plot-container,' +
        '.spiral-chart-container,' +
        '.picture-row-container,' +
        '.picture-column-container,' +
        '.picture-block-container';

    document.addEventListener('touchstart', function (e) {
        var container = e.target.closest(SELECTOR);
        if (!container) return;

        // アニメーションをリスタートするため一度クラスを外してリフロー
        container.classList.remove('overlay-flash');
        void container.offsetWidth;
        container.classList.add('overlay-flash');

        // アニメーション終了後にクラスを除去（2.5s = CSS animation duration）
        clearTimeout(container._overlayTimer);
        container._overlayTimer = setTimeout(function () {
            container.classList.remove('overlay-flash');
        }, 2500);
    }, { passive: true });
}());
