# NativeAOT-LLVM の今後の見通し（SortVivo 本番採用の判断材料、確認日 2026-11-19）

前提: 既存ノート [nativeaot_llvm.md](../NET%2011%20WASM%20実行方式監査/nativeaot_llvm.md)（使い方・機能・性能・制約）と [coreclr_wasm.md](../NET%2011%20WASM%20実行方式監査/coreclr_wasm.md)（CoreCLR WASM の現状）で確定した事項は繰り返さず、将来に関わる点だけを扱う。

凡例:
- **merged** = マージ済み、**in-progress** = open の PR / issue、**planned** = チェックリスト上の未着手項目。
- 「推論」と書いた箇所は一次情報からの推測で、断定ではない。
- 日付はすべて UTC。ライブ確認（GitHub API、feed、Azure Artifacts API）は 2026-11-19 に実施した。ビルドと実行はしていない。

## 0. 結論の要約

- **上流は NativeAOT-LLVM の後継に当たる系統を作っている。ただし LLVM は使っていない。** dotnet/runtime main では、RyuJIT の Wasm バックエンドと ILC 自身の Wasm object writer で NativeAOT WASM を構築中である。ランタイム部分は NativeAOT-LLVM から移植している。2026-11-17 時点でリンクできるのは WASI だけで、browser へのリンクはビルド定義がエラーにする。milestone は 12.0.0（期日 2027-11-09）。
- **NativeAOT-LLVM の上流追従は止まっている。** 最後の上流取り込みは 2025-12-04 マージ。2026-04 の取り込み PR は 2026-08-09 に未マージのまま閉じられた。Emscripten を 3.1.56 から上げる PR は存在しない。feed への最終公開は 2026-07-08 である。
- **継続・凍結・廃止の明言は見つからなかった。** runtimelab の README は今も「Active Experimental Projects」に載せている。一方、主要コントリビューターの 2026 年後半の活動は、上流の RyuJIT Wasm へのレビューが中心である。
- **サポートはない。** runtimelab のパッケージは本番利用を .NET チームがサポートせず、安定版もサービシング修正も出ない（2020 年の方針）。パッケージは本物の署名を経ずに公開される設定になっている。feed に保持ポリシーは設定されておらず、2020 年の版も残っている。
- **採用するなら「捨てられる境界層」として入れるべきである（推論）。** C# 側の `[UnmanagedCallersOnly]` / `[JSExport]` の宣言は移行後も概ね残せる。一方、Emscripten 固有の MSBuild 設定と JS グルーは、どのシナリオでも書き直しになる可能性が高い。

## 1. 上流 NativeAOT WASM（dotnet/runtime#132798）

### Takeaway
上流の NativeAOT WASM は **RyuJIT Wasm バックエンド + ILC の Wasm object writer + NativeAOT-LLVM から移植したランタイム**という構成である。2026-10〜11 に WASI 向けで大量にマージが進み、Hello World・例外処理・ファイナライザ・スタックトレース・SIMD まで入った。browser 向けのリンクは未対応で、`[JSExport]` の検証も未完了である。

### Cited Findings

