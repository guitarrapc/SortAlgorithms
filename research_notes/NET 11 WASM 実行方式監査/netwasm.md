# NetWasm 調査ノート（2026年11月時点）

調査日は 2026-11-19。各項目の `[日付]` は、出典側の公開日が分かるものはその日付、分からないものは確認日（`確認 2026-11-19`）を示す。GitHub の `main` ブランチの文書とソースは同日時点の内容で、最終 push は 2026-11-16。NetWasm のインストールや実行は行っていない。数値はすべて公開情報の転記で、本リポジトリでの実測値ではない。**[ベンダー主張]** は NetWasm プロジェクト自身が公表した値や説明を指す。

## 開発者・ライセンス・ソース公開・配布・活動度・継続リスク

### Takeaway
NetWasm は Microsoft 外の個人開発者（Zion Sati 名義）による、ソース公開型だが OSI オープンソースではない C#/CIL → Wasm AOT ツールチェーンで、最新は `NetWasm.Sdk` 0.14.2（2026-11-15）。開発は非常に活発だが、公開から約2か月の 1.0 前プロジェクトで、コミットはほぼ1人に集中しており、継続性と互換性のリスクが最大の懸念となる。

### Cited Findings
- **開発者**: サイトは Microsoft 外の独立実装で、コンパイラ、CoreLib、ランタイム、精密 GC、SDK、WASI ホスト、ブラウザツールチェーンなどを1人の開発者が構築したと説明している。1.0 前で API/ABI は変わり得る。[確認 2026-11-19] — [netwasm.com](https://www.netwasm.com/)
- **ライセンサー**: オーストラリア登録の個人事業者で、`Zion Sati` の名義で活動している。ライセンス文書はこの名を仮名として扱う。準拠法はオーストラリア・ビクトリア州。[確認 2026-11-19] — [licensing.md](https://github.com/zion-sati/NetWasm/blob/main/docs/licensing.md); [LICENSE.md](https://github.com/zion-sati/NetWasm/blob/main/LICENSE.md)
- **ライセンスはパス単位で分割**: コンパイラ、リンカ、オプティマイザ、CLI、MSBuild タスク、SDK/pack、toolchain は `NetWasm Community License 1.0`。`NetWasm.CoreLib`、ランタイムライブラリ、JS ホスト、テンプレート、生成サポートコードは MIT。NetWasmGC と CompactGC は Apache-2.0。[確認 2026-11-19] — [LICENSE.md](https://github.com/zion-sati/NetWasm/blob/main/LICENSE.md)
- **無償利用の範囲**: 個人、教育、OSI 承認ライセンスの無報酬 OSS 作業（完全なソース公開、顧客・雇用主向けの専有成果物でないこと、その作業への対価がないことが条件）、30日評価、従業員250人未満かつ年商1,000万米ドル未満の非政府組織。[確認 2026-11-19] — [Community License §2, §4](https://github.com/zion-sati/NetWasm/blob/main/LICENSES/LicenseRef-NetWasm-Community-1.0.txt)
- **有償利用**: それ以外の組織は GitHub Sponsors の tier（5人 149、25人 399、100人 999、無制限 2,499 米ドル/月）で社内利用権を得る。ツールの同梱、再販、OEM、ホスト型のコンパイルサービスは規模によらず別契約。[確認 2026-11-19] — [licensing.md](https://github.com/zion-sati/NetWasm/blob/main/docs/licensing.md)
- **生成物**: 生成された Wasm のライセンスは作者が選べ、適格な中小企業は販売・配布もできる。[確認 2026-11-19] — [licensing.md](https://github.com/zion-sati/NetWasm/blob/main/docs/licensing.md)
- **ライセンス条件の変更と保証**: 各コピーは配布時の版の条件に従い、新版は過去の許諾を遡って変更しない。OSS への自動転換日はない。無保証で保守義務はなく、責任上限は総額100米ドル。派生物は同ライセンスのまま。[確認 2026-11-19] — [Community License §7, §13–15](https://github.com/zion-sati/NetWasm/blob/main/LICENSES/LicenseRef-NetWasm-Community-1.0.txt)
- **OSS かどうかの見解の相違**: byteiota は「open-source」と表記するが、Hacker News では OSS ではないとの指摘があり、GitHub API のライセンス判定は `NOASSERTION`。[2026-10-24 / 2026-10-21 / 確認 2026-11-19] — [byteiota](https://byteiota.com/netwasm-csharp-webassembly/); [HN](https://news.ycombinator.com/item?id=50175510); [GitHub API](https://api.github.com/repos/zion-sati/NetWasm)
- **ソース**: `zion-sati/NetWasm` で公開。リポジトリ作成 2026-08-10、最終 push 2026-11-16、star 362、fork 16、open issue 14。[確認 2026-11-19] — [GitHub API](https://api.github.com/repos/zion-sati/NetWasm)
- **コミット集中**: 既定ブランチは473コミットで、貢献者は `zion-sati` 472、`github-actions[bot]` 1。[確認 2026-11-19] — [contributors API](https://api.github.com/repos/zion-sati/NetWasm/contributors?per_page=20)
- **外部参加**: issue は累計113件で、うち所有者以外の起票が37件。所有者以外が作成した PR の検索結果は0件。PR のマージには CLA 同意が必要。[確認 2026-11-19] — [Issues](https://github.com/zion-sati/NetWasm/issues); [CONTRIBUTING.md](https://github.com/zion-sati/NetWasm/blob/main/CONTRIBUTING.md)
- **リリース頻度**: GitHub Release は preview を含め97件、うち安定版38件。`v0.1.0` が 2026-09-15、`v0.14.2` が 2026-11-15。[確認 2026-11-19] — [Releases](https://github.com/zion-sati/NetWasm/releases)
- **NuGet 配布**: `NetWasm.Sdk` は76バージョンあり、最新は 0.14.2。[確認 2026-11-19] — [flat container](https://api.nuget.org/v3-flatcontainer/netwasm.sdk/index.json)
- **NuGet 統計**: 所有者は `zionsati` のみ、ID プレフィックス予約済み、累計約8.3K DL、0.14.2 は297 DL、依存する NuGet パッケージの表示なし。検索では同所有者の関連パッケージが64件（`NetWasm.Toolchain`、`NetWasm.Runtime.Pack`、`NetWasm.Ref`、`NetWasm.Hosting`、`NetWasm.Compiler.Tasks`、`NetWasm.Templates` など）。[確認 2026-11-19] — [NetWasm.Sdk](https://www.nuget.org/packages/NetWasm.Sdk); [NuGet 検索](https://www.nuget.org/packages?q=netwasm)
- **リリース経路の良い点**: NuGet へは OIDC の短期トークンで publish し、サードパーティ Action はコミット SHA で固定、wasm-tools は SHA-256 を検証する。[確認 2026-11-19] — [release.yml](https://github.com/zion-sati/NetWasm/blob/main/.github/workflows/release.yml)
- **リリース経路の不足点**: `release.yml` に build provenance attestation、SBOM、sigstore/cosign の記述はない。Node は checksum 検証なし、emsdk はタグ指定の clone、Linux の LLVM/clang は apt でバージョン固定なし。`--allowed-signers infrastructure/eng/release-signers` による検証手順はあるが、その対象は同ファイルからは確認できない。[確認 2026-11-19] — [release.yml](https://github.com/zion-sati/NetWasm/blob/main/.github/workflows/release.yml)
- **ビルド時に実行されるもの**: `dotnet restore` がホスト別のツールパッケージを選び、`nwc`、固定版 Node、`wasm-ld`、`wasm-opt`、`wasm-tools` を供給する。[確認 2026-11-19] — [sdk-quickstart.md](https://github.com/zion-sati/NetWasm/blob/main/docs/sdk-quickstart.md)
- **1.0 の見通し**: ロードマップに期日はない。1.0 では `netwasm1.0` プロファイルを新設し、`netwasm0.1` は 0.x 用として残す。[確認 2026-11-19] — [roadmap.md](https://github.com/zion-sati/NetWasm/blob/main/docs/roadmap.md)

### Inferences
- 継続リスクは高い。実質1人の開発で、名義は仮名、外部 PR の実績も確認できない。一方で、取得済みの版の権利は遡って失われず、CoreLib（MIT）と GC（Apache-2.0）は再利用しやすいため、開発停止時も既存版の利用は続けられる。コンパイラ本体をフォークして商用配布する道は限られる。
- サプライチェーン面では、ビルド時に単一発行者のネイティブバイナリを実行する。attestation がないため、発行物とソースの対応を第三者が暗号学的に確認する手段は `release.yml` からは見えない。導入する場合はバージョンを固定し、`packages.lock.json` などでパッケージハッシュを管理するのが妥当。
- 本リポジトリは MIT（[LICENSE.md](../../LICENSE.md)）で、`#if NETWASM` を含むだけならツールの同梱にはあたらない。NetWasm でビルドする場合の無償枠は、作業者の立場（無報酬 OSS か、適格組織か）で決まる。法的判断ではない。

### Gaps
- 開発者の実名、組織体制、後継者や資金状況を示す情報は見つからなかった。
- スポンサー数や商用導入事例は確認できなかった。
- NuGet パッケージの作者署名の有無は確認していない。`release-signers` の中身は公開リポジトリの `release.yml` からは分からない。

## アーキテクチャと .NET / ターゲットフレームワーク対応

### Takeaway
NetWasm は Roslyn が出力した CIL を閉世界で解析し、独自コンパイラ `nwc` で Wasm を直接生成する。Mono や CoreCLR は成果物に含まれず、後段で Binaryen `wasm-opt` をかける。.NET 11 SDK と C# 15 は使えるが、ターゲットは `net11.0` ではなく独自 TFM の `netwasm0.1` である。

### Cited Findings
- **パイプライン**: Roslyn → CIL → エントリポイントからの到達可能性解析 → ジェネリック特殊化とデータレイアウト → Wasm 生成 → 必要な CoreLib/ランタイムのリンク → WASI 0.3 コンポーネント化。CIL を実行時に解釈しない。[確認 2026-11-19] — [Compile C# to WebAssembly](https://www.netwasm.com/compile-csharp-to-webassembly/)
- **コンパイラ構成**: `NetWasm.Compiler.Metadata`（ECMA-335 読み取り）、`ControlFlow`（構造化）、`GarbageCollection`（厳密ルート解析）、`Wasm`（計画とエンコード）、`ComponentModel`（WIT、Canonical ABI、パッケージング）。[確認 2026-11-19] — [ARCHITECTURE.md](https://github.com/zion-sati/NetWasm/blob/main/docs/ARCHITECTURE.md)
- **Mono/CoreCLR との関係**: 成果物に CoreCLR、Mono、`dotnet.js`、個別のアプリ DLL は含まれない。[確認 2026-11-19] — [Compile C# to WebAssembly](https://www.netwasm.com/compile-csharp-to-webassembly/)
- **CoreLib**: .NET から移植した CoreLib と独自ランタイムを持つと説明されている。[2026-11-05] — [NetWasm 0.10 blog](https://www.netwasm.com/blog/compiler-optimization/)
- **Binaryen**: Release の既定では `wasm-opt` が指定レベルのパスをリンク済みモジュールに1回適用する。`None` と `O0` では `wasm-opt` の最適化を行わない。[確認 2026-11-19] — [sdk-quickstart.md](https://github.com/zion-sati/NetWasm/blob/main/docs/sdk-quickstart.md); [ComponentCoreModuleOptimizer.cs](https://github.com/zion-sati/NetWasm/blob/main/src/NetWasm.Compiler.ComponentModel/ComponentCoreModuleOptimizer.cs)
- **LLVM 系**: リンクに `wasm-ld` を使い、ツール類は NuGet から供給される。Emscripten 6.0.7 が Node を供給する経路も文書にある。[確認 2026-11-19] — [sdk-quickstart.md](https://github.com/zion-sati/NetWasm/blob/main/docs/sdk-quickstart.md); [cli-build-deployment.md](https://github.com/zion-sati/NetWasm/blob/main/docs/cli-build-deployment.md)
- **TFM**: アプリとライブラリは `netwasm0.1`。.NET 11 SDK を使っても TFM は変わらない。`netwasm-lib` テンプレートは `netwasm0.1` と `net10.0` のデュアルターゲット。[確認 2026-11-19] — [sdk-quickstart.md](https://github.com/zion-sati/NetWasm/blob/main/docs/sdk-quickstart.md)
- **SDK 要件**: .NET SDK 10.0.303 以上。C# 15 を使うソースには .NET 11 SDK が必要。[確認 2026-11-19] — [sdk-quickstart.md](https://github.com/zion-sati/NetWasm/blob/main/docs/sdk-quickstart.md); [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md)
- **.NET 11 との対応**: `NetWasm.App.Ref` のファサードは `Microsoft.NETCore.App.Ref` 11.0.0 に名前と ID を合わせている。[確認 2026-11-19] — [NetWasm.PackageVersions.props](https://github.com/zion-sati/NetWasm/blob/main/eng/NetWasm.PackageVersions.props)
- **`net8.0` 以降のみのパッケージは使えない**: NuGet は `netwasm0.1`、次に `netstandard2.1`/`2.0` 以前の資産を選ぶ。[確認 2026-11-19] — [netstandard-libraries.md](https://github.com/zion-sati/NetWasm/blob/main/docs/netstandard-libraries.md); [NetWasm.Sdk.Tfm.props](https://github.com/zion-sati/NetWasm/blob/v0.14.2/src/NetWasm.Sdk/Sdk/NetWasm.Sdk.Tfm.props#L48)
- **開発ホスト**: Linux x64/ARM64（glibc 2.28+、musl 不可）、macOS ARM64（13.5+）、Windows x64/ARM64。[確認 2026-11-19] — [sdk-quickstart.md](https://github.com/zion-sati/NetWasm/blob/main/docs/sdk-quickstart.md)

### Inferences
- 本リポジトリの `Directory.Build.props` は `net11.0` 単一ターゲットである。NetWasm 向けには、ライブラリに `netwasm0.1` を追加するか、NetWasm SDK プロジェクトからソースをコンパイルする必要がある。ビルド済み `net11.0` DLL の流用はできない。
- 文書に NativeAOT や ILCompiler の記述はなく、コンパイラ構成も独自のアセンブリで説明されている。NativeAOT-LLVM の派生ではないと読めるが、明示の否定文は確認していない。

### Gaps
- コンパイラ内部の IR の詳細を説明した文書は見つからなかった。
- CoreLib の移植元の dotnet/runtime コミットは `corelib-runtime.md` には記載がない。

## 出力する Wasm 機能と最低ブラウザ / ランタイム

### Takeaway
必須機能は標準 Wasm EH（`try_table`/`exnref`）、128-bit SIMD、bulk memory、reference types、multi-value など。最低ラインは Chrome/Edge 137、Firefox 131、Safari 18.4、Node.js 24.15。Wasm GC、relaxed SIMD、threads、tail call、JS String Builtins を利用している証拠は見つからなかった。

### Cited Findings
- **必須機能**: `try_table`/`exnref` の標準例外処理、固定幅 SIMD、bulk memory、reference types、multi-value、非トラップ float→int、符号拡張、mutable globals。例外処理が最も新しく、最低ブラウザを決めている。[確認 2026-11-19] — [targets-and-outputs.md](https://github.com/zion-sati/NetWasm/blob/main/docs/targets-and-outputs.md)
- **最低バージョン（wasm32）**: Chrome/Edge 137、Firefox 131、Safari（macOS/iOS）18.4、Node.js 24.15。[確認 2026-11-19] — [targets-and-outputs.md](https://github.com/zion-sati/NetWasm/blob/main/docs/targets-and-outputs.md)
- **最低バージョン（wasm64）**: Chrome/Edge 137、Firefox 134、Safari は非対応。JS 境界のアドレスは `BigInt`。上限ポリシーは wasm32 が 2 GiB、wasm64 が 8 GiB。[確認 2026-11-19] — [targets-and-outputs.md](https://github.com/zion-sati/NetWasm/blob/main/docs/targets-and-outputs.md); [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md)
- **未対応ブラウザの挙動**: EH と SIMD がないブラウザでは `open()`/`run()` が `NetWasmUnsupportedBrowserError` を返す。[2026-11-02, v0.10.7] — [Release 0.10.7](https://github.com/zion-sati/NetWasm/releases/tag/v0.10.7)
- **ブラウザの優先度**: Chromium が主対象、Firefox は互換性カバレッジ、WebKit は wasm32 のみテスト。[確認 2026-11-19] — [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md)
- **wasm-opt に渡す機能フラグ**: `--enable-multimemory`、`--enable-exception-handling`、`--enable-reference-types`、`--enable-bulk-memory`、`--enable-nontrapping-float-to-int`、`--enable-simd`、wasm64 時のみ `--enable-memory64`。[確認 2026-11-19] — [ComponentCoreModuleOptimizer.cs](https://github.com/zion-sati/NetWasm/blob/main/src/NetWasm.Compiler.ComponentModel/ComponentCoreModuleOptimizer.cs)
- **SIMD**: `Vector128<T>`、`Vector<T>`、`PackedSimd` は 128-bit Wasm SIMD に変換される。[確認 2026-11-19] — [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md)
- **relaxed SIMD / threads / tail call（ネイティブ連携）**: 静的ネイティブ連携のプロファイルは、これらを使うアーカイブを拒否するテストを持つ。[確認 2026-11-19] — [NativeRuntimeValidationRejectionIntegrationTests.cs](https://github.com/zion-sati/NetWasm/blob/main/tests/NetWasm.Runtime.Pack.Tests/Integration/NativeRuntimeValidationRejectionIntegrationTests.cs)
- **CIL の `tail.`**: 未実装と明記されている。[確認 2026-11-19] — [cil-opcode-policy.json](https://github.com/zion-sati/NetWasm/blob/main/compiler-qualification/cil-opcode-policy.json)
- **Wasm GC**: NetWasm は GC を成果物に同梱し、線形メモリ上で動かす。ホストの WasmGC に依存する Kotlin とは対照的と文書で説明される。[確認 2026-11-19] — [size-and-methodology.md](https://github.com/zion-sati/NetWasm/blob/main/docs/size-and-methodology.md); [runtime-garbage-collection.md](https://github.com/zion-sati/NetWasm/blob/main/docs/runtime-garbage-collection.md)
- **スレッド**: マネージドスレッド、ヒープ共有ワーカー、スレッドプール、`Task.Run` による並列実行は非対応。COOP/COEP ヘッダーも不要。[確認 2026-11-19] — [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md); [javascript-interop.md](https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md)
- **Wasmtime**: Wasmtime 50 でのコンポーネント実行例は `wasmtime run -Wcomponent-model-async -Sp3`。[確認 2026-11-19] — [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md)

### Inferences
- `wasm-opt` のフラグに `--enable-relaxed-simd`、`--enable-tail-call`、`--enable-gc`、`--enable-threads` がなく、必須機能一覧にも含まれない。したがって通常の出力はこれらの命令を含まないと考えられる。実バイナリの検査ではないため、計測タスクで `wasm-tools` により確認するのが望ましい。
- GitHub コード検索で `js-string`、`wasm-gc`、`stringref` はヒットしなかった（2026-11-19）。JS String Builtins は使っていない可能性が高いが、検索インデックスの限界があるので断定はしない。
- `--enable-multimemory` は有効化されているが必須機能一覧にはない。実際に multi-memory を出力するかは未確認。

### Gaps
- Wasm バイナリの feature section を実物で確認していない（実行・取得禁止の範囲のため）。
- Wasmtime で EH や SIMD に追加フラグが必要かは、文書からは確認できなかった。

## BCL / ランタイムの対応範囲と制約

### Takeaway
閉世界の AOT なので、具象ジェネリクスの特殊化、`Span<T>`、`Vector128`/`PackedSimd`、`ArrayPool<T>` など CPU バウンドのソートに必要な要素はそろっている。制約は実行時リフレクション、スレッド、`Vector256`/`Vector512` の非高速化、ジェネリック再帰の成長上限である。

### Cited Findings
- **ジェネリクス**: 具象ジェネリックの特殊化と、ジェネリックインターフェース/仮想ディスパッチに対応。実行時に作るジェネリック型は対象外。[確認 2026-11-19] — [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md); [corelib-runtime.md](https://github.com/zion-sati/NetWasm/blob/main/docs/corelib-runtime.md)
- **ジェネリック成長上限**: 無限に成長するインスタンス化は4段階、1メソッド16インスタンスで打ち切られ、到達すると `NotSupportedException`。[確認 2026-11-19] — [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md)
- **`Span<T>` の扱い**: O3 では `Span<T>` のように `ref` フィールドを持つ構造体を Wasm ローカルに置ける。構造体やジェネリック型のメンバーも JIT に近い形でインライン化される。[2026-10-19, v0.9.0] — [0.9.0 release notes](https://github.com/zion-sati/NetWasm/blob/main/docs/releases/0.9.0.md)
- **Span ループの最適化**: 0.11.2 で span の畳み込みが4要素単位の SIMD になり、インデクサの境界チェックをループ前に移せるようになった。[2026-11-07, v0.11.2] — [Release 0.11.2](https://github.com/zion-sati/NetWasm/releases/tag/v0.11.2)
- **インライン化の規則**: `NoInlining` は常に除外。`AggressiveInlining` は優先候補として扱われ、ポリシー（`Disabled`/`Speed`/`Loops`/`Aggressive`）で効き方が変わる。呼び出し先の命令数の目安は small 16、leaf 32、loop body 56（前方分岐6以下）。例外領域や `localalloc` を持つ本体は対象外。[確認 2026-11-19] — [ManagedInliningResolver.cs](https://github.com/zion-sati/NetWasm/blob/main/src/NetWasm.Compiler/Analysis/Inlining/ManagedInliningResolver.cs)
- **最適化パイプライン**: インライン化は最大8ラウンドで、3ラウンド目以降は small のみ。各ラウンドで devirtualization も実行する。構造体のスカラー置換、配列境界チェック除去、ループのベクトル化とアンローリングも含まれる。[確認 2026-11-19] — [ManagedMethodOptimizationPipeline.cs](https://github.com/zion-sati/NetWasm/blob/main/src/NetWasm.Compiler/Analysis/ManagedMethodOptimizationPipeline.cs)
- **SIMD の範囲**: `Vector128.IsHardwareAccelerated` と `Vector.IsHardwareAccelerated` は true。x86/Arm intrinsics の `IsSupported` は false。`Vector64`/`Vector256`/`Vector512` は高速化されない。O2/O3 では要素単位ループを自動ベクトル化する。[確認 2026-11-19] — [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md)
- **リフレクション**: 広範な実行時リフレクション、`dynamic`、動的アセンブリロードは対象外。Release の `Type.Name` は既定で `type#<id>` を返し、`NetWasmTypeNames` で実名を出せる。[確認 2026-11-19] — [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md)
- **GC**: 既定は精密な非移動 mark-sweep の Compact。`NetWasmGarbageCollector` で速度重視の NetWasmGC、回収しない Bump を選べる。[確認 2026-11-19] — [runtime-garbage-collection.md](https://github.com/zion-sati/NetWasm/blob/main/docs/runtime-garbage-collection.md); [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md)
- **`ArrayPool<T>`**: 0.14.0 から .NET の pool 実装。`wasi:clocks/monotonic-clock` がないと返却配列を時間経過で解放しない。[2026-11-14, v0.14.0] — [0.14.0 release notes](https://github.com/zion-sati/NetWasm/blob/main/docs/releases/0.14.0.md)
- **スタック**: `RuntimeHelpers.EnsureSufficientExecutionStack` は現行プロファイルでは no-op。[確認 2026-11-19] — [spec-deviations.md](https://github.com/zion-sati/NetWasm/blob/main/docs/spec-deviations.md)
- **文書間の不一致**: サイトと `corelib-runtime.md` は一般ファイル API と raw socket を対象外とするが、`support-status.md` は WASI 0.3 上の `System.IO.File` や `System.Net.Sockets` を対応と記載する。[確認 2026-11-19] — [netwasm.com](https://www.netwasm.com/); [corelib-runtime.md](https://github.com/zion-sati/NetWasm/blob/main/docs/corelib-runtime.md); [support-status.md](https://github.com/zion-sati/NetWasm/blob/main/docs/support-status.md)

### Inferences
- `SortSpan<T, TComparer, TContext>` は `Span<T>` を持つ `ref struct` で（[SortSpan.cs](../../src/SortAlgorithm/Algorithms/SortSpan.cs)）、ローカル配置やインライン化は O3 の恩恵を受けやすい構造である。逆に既定の Oz では同じ最適化が効くとは限らない。
- struct の `TComparer`/`TContext` は具象特殊化の対象になるはずだが、組み合わせごとにコードが複製されるのでサイズは組み合わせ数に応じて増える可能性が高い。
- `src/SortAlgorithm` には `Parallel.`、`Task.Run`、`new Thread`、`EnsureSufficientExecutionStack` の使用がなく、スレッド制約の直接の影響はない。
- Driftsort と Glidesort の AVX-512 経路は `Avx512F.IsSupported` で保護されており、NetWasm では false となるためスカラー経路に落ちるはずである。
- 敵対的入力で再帰が深くなるソートでは、スタック枯渇時の挙動を計測タスクで確認する必要がある。

### Gaps
- 最適化レベルと `ManagedMethodInliningPolicy` の対応表は確認できなかった（GitHub API のレート制限で該当コードの探索を打ち切った）。
- struct 型引数の特殊化、`constrained.` 呼び出しの直接化を明示した文書は見つからなかった。
- `Array.Sort`/`MemoryExtensions.Sort` の実装と性能は確認できなかった。
- `Unsafe`、`MemoryMarshal`、`stackalloc` の網羅的な対応表はない。`support-status.md` は、メンバー単位の互換カタログはまだないとしている。

## JS interop

### Takeaway
`OutputType=JsModule` と `[JSExport]`/`[JSImport]` で、型付きの `.mjs` と `.d.mts` が生成される。配列は呼び出しごとにコピーされ、JS から export へのゼロコピー経路はない。ゼロコピーは同期 `[JSImport]` に `Span<T>` を渡す C# → JS 方向の view のみである。

### Cited Findings
- **プロジェクト形式**: `<OutputType>JsModule</OutputType>` で `browser/<AssemblyName>.mjs` と `.d.mts` を出力する。`open()` は export を持つ凍結オブジェクトを返し、呼び出しごとに独立したインスタンスを作る。[確認 2026-11-19] — [javascript-interop.md](https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md)
- **ホスト**: `NetWasmJavaScriptHosts` に `page`（既定）、`worker`、`page;worker`、`node` を指定する。`worker` は `<AssemblyName>.worker-client.mjs`、`node` は `<AssemblyName>.node.mjs` を出力する。[確認 2026-11-19] — [javascript-interop.md](https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md)
- **スレッドとの関係**: `page` の同期 export は完了までページスレッドを止める。`worker` の export はすべて Promise を返す。[確認 2026-11-19] — [javascript-interop.md](https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md)
- **数値配列の対応**: 1次元の数値配列は型付き配列に対応する（`byte[]`→`Uint8Array`、`long[]`→`BigInt64Array`、`double[]`→`Float64Array` など）。要素型が違う型付き配列は拒否される。[確認 2026-11-19] — [javascript-interop.md](https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md)
- **コピーの方向**: 文字列と配列は境界を両方向ともコピーで渡る。JS 側のコピーへの書き込みは .NET に届かない。[確認 2026-11-19] — [javascript-interop.md](https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md)
- **export と Span**: 同期 `[JSExport]` の対応シグネチャに span は含まれない。非同期の import/export も span 不可で、数値配列を使う。[確認 2026-11-19] — [javascript-interop.md](https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md)
- **ゼロコピー view**: 同期 `[JSImport]` は数値配列の要素型の `Span<T>`/`ReadOnlySpan<T>` を受け取れる。view は呼び出しの間だけ有効で、JS は線形メモリを直接読み書きでき、チェックはない。メモリ拡張で detach し得る。境界にアラインしていない span は `JSException`。[確認 2026-11-19] — [javascript-interop.md](https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md)
- **スカラーのみの import**: 引数と戻り値がプリミティブだけの同期 import は、メモリコピーなしの直接 Wasm 呼び出しになる。[確認 2026-11-19] — [javascript-interop.md](https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md)
- **配信要件**: ES module、`.mjs` の JS MIME、厳格な CSP では `'wasm-unsafe-eval'` と `blob:`、`file://` 不可。[確認 2026-11-19] — [javascript-interop.md](https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md)
- **再入制限**: JS から C# への呼び出しのネストは32段まで。[確認 2026-11-19] — [javascript-interop.md](https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md)

### Inferences
- `int[] Sort(int[] values)` の形では、入力と出力で2回コピーが発生する。性能計測ではソート本体だけを C# 側の `Stopwatch` で測るか、コピー時間を分けて記録する必要がある。
- コピーを避けるなら、C# 側でバッファを確保し、同期 `[JSImport]` の `Span<int>` 経由で JS に書き込ませ、ソート後に別の import で読ませる構成になる。ただし view の寿命と detach に注意が要る。
- 本調査の範囲では Node と browser で同じ export 契約が使えるので、計測ハーネスは両方に流用しやすい。エンジン差は別途の実測が必要である。

### Gaps
- `Memory<T>`、`ArraySegment<T>` の扱いは文書で確認できなかった。
- 境界コピーの実コスト（要素数あたりの時間）の公表値は見つからなかった。

## 速度優先ビルドの設定とサイズへの影響

### Takeaway
Release の既定は `Oz` で、速度優先は `NetWasmOptimization=O3`（必要に応じて `NetWasmGarbageCollector=NetWasm`）を明示する。同一プログラムで O3 と Oz のサイズを比べた公表値は見つからず、公表されているのは O3 のベンチマークプログラムの絶対サイズである。

### Cited Findings
- **既定**: Debug は `O0`、それ以外は `Oz`。`NetWasmOptimizerConverge` の既定は false。[確認 2026-11-19] — [NetWasm.Compiler.Tasks.targets](https://github.com/zion-sati/NetWasm/blob/main/src/NetWasm.Compiler.Tasks/buildTransitive/NetWasm.Compiler.Tasks.targets#L86-L94)
- **選択肢**: `None`、`O0`、`O1`、`O2`、`O3`、`Os`、`Oz`。[確認 2026-11-19] — [FinalWasmOptimizationParser.cs](https://github.com/zion-sati/NetWasm/blob/main/src/NetWasm.Compiler.Tasks/MsBuild/FinalWasmOptimizationParser.cs)
- **O3 の位置づけ**: O3 はサイズと引き換えに速度を取り、`Os`/`Oz` はサイズ重視を維持する。[2026-10-19, v0.9.0] — [0.9.0 release notes](https://github.com/zion-sati/NetWasm/blob/main/docs/releases/0.9.0.md)
- **レベルで変わる項目**: ループのベクトル化は O2/O3 で有効。`UseSizeOptimizedLinq` は O1–O3 で false、それ以外で true。multicast delegate のディスパッチも O1–O3 と `None`/`O0`/`Os`/`Oz` で異なる。[確認 2026-11-19] — [Release 0.10.7](https://github.com/zion-sati/NetWasm/releases/tag/v0.10.7); [Sdk.targets](https://github.com/zion-sati/NetWasm/blob/main/src/NetWasm.Sdk/Sdk/Sdk.targets#L93-L94); [Release 0.11.2](https://github.com/zion-sati/NetWasm/releases/tag/v0.11.2)
- **`NetWasmOptimizerConverge=true`**: `wasm-opt` をサイズが縮まなくなるまで反復する。DoomSharp では最適化時間が約2倍で、モジュールは Oz で 0.04%、O3 で 0.12% 小さくなり、描画は速くならなかった。一部のマイクロベンチは最大3倍速くなった。**[ベンダー主張]** [確認 2026-11-19] — [sdk-quickstart.md](https://github.com/zion-sati/NetWasm/blob/main/docs/sdk-quickstart.md)
- **O3 + converge のプログラムサイズ（0.14.0）**: **[ベンダー主張]** [確認 2026-11-19] — [performance.md](https://github.com/zion-sati/NetWasm/blob/main/docs/performance.md)

| ビルド | 非圧縮 (bytes) | Brotli 11 (bytes) |
| --- | ---: | ---: |
| NetWasm + NetWasmGC | 102,614–217,543 | 25,520–49,415 |
| NetWasm + Compact | 91,891–203,860 | 22,815–46,590 |
| Mono AOT | 7,766,388–7,857,739 | 1,580,676–1,596,981 |

- **Oz の最小例（`Console.WriteLine(42)`、0.14.0）**: **[ベンダー主張]** Compact 28,383 B（Brotli 11,992）、NetWasmGC 32,159 B（13,478）、Bump 23,810 B（9,870）。同じ処理の Blazor AOT は .NET 10 で 12,276,684 B、.NET 11 RC1 で 15,289,241 B。[確認 2026-11-19] — [size-and-methodology.md](https://github.com/zion-sati/NetWasm/blob/main/docs/size-and-methodology.md)
- **実アプリ例**: DoomSharp のブラウザ向けビルドは圧縮後 0.69 MiB、Blazor WebAssembly AOT 版は 3.67 MiB。**[ベンダー主張]** [2026-11-15] — [Micro-benchmarks](https://www.netwasm.com/micro-benchmarks/)
- **O3 のビルドコスト**: DoomSharp の O3 ビルドのピークメモリは 0.11.2 で 3.4 GB から 1.5 GB に低下。[2026-11-07] — [Release 0.11.2](https://github.com/zion-sati/NetWasm/releases/tag/v0.11.2)
- **関数サイズ上限**: O3 のインライン化で関数がブラウザ/Node の上限（7,654,321 バイト、50,000 ローカル）を超えると、0.14.1 以降はインラインなしで再構築する。それでも超えれば `NW1013`。[2026-11-15] — [Release 0.14.1](https://github.com/zion-sati/NetWasm/releases)
- **既知の問題**: `wasm-opt` はメモリを多く使い、`-Oz` は特に Windows で非常に遅いことがある。回避策は `BINARYEN_CORES=4`、`O2`、Linux/WSL でのビルド。[2026-11-14] — [0.14.0 release notes](https://github.com/zion-sati/NetWasm/blob/main/docs/releases/0.14.0.md)

### Inferences
- 既定の Release ビルドのまま計測すると Oz の性能を測ることになり、ベンダーの公表値（O3）とは比較できない。計測では `O3` と GC を明示する必要がある。
- ベンチマーク対象のソートは多数のジェネリック組み合わせを含み得るので、上の 100–200 KB は目安にならない。実サイズは計測タスクで確認すべきである。
- 開発環境が Windows の場合、`-Oz` の遅さは検証の手間に影響し得る。

### Gaps
- 同一プログラムの O3 と Oz のサイズ比、速度比の公表値は見つからなかった。

## 公表されている性能値、方法論、独立検証

### Takeaway
ベンダー値では全体で Mono AOT の 0.28 倍の時間だが、デスクトップ .NET 11 JIT の 1.51 倍の時間である。ソートに近い処理の結果は幅が大きく、Mono AOT 比 0.24 倍から 1.10 倍まで分かれる。第三者による再現は見つからなかった。

### Cited Findings
- **方法論（マイクロベンチ）**: **[ベンダー主張]** NetWasm 0.14.0（NuGet）、`O3`、NetWasmGC、converge 有効、事前コンパイル。比較対象は Mono AOT（.NET 11.0.0 wasi-experimental、Wasm SIMD）と desktop .NET 11 JIT（tiering 無効）。Wasmtime 50.0.1、Apple M1 Max、macOS 26.6.2。25件単位で順序を入れ替え、2ラウンドの最速値を採用。[2026-11-15] — [Micro-benchmarks](https://www.netwasm.com/micro-benchmarks/)
- **ベンチ本体**: dotnet/performance のコミット `ceb046992aa203f881d7c8a29d50a105805ecb42` の2,213件。BenchmarkDotNet は共通の単純なタイミングループに置き換えている。[確認 2026-11-19] — [performance.md](https://github.com/zion-sati/NetWasm/blob/main/docs/performance.md)
- **ファミリー別の幾何平均**: **[ベンダー主張]** [確認 2026-11-19] — [micro-benchmarks.md](https://github.com/zion-sati/NetWasm/blob/main/docs/micro-benchmarks.md)

| ファミリー | 件数 | NetWasm / Mono AOT | NetWasm / desktop JIT |
| --- | ---: | ---: | ---: |
| NetWasm workloads | 5 | 0.46 | 0.68 |
| Runtime | 384 | 0.39 | 0.70 |
| Math | 96 | 0.80 | 1.31 |
| `System.Memory` spans | 370 | 0.41 | 1.59 |
| `System.Numerics` vectors | 770 | 0.27 | 2.24 |
| `System.Runtime.Intrinsics` | 588 | 0.15 | 1.46 |
| 全体 | 2,213 | 0.28 | 1.51 |

- **除外**: Mono の LLVM が処理を消した5件を Mono 比の平均から除外している。含めると 0.29。[確認 2026-11-19] — [performance.md](https://github.com/zion-sati/NetWasm/blob/main/docs/performance.md)
- **ソート関連のケース（ns）**: **[ベンダー主張]** [確認 2026-11-19] — [micro-benchmarks.md](https://github.com/zion-sati/NetWasm/blob/main/docs/micro-benchmarks.md)

| ケース | NetWasm | Mono AOT | desktop JIT | / Mono | / JIT |
| --- | ---: | ---: | ---: | ---: | ---: |
| `Span.Sorting.QuickSortSpan(Size=512)` | 649,014 | 2,663,693 | 449,761 | 0.24 | 1.44 |
| `Span.Sorting.QuickSortArray(Size=512)` | 631,918 | 2,661,130 | 452,382 | 0.24 | 1.40 |
| `Span.Sorting.BubbleSortSpan(Size=512)` | 789,349 | 2,944,162 | 579,902 | 0.27 | 1.36 |
| `Span.Sorting.BubbleSortArray(Size=512)` | 775,244 | 2,778,271 | 578,838 | 0.28 | 1.34 |
| `Benchstone.BenchI.QuickSort.Test()` | 6,665 | 6,062 | 5,719 | 1.10 | 1.17 |
| `Benchstone.BenchI.HeapSort.Test()` | 137,492 | 167,264 | 209,239 | 0.82 | 0.66 |
| `Benchstone.BenchI.BubbleSort.Test()` | 11,137 | 14,018 | 10,563 | 0.79 | 1.05 |
| `Benchstone.BenchI.TreeSort.Test()` | 447,217 | 472,870 | 278,966 | 0.95 | 1.60 |
| `ByteMark.BenchNumericSortJagged()` | 695,756,833 | 715,169,292 | 884,158,417 | 0.97 | 0.79 |

- **独自ワークロードの quicksort**: **[ベンダー主張]** 100,000 個の整数をインプレースでソートし、ラウンドごとに seed データから再充填する。中央値は NetWasmGC 53.9 ms、Compact 53.5 ms、Mono AOT 59.7 ms、desktop JIT 48.7 ms、NativeAOT 48.6 ms。[確認 2026-11-19] — [performance.md](https://github.com/zion-sati/NetWasm/blob/main/docs/performance.md)
- **方法論（独自ワークロード）**: 各プログラムが `Stopwatch` で自己計測し、3回のウォームアップ後に15回を交互実行、2コホートの中央値の小さい方を採用する。チェックサムは desktop .NET 11 と一致を要求する。[確認 2026-11-19] — [performance.md](https://github.com/zion-sati/NetWasm/blob/main/docs/performance.md)
- **割り当て中心のワークロード**: **[ベンダー主張]** Full transaction は NetWasmGC 27.3 ms、Mono AOT 57.4 ms、JIT 51.1 ms、NativeAOT 33.7 ms。ベンダー自身が、desktop に対する優位は割り当てと回収に由来すると説明している。[確認 2026-11-19] — [performance.md](https://github.com/zion-sati/NetWasm/blob/main/docs/performance.md)
- **非割り当てのワークロード**: **[ベンダー主張]** preallocated transaction と quicksort では desktop .NET が両 Wasm ビルドより速い。[確認 2026-11-19] — [performance.md](https://github.com/zion-sati/NetWasm/blob/main/docs/performance.md)
- **比較子に関するケース**: **[ベンダー主張]** `Devirtualization.EqualityComparer.ValueTupleCompare()` は NetWasm 16.7 ns、Mono AOT 211.8 ns、JIT 3.1 ns（JIT の 5.40 倍）。[確認 2026-11-19] — [micro-benchmarks.md](https://github.com/zion-sati/NetWasm/blob/main/docs/micro-benchmarks.md)
- **ブラウザでの結果**: **[ベンダー主張]** DoomSharp の headless Chromium で NetWasm 1,750 ms、Blazor WebAssembly AOT 2,236 ms（1.28 倍速い）。Wasmtime では NetWasmGC 1,580 ms、Mono AOT 2,510 ms、JIT 1,054 ms、NativeAOT 1,192 ms。[2026-11-15] — [Micro-benchmarks](https://www.netwasm.com/micro-benchmarks/)
- **ベンダーが認める残差**: vector 系で desktop との差が残るのは、desktop がホストの SIMD を使い、Wasm は 128-bit 固定のためと説明している（0.10 時点の記述）。[2026-11-05] — [NetWasm 0.10 blog](https://www.netwasm.com/blog/compiler-optimization/)
- **計測の安定性**: 関数を 4 KiB 境界に揃えて A/B 比較している。128 バイト境界では同一ループが 1.37 倍遅くなる例があったとしている。[2026-11-05] — [NetWasm 0.10 blog](https://www.netwasm.com/blog/compiler-optimization/)
- **第三者の記事**: byteiota の記事（署名は ByteBot）は NetWasm の公表値の紹介で、独自の計測はない。[2026-10-24] — [byteiota](https://byteiota.com/netwasm-csharp-webassembly/)
- **Hacker News の議論**: 161 points のスレッドで、Rust との比較は C# 側だけ Binaryen の後処理を受けており不公平だとの批判が出た。サイズの議論が中心で、速度の再現報告は確認できなかった。[2026-10-21] — [HN](https://news.ycombinator.com/item?id=50175510); [HN API](https://hn.algolia.com/api/v1/items/50175510)
- **サイズ比較の更新**: 現行の文書では Rust 側にも手動 `wasm-opt` の条件を載せ、WASI 0.2 の Rust サイズプロファイル + wasm-opt は 41,185 B としている。[確認 2026-11-19] — [size-and-methodology.md](https://github.com/zion-sati/NetWasm/blob/main/docs/size-and-methodology.md)

### Inferences
- quicksort の公表値から計算すると、NetWasmGC は Mono AOT より約 10% 短く、desktop JIT の約 1.11 倍の時間である（53.9/59.7、53.9/48.7）。リポジトリ説明文の「3倍超」はマイクロベンチ全体の幾何平均に近い数字で、ソート処理にそのまま当てはめられない。
- 同じ quicksort 系でも `Span.Sorting` は Mono の 0.24 倍、`Benchstone.BenchI.QuickSort` は 1.10 倍と大きく異なる。Mono 側の特定コードの遅さが比に影響している可能性があるが、原因は公表資料からは分からない。
- 公表値の大半は Wasmtime（Cranelift）上の値で、V8、SpiderMonkey、JavaScriptCore の結果ではない。ブラウザや Node での順位は別に測る必要がある。
- desktop JIT は tiering 無効で計測しており、既定の tiered/PGO 構成の .NET とは条件が異なる。
- `Span.IndexerBench.Indexer1` で JIT の 0.09 倍など極端に速い結果は、加算ループのベクトル化の効果と考えられ、分岐の多いソートの指標にはならない。
- `ValueTupleCompare` の結果は `EqualityComparer` の devirtualization を測るもので、struct generic の `TComparer` とは別物である。比較子の性能は本リポジトリの構成で直接測る必要がある。

### Gaps
- 第三者による性能の再現報告は見つからなかった。Reddit は検索対象にできなかった。
- V8/Node でのマイクロベンチ全体の結果は公表されていない。
- struct generic の比較子・コンテキストを使うソートの公表ベンチマークは見つからなかった。
- x86-64 ホストでの結果は確認できなかった（公表値は Apple M1 Max）。

## `NETWASM` シンボルと 0.13.0 以降の変更

### Takeaway
0.13.0 より新しい版は存在し、最新は 0.14.2（2026-11-15）である。`NETWASM` シンボルは 0.14.2 でも `netwasm0.1` TFM で定義され、仕様書の記述は引き続き成立する。性能はほぼ不変だが、0.14.0 に破壊的変更があり、`netwasm0.1` ライブラリの再ビルドが必要である。

### Cited Findings
- **本リポジトリの記述**: `NETWASM` でコンパイルしたときだけ `SortSpan` の比較報告ヘルパー4つに aggressive inlining を要求し、SDK のアップグレード時に再検証すると定めている。[確認 2026-11-19] — [performance.md](../../.github/docs/specs/performance.md); [SortSpan.cs](../../src/SortAlgorithm/Algorithms/SortSpan.cs)
- **シンボル定義**: `TargetFramework` が `netwasm0.1` のとき `DefineConstants` に `NETWASM;NETWASM0_1` が追加される。v0.13.0 と v0.14.2 の両方で確認。[確認 2026-11-19] — [v0.14.2 Tfm.props](https://github.com/zion-sati/NetWasm/blob/v0.14.2/src/NetWasm.Sdk/Sdk/NetWasm.Sdk.Tfm.props#L49); [v0.13.0 Tfm.props](https://github.com/zion-sati/NetWasm/blob/v0.13.0/src/NetWasm.Sdk/Sdk/NetWasm.Sdk.Tfm.props#L44)
- **0.13.0 [2026-11-13]**: 型名をプログラムから外して既定で `type#<id>` に変更（破壊的）。型シンボル・メソッドシンボルのサイドカーを導入。`delegate* unmanaged` に対応。async 周りのサイズを削減。 — [Releases](https://github.com/zion-sati/NetWasm/releases)
- **0.14.0 [2026-11-14] の破壊的変更**: `NetWasm.System.*` パッケージを廃止し、フレームワーク全体を参照する形に変更。`EventSourceSupport`、`MetricsSupport`、`HttpActivityPropagationSupport` の既定を false に、`UseSystemResourceKeys` の既定を true に変更。 — [Releases](https://github.com/zion-sati/NetWasm/releases)
- **0.14.0 のその他**: .NET Standard パッケージの restore に対応、`ArrayPool<T>` を .NET 実装へ、`NoInlining` メソッドを呼び出しのまま維持。`netwasm0.1` ライブラリは 0.14 向けに再ビルドが必要。 — [Releases](https://github.com/zion-sati/NetWasm/releases); [0.14.0 release notes](https://github.com/zion-sati/NetWasm/blob/main/docs/releases/0.14.0.md)
- **0.14.0 の性能とサイズ**: **[ベンダー主張]** 2,213 件の幾何平均は 0.13.0 比 1.001。print-42 は 28,377 B から 28,383 B。 — [Releases](https://github.com/zion-sati/NetWasm/releases)
- **0.14.1 [2026-11-15]**: O3 のインライン化で関数が上限を超える場合の再構築を追加。全関数が上限内なら 0.14.0 と同一バイト列。 — [Releases](https://github.com/zion-sati/NetWasm/releases)
- **0.14.2 [2026-11-15]**: Bump GC で `ArrayPool<T>.Shared` を使うと `NWPACK001` で失敗する問題を修正。Compact と NetWasmGC は変更なし。 — [Releases](https://github.com/zion-sati/NetWasm/releases)

### Inferences
- `src/SortAlgorithm` は `ArrayPool<T>` を広く使っているので、Bump GC を試す場合は 0.14.2 以上が前提になる。
- 0.14.x は幾何平均では 0.13.0 と同等だが、`NoInlining` の扱いと O3 のインライン化経路に変更があるため、`SortSpan` の `#if NETWASM` の効果は 0.14.2 で再計測する価値がある。`SortSpan.cs` のコメントは 0.13 を名指ししている。
- リリース間隔が数時間から数日と短いので、計測結果には正確なパッケージ版を併記する必要がある。

### Gaps
- `#if NETWASM` の inlining ヒントが 0.14.2 で速度にどう影響するかの公表値はない（実測タスクの範囲）。
- 0.14.2 より後の版は、確認時点の NuGet インデックスには存在しなかった。
