# Phase 8.4 音（Sound）仕様書

## 📋 概要

**目的:** ソートアルゴリズム実行中に、配列への値アクセスを音で表現する。
値の大きさが音程に対応するため、ソートの進行が音でも感じ取れる。

**設計方針:**
- Web Audio API ネイティブのみ使用（外部ライブラリなし）
- デフォルト OFF（ページ訪問時に音が鳴らないように）
- 0アロケーション設計（毎フレーム GC を引き起こさない）
- 描画と独立（`PlaybackService` の再生ループに統合）

---

## 🎵 音の基本仕様

### 音源

| 項目 | 仕様 |
|------|------|
| API | Web Audio API（`AudioContext` + `OscillatorNode`） |
| 波形 | サイン波（`sine`） |
| 周波数レンジ | 200 Hz〜1200 Hz |
| 周波数マッピング | 配列内の相対値（最小値→200 Hz、最大値→1200 Hz）に線形マッピング |
| デフォルト状態 | **OFF** |
| UI | トグルスイッチ（Auto Reset の隣に配置） |

#### 周波数マッピング式

```
frequency = 200 + (value / maxValue) * (1200 - 200)
           = 200 + (value / maxValue) * 1000
```

> `maxValue` は配列サイズ（要素数）と等値。値の範囲は `1〜N`（`N` = 配列サイズ）。

---

## 🎹 操作タイプ別の発音仕様

### 鳴らす操作: **Read + Write のみ**（D2方式）

| 操作 | 発音 | 理由 |
|------|------|------|
| **IndexRead** | ✅ 鳴らす | 値を読んだ = アクセスした音 |
| **IndexWrite** | ✅ 鳴らす | 値を書いた = 変化した音 |
| Compare | ❌ 鳴らさない | 高頻度で騒音になりやすい |
| Swap | ❌ 直接は鳴らさない | 内部で Read + Write が記録される |

> **Swap について:** Swap 操作は内部的に 2回の Read + 2回の Write として `VisualizationContext` に記録されるため、自然に2音が発音される。Swap 専用の処理は不要。

---

## ⚡ OpsPerFrame が多い時の発音制御（A4方式）

1フレームに複数の操作が含まれる場合、発音する操作を自動的に間引く。

| OpsPerFrame | 発音するop数 | 選択方法 |
|-------------|-------------|----------|
| **1〜3** | 全操作 | フィルタなし |
| **4〜10** | 最大3音 | 等間隔サンプリング（先頭・中間・末尾） |
| **11以上** | 最大1音 | 末尾の1操作のみ |

#### サンプリング方法（4〜10の場合）

```
// 等間隔3点サンプリング
indices = [0, floor(count / 2), count - 1]
```

> 先頭・中間・末尾を取ることで「フレームの全体的な変化」を音で表現する。

---

## 🚀 高速再生時の発音制御（B4方式）

Speed Multiplier が高い場合、音の持続時間を短縮し、一定速度を超えたら自動で音を無効化する。

### 持続時間テーブル

| Speed Multiplier | 持続時間 | 状態 |
|-----------------|----------|------|
| 0.1x 〜 2x | 150 ms | 通常 |
| 2x 〜 5x | 80 ms | 短縮 |
| 5x 〜 20x | 40 ms | 短縮 |
| 20x 〜 50x | 20 ms | 短縮 |
| **50x 超** | — | **自動無効化**（音なし） |

> 50x 超で自動無効化されるが、UI トグルは ON のまま保持する（速度を下げると再び音が出る）。

#### 持続時間の計算ロジック

```javascript
function getSoundDuration(speedMultiplier) {
    if (speedMultiplier > 50)  return 0;   // 無効化
    if (speedMultiplier > 20)  return 20;
    if (speedMultiplier > 5)   return 40;
    if (speedMultiplier > 2)   return 80;
    return 150;
}
```

---

## 🔊 発音の実装仕様

### ノードグラフ

```
OscillatorNode → GainNode → AudioContext.destination
  (freq, sine)   (fade out)
```

### エンベロープ（音の形）

```
音量
 1.0 |─────────────────┐
     |                 │ (線形フェードアウト)
 0.0 |─────────────────┘──── 時間
     0            duration
```

- **アタック:** なし（即座に最大音量）
- **ディケイ/リリース:** 持続時間の終わりに向けて線形フェードアウト（ `linearRampToValueAtTime`）
- **最大ゲイン:** `0.3`（複数音の重なりによるクリッピング防止）

### 発音の流れ

```javascript
function playNote(frequency, duration) {
    const ctx = getAudioContext();
    if (!ctx) return;

    const oscillator = ctx.createOscillator();
    const gainNode = ctx.createGain();

    oscillator.connect(gainNode);
    gainNode.connect(ctx.destination);

    oscillator.type = 'sine';
    oscillator.frequency.setValueAtTime(frequency, ctx.currentTime);

    gainNode.gain.setValueAtTime(0.3, ctx.currentTime);
    gainNode.gain.linearRampToValueAtTime(0.0, ctx.currentTime + duration / 1000);

    oscillator.start(ctx.currentTime);
    oscillator.stop(ctx.currentTime + duration / 1000);
}
```

### AudioContext の初期化