**追跡 issue の状態（2026-11-19 確認）**
- 起票 2026-08-26（jtschuster）、本文の最終更新 2026-10-01、open、milestone `12.0.0`、ラベル `arch-wasm` / `area-NativeAOT-coreclr` — [dotnet/runtime#132798](https://github.com/dotnet/runtime/issues/132798)
- milestone `12.0.0` の期日は 2027-11-09 — [milestone 207](https://github.com/dotnet/runtime/milestone/207)
- チェック済みはサブ項目 [#132812](https://github.com/dotnet/runtime/issues/132812)（AssemblyStubNodes、2026-10-01 close）のみ。サブ issue [#134206](https://github.com/dotnet/runtime/issues/134206)（import のシグネチャ、2026-09-18 起票）は open — [#132798](https://github.com/dotnet/runtime/issues/132798)
- **planned（未チェック、2026-10-01 時点の本文）**: Linking セクション出力、portable entry point の JIT フラグ判定、CLR WASM の EH/GC が NativeAOT で使えるかの判断、CoreLib とランタイムのビルド通過、`[JSExport]` 体験の検証、バイナリ形式の型の整理、LLVM 実験ブランチとの性能・サイズ比較（低優先度） — [#132798](https://github.com/dotnet/runtime/issues/132798)

**コード生成器: RyuJIT Wasm（LLVM ではない）**
- [#131556](https://github.com/dotnet/runtime/pull/131556)（agocke、2026-07-29 作成、2026-10-14 に未マージで close）は、ILC から WebAssembly RyuJIT と object writer を通す単一メソッドのコンパイル経路を説明している
- [#135761](https://github.com/dotnet/runtime/pull/135761)（2026-10-15 merged）は `-component wasmjit` でビルドし、NativeAOT の try 区間を CoreCLR の `try_table` ではなく通常の Wasm `block` で出す
- オブジェクト再配置の解決は [#133537](https://github.com/dotnet/runtime/pull/133537)（2026-09-15 merged）
- WASI のリンクは WASI SDK の `clang` と `llvm-ar` を使い、`--target=wasm32-wasip2` を渡す。Emscripten は使わない — [Microsoft.NETCore.Native.Wasm.targets @3ca7cb218f](https://github.com/dotnet/runtime/blob/3ca7cb218f1f2f244dc69a5c9ad774097883bb59/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.Wasm.targets)
- 上流の ILC 設計文書は、LLVM バックエンドを NativeAOT-LLVM ブランチにあるものとして記述したまま — [ilc-architecture.md](https://github.com/dotnet/runtime/blob/main/docs/design/coreclr/botr/ilc-architecture.md)

**ランタイムは NativeAOT-LLVM から移植**
- [#126633](https://github.com/dotnet/runtime/pull/126633)（MichalStrehovsky、2026-04-08 作成）は、NativeAOT-LLVM の WASM ランタイムを上流へ移す PR。2026-09-28 に未マージで close され、小さな単位で進めると述べて [#134781](https://github.com/dotnet/runtime/pull/134781) に置き換えた
- [#134781](https://github.com/dotnet/runtime/pull/134781)（2026-09-29 merged）は Browser/WASI 下でネイティブランタイムをビルドする最小差分

**merged（WASI を中心に、日付順）**

| マージ日 | PR | 内容 |
|---|---|---|
| 2026-09-07〜09-16 | [#133248](https://github.com/dotnet/runtime/pull/133248)、[#133631](https://github.com/dotnet/runtime/pull/133631)、[#133739](https://github.com/dotnet/runtime/pull/133739) | jump stub、unboxing stub、ReadyToRunHelperNode |
| 2026-10-13〜10-21 | [#135666](https://github.com/dotnet/runtime/pull/135666)、[#135760〜#135769 系列](https://github.com/dotnet/runtime/pull/135765)、[#135763](https://github.com/dotnet/runtime/pull/135763) | WASI で `Console.WriteLine` の Hello World を動かすための一連の変更、インターフェイスディスパッチ |
| 2026-10-15 | [#134210](https://github.com/dotnet/runtime/pull/134210) | スタブ版 CoreLib のビルド |
| 2026-10-21 | [#135955](https://github.com/dotnet/runtime/pull/135955) | WASI 専用のビルド統合。browser のリンク対応は含まない、と本文に明記 |
| 2026-10-29 | [#136564](https://github.com/dotnet/runtime/pull/136564) | `WasmImportLinkage` 対応 |
| 2026-10-31 | [#136511](https://github.com/dotnet/runtime/pull/136511) | WASI での例外処理 |
| 2026-11-01〜11-02 | [#136784](https://github.com/dotnet/runtime/pull/136784)、[#136680](https://github.com/dotnet/runtime/pull/136680) | perf tracing、スタックトレース |
| 2026-11-05 | [#136682](https://github.com/dotnet/runtime/pull/136682) | ファイナライザ（`TARGET_BROWSER` 用のフックも含む） |
| 2026-11-07〜11-08 | [#137058](https://github.com/dotnet/runtime/pull/137058)、[#137074](https://github.com/dotnet/runtime/pull/137074)、[#136999](https://github.com/dotnet/runtime/pull/136999) | シンボル除去の既定化、dehydration、成果物名の整理 |
| 2026-11-10 | [#137001](https://github.com/dotnet/runtime/pull/137001) | WASI スモークテストを Helix で実行 |
| 2026-11-15 | [#137112](https://github.com/dotnet/runtime/pull/137112) | NativeAOT の CoreLib が Wasm intrinsics をコンパイルし、NativeAOT ABI 下で ISA 対応がコンパイル時定数になる。WASI は relaxed SIMD 既定オン |
| 2026-11-17 | [#137694](https://github.com/dotnet/runtime/pull/137694) | NativeAOT の WASI リンクに wasi:http の component type を渡す（#132798 に関連付け） |

**in-progress（2026-11-19 時点 open）**
- [#137811](https://github.com/dotnet/runtime/pull/137811)（2026-11-18）は `BUILD_LIBS_NATIVE_BROWSER` を削除する。本文は、この回避策を残すと native AOT 向けに伝播させる必要があると述べている
- その他の open issue: EventPipe [#136806](https://github.com/dotnet/runtime/issues/136806)、スタックトレースの行番号 [#136676](https://github.com/dotnet/runtime/issues/136676)、単一スレッドでの `Interlocked` のコード品質 [#137061](https://github.com/dotnet/runtime/issues/137061)（milestone 12.0.0）
- JSExport 登録 API の提案 [#136464](https://github.com/dotnet/runtime/issues/136464)（2026-10-26 起票、open）

**browser の状態**
- main のビルド定義は `_targetOS` が `wasi` 以外だとエラーにする。共有ライブラリと multi-module もエラー — [Wasm.targets @3ca7cb218f](https://github.com/dotnet/runtime/blob/3ca7cb218f1f2f244dc69a5c9ad774097883bb59/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.Wasm.targets)
- それでも nightly feed には `Microsoft.NETCore.App.Runtime.NativeAOT.browser-wasm` と `.wasi-wasm` があり、各 22 版。初版は 2026-11-10、最新は `12.0.0-alpha.1.26568.115`（2026-11-19）。説明文は直接参照しない内部パッケージ — [browser-wasm index](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet12/nuget/v3/flat2/microsoft.netcore.app.runtime.nativeaot.browser-wasm/index.json)、[wasi-wasm index](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet12/nuget/v3/flat2/microsoft.netcore.app.runtime.nativeaot.wasi-wasm/index.json)、[nuspec](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet12/nuget/v3/flat2/microsoft.netcore.app.runtime.nativeaot.browser-wasm/12.0.0-alpha.1.26568.115/microsoft.netcore.app.runtime.nativeaot.browser-wasm.nuspec)、[公開日（Azure Artifacts API）](https://feeds.dev.azure.com/dnceng/public/_apis/packaging/feeds/dotnet12/packages?packageNameQuery=Microsoft.NETCore.App.Runtime.NativeAOT&includeAllVersions=true&protocolType=NuGet&api-version=7.1)
- 上流に NativeAOT WASM の利用手順文書は無い。`docs/workflow/building/coreclr/` にあるのは `wasm.md` と `wasi-r2r.md`（CoreCLR 用）だけ — [ディレクトリ](https://github.com/dotnet/runtime/tree/main/docs/workflow/building/coreclr)

**NativeAOT-LLVM を置き換えるのかについての発言（言い換え）**
- 2026-04-08、MichalStrehovsky（[#126633](https://github.com/dotnet/runtime/pull/126633)）: NativeAOT-LLVM の WASM ランタイムを上流へ持ってくる。方針は SingleAccretion の提案に合わせた。コミットを分けたのは、NativeAOT-LLVM ブランチ側が上流を取り込むときの衝突を減らすため。
- 同 PR のレビュー（2026-04-08）: SingleAccretion は、C++ 例外方式は捨ててよいが、互換性のため emulated 方式は残すべきだと述べた。jkotas は、実験用の手早い実装（割り当て時のインラインのファイナライザ実行）は上流では戻すべきだと指摘した — [#126633 のレビューコメント（Files タブ）](https://github.com/dotnet/runtime/pull/126633/files)
- 2026-08-26、#132798 の本文: NativeAOT-LLVM ブランチを関連資料として挙げ、それとの性能・サイズ比較を低優先度の項目にしている — [#132798](https://github.com/dotnet/runtime/issues/132798)
- 2026-10-26、jkoritzinsky（[#136464](https://github.com/dotnet/runtime/issues/136464)）: NativeAOT-LLVM は JSExport generator を UCO ヘルパーを出す形に変えていると指摘し、管理シグネチャの設計が、NativeAOT に WASM 対応を足すときにも通用するかを確認した。2026-10-27、pavelsavara は互換だと思うと答え、UCO 化は性能の裏付けが出たら後で検討すると述べた。
- **「NativeAOT-LLVM を置き換える」「アーカイブする」と明言した一次情報は見つからなかった**（runtimelab と runtime の issue / PR を検索した範囲）。
- 実験が製品化するときの一般的な手順は 2020-04-29 の方針文書にある。コードを dotnet/runtime へ手作業で写し、runtimelab 側からは削除する — [dotnet/runtime#35609](https://github.com/dotnet/runtime/issues/35609)

### Inferences
- 上流の系統は、ランタイム（shadow stack、EH モデル、GC 連携）を NativeAOT-LLVM から受け継ぎ、コード生成器だけを RyuJIT Wasm に差し替えたものと読める。#35609 の手順と合わせると、事実上の後継である。
- #132798 のチェックリストは 2026-10-01 から更新されていない。CoreLib のスタブ化や WASI のリンクなど、本文より後にマージされた作業があるので、進捗はチェックリストより先行している。
- browser 向けは、ファイナライザのフック（#136682）と #137811 の整理が始まった段階である。WASI と同程度に動くまでには、ホスト・JS 連携・JSExport の作業が残る。

### Gaps
- browser 向け NativeAOT のリンクに Emscripten を使うのか、CoreCLR browser の `browserhost` / `dotnet.js` に寄せるのかを示す文書や PR は見つからなかった。
- `[UnmanagedCallersOnly(EntryPoint=...)]` を Wasm の export にする上流の経路（WASI component の export を含む）は確認できなかった。共有ライブラリは明示的に未対応である。
- `.NET 12` で NativeAOT WASM を出荷するという公式声明は無い。milestone だけが根拠である。

## 2. NativeAOT-LLVM の上流追従、Emscripten 更新、2026-06 以降の活動

### Takeaway
.NET 11 以降の main を取り込む PR は 1 本あったが、2026-08-09 に未マージで閉じられた。Emscripten の更新 PR は無い。2026-07-07 より後の製品コードの変更はゼロで、open の外部 PR 3 本（PGO の NRE 修正を含む）にもレビューが付いていない。

### Cited Findings

**上流取り込みの履歴**

| PR | 作成 | 結果 | 備考 |
|---|---|---|---|
| [#3160](https://github.com/dotnet/runtimelab/pull/3160) Merge sep 25 | 2025-09-03 | 2025-11-02 merged | |
| [#3206](https://github.com/dotnet/runtimelab/pull/3206) Merge nov25 | 2025-11-08 | 2025-12-04 merged | 最後に成功した取り込み |
| [#3261](https://github.com/dotnet/runtimelab/pull/3261) WIP Merge apr26 | 2026-04-30 | 2026-08-09 未マージで close | draft、100 コミット。上流 main の 2026-04-23 頃のコミットを含む。close 時に yowl は "Will have a rethink, this is just taking too much time." と書いた |

- 2024 年は 1〜2 か月ごとに取り込んでいた（例: [#2688](https://github.com/dotnet/runtimelab/pull/2688)、[#2730](https://github.com/dotnet/runtimelab/pull/2730)、[#2762](https://github.com/dotnet/runtimelab/pull/2762)）。2025-12-04 以降は 11 か月以上取り込みが無い — [PR 一覧](https://github.com/dotnet/runtimelab/pulls?q=is%3Apr+base%3Afeature%2FNativeAOT-LLVM+Merge+in%3Atitle)
- 上流との衝突回避: SingleAccretion が [#3202](https://github.com/dotnet/runtimelab/issues/3202)（2025-11-05 起票）で、上流の WASM Jit と名前がぶつからないよう `wasmjit` を `llvmjit` に改名すると提案した。[#3228](https://github.com/dotnet/runtimelab/pull/3228)（yowl、2026-01-28 merged）で実施
- 上流ブランチの一覧に、取り込み用の別ブランチは無い。`feature/NativeAOT-LLVM` だけである（GitHub API、2026-11-19）。yowl の fork の `merge-apr26` の最終コミットは 2026-08-09 — [yowl/runtimelab:merge-apr26](https://github.com/yowl/runtimelab/tree/merge-apr26)

**Emscripten**
- 先頭 `527a565752`（2026-09-22）でも CI は `3.1.56` を固定している — [install-emscripten.ps1](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/eng/pipelines/runtimelab/install-emscripten.ps1)
- Emscripten 3.1.56 のリリースは 2024-03-14。最新は 7.0.0（2026-11-06）。上流 dotnet/runtime は 6.0.2（2026-07-01） — [Emscripten ChangeLog](https://github.com/emscripten-core/emscripten/blob/main/ChangeLog.md)、[runtime Versions.props](https://github.com/dotnet/runtime/blob/main/eng/Versions.props)
- 3.1.56 を超える更新の issue / PR は無い。関連するのは次の 2 つだけ:
  - [#3273](https://github.com/dotnet/runtimelab/issues/3273)（2026-06-24）。SingleAccretion は同日、Emscripten を上げるときに直す、当面は古い版だけ対応と回答した
  - [#2472](https://github.com/dotnet/runtimelab/issues/2472)（取り込み後の作業リスト、2026-03-16 更新）。次の Emscripten（> 4.0.0）更新時に `WASM_BIGINT` などの条件を外す、という項目がある

**2026-06 以降の issue / PR（NativeAOT-LLVM 関連のみ。Satori、ZeroGC、gc 系ブランチと arcade の自動更新は除外）**

| 番号 | 種別 | 作成 | 状態（2026-11-19） | 内容とメンテナの反応 |
|---|---|---|---|---|
| [#3333](https://github.com/dotnet/runtimelab/pull/3333) | PR | 2026-11-12 | open | 診断ログの復元（zbalkan）。レビューなし |
| [#3332](https://github.com/dotnet/runtimelab/pull/3332) | PR | 2026-11-11 | open | publish レイアウトのテスト（zbalkan）。レビューなし |
| [#3329](https://github.com/dotnet/runtimelab/issues/3329) | issue | 2026-11-02 | open | `ImmutableArray<T>` で ILC がクラッシュ。jkotas がラベル付けのみ |
| [#3319](https://github.com/dotnet/runtimelab/pull/3319) | PR | 2026-10-18 | open | PGO の NRE 修正（dsherret）。レビューなし |
| [#3307](https://github.com/dotnet/runtimelab/pull/3307) | PR | 2026-09-22 | merged 2026-09-22 | 公開対象ファイルの整理（akoeplinger、全ブランチ共通の基盤変更）。マージは jkotas |
| [#3261](https://github.com/dotnet/runtimelab/pull/3261) | PR | 2026-04-30 | closed 2026-08-09 | 上流取り込み。未マージ |
| [#3279](https://github.com/dotnet/runtimelab/issues/3279) | issue | 2026-07-14 | open | osx-arm64 ホストパックの公開要望。ラベルのみ |
| [#3277](https://github.com/dotnet/runtimelab/pull/3277) | PR | 2026-07-06 | merged 2026-07-07 | WASI の async Main（lewing） |
| [#3274](https://github.com/dotnet/runtimelab/pull/3274) | PR | 2026-06-24 | merged 2026-07-05 | `cpobj` の write barrier（SingleAccretion）。[#3271](https://github.com/dotnet/runtimelab/issues/3271)、[#3268](https://github.com/dotnet/runtimelab/issues/3268) を解決 |
| [#3273](https://github.com/dotnet/runtimelab/issues/3273) | issue | 2026-06-24 | open | `emcc.bat`。上記の回答のみ |
| [#3272](https://github.com/dotnet/runtimelab/pull/3272)、[#3270](https://github.com/dotnet/runtimelab/pull/3270) | PR | 2026-06-19、06-16 | merged 2026-06-22 | 文書とサンプルの更新（外部） |

- 検索に使ったクエリ: [issue](https://github.com/dotnet/runtimelab/issues?q=is%3Aissue+updated%3A%3E%3D2026-06-01)、[PR](https://github.com/dotnet/runtimelab/pulls?q=is%3Apr+updated%3A%3E%3D2026-06-01)
- 最新パッケージと先頭の差: パッケージの出所コミット `9954350a58`（2026-07-07）と先頭 `527a565752` の差は `prepare-signed-artifacts.yml` の 1 ファイルだけ — [compare](https://github.com/dotnet/runtimelab/compare/9954350a58ede8b8eaaeb24112ca4f1e78cc527c...527a565752062b20bb9693eea6c884b34ecce7bf)
- 公開の間隔: `Microsoft.DotNet.ILCompiler.LLVM` の 2026 年の公開日は 02-04、02-11、02-15、02-18、06-06、06-22、07-05、07-08。2026-07-08 より後は無い — [Azure Artifacts API](https://feeds.dev.azure.com/dnceng/public/_apis/packaging/feeds/dotnet-experimental/packages?packageNameQuery=Microsoft.DotNet.ILCompiler.LLVM&includeAllVersions=true&protocolType=NuGet&api-version=7.1)
- 11.x 以上の `ILCompiler.LLVM` は `dotnet-experimental`、`dotnet11`、`dotnet12` のどの feed にも無い（`dotnet11` / `dotnet12` の flat index は 404） — [dotnet-experimental index](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/flat2/microsoft.dotnet.ilcompiler.llvm/index.json)

### Inferences
- NativeAOT-LLVM を .NET 11 以降へ載せ替えるには、#3261 の再挑戦か、別方式の取り込みが要る。いずれも 2026-11-19 時点で着手の痕跡は無い。
- Emscripten の更新は上流取り込みと一緒に行う前提（#2472、#3273）なので、取り込みが止まっている限り 3.1.56 のままと見るのが自然である。
- 2026-02-18 から 06-06 の間、#3241（03-12）や #3259（04-23）がマージされたのに公開が無い。公式ビルドが不安定だった時期と重なるが（[#3200](https://github.com/dotnet/runtimelab/issues/3200) は 2025-11〜2026-02 の公式ビルド障害）、2026 年春の原因は確認していない。

## 3. 継続・凍結・廃止についての発言と、現在の担い手

### Takeaway
継続・凍結・廃止のどれも、メンテナは明言していない。実際の作業は SingleAccretion と yowl のボランティア的な貢献が中心で、マージ権限は jkotas が担っている。2026 年後半、SingleAccretion の公開活動は上流の RyuJIT Wasm へのレビューに移っている。

### Cited Findings
- **プロジェクトの掲示**: runtimelab の既定ブランチ（`docs`）の README は、今も NativeAOT-LLVM を Active Experimental Projects に載せている（README の最終更新 2026-07-22、ZeroGC 追加） — [README](https://github.com/dotnet/runtimelab/blob/docs/README.md)、[履歴](https://github.com/dotnet/runtimelab/commits/docs/README.md)
- **ブランチの README**: 実験的な fork で、対応ターゲットは WebAssembly のみ。連絡先は C# Discord の `#allow-unsafe-blocks` チャンネル — [branch README](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/README.md)
- **CODEOWNERS** はブランチに無い（API で 404）
- **2025-11-19 以降の製品コミットの作者**: SingleAccretion（#3226、#3167、#3237、#3243、#3241、#3259、#3274）、yowl（#3206、#3219、#3228）、lewing（#3277）、外部（#3270、#3272） — [コミット一覧](https://github.com/dotnet/runtimelab/commits/feature/NativeAOT-LLVM)
- **マージした人**: #3206、#3219、#3241、#3259、#3270、#3272、#3274、#3307 は jkotas がマージ。#3277 は lewing の自己マージ — 各 PR（例: [#3274](https://github.com/dotnet/runtimelab/pull/3274)、[#3307](https://github.com/dotnet/runtimelab/pull/3307)）
- **公式ビルドの保守**: 2025-11〜2026-02 の公式ビルド障害（[#3200](https://github.com/dotnet/runtimelab/issues/3200)）では、SingleAccretion が jkotas に繰り返し確認を頼み、jkotas と hoyosjs がパイプラインのエラーを調べた。2026-02-05 に SingleAccretion が復旧を報告した。公式パイプラインの調査には Microsoft 側の手が要る体制と読める（推論）。
- **2026-07 以降の SingleAccretion の公開活動**（GitHub 検索、2026-11-19）:
  - dotnet/runtime では issue 17 件、PR 18 件にコメント。大半は RyuJIT Wasm と wasm R2R — [issue 検索](https://github.com/dotnet/runtime/issues?q=is%3Aissue+commenter%3ASingleAccretion+updated%3A%3E%3D2026-07-01)、[PR 検索](https://github.com/dotnet/runtime/pulls?q=is%3Apr+commenter%3ASingleAccretion+updated%3A%3E%3D2026-07-01)
  - runtimelab では PR 4 件のみ — [検索](https://github.com/dotnet/runtimelab/pulls?q=is%3Apr+commenter%3ASingleAccretion+updated%3A%3E%3D2026-07-01)
- **yowl の 2026-06 以降の発言**: macOS 対応への協力の申し出に、参考 PR を示して歓迎した（2026-06-01） — [#2119](https://github.com/dotnet/runtimelab/issues/2119)。#3261 は 2026-08-09 に見直すとして閉じた — [#3261](https://github.com/dotnet/runtimelab/pull/3261)
- **第三者の見方**: hez2010（コントリビューター）は 2025-03-28、コミュニティ主導で活発に開発中の実験的バックエンドだと説明した — [dotnet/runtime#113979](https://github.com/dotnet/runtime/issues/113979)
- **SingleAccretion の dotnet/runtime での関連付け**: `CONTRIBUTOR`（MEMBER ではない）と表示される — [#121141 のコメント](https://github.com/dotnet/runtime/issues/121141)

### Inferences
- 「凍結の宣言は無いが、事実上の停滞期にある」と見るのが妥当である。根拠は、上流取り込みの放棄（2026-08-09）、最終公開（2026-07-08）、外部 PR が無反応（最長で 2026-10-18 から 1 か月）、主要コントリビューターの関心が上流へ移っていること。
- 上流の NativeAOT WASM（§1）は NativeAOT-LLVM のランタイムと設計判断を引き継いでいる。主要コントリビューターがそちらを後押しする構図なので、NativeAOT-LLVM 単独の長期保守に資源が戻る可能性は低いと見る。

### Gaps
- C# Discord（`#allow-unsafe-blocks`）での議論は確認していない（アクセス手段なし）。方針の表明がそこにある可能性は残る。
- SingleAccretion / yowl の所属と、作業がどれだけ業務として担保されているかは、一次情報では確認できなかった。

## 4. ブラウザ向けソートアプリに効く既知問題の状況

### Takeaway
SortVivo に直結する未解決問題は 4 つある。(1) `OptimizationPreference=Speed` で起きる PGO の NRE（修正 PR はあるが未レビュー）、(2) Emscripten 3.1.56 固定に伴う Windows の `emcc.bat` 問題、(3) legacy Wasm EH、(4) コレクション式まわりのコンパイラ不具合。どれも 2026-11-19 時点で修正版パッケージは無い。

### Cited Findings

| 問題 | 状態（2026-11-19） | 根拠 |
|---|---|---|
| `getPgoInstrumentationResults` の NRE | **issue は無い**。修正 PR [#3319](https://github.com/dotnet/runtimelab/pull/3319)（2026-10-18、open、レビューなし）。原因は `LLVMCodegenCompilation` が base に `ProfileDataManager` として `null` を渡すこと。`.mibc` を与えると JIT がプロファイルを要求して例外になる、と PR が説明している | [LLVMCodegenCompilation.cs L48-L49](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/tools/aot/ILCompiler.LLVM/Compiler/LLVMCodegenCompilation.cs#L48-L49)、[検索 getPgoInstrumentationResults](https://github.com/dotnet/runtimelab/issues?q=getPgoInstrumentationResults) |
| `OptimizationPreference=Speed` との関係 | `Speed` で `IlcPgoOptimize=true` が既定になり、`$(IlcMibcPath)*.mibc` が `MibcFile` に入る | [Native.targets L33](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L33)、[L193](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L193)、[L206](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L206) |
| [#3273](https://github.com/dotnet/runtimelab/issues/3273) `emcc.bat` | open。Emscripten 更新時に直すという回答のみ（2026-06-24） | 同 issue |
| [#3329](https://github.com/dotnet/runtimelab/issues/3329) ILC クラッシュ | open（2026-11-02）。条件付きの `ImmutableArray<T>` ローカルに加え、`IReadOnlyList<int>` のコレクション式で spread する形でも落ちると追記された。報告者は、NetWasm コンパイラが実際に当たった形だと述べている。メンテナのコメントなし | 同 issue |
| [#3246](https://github.com/dotnet/runtimelab/issues/3246) spread で `InvalidProgramException` | open。2026-03-11 に SingleAccretion へ割り当て。その後コメントなし | [timeline](https://github.com/dotnet/runtimelab/issues/3246) |
| Span / struct コピーと GC | [#3271](https://github.com/dotnet/runtimelab/issues/3271)（入れ子 struct を含む `Span<T>`）、[#3268](https://github.com/dotnet/runtimelab/issues/3268)（`[InlineArray]`）、[#3257](https://github.com/dotnet/runtimelab/issues/3257) は修正済み。最新パッケージの出所 `9954350a58` は #3274 を含む | [compare](https://github.com/dotnet/runtimelab/compare/9954350a58ede8b8eaaeb24112ca4f1e78cc527c...527a565752062b20bb9693eea6c884b34ecce7bf)、[#3274](https://github.com/dotnet/runtimelab/pull/3274) |
| null check と byref | [#3127](https://github.com/dotnet/runtimelab/issues/3127)（一時的な不正 byref と null check のモデル、open、2025-07）、[#3128](https://github.com/dotnet/runtimelab/issues/3128)（open） | 各 issue |
| generics | [#2693](https://github.com/dotnet/runtimelab/issues/2693)（generic `IDynamicInterfaceCastable`、open、2024-09）。ソートの struct generic comparer に直接当たる open issue は見つからなかった | [検索](https://github.com/dotnet/runtimelab/issues?q=is%3Aissue+label%3Aarea-NativeAOT-LLVM+generic) |
| 例外 | JS に漏れた例外の情報欠落 [#2710](https://github.com/dotnet/runtimelab/issues/2710)、EH の残作業 [#2169](https://github.com/dotnet/runtimelab/issues/2169)。どちらも open で 2026 年の更新なし | 各 issue |
| legacy Wasm EH | browser の既定は `IlcLlvmExceptionHandlingModel=wasm`（`-fwasm-exceptions`）。exnref 用のフラグは渡していない | [Native.targets L135-L137、L425](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L135-L137) |
| （同上の背景） | `-sWASM_LEGACY_EXCEPTIONS` が入ったのは Emscripten 4.0.0（2025-01-14）。Firefox は legacy `try` 命令に非推奨の警告を出している（2025-07-30 報告）。上流 CoreCLR は exnref へ移行済み（2026-06-26） | [ChangeLog](https://github.com/emscripten-core/emscripten/blob/main/ChangeLog.md)、[dotnet/runtime#118200](https://github.com/dotnet/runtime/issues/118200)、[#129851](https://github.com/dotnet/runtime/pull/129851) |
| メモリ拡張 | `EmccMaximumHeapSize=16MB` で `unreachable` になる [#3150](https://github.com/dotnet/runtimelab/issues/3150)（open、2025-08-20 以降更新なし）。ほかに memory growth 専用の issue は無い | [検索](https://github.com/dotnet/runtimelab/issues?q=is%3Aissue+label%3Aarea-NativeAOT-LLVM+heap) |
| `DotNetJsApi` / `[JSExport]` | [#2434](https://github.com/dotnet/runtimelab/issues/2434)（dotnet.js 対応の追跡、2024-10-31 以降更新なし）、[#2626](https://github.com/dotnet/runtimelab/issues/2626)（クラスライブラリ内の JSExport 失敗）、[#2627](https://github.com/dotnet/runtimelab/issues/2627)（Mono 版 SDK との差異）。いずれも open | 各 issue |
| Web Worker / ES module | 該当する issue は見つからなかった。`DotNetJsApi` 経路は `EXPORT_ES6=1`、`MODULARIZE=1`、`ENVIRONMENT=web,webview,worker,node,shell` を設定し、`dotnet*.js` をパッケージから出力先へコピーする | [DotNetJsApi.targets L74-L77](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/DotNetJsApi.targets#L74-L77)、[L109-L116](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/DotNetJsApi.targets#L109-L116)、[検索 worker](https://github.com/dotnet/runtimelab/issues?q=is%3Aissue+label%3Aarea-NativeAOT-LLVM+worker) |

### Inferences
- **PGO の NRE の回避策（実機未検証）**: `OptimizationPreference=Speed` のまま `IlcPgoOptimize=false` を明示すれば、`MibcFile` が空になる。`--Ot` と LLVM の `-O3` を残したまま例外を避けられる見込みである（targets の条件式から読める）。#3319 の作者も、`MibcFile` が無い場合の出力は公開パッケージと同一だったと報告している。
- **legacy EH**: Emscripten 3.1.56 は `-sWASM_LEGACY_EXCEPTIONS` より前の版なので、browser の既定ビルドは legacy EH 命令（`try` / `catch`）を出すと考えられる。ブラウザが legacy EH を削除した時点で、既定ビルドは読み込めなくなるおそれがある。`IlcLlvmExceptionHandlingModel=emulated` に切り替えれば回避できる見込みだが、コードサイズが増える（#126633 のレビューで SingleAccretion がこの方式の欠点として挙げている）。
- `ImmutableArray<T>` やコレクション式の spread（#3246、#3329）は、ソート本体よりも周辺コード（UI 状態、記録データ構築）で使われがちである。採用時は、C# 側の境界層でこれらの構文を避ける規約が要る。

### Gaps
- browser の主要エンジンが legacy Wasm EH を削除する日付は確認できなかった。
- `runtime.browser-wasm.Microsoft.DotNet.ILCompiler.LLVM` に `.mibc` が同梱されているかは、nupkg をダウンロードしていないので確認していない。NRE が `Speed` で再現するという前提は、既存の計測での観測に依拠している。

## 5. 下流プロジェクトと、その見通し

### Takeaway
NativeAOT-LLVM の主な利用者は WASI 系（SpacetimeDB、componentize-dotnet、Spin 系）と、少数の browser 系（Bootsharp、HyperionEngine、cloudflare-workers-dotnet）である。上流への移行計画を公表している利用者は見つからなかった。Uno は依存しておらず、Avalonia も 2024 年の実験にとどまる。

### Cited Findings
- **GitHub コード検索**: `Microsoft.DotNet.ILCompiler.LLVM` を含むファイルは 468 件（検索インデックスの範囲）。主なリポジトリは下表のとおり — [検索](https://github.com/search?q=%22Microsoft.DotNet.ILCompiler.LLVM%22&type=code)

| プロジェクト | 用途 | 2026 年の状況 | 将来に関する記述 |
|---|---|---|---|
| [Bootsharp](https://github.com/elringus/bootsharp) | browser / Node 向けの C# ライブラリ化 | v0.8.0（2026-06-01）から Release で NativeAOT-LLVM が既定（[#190](https://github.com/elringus/bootsharp/pull/190)、2026-04-15 merged）。最新は v0.9.0（2026-06-09）、最終 push は 2026-06-11。`llvm.sh` は `10.0.0-rc.1.26117.1` を固定 | 上流移行や CoreCLR の記述はない。macOS 対応は上流（runtimelab）待ちと記載（[#196](https://github.com/elringus/bootsharp/issues/196)、2026-04-18 起票、open） |
| [SpacetimeDB](https://github.com/clockworklabs/SpacetimeDB/blob/master/crates/bindings-csharp/NATIVEAOT-LLVM.md) | WASI モジュール | .NET 10 対応を 2026-07-17 に追加、文書の更新は 2026-11-05。実験的と警告し、`10.0.0-*` を参照 | 移行計画の記述はない |
| [componentize-dotnet](https://github.com/bytecodealliance/componentize-dotnet) | WASI component | v0.8.0-preview00011（2026-06-12）、最終 push は同日 | 記述なし |
| [HyperionEngine](https://github.com/HyperionGameTech/HyperionEngine/blob/HEAD/Source/Engine/DotNET/Sdk/Hyperion.Web.targets) | ゲームエンジンの Web ターゲット | 2026-10-20 に追加。`10.0.0-rc.1.26357.1` を固定し、EH は `emulated` | 記述なし |
| [cloudflare-workers-dotnet](https://github.com/nenoNaninu/cloudflare-workers-dotnet) | Cloudflare Workers | README は、SDK が Wasm 向け NativeAOT を標準で持たないため実験的パッケージを使うと説明。最終 push は 2026-10-30 | 記述なし |
| [Avalonia](https://github.com/AvaloniaUI/Avalonia/issues/16211) | UI フレームワーク | 2024-07-03 起票の実験 issue（open、最終更新 2024-07-17）。本番依存ではない | — |
| Uno Platform | — | `unoplatform/uno` と `Uno.Wasm.Bootstrap` で NativeAOT-LLVM 依存は見つからなかった | — |

### Inferences
- browser 系の利用者は少なく、どれも個人か小規模チームの主導である。上流に圧力をかけられるほどの利用者層は見当たらない。
- 利用者は軒並み `10.0.0-rc.1.*` に固定するか `10.0.0-*` で追従している。NativeAOT-LLVM が止まれば一斉に影響を受けるが、移行先を示している利用者は無い。SortVivo が先行事例を参考にできる余地は小さい。

### Gaps
- 非公開プロジェクトや、検索インデックス外のリポジトリの利用状況は不明。

## 6. サポートとサプライチェーン

### Takeaway
ライセンスは MIT で問題ない。ただし、本番サポートもサービシングも無い実験的パッケージである。署名は本物の署名を経ない公開設定で、feed に保持ポリシーは設定されていない。古い版も残っているが、削除されない保証は無い。

### Cited Findings
- **ライセンス**: nuspec は `MIT`、リポジトリも MIT。第三者通知には LLVM などが含まれる — [nuspec](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/flat2/microsoft.dotnet.ilcompiler.llvm/10.0.0-rc.1.26357.1/microsoft.dotnet.ilcompiler.llvm.nuspec)、[LICENSE.TXT](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/LICENSE.TXT)、[THIRD-PARTY-NOTICES.TXT](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/THIRD-PARTY-NOTICES.TXT)
- **サポート方針（2020-04-29、jkotas）**: runtimelab のパッケージは本番利用を .NET チームがサポートせず、安定版もサービシング修正も出さない。nuget.org には公開しない — [dotnet/runtime#35609](https://github.com/dotnet/runtime/issues/35609)
- **共有 feed**: すべての実験が同じ `dotnet-experimental` feed に公開する — [CreateAnExperiment.md](https://github.com/dotnet/runtimelab/blob/docs/CreateAnExperiment.md)
- **feed の設定（2026-11-19 取得）**: `hideDeletedPackageVersions=true`、upstream source なし、プロジェクトは Public。保持ポリシーの API は `null` を返した — [feed](https://feeds.dev.azure.com/dnceng/public/_apis/packaging/feeds/dotnet-experimental?api-version=7.1)、[retentionpolicies](https://feeds.dev.azure.com/dnceng/public/_apis/packaging/feeds/dotnet-experimental/retentionpolicies?api-version=7.1)
- **古い版の残存（2026-11-19）**: `Microsoft.DotNet.ILCompiler.LLVM` は 294 版で、最古は `6.0.0-alpha.1.20606.1`（公開 2020-12-06）。どの版も listed — [Azure Artifacts API](https://feeds.dev.azure.com/dnceng/public/_apis/packaging/feeds/dotnet-experimental/packages?packageNameQuery=Microsoft.DotNet.ILCompiler.LLVM&includeAllVersions=true&protocolType=NuGet&api-version=7.1)
- **ホストパックの途絶**: `runtime.osx-x64.*` と `runtime.win-arm64.*` の最終版は `9.0.0-alpha.1.24173.1`（2024-03-24）、`runtime.linux-musl-x64.*` は `7.0.0-alpha.1.22567.1`（2022-11-18） — 同 API
- **署名**:
  - 2025-11-02、test-only 署名のパッケージで NuGet の NU3018 / NU3027 警告が出るとして、SingleAccretion が [#3200](https://github.com/dotnet/runtimelab/issues/3200) を起票した。runtimelab の署名は [#3184](https://github.com/dotnet/runtimelab/pull/3184)（2025-10-08）で無効化されている
  - 現行の公式パイプラインは build 段階で `enableSigning: false`、publish 段階で `isOfficialBuild: false` を渡す。`prepare-signed-artifacts.yml` は `isOfficialBuild` が false のとき `-sign` を付けず、`SignType=test` にする — [runtimelab-official.yml @527a565752](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/eng/pipelines/runtimelab-official.yml)、[prepare-signed-artifacts.yml @527a565752](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/eng/pipelines/official/jobs/prepare-signed-artifacts.yml)
- **フレームワークの出所**: `PublishAotUsingRuntimePack` が true でない場合、`IlcFrameworkPath` は NativeAOT-LLVM パッケージ内の `runtimes/<RID>/lib/` を指す — [Native.targets L183-L200](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L183-L200)
- **.NET 側のサービシング状況**: .NET 10 は 10.0.15（2026-11-10）、.NET 10 / 11 とも EOL は 2028-11-14 — [releases-index.json](https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json)

### Inferences
- 現行パッケージは Microsoft の本物の署名を持たない（未署名か test 署名）と見られる。社内ポリシーで `signatureValidationMode=require` や信頼済み署名者を要求している場合、そのままでは復元できない。
- 保持ポリシーが無く 2020 年の版も残っているので、自動削除で版が消える可能性は低い。ただし feed 管理者による手動削除や feed の再編を防ぐ保証は無い。本番では、使う版の nupkg を自前の feed かリポジトリに保管すべきである。
- アプリに組み込まれる BCL は、NativeAOT-LLVM パッケージに同梱されたもの（上流 main の 2025-11-08 時点のスナップショット）である。.NET 10.0.x のセキュリティ修正は流れ込まない。SortVivo はネットワークや解凍処理を持たないので影響は小さいが、監査では「BCL のパッチ水準は追えない」と記録する必要がある。

### Gaps
- nupkg 内の `.signature.p7s` は、ダウンロードしていないので直接確認していない。確定させるには `dotnet nuget verify --all` を実行する。
- `PublishAotUsingRuntimePack` が browser-wasm で既定 false かは、SDK 側の定義を確認していない。

## 7. シナリオ（すべて推論。日付つきの事実を前提にする）

### 判断の前提になる事実（再掲）

| 日付 | 事実 | 出典 |
|---|---|---|
| 2025-12-04 | NativeAOT-LLVM の最後の上流取り込み | [#3206](https://github.com/dotnet/runtimelab/pull/3206) |
| 2026-07-08 | NativeAOT-LLVM の最終パッケージ公開（`10.0.0-rc.1.26357.1`） | [Azure Artifacts API](https://feeds.dev.azure.com/dnceng/public/_apis/packaging/feeds/dotnet-experimental/packages?packageNameQuery=Microsoft.DotNet.ILCompiler.LLVM&includeAllVersions=true&protocolType=NuGet&api-version=7.1) |
| 2026-08-09 | 2026-04 の上流取り込み PR を未マージで close | [#3261](https://github.com/dotnet/runtimelab/pull/3261) |
| 2026-10-21 | 上流 NativeAOT WASM に WASI のビルド統合（browser は対象外） | [#135955](https://github.com/dotnet/runtime/pull/135955) |
| 2026-11-10 | 上流の `Microsoft.NETCore.App.Runtime.NativeAOT.browser-wasm` / `.wasi-wasm` が nightly feed に登場 | [dotnet12 feed](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet12/nuget/v3/flat2/microsoft.netcore.app.runtime.nativeaot.browser-wasm/index.json) |
| 2026-11-17 | 上流のビルド定義は wasi 以外のリンクをエラーにする | [Wasm.targets @3ca7cb218f](https://github.com/dotnet/runtime/blob/3ca7cb218f1f2f244dc69a5c9ad774097883bb59/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.Wasm.targets) |
| 2027-11-09 | milestone 12.0.0 の期日 | [milestone 207](https://github.com/dotnet/runtime/milestone/207) |
| 2028-11-14 | .NET 11（Mono が browser の既定）の EOL | [releases-index.json](https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json) |
| .NET 12（planned） | Mono WebAssembly パックの出荷停止と CoreCLR の既定化（epic の未完了項目） | [#134337](https://github.com/dotnet/runtime/issues/134337)、[#134397](https://github.com/dotnet/runtime/issues/134397) |

### (a) NativeAOT-LLVM が継続する
- **起こり方（推論）**: #3261 の再挑戦か別方式で上流 main（.NET 12 開発ライン）を取り込み、Emscripten を 6.x / 7.x に上げ、11.x か 12.x のパッケージを出す。
- **観測点**: 新しい「Merge」PR、Emscripten 更新 PR、#3319 / #3332 / #3333 へのレビュー、2026-07-08 より後のパッケージ公開。2026-11-19 時点ではどれも無い。
- **SortVivo への影響**:
  - 版は 10.0.0-rc.1 から一気に飛ぶ見込みで、TFM（`net10.0`）や BCL も同時に変わる。
  - Emscripten が 3.1.56 → 4.0.7 以降に上がると、`HEAP8` / `HEAP32` などの既定エクスポートが無くなる（既存ノート §4）。JS 側の線形メモリ参照は書き直しになる。
  - EH は exnref に移る可能性が高く、legacy EH の懸念は解消する方向である。
  - C# 側の `[UnmanagedCallersOnly]` export と `[JSExport]` は維持される見込み。
- **可能性の見立て（推論）**: 低い。主要な担い手の関心が上流へ移り、取り込みは「時間がかかりすぎる」として止まっている。

### (b) 上流の NativeAOT Wasm が .NET 12 で届く
- **起こり方（推論）**: WASI の経路（2026-11 時点で EH、ファイナライザ、スタックトレース、SIMD、wasi:http まで main に入った）に続いて、browser のホストと JS 連携が入り、`[JSExport]` の検証（#132798 の未完了項目）が終わる。
- **観測点**: `Microsoft.NETCore.Native.Wasm.targets` の `_targetOS != 'wasi'` エラーが外れること、browser 向け NativeAOT のスモークテスト、#132798 の `[JSExport]` 項目のチェック、利用手順の文書。
- **SortVivo への影響**:
  - パッケージ参照（`Microsoft.DotNet.ILCompiler.LLVM` + 実験 feed）と、`MSBuildEnableWorkloadResolver=false` などの回避設定は不要になる。標準 SDK の `PublishAot` 系に置き換わる見込みである。
  - コード生成器が LLVM から RyuJIT Wasm に変わるので、NativeAOT-LLVM で測った性能（LLVM の `-O3` に依存）は引き継げない。再計測が必要である。一方、上流は NativeAOT でも `PackedSimd` と `Vector128` のハードウェア実行に対応した（#137112）。NativeAOT-LLVM には無い利点である。
  - 上流 NativeAOT の WASI リンクは Emscripten を使わない。browser 向けが Emscripten を使うかは未定なので、Emscripten 固有のグルーは移植できない前提で考えるべきである。
- **可能性の見立て（推論）**: 中程度。WASI の進み方は速いが、2026-11-19 時点で browser 向けはほぼ未着手で、milestone 以外に出荷の約束は無い。.NET 12 で WASI だけが先行し、browser は後回しになる展開も十分ありうる。

### (c) どちらも起きない
- **起こり方（推論）**: NativeAOT-LLVM は `10.0.0-rc.1.26357.1` のまま止まり、上流の browser 向けは .NET 12 に間に合わない。
- **SortVivo への影響**:
  - パッケージは feed に残る見込み（§6）なので、固定版でビルドは続けられる。ただし、コンパイラの不具合（#3246、#3329、PGO の NRE）は自分で回避し続けることになる。Windows のビルド環境は Emscripten 3.1.56 を固定し続ける必要がある（#3273）。
  - browser が legacy EH を削除した時点で、既定ビルドは動かなくなるおそれがある。`emulated` へ切り替える準備が要る。
  - 退避先は、.NET 11 の Mono（2028-11-14 まで）か、.NET 12 の CoreCLR（インタープリタ / R2R、既存ノート coreclr_wasm.md）になる。
- **可能性の見立て（推論）**: (b) が browser に届かない場合の現実的な既定シナリオである。

### 互換性の見立て（推論）

| 要素 | NativeAOT-LLVM での形 | (a) 継続 | (b) 上流 NativeAOT | (c) 停滞 → Mono / CoreCLR へ退避 |
|---|---|---|---|---|
| `[UnmanagedCallersOnly(EntryPoint=...)]` の C# 宣言 | Emscripten の `_name` export になる | そのまま | C# 宣言は残せる。ただし export の経路は上流で未確認で、共有ライブラリは現在未対応 | C# 宣言は残せる。CoreCLR browser では、アプリ側の reverse thunk は再リンクで入る（既存ノート coreclr_wasm.md §4） |
| JS 側から export を呼ぶコード（`Module._sort`、`cwrap`、`HEAP32`） | Emscripten 3.1.56 の Module API | Emscripten 4.0.7 以降で `HEAP*` の扱いが変わり、修正が要る | 書き直し前提（ホスト未定） | 書き直し（`dotnet.js` の API へ） |
| `DotNetJsApi=true` + `[JSExport]` / `[JSImport]` | 独自の generator 差し替えと、パッケージ同梱の `dotnet*.js` | そのまま。取り込み時に `dotnet.js` も新しくなる | C# 側はそのまま使える見込み（#136464 で NativeAOT との互換が意識されている）。`DotNetJsApi` プロパティと生成物の形は変わる | C# 側はそのまま。JS 側の `dotnet.js` 起動コードは #2627 程度の差分を直す |
| `WasmImportLinkage` + `DllImport` | 対応済み | そのまま | 上流も対応済み（#136564） | Mono / CoreCLR 側の対応状況は別途確認が必要 |
| MSBuild 設定（`EmccFlags`、`LinkerArg -s...`、`IlcLlvmExceptionHandlingModel`、`IlcMaxLlvmModuleCount`、`MSBuildEnableWorkloadResolver=false`、実験 feed） | NativeAOT-LLVM 固有 | 一部見直し | 捨てる | 捨てる |
| 性能の前提 | LLVM の最適化、SIMD なし | 同等か改善 | RyuJIT Wasm、SIMD あり、要再計測 | CoreCLR R2R / Mono AOT の特性に戻る |

### 採用するなら（推論）
- export と JS グルーを 1 か所（Worker 内のアダプタ）に閉じ込め、`[JSExport]` を使う版と `[UnmanagedCallersOnly]` + ポインタ渡しの版を差し替えられるようにする。ソート本体（`SortAlgorithms`）は NativeAOT-LLVM 固有の API に触れないままにする。
- 版は `10.0.0-rc.1.26357.1` に固定し、nupkg を自前で保管する。Emscripten 3.1.56 と emsdk のコミット `ca7b40ae22` も固定する。
- `OptimizationPreference=Speed` は `IlcPgoOptimize=false` と組み合わせて検証する。EH は `wasm` と `emulated` の両方でビルドが通ることを CI で確認する。
- Mono（.NET 11）か CoreCLR（.NET 12 alpha）でのビルドを並行して維持し、退避先として常に動く状態にしておく。
- 2026-11-19 時点の観測点: (a) runtimelab の新しい Merge PR とパッケージ公開、(b) 上流 Wasm.targets の browser 対応と #132798 の `[JSExport]` 項目、legacy Wasm EH の削除予定。
