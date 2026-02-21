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

    element.addEventListener('mousedown', onMouseDown);
    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);

    // クリーンアップ関数を返す
    return {
      dispose: () => {
        element.removeEventListener('mousedown', onMouseDown);
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);
      }
    };
  }
};
