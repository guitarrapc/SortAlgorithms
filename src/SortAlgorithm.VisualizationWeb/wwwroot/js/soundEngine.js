'use strict';
// Sound Engine - Web Audio API ラッパー
//
// 設計方針:
//   - AudioContext はユーザー操作後に initAudio() で初期化する（ブラウザ Autoplay Policy 対応）
//   - playNotes(frequencies, duration) で複数音を一括発音（JS Interop 1回/フレーム）
//   - OscillatorNode + GainNode で正弦波を生成し、5ms アタック + 線形フェードアウトで発音
//   - DynamicsCompressor をマスター出力に挟み、フレームをまたいだノート重なりのクリッピングを防止

window.soundEngine = {
    _audioContext: null,
    _output: null,  // DynamicsCompressor → destination

    /**
     * AudioContext を初期化・再開する。ユーザー操作（トグル押下など）後に呼ぶ。
     */
    initAudio: function () {
        if (!this._audioContext) {
            try {
                this._audioContext = new AudioContext();
                this._setupOutput();
            } catch (e) {
                return;
            }
        }
        if (this._audioContext.state === 'suspended') {
            this._audioContext.resume();
        }
    },

    /**
     * DynamicsCompressor をマスター出力として設定する。
     * フレームをまたいで複数ノートが重なってもクリッピングを防ぐ。
     */
    _setupOutput: function () {
        const ctx = this._audioContext;
        const compressor = ctx.createDynamicsCompressor();
        compressor.threshold.setValueAtTime(-18, ctx.currentTime);
        compressor.knee.setValueAtTime(6, ctx.currentTime);
        compressor.ratio.setValueAtTime(8, ctx.currentTime);
        compressor.attack.setValueAtTime(0.003, ctx.currentTime);
        compressor.release.setValueAtTime(0.05, ctx.currentTime);
        compressor.connect(ctx.destination);
        this._output = compressor;
    },

    /**
     * 複数の周波数を同時に発音する。
     * @param {number[]} frequencies - 発音する周波数の配列（Hz）
     * @param {number} duration - 発音時間（ms）。0 の場合は何もしない。
     * @param {number} volume - 音量（0.0〜1.0）
     */
    playNotes: function (frequencies, duration, volume) {
        if (duration <= 0 || !frequencies || frequencies.length === 0) return;

        // AudioContext が未初期化の場合は遅延初期化（フォールバック）
        if (!this._audioContext) {
            try {
                this._audioContext = new AudioContext();
                this._setupOutput();
            } catch (e) {
                return;
            }
        }
        if (this._audioContext.state === 'suspended') {
            this._audioContext.resume();
        }

        const ctx = this._audioContext;
        const output = this._output || ctx.destination;
        const now = ctx.currentTime;
        const durationSec = duration / 1000;
        const attackSec = 0.005;  // 5ms アタック: ゼロからランプアップしてクリックノイズを除去

        // 同時発音数に応じてゲインを按分し、volume を乗算
        const vol = Math.max(0, Math.min(1, volume ?? 1));
        const gainPerNote = (0.2 * vol) / Math.max(1, frequencies.length);

        for (let i = 0; i < frequencies.length; i++) {
            const freq = frequencies[i];
            if (freq <= 0) continue;

            const oscillator = ctx.createOscillator();
            const gainNode = ctx.createGain();

            oscillator.connect(gainNode);
            gainNode.connect(output);

            oscillator.type = 'sine';
            oscillator.frequency.setValueAtTime(freq, now);

            // 0 → ピーク（5ms）→ 0（duration末尾）: 開始・終了ともにゼロ交差でクリック除去
            gainNode.gain.setValueAtTime(0.0, now);
            gainNode.gain.linearRampToValueAtTime(gainPerNote, now + attackSec);
            gainNode.gain.linearRampToValueAtTime(0.0, now + durationSec);

            oscillator.start(now);
            oscillator.stop(now + durationSec + 0.001);  // gain が 0 に達した後に停止
        }
    },

    /**
     * AudioContext を閉じてリソースを解放する。
     */
    disposeAudio: function () {
        if (this._output) {
            this._output.disconnect();
            this._output = null;
        }
        if (this._audioContext) {
            this._audioContext.close();
            this._audioContext = null;
        }
    }
};

