# Sort Algorithm Visualization Web App

ソートアルゴリズムの動作をリアルタイムでグラフィカルに可視化するBlazor WebAssemblyアプリケーション。

## 🎯 機能

### ✅ 実装済み機能

- **52種類のソートアルゴリズム対応**
  - Exchange Sorts (4種類): BubbleSort, CocktailShakerSort, CombSort, OddEvenSort
  - Selection Sorts (4種類): SelectionSort, DoubleSelectionSort, CycleSort, PancakeSort
  - Insertion Sorts (4種類): InsertionSort, BinaryInsertSort, ShellSort, GnomeSort
  - Merge Sorts (4種類): MergeSort, TimSort, PowerSort, ShiftSort
  - Heap Sorts (5種類): HeapSort, BottomupHeapSort, WeakHeapSort, SmoothSort, TernaryHeapSort
  - Partition Sorts (9種類): QuickSort, QuickSortMedian3/9, QuickSortDualPivot, BlockQuickSort, StableQuickSort, IntroSort, PDQSort, StdSort
  - Adaptive Sorts (1種類): DropMergeSort
  - Distribution Sorts (4種類): CountingSort, BucketSort, RadixLSD4Sort, RadixLSD10Sort
  - Network Sorts (3種類): BitonicSort, BitonicSortFill, BitonicSortParallel
  - Tree Sorts (2種類): BinaryTreeSort, BalancedBinaryTreeSort
  - Joke Sorts (3種類): BogoSort, SlowSort, StoogeSort

- **棒グラフ可視化**
  - SVGベースの高速レンダリング
  - 操作ごとの色分け（読み込み・書き込み・比較・スワップ）
  - レスポンシブデザイン

- **再生制御（動画プレイヤー方式）**
- グラフクリックで再生/一時停止切り替え
- シークバーで任意の位置にジャンプ
- 速度調整（1-1000 ops/frame、60 FPS固定）
- デフォルト10 ops/frame（600 ops/sec）
- リセットボタン

- **リアルタイム統計情報**
  - 比較回数、スワップ回数
  - インデックス読み書き回数
  - 進捗率、再生状態

- **アルゴリズム選択**
  - カテゴリフィルター
  - 配列サイズ選択（16-256要素）
  - 要素数制限（アルゴリズムの計算量に応じて自動調整）

## 🚀 使い方

### ビルドと実行

```powershell
cd sandbox/SortAlgorithm.VisualizationWeb
dotnet build
dotnet run
```

ブラウザで `https://localhost:5001` にアクセス

### 基本操作

1. **アルゴリズム選択**: サイドバーからソートアルゴリズムを選択
2. **配列サイズ設定**: 16, 32, 64, 128, 256から選択
3. **配列生成**: "Generate & Sort" ボタンをクリック
4. **再生開始**: 可視化エリアをクリックして再生開始
5. **一時停止**: 再生中に可視化エリアをクリック
6. **シーク**: 下部のシークバーをクリック/ドラッグして任意の位置に移動
7. **速度調整**: FPSスライダーで再生速度を調整
8. **リセット**: "Reset" ボタンで初期状態に戻る

## 📊 色分けスキーム

| 色 | 意味 |
|---|---|
| 🔵 青 (`#3B82F6`) | 通常状態 |
| 🟡 黄 (`#FBBF24`) | インデックス読み込み |
| 🟠 橙 (`#F97316`) | インデックス書き込み |
| 🟣 紫 (`#A855F7`) | 比較操作 |
| 🔴 赤 (`#EF4444`) | スワップ操作 |
| 🟢 緑 (`#10B981`) | ソート済み（将来実装） |
| ⚫ 灰 (`#6B7280`) | バッファー配列（将来実装） |

## 🏗️ アーキテクチャ

```
SortAlgorithm.VisualizationWeb/
├── Models/              # データモデル
│   ├── OperationType.cs
│   ├── VisualizationMode.cs
│   ├── PlaybackState.cs
│   ├── SortOperation.cs
│   ├── VisualizationState.cs
│   └── AlgorithmMetadata.cs
├── Services/            # ビジネスロジック
│   ├── SortExecutor.cs       # ソート実行と操作記録
│   ├── PlaybackService.cs    # 再生制御
│   └── AlgorithmRegistry.cs  # アルゴリズム管理
├── Components/          # Blazorコンポーネント
│   ├── BarChartRenderer.razor
│   ├── SeekBar.razor
│   └── StatisticsPanel.razor
├── Pages/               # ページ
│   └── Index.razor           # メインページ
└── wwwroot/
    └── css/
        └── app.css           # スタイルシート
```

## 🔮 将来の拡張予定

### 次のフェーズ
- [ ] **円形モード可視化** - 円周上に要素を配置したビジュアライゼーション
- [ ] **JavaScript Interop** - Canvas APIを使った高速描画
- [ ] **バッファー配列表示** - マージソートなどの補助配列を可視化
- [ ] **キーボードショートカット** - スペースキーで再生/一時停止など
- [ ] **配列パターン選択** - ランダム、逆順、ほぼソート済みなど
- [ ] **複数アルゴリズム比較** - 2つのアルゴリズムを並べて比較

### 高度な機能
- [ ] **音響フィードバック** - Sound of Sortingスタイルの音
- [ ] **録画機能** - GIF/動画エクスポート
- [ ] **教育コンテンツ** - アルゴリズムの説明とコード表示
- [ ] **カスタムデータ入力** - ユーザー定義の配列
- [ ] **3D可視化** - WebGLによる3D表現
- [ ] **パフォーマンス最適化** - Web Worker、仮想化

## 📝 技術スタック

- **フロントエンド**: Blazor WebAssembly (.NET 10)
- **描画**: SVG (将来的にCanvas/WebGL)
- **ソートライブラリ**: SortAlgorithm (既存プロジェクト)
- **状態管理**: Blazor Component State
- **スタイル**: CSS (ダークモード)

## 🎨 デザインコンセプト

- **画面占有率**: 可視化エリアが画面の75-80%を占める
- **ダークテーマ**: 黒背景に鮮やかな色でコントラストを強調
- **2カラムレイアウト**: 左側に統計・設定、右側に可視化エリア
- **動画プレイヤーUI**: 直感的な再生制御

## 📚 参考

- [仕様書](../../Docs/VisualizationWeb.md)
- [SortAlgorithm プロジェクト](../../src/SortAlgorithm/)
- [VisuAlgo](https://visualgo.net/en/sorting)
- [Sound of Sorting](https://github.com/bingmann/sound-of-sorting)

## 📄 ライセンス

このプロジェクトはSortAlgorithmsリポジトリの一部です。

---

**Version**: 1.0  
**Last Updated**: 2025-01-27
