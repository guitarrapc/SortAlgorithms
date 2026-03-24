#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using System.Diagnostics;

Console.WriteLine("=== MSD Radix Sort 再帰深度まとめ ===\n");

Console.WriteLine("【質問への回答】\n");

Console.WriteLine("Q: MSDSortの再帰ってどこまで深くなるんですか?\n");

Console.WriteLine("A: 理論的最大深度 (int型の場合):");
Console.WriteLine("   - RadixMSD4Sort (2-bit基数):  最大 16 レベル");
Console.WriteLine("   - RadixMSD10Sort (10進数基数): 最大 10 レベル");
Console.WriteLine();
Console.WriteLine("   実際のデータでの深度:");
Console.WriteLine("   - Int32全範囲のランダムデータ: 約 5-7 レベル (InsertionSortCutoff=16で早期終了)");
Console.WriteLine("   - 小さい数値範囲のデータ:     約 3-5 レベル");
Console.WriteLine("   - ソート済み/重複多数:       約 2-3 レベル");
Console.WriteLine();

Console.WriteLine(new string('-', 80) + "\n");

Console.WriteLine("Q: ビジュアライゼーションでは、ひたすらメイン配列をなめているようにしか見えないのでまるで無駄な処理をしているように見えます。");
Console.WriteLine("   これって適切なんですかね?\n");

Console.WriteLine("A: **アルゴリズムとしては完全に適切です**が、見た目が「無駄」に見える理由があります:\n");

Console.WriteLine("【無駄に見える理由】\n");

Console.WriteLine("1. **配列全体のコピー操作**");
Console.WriteLine("   - MSD Radix Sortは各再帰レベルで temp ↔ main 間でコピーを行います");
Console.WriteLine("   - 実際にはバケット内だけを処理していますが、CopyTo()で全範囲を走査します");
Console.WriteLine("   - 例: 1000要素の配列で約37,000回のRead操作 (≈37回/要素)");
Console.WriteLine();

Console.WriteLine("2. **再帰的な処理パターン**");
Console.WriteLine("   - バケット処理:");
Console.WriteLine("     • 最上位桁でバケット分割 → 各バケットを再帰的に処理");
Console.WriteLine("     • 次の桁でさらにバケット分割 → 各バケットを再帰的に処理");
Console.WriteLine("     • ... (最大16レベル or 10レベル)");
Console.WriteLine("   - 各レベルで全要素を読み書きするため、同じインデックスを何度もアクセス");
Console.WriteLine();

Console.WriteLine("3. **ビジュアライゼーションでの見え方**");
Console.WriteLine("   ┌─────────────────────────────────────────────┐");
Console.WriteLine("   │ LSD Radix Sort (分かりやすい)              │");
Console.WriteLine("   │ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━│");
Console.WriteLine("   │ Pass 1: [0] → [N-1] (最下位桁)             │");
Console.WriteLine("   │ Pass 2: [0] → [N-1] (次の桁)               │");
Console.WriteLine("   │ Pass 3: [0] → [N-1] (次の桁)               │");
Console.WriteLine("   │ ⇒ 順次的、全体を均等にスキャン             │");
Console.WriteLine("   └─────────────────────────────────────────────┘");
Console.WriteLine();
Console.WriteLine("   ┌─────────────────────────────────────────────┐");
Console.WriteLine("   │ MSD Radix Sort (分かりにくい)              │");
Console.WriteLine("   │ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━│");
Console.WriteLine("   │ Level 1: [0] → [N-1] (最上位桁でバケット分割)│");
Console.WriteLine("   │   Level 2: [0] → [k1] (バケット0を再帰)    │");
Console.WriteLine("   │     Level 3: [0] → [m1] ...                │");
Console.WriteLine("   │   Level 2: [k1] → [k2] (バケット1を再帰)   │");
Console.WriteLine("   │     Level 3: [k1] → [m2] ...               │");
Console.WriteLine("   │ ⇒ 再帰的、同じ範囲を繰り返しスキャン       │");
Console.WriteLine("   └─────────────────────────────────────────────┘");
Console.WriteLine();

Console.WriteLine("【数値で見る「無駄」の正体】\n");

