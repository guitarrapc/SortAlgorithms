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
        Add("Bubble sort", "Exchange Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => BubbleSort.Sort(arr.AsSpan(), ctx));
        Add("Cocktail shaker sort", "Exchange Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => CocktailShakerSort.Sort(arr.AsSpan(), ctx));
        Add("Odd-even sort", "Exchange Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => OddEvenSort.Sort(arr.AsSpan(), ctx));
        Add("Comb sort", "Exchange Sorts", "O(n²)", MAX_SIZE, 512, (arr, ctx) => CombSort.Sort(arr.AsSpan(), ctx));

        // Selection Sorts - O(n²) - 推奨256
        Add("Selection sort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => SelectionSort.Sort(arr.AsSpan(), ctx));
        Add("Double selection sort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => DoubleSelectionSort.Sort(arr.AsSpan(), ctx));
        Add("Cycle sort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => CycleSort.Sort(arr.AsSpan(), ctx));
        Add("Pancake sort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => PancakeSort.Sort(arr.AsSpan(), ctx));

        // Insertion Sorts - O(n²) ~ O(n log n) - 推奨256-2048
        Add("Insertion sort", "Insertion Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => InsertionSort.Sort(arr.AsSpan(), ctx));
        Add("Pair insertion sort", "Insertion Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => PairInsertionSort.Sort(arr.AsSpan(), ctx));
        Add("Binary insert sort", "Insertion Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => BinaryInsertionSort.Sort(arr.AsSpan(), ctx));
        Add("Library sort", "Insertion Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => LibrarySort.Sort(arr.AsSpan(), ctx));
        Add("Shell sort (Knuth 1973)", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortKnuth1973.Sort(arr.AsSpan(), ctx));
        Add("Shell sort (Sedgewick 1986)", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortSedgewick1986.Sort(arr.AsSpan(), ctx));
        Add("Shell sort (Tokuda 1992)", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortTokuda1992.Sort(arr.AsSpan(), ctx));
        Add("Shell sort (Ciura 2001)", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortCiura2001.Sort(arr.AsSpan(), ctx));
        Add("Shell sort (Lee 2021)", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortLee2021.Sort(arr.AsSpan(), ctx));
        Add("Gnome sort", "Insertion Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => GnomeSort.Sort(arr.AsSpan(), ctx));

        // Merge Sorts - O(n log n) - 推奨2048
        Add("Merge sort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => MergeSort.Sort(arr.AsSpan(), ctx));
        Add("Bottom-up merge sort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => BottomupMergeSort.Sort(arr.AsSpan(), ctx));
        Add("Rotate merge sort", "Merge Sorts", "O(n log² n)", MAX_SIZE, 1024, (arr, ctx) => RotateMergeSort.Sort(arr.AsSpan(), ctx));
        Add("Timsort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => TimSort.Sort(arr.AsSpan(), ctx));
        Add("Powersort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => PowerSort.Sort(arr.AsSpan(), ctx));
        Add("ShiftSort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => ShiftSort.Sort(arr.AsSpan(), ctx));

        // Heap Sorts - O(n log n) - 推奨2048
        Add("Heapsort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => HeapSort.Sort(arr.AsSpan(), ctx));
        Add("Ternary heapsort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => TernaryHeapSort.Sort(arr.AsSpan(), ctx));
        Add("Bottom-up heapSort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => BottomupHeapSort.Sort(arr.AsSpan(), ctx));
        Add("Weak heapSort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => WeakHeapSort.Sort(arr.AsSpan(), ctx));
        Add("Smoothsort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => SmoothSort.Sort(arr.AsSpan(), ctx));

        // Partition Sorts - O(n log n) - 推奨2048-4096
        Add("Quicksort", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSort.Sort(arr.AsSpan(), ctx));
        Add("Quicksort (Median3)", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSortMedian3.Sort(arr.AsSpan(), ctx));
        Add("Quicksort (Median9)", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSortMedian9.Sort(arr.AsSpan(), ctx));
        Add("Quicksort (DualPivot)", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSortDualPivot.Sort(arr.AsSpan(), ctx));
        Add("Quicksort (Stable)", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => StableQuickSort.Sort(arr.AsSpan(), ctx));
        Add("BlockQuickSort", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => BlockQuickSort.Sort(arr.AsSpan(), ctx));
        Add("Introsort", "Partition Sorts", "O(n log n)", MAX_SIZE, 4096, (arr, ctx) => IntroSort.Sort(arr.AsSpan(), ctx));
        Add("IntrosortDotnet", "Partition Sorts", "O(n log n)", MAX_SIZE, 4096, (arr, ctx) => IntroSortDotnet.Sort(arr.AsSpan(), ctx));
        Add("Pattern-defeating quicksort", "Partition Sorts", "O(n log n)", MAX_SIZE, 4096, (arr, ctx) => PDQSort.Sort(arr.AsSpan(), ctx));
        Add("C++ std::sort", "Partition Sorts", "O(n log n)", MAX_SIZE, 4096, (arr, ctx) => StdSort.Sort(arr.AsSpan(), ctx));

        // Adaptive Sorts - O(n log n) - 推奨2048
        Add("Drop-Merge sort", "Adaptive Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => DropMergeSort.Sort(arr.AsSpan(), ctx));

        // Distribution Sorts - O(n) ~ O(nk) - 推奨4096
        Add("Counting sort", "Distribution Sorts", "O(n+k)", MAX_SIZE, 4096, (arr, ctx) => CountingSortInteger.Sort(arr.AsSpan(), ctx));
        Add("Pigeonhole sort", "Distribution Sorts", "O(n+k)", MAX_SIZE, 4096, (arr, ctx) => PigeonholeSortInteger.Sort(arr.AsSpan(), ctx));
        Add("Bucket sort", "Distribution Sorts", "O(n)", MAX_SIZE, 4096, (arr, ctx) => BucketSortInteger.Sort(arr.AsSpan(), ctx));
        Add("LSD Radix sort (b=4)", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixLSD4Sort.Sort(arr.AsSpan(), ctx));
        Add("LSD Radix sort (b=10)", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixLSD10Sort.Sort(arr.AsSpan(), ctx));
        Add("LSD Radix sort (b=256)", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixLSD256Sort.Sort(arr.AsSpan(), ctx));
        Add("MSD Radix sort (b=4)", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixMSD4Sort.Sort(arr.AsSpan(), ctx));
        Add("MSD Radix sort (b=10)", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixMSD10Sort.Sort(arr.AsSpan(), ctx));
        Add("American flag sort", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => AmericanFlagSort.Sort(arr.AsSpan(), ctx));

        // Network Sorts - O(log²n) - 推奨2048
        Add("Bitonic sort", "Network Sorts", "O(log²n)", MAX_SIZE, 2048, (arr, ctx) => BitonicSort.Sort(arr.AsSpan(), ctx));
        Add("Bitonic sort (Recursive)", "Network Sorts", "O(log²n)", MAX_SIZE, 1024, (arr, ctx) => BitonicSortNonOptimized.Sort(arr.AsSpan(), ctx));

        // Tree Sorts - O(n log n) - 推奨1024
        Add("Unbalanced binary tree sort", "Tree Sorts", "O(n log n)", MAX_SIZE, 1024, (arr, ctx) => BinaryTreeSort.Sort(arr.AsSpan(), ctx));
        Add("Balanced binary tree sort", "Tree Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => BalancedBinaryTreeSort.Sort(arr.AsSpan(), ctx));

        // Joke Sorts - O(n!) ~ O(∞) - 推奨8（注意: 極めて遅い）
        Add("Bogo sort", "Joke Sorts", "O(n!)", 8, 8, (arr, ctx) => BogoSort.Sort(arr.AsSpan(), ctx), "⚠️ Extremely slow!");
        Add("Slow sort", "Joke Sorts", "O(n^(log n))", MAX_SIZE, 16, (arr, ctx) => SlowSort.Sort(arr.AsSpan(), ctx), "⚠️ Extremely slow!");
        Add("Stooge sort", "Joke Sorts", "O(n^2.7)", MAX_SIZE, 16, (arr, ctx) => StoogeSort.Sort(arr.AsSpan(), ctx), "⚠️ Extremely slow!");
    }

    private void Add(string name, string category, string complexity, int maxElements, int recommendedSize,
        Action<int[], ISortContext> sortAction, string description = "")
    {
        _algorithms.Add(new AlgorithmMetadata
        {
            Name = name,
            Category = category,
            TimeComplexity = complexity,
            MaxElements = maxElements,
            RecommendedSize = recommendedSize,
            SortAction = sortAction,
            Description = description
        });
    }
}
