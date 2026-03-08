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
    _soundType: 'sine', // 現在のサウンドタイプ: 'sine' | 'poko'

    /**
     * サウンドタイプを設定する。'sine'（デフォルト）または 'poko'。
     * AudioContext 初期化前でも呼び出し可能。
     * @param {string} type - 'sine' | 'poko'
     */
    setSoundType: function (type) {
        this._soundType = (type === 'poko') ? 'poko' : 'sine';
    },

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
     * 安全リミッターをマスター出力として設定する。
     * 適応ゲインにより通常動作では threshold に達しないためポンピングなし。
     * 万が一クリッピングする場合の最後の安全網。
     */
    _setupOutput: function () {
        const ctx = this._audioContext;
        const limiter = ctx.createDynamicsCompressor();
        // threshold -1dBFS: 実際の信号は ∼-16dB なのでほぼ触れない → ポンピングなし
        limiter.threshold.setValueAtTime(-1, ctx.currentTime);
        limiter.knee.setValueAtTime(0, ctx.currentTime);        // ハードニー
        limiter.ratio.setValueAtTime(20, ctx.currentTime);      // リミッター動作
        limiter.attack.setValueAtTime(0.001, ctx.currentTime);  // 高速アタック
        limiter.release.setValueAtTime(0.1, ctx.currentTime);   // リリース
        limiter.connect(ctx.destination);
        this._output = limiter;
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

        const vol = Math.max(0, Math.min(1, volume ?? 1));
        // オーバーラップ適応ゲイン:速度に関わらず総ゲイン ≈ 0.15 で一定化しポンピングを防止する。
        // 同時発音数 expectedOverlap ≈ duration × 60fps。
        // 総ゲイン = expectedOverlap × notes × gainPerNote = 0.15 × vol（リミッターの -1dBFS 閾より常に小）
        const expectedOverlap = Math.max(1, durationSec * 60);
        const gainPerNote = (0.15 * vol) / (expectedOverlap * Math.max(1, frequencies.length));

        const soundFn = this._soundType === 'poko' ? this._pokoSound : this._sineSound;

        for (let i = 0; i < frequencies.length; i++) {
            const freq = frequencies[i];
            if (freq <= 0) continue;

            const voice = this._acquireVoice(now);
            const stealing = voice.freeAt > now;
            const startAt = stealing ? now + 0.002 : now;

            // 既存スケジュールをキャンセル
            voice.gain.gain.cancelScheduledValues(now);
            voice.osc.frequency.cancelScheduledValues(now);

            if (stealing) {
                // スムーズな横取り: 現在値から 2ms でフェードアウトして新音を開始
                voice.gain.gain.setValueAtTime(voice.gain.gain.value, now);
                voice.gain.gain.linearRampToValueAtTime(0.0, startAt);
            }

            soundFn(voice, freq, startAt, gainPerNote, durationSec);
        }
    },

    /**
     * サイン波: ピッチ固定・5ms アタック・リニアディケイ。
     * @param {object} voice - ボイスプールエントリ { osc, gain, freeAt }
     * @param {number} freq - 周波数（Hz）
     * @param {number} startAt - 発音開始時刻（AudioContext 秒）
     * @param {number} gainPerNote - ノートあたりのゲイン
     * @param {number} durationSec - 発音時間（秒）
     */
    _sineSound: function (voice, freq, startAt, gainPerNote, durationSec) {
        const attackSec = 0.005;  // 5ms アタック: クリックノイズを除去

        voice.osc.frequency.setValueAtTime(freq, startAt);

        voice.gain.gain.setValueAtTime(0.0, startAt);
        voice.gain.gain.linearRampToValueAtTime(gainPerNote, startAt + attackSec);
        voice.gain.gain.linearRampToValueAtTime(0.0, startAt + durationSec);

        voice.freeAt = startAt + durationSec;
    },

    /**
     * ポコポコ: 高速アタック + ピッチドロップ + 短めディケイ。
     * 音程が durationSec の 50% にかけて freq * 0.4 まで落ち、65% で消音する。
     * @param {object} voice - ボイスプールエントリ { osc, gain, freeAt }
     * @param {number} freq - 周波数（Hz）
     * @param {number} startAt - 発音開始時刻（AudioContext 秒）
     * @param {number} gainPerNote - ノートあたりのゲイン
     * @param {number} durationSec - 発音時間（秒）
     */
    _pokoSound: function (voice, freq, startAt, gainPerNote, durationSec) {
        const pokoAttack = 0.003;  // 3ms 高速アタック
        const pitchEnd = startAt + durationSec * 0.5;
        const gainEnd  = startAt + durationSec * 0.65;

        voice.osc.frequency.setValueAtTime(freq, startAt);
        voice.osc.frequency.exponentialRampToValueAtTime(Math.max(freq * 0.4, 20), pitchEnd);

        voice.gain.gain.setValueAtTime(0.0, startAt);
        voice.gain.gain.linearRampToValueAtTime(gainPerNote, startAt + pokoAttack);
        voice.gain.gain.linearRampToValueAtTime(0.0, gainEnd);

        voice.freeAt = gainEnd;
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