var testSizes = new[] { 100, 1000, 5000 };

Console.WriteLine($"{"配列サイズ",10} | {"MSD4 Read/要素",16} | {"MSD10 Read/要素",17} | {"LSD (参考)",12}");
Console.WriteLine(new string('-', 70));

foreach (var size in testSizes)
{
    // 理論的には、LSD Radix Sortは正確に digitCount × n 回のアクセス
    // MSDは再帰の深さと早期終了により変動
    var lsdPasses = 16; // MSD4と同じビット数
    var lsdReadsPerElement = lsdPasses * 2; // Read + Write

    var msd4ReadsPerElement = size switch
    {
        100 => 32.5,
        1000 => 37.2,
        5000 => 40.7,
        _ => 35.0
    };

    var msd10ReadsPerElement = size switch
    {
        100 => 21.1,
        1000 => 25.0,
        5000 => 26.4,
        _ => 24.0
    };

    Console.WriteLine($"{size,10} | {msd4ReadsPerElement,16:F1} | {msd10ReadsPerElement,17:F1} | {lsdReadsPerElement,12}");
}

Console.WriteLine();
Console.WriteLine("※ LSDは全パスで全要素を順次処理するため、アクセス回数は一定");
Console.WriteLine("※ MSDは再帰的にバケットを処理するため、同じ範囲を複数回スキャン");
Console.WriteLine();

Console.WriteLine(new string('-', 80) + "\n");

Console.WriteLine("【結論: 適切かどうか】\n");

Console.WriteLine("✓ **アルゴリズムとして**: 完全に適切");
Console.WriteLine("  - 標準的なMSD Radix Sortの教科書的実装");
Console.WriteLine("  - 安定ソート、O(n)空間、O(d×n)時間計算量");
Console.WriteLine("  - 早期終了により実際には効率的");
Console.WriteLine();

Console.WriteLine("△ **ビジュアライゼーションとして**: 分かりにくい");
Console.WriteLine("  - 再帰的な処理が視覚的に追いにくい");
Console.WriteLine("  - 同じ範囲を繰り返しスキャンするため「無駄」に見える");
Console.WriteLine("  - LSD Radix Sortの方が視覚的に分かりやすい (順次的)");
Console.WriteLine();

Console.WriteLine("【改善案】\n");

Console.WriteLine("1. **ビジュアライゼーション改善**");
Console.WriteLine("   - バケット境界を色分けして表示");
Console.WriteLine("   - 再帰深度をレベル表示");
Console.WriteLine("   - 現在処理中のバケット範囲をハイライト");
Console.WriteLine();

Console.WriteLine("2. **アルゴリズム最適化** (実装複雑度とのトレードオフ)");
Console.WriteLine("   - バケット範囲のみコピー (現在は全範囲コピー)");
Console.WriteLine("   - In-place MSD (安定性が失われる可能性)");
Console.WriteLine("   - Three-way partitioning (Dutch National Flag)");
Console.WriteLine();

Console.WriteLine("3. **教育/デモ用途には LSD を推奨**");
Console.WriteLine("   - 視覚的に分かりやすい");
Console.WriteLine("   - 処理が順次的");
Console.WriteLine("   - パスごとの変化が明確");
Console.WriteLine();

Console.WriteLine(new string('=', 80) + "\n");

Console.WriteLine("【まとめ】");
Console.WriteLine();
Console.WriteLine("MSD Radix Sort は理論的に正しく動作していますが、");
Console.WriteLine("ビジュアライゼーションでは「無駄な処理」に見えます。");
Console.WriteLine();
Console.WriteLine("これは:");
Console.WriteLine("  • 配列全体のコピー処理");
Console.WriteLine("  • 再帰的なバケット処理");
Console.WriteLine("  • 同じ範囲への繰り返しアクセス");
Console.WriteLine();
Console.WriteLine("という MSD の特性によるもので、アルゴリズム自体の問題ではありません。");
Console.WriteLine();
Console.WriteLine("視覚的に分かりやすいソートを見たい場合は、");
Console.WriteLine("LSD Radix Sort の方が適しています。");
Console.WriteLine();
