'use strict';
// PlaybackService 用 rAF ドリブンループ
//
// 設計方針:
//   - 全 PlaybackService インスタンスを単一 rAF ループで管理する。
//     ComparisonMode の 9 インスタンスも 1 つの rAF ループ内で直列処理されるため効率的。
//   - dotNetRef.invokeMethod(...) の同期呼び出し（Blazor WASM 専用）を使用することで、
//     Promise チェーンのオーバーヘッドと非同期タイミングのズレを排除する。
//   - rAF は vsync に同期しているため Task.Delay の ~16ms 精度問題が解消される。

window.playbackHelper = {
    // instanceId → DotNetObjectReference
    _instances: new Map(),
    _rafId: null,

    /**
     * インスタンスを登録して rAF ループを開始する。
     * @param {string} instanceId - PlaybackService の一意 ID
     * @param {object} dotNetRef  - DotNetObjectReference<PlaybackService>
     */
    start: function(instanceId, dotNetRef) {
        this._instances.set(instanceId, dotNetRef);
        if (this._rafId === null) {
            this._startLoop();
        }
    },

    /**
     * インスタンスを解除する。全インスタンスがなくなれば rAF ループも停止する。
     * @param {string} instanceId
     */
    stop: function(instanceId) {
        this._instances.delete(instanceId);
        if (this._instances.size === 0 && this._rafId !== null) {
            cancelAnimationFrame(this._rafId);
            this._rafId = null;
        }
    },

    _startLoop: function() {
        const tick = () => {
            if (this._instances.size === 0) {
                this._rafId = null;
                return;
            }

            const toStop = [];

            this._instances.forEach((dotNetRef, id) => {
                try {
                    // invokeMethod（同期）は Blazor WASM 専用 API。
                    // C# の OnAnimationFrame() を同期呼び出しし bool を受け取る。
                    // false が返ったらループからこのインスタンスを削除する。
                    const shouldContinue = dotNetRef.invokeMethod('OnAnimationFrame');
                    if (!shouldContinue) {
                        toStop.push(id);
                    }
                } catch (e) {
                    // ObjectDisposedException など、インスタンスが無効になった場合は除去
                    toStop.push(id);
                }
            });

            toStop.forEach(id => this._instances.delete(id));

            if (this._instances.size > 0) {
                this._rafId = requestAnimationFrame(tick);
            } else {
                this._rafId = null;
            }
        };

        this._rafId = requestAnimationFrame(tick);
    }
};
