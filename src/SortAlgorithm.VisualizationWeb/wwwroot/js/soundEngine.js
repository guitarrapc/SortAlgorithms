'use strict';
// Sound Engine - Web Audio API ラッパー
//
// 設計方針:
//   - AudioContext はユーザー操作後に initAudio() で初期化する（ブラウザ Autoplay Policy 対応）
//   - playNotes(frequencies, duration) で複数音を一括発音（JS Interop 1回/フレーム）
//   - OscillatorNode + GainNode で正弦波を生成し、linearRamp でフェードアウト

window.soundEngine = {
    _audioContext: null,

    /**
     * AudioContext を初期化・再開する。ユーザー操作（トグル押下など）後に呼ぶ。
     */
    initAudio: function () {
        if (!this._audioContext) {
            try {
                this._audioContext = new AudioContext();
            } catch (e) {
                return;
            }
        }
        if (this._audioContext.state === 'suspended') {
            this._audioContext.resume();
        }
    },

    /**
     * 複数の周波数を同時に発音する。
     * @param {number[]} frequencies - 発音する周波数の配列（Hz）
     * @param {number} duration - 発音時間（ms）。0 の場合は何もしない。
     */
    playNotes: function (frequencies, duration) {
        if (duration <= 0 || !frequencies || frequencies.length === 0) return;

        // AudioContext が未初期化の場合は遅延初期化（フォールバック）
        if (!this._audioContext) {
            try {
                this._audioContext = new AudioContext();
            } catch (e) {
                return;
            }
        }
        if (this._audioContext.state === 'suspended') {
            this._audioContext.resume();
        }

        const ctx = this._audioContext;
        const now = ctx.currentTime;
        const durationSec = duration / 1000;

        for (let i = 0; i < frequencies.length; i++) {
            const freq = frequencies[i];
            if (freq <= 0) continue;

            const oscillator = ctx.createOscillator();
            const gainNode = ctx.createGain();

            oscillator.connect(gainNode);
            gainNode.connect(ctx.destination);

            oscillator.type = 'sine';
            oscillator.frequency.setValueAtTime(freq, now);

            // アタック即最大 → 終端に向けて線形フェードアウト（クリッピング防止）
            gainNode.gain.setValueAtTime(0.3, now);
            gainNode.gain.linearRampToValueAtTime(0.0, now + durationSec);

            oscillator.start(now);
            oscillator.stop(now + durationSec);
        }
    },

    /**
     * AudioContext を閉じてリソースを解放する。
     */
    disposeAudio: function () {
        if (this._audioContext) {
            this._audioContext.close();
            this._audioContext = null;
        }
    }
};
