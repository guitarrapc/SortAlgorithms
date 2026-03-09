// SeekBar用のJavaScript互換モジュール

window.seekBarInterop = {
  // クリック位置からパーセンテージを計算
  getClickPercentage: function (elementId, clientX) {
    const element = document.getElementById(elementId);
    if (!element) return 0;

    const rect = element.getBoundingClientRect();
    const x = clientX - rect.left;
    const percentage = Math.max(0, Math.min(1, x / rect.width));
    return percentage;
  },

  // ドラッグ&ドロップのセットアップ
  setupDragDrop: function (elementId, dotnetHelper) {
    const element = document.getElementById(elementId);
    if (!element) return;

    let isDragging = false;

    const updatePosition = (clientX) => {
      const percentage = this.getClickPercentage(elementId, clientX);
      dotnetHelper.invokeMethodAsync('OnDrag', percentage);
    };

    // ─── マウスイベント ──────────────────────────────────────────────────
    const onMouseDown = (e) => {
      isDragging = true;
      updatePosition(e.clientX);
      e.preventDefault();
    };

    const onMouseMove = (e) => {
      if (isDragging) {
        updatePosition(e.clientX);
        e.preventDefault();
      }
    };

    const onMouseUp = () => {
      isDragging = false;
    };

    // ─── タッチイベント ──────────────────────────────────────────────────
    const onTouchStart = (e) => {
      isDragging = true;
      updatePosition(e.touches[0].clientX);
      e.preventDefault();
    };

    const onTouchMove = (e) => {
      if (isDragging) {
        updatePosition(e.touches[0].clientX);
        e.preventDefault();
      }
    };

    const onTouchEnd = () => {
      isDragging = false;
    };

    element.addEventListener('mousedown', onMouseDown);
    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);

    // passive: false でタッチ中のページスクロールを抑止する
    element.addEventListener('touchstart', onTouchStart, { passive: false });
    document.addEventListener('touchmove', onTouchMove, { passive: false });
    document.addEventListener('touchend', onTouchEnd);
    document.addEventListener('touchcancel', onTouchEnd);

    // クリーンアップ関数を返す
    return {
      dispose: () => {
        element.removeEventListener('mousedown', onMouseDown);
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);
        element.removeEventListener('touchstart', onTouchStart);
        document.removeEventListener('touchmove', onTouchMove);
        document.removeEventListener('touchend', onTouchEnd);
        document.removeEventListener('touchcancel', onTouchEnd);
      }
    };
  }
};
