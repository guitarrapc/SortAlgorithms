using SortAlgorithm.VisualizationWeb.Models;
using SortAlgorithm.Contexts;
using SortAlgorithm.Algorithms;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// 全ソートアルゴリズムのメタデータを管理するレジストリ
/// </summary>
public class AlgorithmRegistry
{
    private readonly List<AlgorithmMetadata> _algorithms = [];

    public AlgorithmRegistry()
    {
        RegisterAlgorithms();
    }

    public IReadOnlyList<AlgorithmMetadata> GetAllAlgorithms() => _algorithms.AsReadOnly();

    public IEnumerable<AlgorithmMetadata> GetByCategory(string category)
        => _algorithms.Where(a => a.Category == category);

    public IEnumerable<string> GetCategories()
        => _algorithms.Select(a => a.Category).Distinct().OrderBy(c => c);

    private void RegisterAlgorithms()
    {
        // 最大サイズは全て4096、推奨サイズは計算量に応じて設定
        const int MAX_SIZE = 4096;

        // Exchange Sorts - O(n²) - 推奨256
        Add("Bubble sort", "Exchange Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => BubbleSort.Sort(arr, ctx),
            tutorialDescription: "隣り合う2要素を比較し、大きい方を右へ移動させる操作を繰り返します。各パスで最大値が末尾へ「泡のように」浮き上がり、確定済み要素が1つずつ増えていきます。");
        Add("Cocktail shaker sort", "Exchange Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => CocktailShakerSort.Sort(arr, ctx),
            tutorialDescription: "バブルソートを双方向に行うアルゴリズムです。左から右へのパスと右から左へのパスを交互に繰り返し、両端から確定済み要素を増やしていきます。");
        Add("Odd-even sort", "Exchange Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => OddEvenSort.Sort(arr, ctx));
        Add("Comb sort", "Exchange Sorts", "O(n²)", MAX_SIZE, 512, (arr, ctx) => CombSort.Sort(arr, ctx));

        // Selection Sorts - O(n²) - 推奨256
        Add("Selection sort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => SelectionSort.Sort(arr, ctx),
            tutorialDescription: "未ソート部分から最小値を選択し、未ソート先頭の要素と交換することを繰り返します。各パスでスワップは高々1回のため、スワップ回数が最小となるのが特徴です。");
        Add("Double selection sort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => DoubleSelectionSort.Sort(arr, ctx));
        Add("Cycle sort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => CycleSort.Sort(arr, ctx));
        Add("Pancake sort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => PancakeSort.Sort(arr, ctx));

        // Insertion Sorts - O(n²) ~ O(n log n) - 推奨256-2048
        Add("Insertion sort", "Insertion Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => InsertionSort.Sort(arr, ctx),
            tutorialDescription: "未ソート部分の先頭要素を、ソート済み部分の正しい位置へ挿入することを繰り返します。トランプのカード整列に似た直感的な動作で、ほぼ整列済みのデータに対して非常に高速です。");
        Add("Pair insertion sort", "Insertion Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => PairInsertionSort.Sort(arr, ctx));
        Add("Binary insert sort", "Insertion Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => BinaryInsertionSort.Sort(arr, ctx));
        Add("Library sort", "Insertion Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => LibrarySort.Sort(arr, ctx));
        Add("Shell sort (Knuth 1973)", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortKnuth1973.Sort(arr, ctx));
        Add("Shell sort (Sedgewick 1986)", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortSedgewick1986.Sort(arr, ctx));
        Add("Shell sort (Tokuda 1992)", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortTokuda1992.Sort(arr, ctx));
        Add("Shell sort (Ciura 2001)", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortCiura2001.Sort(arr, ctx));
        Add("Shell sort (Lee 2021)", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortLee2021.Sort(arr, ctx));
        Add("Gnome sort", "Insertion Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => GnomeSort.Sort(arr, ctx));

        // Merge Sorts - O(n log n) - 推奨2048
        Add("Merge sort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => MergeSort.Sort(arr, ctx),
            tutorialDescription: "配列を半分に再帰的に分割し、分割したものを順序を保ちながらマージ（結合）するアルゴリズムです。安定ソートで最悪計算量も O(n log n) が保証されますが、マージのための補助配列が必要です。");
        Add("Bottom-up merge sort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => BottomupMergeSort.Sort(arr, ctx));
        Add("Rotate merge sort", "Merge Sorts", "O(n log² n)", MAX_SIZE, 1024, (arr, ctx) => RotateMergeSort.Sort(arr, ctx));
        Add("Timsort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => TimSort.Sort(arr, ctx));
        Add("Powersort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => PowerSort.Sort(arr, ctx));
        Add("ShiftSort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => ShiftSort.Sort(arr, ctx));

        // Heap Sorts - O(n log n) - 推奨2048
        Add("Heapsort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => HeapSort.Sort(arr, ctx),
            tutorialDescription: "配列をヒープ（最大値が常に先頭にある完全二分木）に変換し、先頭の最大値を末尾と交換して確定させる操作を繰り返します。最悪計算量も O(n log n) が保証され、追加のメモリを必要としません。");
        Add("Ternary heapsort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => TernaryHeapSort.Sort(arr, ctx));
        Add("Bottom-up heapSort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => BottomupHeapSort.Sort(arr, ctx));
        Add("Weak heapSort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => WeakHeapSort.Sort(arr, ctx));
        Add("Smoothsort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => SmoothSort.Sort(arr, ctx));

        // Partition Sorts - O(n log n) - 推奨2048-4096
        Add("Quicksort", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSort.Sort(arr, ctx),
            tutorialDescription: "ピボット要素を選び「ピボット以下」と「ピボット超」の2つに分割し、再帰的にソートするアルゴリズムです。平均計算量は O(n log n) で実用的に最速クラスですが、ピボット選択が悪い場合は最悪 O(n²) になります。");
        Add("Quicksort (Median3)", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSortMedian3.Sort(arr, ctx));
        Add("Quicksort (Median9)", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSortMedian9.Sort(arr, ctx));
        Add("Quicksort (DualPivot)", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSortDualPivot.Sort(arr, ctx));
        Add("Quicksort (Stable)", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => StableQuickSort.Sort(arr, ctx));
        Add("BlockQuickSort", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => BlockQuickSort.Sort(arr, ctx));
        Add("Introsort", "Partition Sorts", "O(n log n)", MAX_SIZE, 4096, (arr, ctx) => IntroSort.Sort(arr, ctx));
        Add("IntrosortDotnet", "Partition Sorts", "O(n log n)", MAX_SIZE, 4096, (arr, ctx) => IntroSortDotnet.Sort(arr, ctx));
        Add("Pattern-defeating quicksort", "Partition Sorts", "O(n log n)", MAX_SIZE, 4096, (arr, ctx) => PDQSort.Sort(arr, ctx));
        Add("C++ std::sort", "Partition Sorts", "O(n log n)", MAX_SIZE, 4096, (arr, ctx) => StdSort.Sort(arr, ctx));

        // Adaptive Sorts - O(n log n) - 推奨2048
        Add("Drop-Merge sort", "Adaptive Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => DropMergeSort.Sort(arr, ctx));

        // Distribution Sorts - O(n) ~ O(nk) - 推奨4096
        Add("Counting sort", "Distribution Sorts", "O(n+k)", MAX_SIZE, 4096, (arr, ctx) => CountingSortInteger.Sort(arr, ctx));
        Add("Pigeonhole sort", "Distribution Sorts", "O(n+k)", MAX_SIZE, 4096, (arr, ctx) => PigeonholeSortInteger.Sort(arr, ctx));
        Add("Bucket sort", "Distribution Sorts", "O(n)", MAX_SIZE, 4096, (arr, ctx) => BucketSortInteger.Sort(arr, ctx));
        Add("LSD Radix sort (b=4)", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixLSD4Sort.Sort(arr, ctx));
        Add("LSD Radix sort (b=10)", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixLSD10Sort.Sort(arr, ctx));
        Add("LSD Radix sort (b=256)", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixLSD256Sort.Sort(arr, ctx));
        Add("MSD Radix sort (b=4)", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixMSD4Sort.Sort(arr, ctx));
        Add("MSD Radix sort (b=10)", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixMSD10Sort.Sort(arr, ctx));
        Add("American flag sort", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => AmericanFlagSort.Sort(arr, ctx));

        // Network Sorts - O(log²n) - 推奨2048
        Add("Bitonic sort", "Network Sorts", "O(log²n)", MAX_SIZE, 2048, (arr, ctx) => BitonicSort.Sort(arr, ctx));
        Add("Bitonic sort (Recursive)", "Network Sorts", "O(log²n)", MAX_SIZE, 1024, (arr, ctx) => BitonicSortNonOptimized.Sort(arr, ctx));

        // Tree Sorts - O(n log n) - 推奨1024
        Add("Unbalanced binary tree sort", "Tree Sorts", "O(n log n)", MAX_SIZE, 1024, (arr, ctx) => BinaryTreeSort.Sort(arr, ctx));
        Add("Balanced binary tree sort", "Tree Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => BalancedBinaryTreeSort.Sort(arr, ctx));

        // Joke Sorts - O(n!) ~ O(∞) - 推奨8（注意: 極めて遅い）
        Add("Bogo sort", "Joke Sorts", "O(n!)", 8, 8, (arr, ctx) => BogoSort.Sort(arr, ctx), "⚠️ Extremely slow!");
        Add("Slow sort", "Joke Sorts", "O(n^(log n))", MAX_SIZE, 16, (arr, ctx) => SlowSort.Sort(arr, ctx), "⚠️ Extremely slow!");
        Add("Stooge sort", "Joke Sorts", "O(n^2.7)", MAX_SIZE, 16, (arr, ctx) => StoogeSort.Sort(arr, ctx), "⚠️ Extremely slow!");
    }

    private void Add(string name, string category, string complexity, int maxElements, int recommendedSize,
        Action<Span<int>, ISortContext> sortAction, string description = "", string tutorialDescription = "")
    {
        _algorithms.Add(new AlgorithmMetadata
        {
            Name = name,
            Category = category,
            TimeComplexity = complexity,
            MaxElements = maxElements,
            RecommendedSize = recommendedSize,
            SortAction = sortAction,
            Description = description,
            TutorialDescription = tutorialDescription
        });
    }
}