ブラウザのAutoplay Policy制約により、`AudioContext` はユーザー操作（クリック等）の後に初期化する。

```javascript
// 最初のユーザーインタラクション（再生ボタン押下時等）で初期化
function ensureAudioContext() {
    if (!_audioContext) {
        _audioContext = new AudioContext();
    }
    if (_audioContext.state === 'suspended') {
        _audioContext.resume();
    }
    return _audioContext;
}
```

---

## 🖥️ UI 仕様

### トグルスイッチの配置

```
[⏩ Auto Reset  ●──○]  [🔊 Sound  ○──●]
```

- Auto Reset トグルの **右隣** に配置
- ラベル: `🔊 Sound`
- デフォルト: **OFF**（`false`）
- 50x 超で音が無効化されている場合、トグルは ON のまま表示（速度を下げると復活）

### 無効化中の視覚フィードバック

Speed Multiplier が 50x 超で音トグルが ON の場合:

```
[🔊 Sound  ●──○]  ← ONだが速度超過で無音
                  ⚠️ Muted (speed > 50x)  ← 小文字で注釈
```

---

## 📁 実装ファイル構成

```
src/SortAlgorithm.VisualizationWeb/
├── wwwroot/
│   └── js/
│       └── soundEngine.js          # 新規: Web Audio API ラッパー
├── Services/
│   └── PlaybackService.cs          # 変更: 発音タイミング制御を追加
└── Pages/
    └── Index.razor                 # 変更: Sound トグル UI 追加
```

### soundEngine.js の公開API

| 関数 | 引数 | 説明 |
|------|------|------|
| `initAudio()` | — | AudioContext 初期化（ユーザー操作後に呼ぶ） |
| `playNotes(frequencies, duration)` | `number[]`, `number` | 複数音を同時発音（サンプリング済みの周波数配列） |
| `disposeAudio()` | — | AudioContext を閉じてリソース解放 |

> `playNotes` は配列を受け取ることで、Blazor→JS 間のインターオペレーション回数を **1回/フレーム** に抑える。

### PlaybackService への統合ポイント

```csharp
// OnTimerElapsed / OnAnimationFrame 内の発音制御
if (SoundEnabled)
{
    var frequencies = SampleFrequencies(frameOps, OperationsPerFrame, _currentArraySize);
    var duration = GetSoundDuration(SpeedMultiplier);
    if (duration > 0 && frequencies.Length > 0)
    {
        await _js.InvokeVoidAsync("soundEngine.playNotes", frequencies, duration);
    }
}
```

#### SampleFrequencies のロジック

```csharp
private float[] SampleFrequencies(IReadOnlyList<SortOperation> frameOps, int opsPerFrame, int arraySize)
{
    // Read/Write のみを対象にフィルタ
    var readWriteOps = frameOps
        .Where(op => op.Type is SortOperationType.IndexRead or SortOperationType.IndexWrite)
        .ToList(); // ← 実装時はアロケーション回避のためスタック割り当てや再利用バッファを使う

    if (readWriteOps.Count == 0) return [];

    // OpsPerFrame に応じてサンプリング
    var sampled = opsPerFrame <= 3
        ? readWriteOps                                            // 全部
        : opsPerFrame <= 10
            ? Sample3(readWriteOps)                               // 等間隔3点
            : [readWriteOps[^1]];                                 // 末尾1点

    return sampled
        .Select(op => 200f + (op.Value / (float)arraySize) * 1800f)
        .ToArray();
}
```

> **Note:** 実際の実装では `ArrayPool<float>` を使用してアロケーションを回避する。

---

## ✅ 受け入れ条件

| 条件 | 内容 |
|------|------|
| デフォルト OFF | ページロード時に音が鳴らない |
| Autoplay 対応 | ユーザー操作前に `AudioContext` を生成しない |
| OpsPerFrame 多い時 | 最大発音数が 3（4〜10） or 1（11以上）に制限される |
| 高速時の自動無効化 | SpeedMultiplier > 50 で自動的に音が止まる |
| 速度戻し後の復活 | 50x 以下に戻すと再び音が出る |
| 持続時間の段階制御 | 速度に応じて 150/80/40/20ms に変化する |
| エンベロープ | 急激に音が切れない（フェードアウトあり） |
| 音量制限 | 複数音の重なりでクリッピングしない（gain ≤ 0.3） |
| JS 呼び出し最適化 | フレームあたり最大 1回のみ JS Interop を呼ぶ |
| リソース解放 | `PlaybackService.Dispose()` で `AudioContext` を閉じる |

---

## 🚫 対象外（スコープ外）

- 波形の変更（ユーザーが square/sawtooth 等を選べるUI）
- 操作タイプ別の音色変化（Compare を鳴らす、Swap で特殊音など）
- ボリューム調整スライダー
- MIDI 出力
- 音楽的スケール（平均律等）への補正

---

## 📊 推定工数

| 作業 | 工数 |
|------|------|
| `soundEngine.js` 実装 | 0.5日 |
| `PlaybackService.cs` への統合 | 0.5日 |
| `Index.razor` UI 追加 | 0.25日 |
| テスト・調整 | 0.25日 |
| **合計** | **約 1.5日** |
