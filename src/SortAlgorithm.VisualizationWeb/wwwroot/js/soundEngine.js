'use strict';
// Sound Engine - Web Audio API ラッパー
//
// 設計方針:
//   - AudioContext はユーザー操作後に initAudio() で初期化する（ブラウザ Autoplay Policy 対応）
//   - playNotes(frequencies, duration, volume) で複数音を一括発音（JS Interop 1回/フレーム）
//   - 固定ボイスプール（16ボイス）を初期化時に生成し、再生中は AudioNode を生成しない
//     → 頻繁な AudioNode 生成・GC によるプツプツノイズを防止
//   - DynamicsCompressor をマスター出力に挟み、重なりによるクリッピングを防止

window.soundEngine = {
    _audioContext: null,
    _output: null,    // DynamicsCompressor → destination
    _voices: [],      // 固定ボイスプール: { osc, gain, freeAt }
    _voiceCount: 32,  // 最大ポリフォニー数（240Hz 表示でも横取りなし: 3音 × ceil(40ms/4ms) = 30）

    /**
     * AudioContext を初期化・再開する。ユーザー操作（トグル押下など）後に呼ぶ。
     */
    initAudio: function () {
        if (!this._audioContext) {
            try {
                this._initAudioContext();
            } catch (e) {
                return;
            }
        }
        if (this._audioContext.state === 'suspended') {
            this._audioContext.resume();
        }
    },

    /**
     * AudioContext・Compressor・ボイスプールをまとめて初期化する。
     */
    _initAudioContext: function () {
        this._audioContext = new AudioContext();
        this._setupOutput();
        this._setupVoices();
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
     * ボイスプールを初期化する。
     * 全オシレーターをゲイン 0 で起動し、playNotes から周波数・ゲインだけ上書きして再利用する。
     * AudioNode の生成ゼロ化 → GC によるプツプツノイズを防止。
     */
    _setupVoices: function () {
        const ctx = this._audioContext;
        const output = this._output || ctx.destination;
        this._voices = [];
        for (let i = 0; i < this._voiceCount; i++) {
            const osc = ctx.createOscillator();
            const gain = ctx.createGain();
            osc.connect(gain);
            gain.connect(output);
            osc.type = 'sine';
            gain.gain.setValueAtTime(0.0, ctx.currentTime);
            osc.start();
            this._voices.push({ osc, gain, freeAt: 0 });
        }
    },

    /**
     * 空きボイスを取得する。空きがない場合は最も早く終わるボイスを横取りする。
     * @param {number} now - AudioContext の現在時刻（秒）
     */
    _acquireVoice: function (now) {
        let best = this._voices[0];
        for (let i = 0; i < this._voices.length; i++) {
            const v = this._voices[i];
            if (v.freeAt <= now) return v;     // 空きボイスが見つかった
            if (v.freeAt < best.freeAt) best = v;
        }
        return best;  // 最も早く終わるボイスを横取り
    },

    /**
     * 複数の周波数を同時に発音する。
     * @param {number[]} frequencies - 発音する周波数の配列（Hz）
     * @param {number} duration - 発音時間（ms）。0 の場合は何もしない。
     * @param {number} volume - 音量（0.0〜1.0）
     */
    playNotes: function (frequencies, duration, volume) {
        if (duration <= 0 || !frequencies || frequencies.length === 0) return;

        if (!this._audioContext) {
            try {
                this._initAudioContext();
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
        const attackSec = 0.005;  // 5ms アタック: ゼロからランプアップしてクリックノイズを除去

        const vol = Math.max(0, Math.min(1, volume ?? 1));
        const gainPerNote = (0.2 * vol) / Math.max(1, frequencies.length);

        for (let i = 0; i < frequencies.length; i++) {
            const freq = frequencies[i];
            if (freq <= 0) continue;

            const voice = this._acquireVoice(now);
            const stealing = voice.freeAt > now;

            // 周波数を上書き（横取り時は 2ms 後から変化）
            voice.osc.frequency.setValueAtTime(freq, stealing ? now + 0.002 : now);

            // 既存スケジュールをキャンセルしてエンベロープを設定
            voice.gain.gain.cancelScheduledValues(now);
            if (stealing) {
                // スムーズな横取り: 現在値から 2ms でフェードアウトして新音を開始
                const currentGain = voice.gain.gain.value;
                voice.gain.gain.setValueAtTime(currentGain, now);
                voice.gain.gain.linearRampToValueAtTime(0.0, now + 0.002);
                voice.gain.gain.linearRampToValueAtTime(gainPerNote, now + 0.002 + attackSec);
                voice.gain.gain.linearRampToValueAtTime(0.0, now + 0.002 + durationSec);
                voice.freeAt = now + 0.002 + durationSec;
            } else {
                // 通常割り当て: 0 → ピーク（5ms）→ 0
                voice.gain.gain.setValueAtTime(0.0, now);
                voice.gain.gain.linearRampToValueAtTime(gainPerNote, now + attackSec);
                voice.gain.gain.linearRampToValueAtTime(0.0, now + durationSec);
                voice.freeAt = now + durationSec;
            }
        }
    },

    /**
     * AudioContext を閉じてリソースを解放する。
     */
    disposeAudio: function () {
        for (const v of this._voices) {
            try { v.osc.stop(); } catch (e) { /* already stopped */ }
        }
        this._voices = [];
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

