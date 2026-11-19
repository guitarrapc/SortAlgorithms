# WebAssembly 標準とエンジン対応の現況（2026年11月19日時点）: .NET-on-Wasm と CPU バウンドなソートライブラリの判定基準

## WebAssembly 2.0 / 3.0 の内容と W3C / CG での位置付け

### Takeaway
Wasm 3.0 は CG が 2025-09-17 に完成を告知し、W3C の TR は `wasm-core-2` シリーズのまま 2026-02-05 以降 Release 3.0 の本文に切り替わった。2.0 も 3.0 も W3C Recommendation ではなく、WG は Candidate Recommendation Draft を living standard として維持する方針である（Recommendation は 2019-12-05 の 1.0 のみ）。

### Cited Findings
**W3C の文書状態**
- Wasm 1.0 Core は W3C Recommendation（2019-12-05） — [WebAssembly Core Specification 1.0](https://www.w3.org/TR/wasm-core-1/)
- `wasm-core-2` シリーズの履歴: First Public Working Draft 2022-04-19、Working Draft 2024-02-17〜2024-12-13（26版）、Candidate Recommendation Snapshot 2024-12-17（1版）、Candidate Recommendation Draft 2024-12-18〜2026-11-16（61版） — [W3C API: wasm-core-2 versions](https://api.w3.org/specifications/wasm-core-2/versions?items=200&embed=true)
- 2024-12-17 の CR Snapshot の本文は Release 2.0 を記述する — [CR Snapshot 2024-12-17](https://www.w3.org/TR/2024/CR-wasm-core-2-20241217/)
- TR 本文の切り替わり: 2025-06-16 版までが Release 2.0、直後の 2026-02-05 版から Release 3.0（各版の Abstract を二分探索で確認） — [CRD 2025-06-16](https://www.w3.org/TR/2025/CRD-wasm-core-2-20250616/); [CRD 2026-02-05](https://www.w3.org/TR/2026/CRD-wasm-core-2-20260205/)
- 最新版は CRD 2026-11-16 で、Abstract は Release 3.0 を記述すると明記する。`https://www.w3.org/TR/wasm-core/` も同じ文書に解決され、`https://www.w3.org/TR/wasm-core-3/` は 404 — [CRD 2026-11-16](https://www.w3.org/TR/2026/CRD-wasm-core-2-20261116/)
- 同文書の Status 節によれば、WG は本仕様を CR 状態に維持して継続更新する living standard とし、Recommendation へは移行しない意向である。Implementation Report の参照先は webassembly.org/features — [CRD 2026-11-16](https://www.w3.org/TR/2026/CRD-wasm-core-2-20261116/)
- JS API と Web API も同じく CRD 2026-11-16 — [JS Interface](https://www.w3.org/TR/wasm-js-api-2/); [Web API](https://www.w3.org/TR/wasm-web-api-2/)
- Editor's Draft の表記は Release 3.0 (2026-11-16) — [WebAssembly Specification](https://webassembly.github.io/spec/core/)

**CG の発表**
- Wasm 3.0 は 2025-09-17 に Andreas Rossberg が告知。主要ブラウザでは既に大半が出荷済みで、Wasmtime などスタンドアロンエンジンも完了へ進行中とされ、仕様は SpecTec ツールチェーンで作られた最初の版 — [Wasm 3.0 Completed](https://webassembly.org/news/2025-09-17-wasm-3.0/)

**各版の内容**
- Release 2.0 の変更点: sign extension、non-trapping float-to-int、multiple values、reference types、table instructions、multiple tables、bulk memory and table instructions、vector instructions（SIMD128） — [Change History](https://webassembly.github.io/spec/core/appendix/changes.html)
- 2.0 に紐づく完了提案には JS API 側の BigInt-to-i64 統合も含まれる — [finished-proposals.md](https://github.com/WebAssembly/proposals/blob/main/finished-proposals.md)
- Release 3.0 の変更点: extended constant expressions、tail calls、exception handling、multiple memories、64-bit address space（memory64）、typeful references、garbage collection、relaxed vector instructions、profiles、custom annotations — [Change History](https://webassembly.github.io/spec/core/appendix/changes.html)
- 完了提案一覧では JS String Builtins と Branch Hinting も Spec Version 3.0 に分類される — [finished-proposals.md](https://github.com/WebAssembly/proposals/blob/main/finished-proposals.md)
- 3.0 の deterministic profile は NaN や relaxed SIMD の非決定性に決定的な挙動を定める — [Wasm 3.0 Completed](https://webassembly.org/news/2025-09-17-wasm-3.0/)
- Threads は 3.0 に含まれない。Phase 4 のままで、JS API の Editor's Draft 本文にも `SharedArrayBuffer` の語は現れない（2026-11-19 に全文検索） — [WebAssembly/proposals](https://github.com/WebAssembly/proposals); [JS API Editor's Draft](https://webassembly.github.io/spec/js-api/)

### Inferences
- 「Wasm 3.0 準拠」を W3C 勧告への準拠として語るのは誤りで、判定基準としては「Release 3.0 の CRD（2026-11-16）に含まれる機能」と表現するのが正確である。
- W3C のショートネームに `-2` が残るため、`wasm-core-2` の URL だけを根拠に「2.0 の仕様」と記述すると内容を取り違える。

### Gaps
- 2.0 の CG 側「完成」告知に相当する単一の日付は確認できなかった。確認できたのは W3C の CR Snapshot（2024-12-17）のみ。

## 機能別の標準化フェーズとエンジン対応

### Takeaway
SIMD128、bulk memory、threads などソートに関わる基盤は全主要エンジンで既定有効である。安定版 Safari 27.2 には relaxed SIMD と memory64 がなく（27.3 ベータで追加）、Node.js 24 は exnref が 24.15.0、JSPI が 24.20.0 からと、マイナー版で差が出る点に注意が要る。

### Cited Findings
**調査時点の各エンジン最新版**
- Chrome 安定版 158.0.8127.37（2026-11-12） — [Chromium Dash](https://chromiumdash.appspot.com/fetch_releases?channel=Stable&platform=Windows&num=2)
- Firefox 安定版 160.0.1（2026-11-10）、ESR 153.7.0 — [product-details](https://product-details.mozilla.org/1.0/firefox_versions.json)
- Safari 27.2（2026-10-26、現行）、27.3 はベータ（engine 626.1.17） — [MDN BCD safari.json](https://github.com/mdn/browser-compat-data/blob/main/browsers/safari.json); [WebKit Features for Safari 27.2](https://webkit.org/blog/18515/webkit-features-for-safari-27-2/)
- Node.js 24 系最新は 24.22.3（2026-11-10、LTS Krypton、V8 13.6.233.17）。26 系最新は 26.13.1（LTS Lithium、V8 14.6.202.34） — [Node.js dist index](https://nodejs.org/dist/index.json)
- Wasmtime 最新は v50.0.2（2026-11-16）、v51.0.0-rc.1 が公開中 — [Wasmtime releases](https://github.com/bytecodealliance/wasmtime/releases)

**対応表**

基本データは [features.json](https://github.com/WebAssembly/website/blob/main/features.json)（[webassembly.org/features](https://webassembly.org/features/) の元データ、2026-11-19 取得）。フェーズは同ファイルと [WebAssembly/proposals](https://github.com/WebAssembly/proposals) による。行末の「根拠」は features.json を補正または補強する一次情報。`flag` は既定無効。Node.js 列は「Node 全体の初出 / 24 系での状態」。

| 機能 | Phase / 版 | Chrome (V8) | Firefox | Safari | Node.js（24 系の状態） | Wasmtime | 根拠・補足 |
|---|---|---|---|---|---|---|---|
| SIMD128 | 5 / 2.0 | 91 | 89 | 16.4 | 16.4 / 24.0 で有効 | 0.33 | |
| Relaxed SIMD | 5 / 3.0 | 114 | 145 | 安定版は無効。27.3 ベータで追加 | 21.0 / 24.0 で有効 | 15 | [Bug 1930192](https://bugzilla.mozilla.org/show_bug.cgi?id=1930192), [Safari 27.3 notes](https://developer.apple.com/documentation/safari-release-notes/safari-27_3-release-notes) |
| Bulk memory | 5 / 2.0 | 75 | 79 | 15 | 12.5 / 有効 | 0.20 | |
| Multi-value | 5 / 2.0 | 85 | 78 | 13.1 | 15.0 / 有効 | 0.17 | |
| Reference types | 5 / 2.0 | 96 | 79 | 15 | 17.2 / 有効 | 0.20 | |
| Tail calls | 5 / 3.0 | 112 | 121 | 18.2 | 20.0 / 有効 | 22 | |
| EH（legacy） | inactive | 95 | 100 | 15.2 | 17.0 / 有効 | 記載なし | 提案は inactive に分類 |
| EH（exnref） | 5 / 3.0 | 137 | 131 | 18.4 | **24.15.0** から既定有効 | 47 | [v24.14.0 flags](https://github.com/nodejs/node/blob/v24.14.0/deps/v8/src/wasm/wasm-feature-flags.h), [v24.15.0 flags](https://github.com/nodejs/node/blob/v24.15.0/deps/v8/src/wasm/wasm-feature-flags.h) |
| Wasm GC | 5 / 3.0 | 119 | 120 | 18.2 | 22.0 / 有効 | 47 | |
| Threads / atomics | 4 / 未収録 | 74 | 79 | 14.1 | 16.4 / 有効 | 15（Tier 2） | [Wasmtime tiers](https://docs.wasmtime.dev/stability-wasm-proposals.html) |
| Memory64 | 5 / 3.0 | 133 | 134 | 安定版は無効。27.3 ベータで追加 | 24.0 / 有効 | 30 | [Node 24 changelog](https://github.com/nodejs/node/blob/main/doc/changelogs/CHANGELOG_V24.md), [Safari 27.3 notes](https://developer.apple.com/documentation/safari-release-notes/safari-27_3-release-notes) |
| Multi-memory | 5 / 3.0 | 120 | 125 | STP 250 で追加。安定版は未対応 | 22.0 / 有効 | 15 | [STP 250](https://webkit.org/blog/18191/release-notes-for-safari-technology-preview-250/) |
| JS String Builtins | 5 / 3.0（JS API） | 130 | 134 | 26.2 | 24.0 から既定有効（features.json の flag 表記は古い） | 対象外 | [v24.0.0 flags](https://github.com/nodejs/node/blob/v24.0.0/deps/v8/src/wasm/wasm-feature-flags.h) |
| JSPI | 5 | 137 | 153 | 27 | **24.20.0** から有効（features.json は 26.0） | 対象外 | [Node 24 changelog](https://github.com/nodejs/node/blob/main/doc/changelogs/CHANGELOG_V24.md), [node.cc v24.22.3](https://github.com/nodejs/node/blob/v24.22.3/src/node.cc), [Firefox 153](https://developer.mozilla.org/en-US/docs/Mozilla/Firefox/Releases/153), [Safari 27.0](https://webkit.org/blog/18325/webkit-features-for-safari-27-0/) |
| ESM integration | 3 | 記載なし | 未完（153 は `import source` 構文のみ） | 記載なし（下記の不一致を参照） | **24.5.0** で unflag | 対象外 | [Node 24 changelog](https://github.com/nodejs/node/blob/main/doc/changelogs/CHANGELOG_V24.md), [Firefox 153](https://developer.mozilla.org/en-US/docs/Mozilla/Firefox/Releases/153) |
| Branch hinting | 5 / 3.0 | 137 | 下記の不一致を参照 | 16 | 24 系は既定無効、26.0 で既定有効 | flag（Tier 3） | [v24.22.3 flags](https://github.com/nodejs/node/blob/v24.22.3/deps/v8/src/wasm/wasm-feature-flags.h), [v26.0.0 flags](https://github.com/nodejs/node/blob/v26.0.0/deps/v8/src/wasm/wasm-feature-flags.h) |
| Wide arithmetic | 4 | flag | 155 | STP 254 | 24 系は既定無効 | Tier 1。Cranelift で既定有効 | [Firefox 155](https://developer.mozilla.org/en-US/docs/Mozilla/Firefox/Releases/155), [config.rs](https://github.com/bytecodealliance/wasmtime/blob/main/crates/wasmtime/src/config.rs) |
| Stack switching | 3 | flag | pref 既定 false | 記載なし | 既定無効 | Tier 3、x86_64 Linux のみ | [StaticPrefList](https://github.com/mozilla-firefox/firefox/blob/release/modules/libpref/init/StaticPrefList.yaml), [Wasmtime tiers](https://docs.wasmtime.dev/stability-wasm-proposals.html) |

**ローカル実測（Node.js v24.18.0 / V8 13.6.233.17-node.50 / x64、2026-11-19）**

機能ごとに手組みした最小モジュールを `WebAssembly.validate` に渡し、既定状態の受理可否を確認した。
- 受理: SIMD128、relaxed SIMD、bulk memory、multi-value、reference types、tail call、legacy EH、exnref（`try_table` と exnref ローカル）、GC、threads、memory64、multi-memory
- 拒否: wide arithmetic（`i64.add128`）、stack switching（cont 型）
- `wasm:js-string` の組み込み import は動作。`WebAssembly.JSTag` は存在。`WebAssembly.Suspending` / `promising` は未定義（JSPI 有効化の 24.20.0 より前のため整合）

**データ源の不一致（解決できたものは一次情報を優先）**
- Node.js の exnref: features.json の 24.15 は、V8 フラグ定義が v24.14.0 で false、v24.15.0 で true である事実と一致する — [v24.14.0](https://github.com/nodejs/node/blob/v24.14.0/deps/v8/src/wasm/wasm-feature-flags.h); [v24.15.0](https://github.com/nodejs/node/blob/v24.15.0/deps/v8/src/wasm/wasm-feature-flags.h)
- Node.js の JSPI: features.json は 26.0 だが、24.20.0 の changelog に `wasm: enable JSPI` があり、v24.22.3 の `node.cc` は起動時に `--experimental-wasm-jspi` を V8 へ渡す — [features.json](https://github.com/WebAssembly/website/blob/main/features.json); [Node 24 changelog](https://github.com/nodejs/node/blob/main/doc/changelogs/CHANGELOG_V24.md); [node.cc](https://github.com/nodejs/node/blob/v24.22.3/src/node.cc)
- Firefox の relaxed SIMD: features.json は 145、MDN BCD は 146。Bugzilla の有効化バグは 145 Branch で修正済み — [features.json](https://github.com/WebAssembly/website/blob/main/features.json); [BCD relaxed-SIMD](https://github.com/mdn/browser-compat-data/blob/main/webassembly/relaxed-SIMD.json); [Bug 1930192](https://bugzilla.mozilla.org/show_bug.cgi?id=1930192)
- Firefox の wide arithmetic: features.json は flag のままだが、Firefox 155 のリリースノートは対応を明記し、release ブランチの pref も既定 true — [Firefox 155](https://developer.mozilla.org/en-US/docs/Mozilla/Firefox/Releases/155); [StaticPrefList](https://github.com/mozilla-firefox/firefox/blob/release/modules/libpref/init/StaticPrefList.yaml)
- Firefox の branch hinting: features.json は Nightly 以外 flag、BCD は 148。release ブランチは configure が `--disable-wasm-branch-hinting` 形式（既定有効）で pref も true。148 はリリースノートで裏付けられなかった — [BCD branch-hinting](https://github.com/mdn/browser-compat-data/blob/main/webassembly/branch-hinting.json); [moz.configure](https://github.com/mozilla-firefox/firefox/blob/release/js/moz.configure)
- Safari の multi-memory: BCD は 27.3 とするが、27.3 ベータノートの WebAssembly 新機能は relaxed SIMD と Memory64 の 2 件のみで、multi-memory の記載は STP 250 にある — [BCD multiMemory](https://github.com/mdn/browser-compat-data/blob/main/webassembly/multiMemory.json); [Safari 27.3 notes](https://developer.apple.com/documentation/safari-release-notes/safari-27_3-release-notes); [STP 250](https://webkit.org/blog/18191/release-notes-for-safari-technology-preview-250/)
- Safari の memory64: STP 251 で追加され、27.3 ベータノートにも記載がある — [STP 251](https://webkit.org/blog/18194/release-notes-for-safari-technology-preview-251/)
- Safari の初出版の軽微な差: threads は features.json 14.1 / BCD 15.2、bulk memory と reference types は 15 / 15.1。Chrome の multi-value も 85 / 86 — [features.json](https://github.com/WebAssembly/website/blob/main/features.json); [BCD webassembly](https://github.com/mdn/browser-compat-data/tree/main/webassembly)
- Wasmtime の wide arithmetic: features.json は flag だが、`config.rs` の Cranelift 既定は `WASM3 | WIDE_ARITHMETIC`。Winch の既定は `WASM2` 基準で別構成 — [config.rs](https://github.com/bytecodealliance/wasmtime/blob/main/crates/wasmtime/src/config.rs)

**その他の提案の現況**
- Phase 4: Threads、Compact Import Section、Wide Arithmetic。Phase 3: ESM Integration、Stack Switching、Custom Page Sizes、Custom Descriptors。Phase 2: Compilation Hints、Acquire-Release Atomics、Multibyte Array Access、FP16 など。Phase 1: Flexible Vectors、Memory Control、Shared-Everything Threads、Component Model など — [WebAssembly/proposals](https://github.com/WebAssembly/proposals)
- `toResizableBuffer` / `toFixedLengthBuffer` は Chrome 144、Firefox 145、Safari 26.2、Node 26.0 — [BCD Memory](https://github.com/mdn/browser-compat-data/blob/main/webassembly/api/Memory.json)

### Inferences
- 「全ブラウザの安定版で使える」前提を置けるのは、SIMD128、bulk memory、tail call、GC、exnref、JS String Builtins、JSPI までである。relaxed SIMD と memory64 を必須にすると、27.3 が正式版になるまで Safari 安定版を切り捨てることになる。
- Node.js 24 系は V8 が 13.6 に固定されたままマイナー版で機能が増えており、「Node 24 対応」は 24.15 以上か 24.20 以上かまで明示しないと判定が曖昧になる。
- Safari 27.3 ベータノートに ES モジュールとして読み込む Wasm の不具合修正があるため、Safari は何らかの Wasm モジュール読み込みを実装済みとみられる（初出版は未確認） — [Safari 27.3 notes](https://developer.apple.com/documentation/safari-release-notes/safari-27_3-release-notes)
- features.json は Node.js と Firefox の項目に古い値が残っており、判定基準には単独で使わず一次情報と照合すべきである。

### Gaps
- Chrome の ESM integration の出荷状況は確認できなかった（blink-dev の Intent to Prototype までしか到達していない）。
- Wasmtime で wide arithmetic が既定有効になった版は特定できなかった。
- Safari 27.3 の正式リリース日は未確認（報道は 12 月初旬見込みとするが一次情報ではない）。
- Node.js 26 の既定有効機能はフラグ定義の確認のみで、実機検証はしていない。

## スレッドの実務制約: SharedArrayBuffer と COOP / COEP

### Takeaway
ブラウザで Wasm スレッドを使うには secure context と cross-origin isolation が必須で、配信側のヘッダー設定を要する。COEP `credentialless` は Safari が未対応のため、3 ブラウザ共通の構成は `COOP: same-origin` と `COEP: require-corp` である。

### Cited Findings
- `SharedArrayBuffer` の共有には secure context と cross-origin isolation が必要。満たさない場合はグローバルのコンストラクタが隠され、`postMessage` は例外を投げる。`Atomics` 自体は常に利用可能 — [MDN SharedArrayBuffer](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/SharedArrayBuffer)
- `shared: true` の `WebAssembly.Memory` の `buffer` は `SharedArrayBuffer` で、同じ要件が適用される。Wasm の atomic 命令自体は無条件に使え、制限されるのはスレッド間共有である — [MDN SharedArrayBuffer](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/SharedArrayBuffer)
- cross-origin isolation の条件は `Cross-Origin-Opener-Policy: same-origin` と、`Cross-Origin-Embedder-Policy: require-corp` または `credentialless`。加えて `Permissions-Policy: cross-origin-isolated` で阻害されないこと — [MDN COEP](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Cross-Origin-Embedder-Policy); [MDN crossOriginIsolated](https://developer.mozilla.org/en-US/docs/Web/API/Window/crossOriginIsolated)
- `require-corp` では cross-origin の `no-cors` リソースに CORP による許可が必要。`credentialless` は CORP なしで読めるが認証情報を送らない — [MDN COEP](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Cross-Origin-Embedder-Policy)
- 対応版: COOP / COEP は Chrome 83、Firefox 79、Safari 15.2。`credentialless` は Chrome 96、Firefox 119、Safari 未対応 — [BCD COEP](https://github.com/mdn/browser-compat-data/blob/main/http/headers/Cross-Origin-Embedder-Policy.json); [BCD COOP](https://github.com/mdn/browser-compat-data/blob/main/http/headers/Cross-Origin-Opener-Policy.json)
- `Document-Isolation-Policy` は Chrome 137 のみ対応（Firefox、Safari は未対応）。フレーム単位で適用され、サブフレームに要件を課さず、SharedArrayBuffer と Wasm スレッドを解放する。Chrome 137 時点ではデスクトップのみ — [Chrome Developers](https://developer.chrome.com/blog/document-isolation-policy); [BCD Document-Isolation-Policy](https://github.com/mdn/browser-compat-data/blob/main/http/headers/Document-Isolation-Policy.json)
- `Atomics.wait` はメインスレッドで使えず `TypeError` になる。非ブロッキングの代替は `Atomics.waitAsync` — [MDN Atomics.wait](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Atomics/wait)
- Wasmtime の threads は Tier 2 で、shared memory には既知の問題があり pooling allocator では使えない — [Wasmtime tiers](https://docs.wasmtime.dev/stability-wasm-proposals.html)
- Wasmtime の threads 提案はスレッド生成を含まず、生成は `wasi-threads` として別に制御される。既定値は `threads` crate feature の有無に従う — [config.rs](https://github.com/bytecodealliance/wasmtime/blob/main/crates/wasmtime/src/config.rs)
- Shared-Everything Threads は Phase 1 に留まり、Wasmtime では未実装 — [WebAssembly/proposals](https://github.com/WebAssembly/proposals); [Wasmtime tiers](https://docs.wasmtime.dev/stability-wasm-proposals.html)

### Inferences
- 並列ソートをブラウザで提供するライブラリは、利用側の配信設定に依存するため、単一スレッド動作へのフォールバックを前提にする必要がある。ヘッダーを制御できない静的ホスティングや、CORP のないサードパーティ資源を埋め込むページでは並列版が動かない。
- メインスレッドでは `Atomics.wait` が使えないため、同期 API のまま並列ソートを完了待ちする設計はワーカー上でしか成立しない。

### Gaps
- Android Chrome での `Document-Isolation-Policy` の現況は確認できなかった（BCD の詳細は未確認で、一次情報は 2025 年時点の予定のみ）。

## wasm32 / memory64 のメモリ上限と memory64 の性能コスト

### Takeaway
Web の実行時上限は wasm32 が 4 GiB、memory64 が 16 GiB で、memory64 は 4 GiB 超が必要な場合にしか利点がない。SpiderMonkey は全アクセスに明示境界チェックを要し 10% 〜 100% 超の低下を公表している一方、V8 はトラップハンドラを併用して定数比較に抑えている。

### Cited Findings
**上限**
- JS API の宣言上限: 32-bit メモリの min / max は 65,536 ページ（4 GiB）、64-bit メモリは 2^37-1 ページ — [JS API Implementation-defined Limits](https://webassembly.github.io/spec/js-api/)
- JS API の実行時上限: 32-bit メモリ 65,536 ページ（4 GiB）、64-bit メモリ 262,144 ページ（16 GiB）。上限内でも資源不足で失敗し得る — [JS API Implementation-defined Limits](https://webassembly.github.io/spec/js-api/)
- V8 の上限は 64-bit ホストで 65,536 / 262,144 ページ、32-bit ホストではどちらも 32,767 ページ（2 GiB - 64 KiB） — [wasm-limits.h](https://github.com/nodejs/node/blob/main/deps/v8/src/wasm/wasm-limits.h)
- SpiderMonkey の実効上限は検証上限と `ArrayBufferObject::ByteLengthLimit` の小さい方で、32-bit では 2 GB 未満 — [WasmMemory.cpp](https://github.com/mozilla-firefox/firefox/blob/release/js/src/wasm/WasmMemory.cpp)
- ローカル実測（Node.js v24.18.0）: `initial: 65536` は 4,294,967,296 バイトを確保でき、65,537 は `RangeError`。`address: 'i64'` の `maximum` は 262,144 まで受理し、262,145 は `RangeError`

**境界チェックの実装**
- V8: wasm32 はトラップハンドラが有効なら `kTrapHandler` 戦略を採り、8 GiB の領域を予約する。無効時は明示チェックへ戻る — [wasm-module.cc](https://github.com/nodejs/node/blob/main/deps/v8/src/wasm/wasm-module.cc); [backing-store.cc](https://github.com/nodejs/node/blob/main/deps/v8/src/objects/backing-store.cc)
- V8: memory64 も x64 / arm64 / riscv64 / loong64 では `wasm_memory64_trap_handling` が既定 true で、16 GiB を予約する。インデックスを定数 `kMaxMemory64Size - end_offset` と比較し、残りをトラップハンドラに任せる — [flag-definitions.h](https://github.com/nodejs/node/blob/main/deps/v8/src/flags/flag-definitions.h); [turboshaft-graph-interface.cc](https://github.com/nodejs/node/blob/main/deps/v8/src/wasm/turboshaft-graph-interface.cc)
- SpiderMonkey: 64-bit 環境の memory32 は明示チェックか huge memory を選べるが、memory64 は常に明示チェック。32-bit 環境は常に明示チェック — [WasmMemory.cpp](https://github.com/mozilla-firefox/firefox/blob/release/js/src/wasm/WasmMemory.cpp)
- JavaScriptCore: `useWasmFastMemory` が既定 true で、4 GiB の仮想確保と 128 ページ分の redzone、シグナルハンドラで境界検査する — [OptionsList.h](https://github.com/WebKit/WebKit/blob/main/Source/JavaScriptCore/runtime/OptionsList.h)
- Wasmtime: 64-bit の既定は 4 GiB の予約と 32 MiB のガード — [config.rs](https://github.com/bytecodealliance/wasmtime/blob/main/crates/wasmtime/src/config.rs)
- Node.js 24.19.0 は仮想メモリが不足する環境で Wasm トラップハンドラを有効にしないよう変更した — [Node 24 changelog](https://github.com/nodejs/node/blob/main/doc/changelogs/CHANGELOG_V24.md)

**性能コスト**
- SpiderMonkey チームは memory64 が 32-bit 比で作業負荷により 10% から 100% 超遅くなると公表（2025-01-15）。原因は各 load / store の明示境界チェックで、CPU サイクル、追加のメモリ読み出し、コードサイズ、分岐予測資源を消費する — [Is Memory64 actually worth using?](https://spidermonkey.dev/blog/2025/01/15/is-memory64-actually-worth-using.html)
- 同記事の結論は、4 GB を超えるメモリが実際に必要な場合にのみ memory64 を使うべきというものである — [Is Memory64 actually worth using?](https://spidermonkey.dev/blog/2025/01/15/is-memory64-actually-worth-using.html)
- Wasm 3.0 の告知も、Web での 64-bit メモリは 16 GB に制限されると明記する — [Wasm 3.0 Completed](https://webassembly.org/news/2025-09-17-wasm-3.0/)

### Inferences
- ソートのようにメモリアクセスが支配的なループは memory64 の境界チェック負担を最も受けやすい種類であり、4 GiB 未満のデータでは wasm32 が合理的な既定である。
- V8 と SpiderMonkey では memory64 のチェック方式が異なるため、SpiderMonkey の 10% 〜 100% 超をそのまま V8 の見積もりに流用すべきではない。V8 にも比較と分岐は残るため、コストがゼロとも言えない。
- 仮想アドレス予約が前提のため、ulimit やコンテナ制限下の Node.js では明示チェックへ後退し、同じコードでも性能が変わる可能性がある。

### Gaps
- V8 の現行 memory64 と wasm32 を同一負荷で比較した公表値は見つからなかった。
- JavaScriptCore の memory64 上限の具体値は確認できなかった（`WasmPageCount.h` を取得できず）。
- iOS / Android の実効メモリ上限について信頼できる一次情報は得られなかった。

## JS と Wasm の境界: 呼び出しコスト、inlining、typed array、memory.grow、resizable buffer

### Takeaway
3 エンジンとも JIT から Wasm への直接経路を持ち、V8 はラッパーの inlining を既定で行うが、Wasm 本体の JS への inlining はごく小さい関数に限られる。`memory.grow` で非共有の固定長バッファは detach されるため、`toResizableBuffer` が使えない環境（Node.js 24 を含む）では grow のたびに view の作り直しが必要である。

### Cited Findings
**呼び出しコストと inlining**
- V8: `turbo_inline_js_wasm_calls` は既定 true（Node 24 の V8 13.6 と V8 15.6 の双方）で、JS-to-Wasm ラッパーを inline し、可能なら Wasm 関数本体も inline する — [flag-definitions.h (main)](https://github.com/nodejs/node/blob/main/deps/v8/src/flags/flag-definitions.h); [flag-definitions.h (v24.22.3)](https://github.com/nodejs/node/blob/v24.22.3/deps/v8/src/flags/flag-definitions.h)
- V8 の `WasmIntoJsInliner` が JS へ inline するのは、特定の命令だけからなる非常に小さい Wasm 関数に限られる — [wasm-inlining-into-js.h](https://github.com/nodejs/node/blob/main/deps/v8/src/compiler/wasm-inlining-into-js.h)
- V8 のラッパー inlining は導入時の 2021 年の報道で、典型的な呼び出しが約 3 分の 1 の時間になったとされる（二次情報） — [InfoQ](https://www.infoq.com/news/2021/03/v8-9-webassemly)
- SpiderMonkey: Baseline IC は Wasm の JitEntry スタブを呼び、Warp は `CallWasmFunction` をより直接的な呼び出しへ変換する。ソース上ではこれを inlined call と呼ぶ箇所があるが、本体の inlining を意味するとは書かれていない — [CacheIR.cpp](https://github.com/mozilla-firefox/firefox/blob/release/js/src/jit/CacheIR.cpp)
- Firefox は 2018 年の時点で、JS-to-Wasm 呼び出しを非 inline の JS-to-JS より高速化したと報告した。1 億回の呼び出しで約 5,500 ms から約 450 ms、単相では約 250 ms（グラフからの概算） — [Mozilla Hacks](https://hacks.mozilla.org/2018/10/calls-between-javascript-and-webassembly-are-finally-fast-%f0%9f%8e%89/)
- JavaScriptCore にも JS-to-Wasm の JIT 共有サンクと `JSToWasmCallee` がある — [JSToWasm.h](https://github.com/WebKit/WebKit/blob/main/Source/JavaScriptCore/wasm/js/JSToWasm.h)

**typed array view と memory.grow**
- JS API: 固定長バッファの場合、grow で旧バッファは detach され、新しい固定長バッファに置き換わる。resizable の場合は既存バッファのデータブロックと長さが更新される。`memory.grow` 命令の成功後も同じ処理が走る — [JS API](https://webassembly.github.io/spec/js-api/)
- ローカル実測（Node.js v24.18.0）: 非共有メモリの `grow(1)` 後、旧 `buffer` は `byteLength` 0 かつ `detached === true`、旧 `Uint8Array` の長さは 0、`memory.buffer` は別オブジェクトで 131,072 バイト
- ローカル実測（同）: 共有メモリの `grow(1)` 後、旧 `SharedArrayBuffer` は 65,536 バイトのまま残り、`memory.buffer` は 131,072 バイトの別オブジェクトになる
- V8 ではトラップハンドラ使用のメモリは移動せず、共有メモリも in-place でのみ拡張する — [wasm-module.h](https://github.com/nodejs/node/blob/main/deps/v8/src/wasm/wasm-module.h)

**resizable buffer**
- `toResizableBuffer()` は maximum のないメモリでは `TypeError`。成功時は最大長を max × 65,536 とする resizable バッファを作り、旧バッファを detach する。`toFixedLengthBuffer()` は逆方向の変換 — [JS API](https://webassembly.github.io/spec/js-api/)
- 対応は Chrome 144、Firefox 145、Safari 26.2、Node 26.0 — [BCD Memory](https://github.com/mdn/browser-compat-data/blob/main/webassembly/api/Memory.json)
- Node.js 24 系の V8 は `rab_integration` が既定 false（v24.22.3）、26 系は true。v24.18.0 の実測でも両メソッドは未定義 — [v24.22.3 flags](https://github.com/nodejs/node/blob/v24.22.3/deps/v8/src/wasm/wasm-feature-flags.h); [v26.13.1 flags](https://github.com/nodejs/node/blob/v26.13.1/deps/v8/src/wasm/wasm-feature-flags.h)

### Inferences
- 大きな配列を一度ソートする用途では、境界コストは呼び出し回数が少ないため支配的にならない。支配的になるのは、要素ごとに JS の比較関数を呼ぶ設計と、JS 配列と線形メモリの間のコピーである。
- ソート本体はループを含み小さくもないため、V8 の Wasm-into-JS inlining の対象にはならないと見るのが妥当である。
- ソート中にマネージドヒープの拡張で `memory.grow` が走り得るランタイムでは、JS 側が保持する view の無効化を前提に、呼び出しごとに view を取り直す必要がある。

### Gaps
- 3 エンジンを同条件で比較した、現行版の JS-to-Wasm 呼び出しコストの公表値は見つからなかった。Firefox の数値は 2018 年のもので現行値ではない。
- JavaScriptCore の JS-to-Wasm 最適化を説明した公式文書は見つからず、ソースの存在確認に留まる。

## ソート性能に最も影響する要素

### Takeaway
影響が大きいのは最適化ティアへの到達、128-bit に固定された SIMD、線形メモリの境界チェック方式である。V8 と SpiderMonkey は Wasm の OSR を持たないため、長時間走る 1 回の呼び出しは基準ティアに留まり得る。Wasm でのソートを直接測った公表値は見つからなかった。

### Cited Findings
**ティアリング**
- V8: Liftoff は一度の走査で高速にコード生成し、ホット関数をバックグラウンドで最適化コンパイラに渡す。実行中の呼び出しは Liftoff のまま完了し、OSR は行わない — [V8 Wasm compilation pipeline](https://v8.dev/docs/wasm-compilation-pipeline)
- V8 の現行既定: `wasm_lazy_compilation`、`wasm_dynamic_tiering`、`wasm_inlining`、`wasm_inlining_call_indirect`、`wasm_deopt`、`wasm_loop_unrolling`、`wasm_loop_peeling`、`wasm_bulkmem_inlining` は有効。`wasm_tiering_budget` は 13,000,000 — [flag-definitions.h](https://github.com/v8/v8/blob/main/src/flags/flag-definitions.h)
- 上記ドキュメントは lazy compilation を実験的と記すが、現行フラグは既定 true であり記述が古い — [V8 Wasm compilation pipeline](https://v8.dev/docs/wasm-compilation-pipeline); [flag-definitions.h](https://github.com/v8/v8/blob/main/src/flags/flag-definitions.h)
- V8 の Wasm パイプラインは全体で Turboshaft を使う（2025-03-25） — [Land ahoy: leaving the Sea of Nodes](https://v8.dev/blog/leaving-the-sea-of-nodes)
- V8 の speculative inlining と deopt は Chrome M137 で出荷。効果は WasmGC が中心で、Dart マイクロベンチは平均約 1.59 倍、非 GC の SQLite は約 1%、richards-wasm は約 2% — [V8 blog](https://v8.dev/blog/wasm-speculative-optimizations)
- SpiderMonkey: `wasm_lazy_tiering` は release ブランチで既定 true。bug 1957504 で全コンテンツに適用された — [StaticPrefList](https://github.com/mozilla-firefox/firefox/blob/release/modules/libpref/init/StaticPrefList.yaml); [Bug 1964966](https://bugzilla.mozilla.org/show_bug.cgi?id=1964966)
- SpiderMonkey の Wasm には OSR がなく、最適化コードがあっても baseline に留まる事例が疑われている — [Bug 1964966](https://bugzilla.mozilla.org/show_bug.cgi?id=1964966)
- JavaScriptCore: IPInt（インタプリタ）、BBQ（基準 JIT）、OMG（最適化 JIT）の 3 段構成。IPInt は ARM64 と x86_64 で SIMD に対応し既定有効 — [Introducing JetStream 3](https://webkit.org/?p=17899)
- JavaScriptCore は OSR を持つ。`useWasmOSR`、`useWasmIPIntLoopOSR` は既定 true で、OMG inlining も有効 — [OptionsList.h](https://github.com/WebKit/WebKit/blob/main/Source/JavaScriptCore/runtime/OptionsList.h)
- Wasm 実行系の比較研究は、最適化コンパイラのコードが基準コンパイラの 2 〜 3 倍速く、コンパイルは 1 桁程度遅いと整理する（2023 年） — [Whose baseline compiler is it anyway?](https://ar5iv.labs.arxiv.org/html/2305.13241)

**SIMD**
- SIMD128 は固定幅 128-bit の操作を標準化したもので、より柔軟な Flexible Vectors は Phase 1 — [V8 WebAssembly SIMD](https://v8.dev/features/simd); [WebAssembly/proposals](https://github.com/WebAssembly/proposals)
- V8 の 128 から 256-bit への revectorization は V8 13.6 で実験的機能の扱い — [flag-definitions.h (v24.22.3)](https://github.com/nodejs/node/blob/v24.22.3/deps/v8/src/flags/flag-definitions.h)
- Wasm SIMD の公表効果の例は MediaPipe の手追跡デモで 14〜15 FPS から 38〜40 FPS（ソートではない） — [V8 WebAssembly SIMD](https://v8.dev/features/simd)
- ネイティブの vqsort は AVX2 / AVX-512 で標準ライブラリ比約 10〜20 倍、128-bit の NEON（Apple M1）で約 3〜8 倍と報告される（Wasm の値ではない） — [Vectorized and performance-portable Quicksort](https://arxiv.org/abs/2205.05982)

**メモリ操作**
- V8 は bulk memory の inlining を既定で行い、JavaScriptCore の OMG は 1 バイトずつコピーまたは充填するループを bulk memory 操作へ置換する — [flag-definitions.h](https://github.com/v8/v8/blob/main/src/flags/flag-definitions.h); [OptionsList.h](https://github.com/WebKit/WebKit/blob/main/Source/JavaScriptCore/runtime/OptionsList.h)

**公表された比較測定**
- Wasm 対ネイティブ（2019）: SPEC CPU で Chrome 平均 1.55 倍、Firefox 平均 1.45 倍遅く、最大は 2.5 倍と 2.08 倍。当時のエンジンの値である — [Not So Fast (USENIX ATC 2019)](https://www.usenix.org/conference/atc19/presentation/jangda)
- Wasm 対ネイティブ（2026-06-23、個人の測定）: libsodium の SIMD なしビルドで Wasmtime 2.41 倍、Node 7.95 倍、WAMR AOT 1.57 倍。wide arithmetic 併用で Wasmtime 46 は 1.46 倍、Wasmer 7.1 は 1.33 倍。反復 3 回で小さいテストはノイズが大きい — [Performance of WebAssembly runtimes in 2026](https://00f.net/2026/06/23/webassembly-runtimes-2026/)
- Wasm 対 JS（Chrome 79、Firefox 71、Edge 79）: 極小入力で 41 ベンチの 97.6% が Wasm 優位（平均 26.99 倍）、小入力で 95.1%（8.22 倍）。中入力では 18 件が平均 1.71 倍遅く、23 件は平均 6.70 倍速い。メモリは Wasm が多い（デスクトップ Chrome で 3.39 倍） — [Benchmarking WebAssembly](https://benchmarkingwasm.github.io/BenchmarkingWebAssembly/); [論文 PDF (IMC 2021)](https://weihang-wang.github.io/papers/imc21.pdf)
- JetStream 3（2026-03-31）は Wasm の Startup / Runtime 分割採点を廃止し、JS と同じ反復採点へ移行した — [Introducing JetStream 3](https://webkit.org/?p=17899)

### Inferences
- 影響度の見立ては、(1) 最適化ティアへの到達、(2) SIMD128 の利用可否と活用度、(3) 境界チェック方式の順である。tail call、GC、JSPI、ESM はソートのループ性能にほぼ関与しない。
- OSR のない V8 と SpiderMonkey では、1 回の呼び出しで全要素を処理する反復型ソートは Liftoff / baseline に留まりやすい。再帰型は新しい呼び出しで最適化コードに移れるため、同じアルゴリズムでも実装形で初回性能が変わり得る。測定では初回と定常状態を分けるべきである。
- wasm32 のメモリアクセス自体はガード領域により明示チェックが不要だが、.NET の配列や Span の境界チェックはエンジンから見れば通常の比較と分岐である。エンジン側の境界チェック除去の話と混同すべきではない。
- SIMD 比較の参考値としては、AVX2 / AVX-512 より 128-bit の NEON の結果のほうが Wasm に近い。ただし Wasm 上の実測ではない。
- branch hinting はソートの比較分岐のように予測困難な分岐には効きにくく、wide arithmetic は比較ソートには直接関係しない。libsodium の結果をソート性能へ外挿すべきではない。
- Node の 7.95 倍という値は原因が記事内で説明されておらず、V8 一般の性能として引用するには根拠が足りない。

### Gaps
- Wasm 上のソート（vqsort の Wasm ターゲットを含む）や、Wasm と JS の `TypedArray.prototype.sort` を比較した信頼できる公表値は見つからなかった。
- 2025〜2026 年の主要ブラウザで SIMD 有効の Wasm 対ネイティブを測った査読付き研究は見つからなかった。
- IMC 2021 論文は PDF 本文を読めず、数値はプロジェクトページに基づく。論文の測定対象に Wasm 上のソートが含まれるかは未確認。
- `wasm_tiering_budget` が具体的にどの実行量でティアアップを起こすかは、公開文書で確認できなかった。

## WASI preview 1 / 2 / 3 と Component Model（非ブラウザ経路）

### Takeaway
WASI 0.3.0 が 2026-06-11 に公開され現行プレビューとなり、最新は 0.3.2（2026-10-13）である。ただし WASI の各 API も Component Model も Wasm 標準としては未完了（API は Phase 3、Component Model は Phase 1）で、1.0 の期日は示されていない。

### Cited Findings
- Preview 1 は witx ベースの API で現在も広く使われる。WASI 0.2（Preview 2）は Wit IDL ベース、WASI 0.3（Preview 3）が現行プレビューで、`future` と `stream` による Component Model のネイティブ async へ置き換える — [WASI README](https://github.com/WebAssembly/WASI/blob/main/README.md)
- 0.2.0 は 2024 年 1 月、0.3.0 は 2026-06-11、0.3.1 は 2026-08-11。以後 2 か月ごとの計画で 0.3.9（2027-12-14）まで列挙される。`wasi:io` は廃止され Canonical ABI に吸収された — [WASI Roadmap](https://wasi.dev/roadmap)
- 最新リリースは v0.3.2（2026-10-13）、0.2 系の最新は v0.2.12（2026-06-02） — [WASI releases](https://github.com/WebAssembly/WASI/releases)
- WASI 1.0 に目標日はなく、1.0 後の更新頻度も未定 — [WASI Roadmap](https://wasi.dev/roadmap)
- WASI 0.3 は Wasmtime 46 以降で既定有効、jco も対応 — [WASI Roadmap](https://wasi.dev/roadmap)
- Clocks、Random、Filesystem、Sockets、CLI、HTTP はいずれも WASI の Phase 3 — [WASI Proposals](https://github.com/WebAssembly/WASI/blob/main/docs/Proposals.md)
- Component Model は WebAssembly CG の Phase 1 — [WebAssembly/proposals](https://github.com/WebAssembly/proposals)
- Wasmtime は Component Model を Phase 4 未満のまま Tier 1 かつ既定有効で扱う。ファジングと C API には未完の部分がある — [Wasmtime tiers](https://docs.wasmtime.dev/stability-wasm-proposals.html)
- Wasmtime の既定機能は Cranelift で `WASM3` に wide arithmetic を加えた構成。Winch は `WASM2` 基準に一部を足した構成で、GC 型は外れる — [config.rs](https://github.com/bytecodealliance/wasmtime/blob/main/crates/wasmtime/src/config.rs)
- WASI の threads は Component Model の threading built-ins が候補段階で、0.3 の範囲には含まれない — [WASI Roadmap](https://wasi.dev/roadmap)

### Inferences
- CPU バウンドなソートは WASI の API にほとんど依存しないため、非ブラウザ経路の性能はランタイムのコンパイラ（Cranelift か AOT か）と SIMD 対応で決まる。
- Wasmtime 上で測るなら既定の Cranelift と Winch で対応機能も性能も異なるため、使用コンパイラの明記が必須である。
- ブラウザと同じ成果物を WASI で動かす前提は置けない。ツールチェーンが想定する WASI の世代（preview 1 か 0.2 / 0.3 か）を個別に確認する必要がある。

### Gaps
- WASI 0.3 における並列実行の確定仕様は確認できなかった（ロードマップ上は候補の記載のみ）。
- Wasmtime 以外のランタイムの WASI 0.3 対応状況は調査範囲外とした。
