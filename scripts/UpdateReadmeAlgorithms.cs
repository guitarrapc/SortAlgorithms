#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
// Update the algorithm list section in README.md from the source tree.
//
// The script replaces everything between the two HTML comment markers:
//     <!-- ALGORITHMS_START -->
//     <!-- ALGORITHMS_END -->
//
// Run from the repository root:
//     dotnet run scripts/UpdateReadmeAlgorithms.cs
using System.Text;
using System.Text.RegularExpressions;

var repoRoot = FindRepoRoot();
var algorithmsDir = Path.Combine(repoRoot, "src", "SortAlgorithm", "Algorithms");
var readmePath = Path.Combine(repoRoot, "README.md");

const string StartMarker = "<!-- ALGORITHMS_START -->";
const string EndMarker = "<!-- ALGORITHMS_END -->";

// Canonical order of category directories shown in README.
// Each entry must correspond to a subdirectory under algorithmsDir.
// When adding a new category, create the directory AND add it here in the desired display order.
string[] categoryOrder =
[
    "Exchange",
    "Selection",
    "Insertion",
    "Merge",
    "Heap",
    "Partition",
    "Adaptive",
    "Distribution",
    "Network",
    "Tree",
    "Joke",
];

// Files inside Algorithms/ that are utilities, not sorting algorithms.
HashSet<string> excludedFiles =
[
    "ComparableComparer",
    "FloatingPointUtils",
    "IKeySelector",
    "SortSpan",
];

// Display name overrides for file stems that need special treatment.
// Add an entry here whenever the automatic CamelCase conversion produces an incorrect result
// (e.g. PDQSort → "PDQ Sort" instead of "Pattern-Defeating Quick Sort", or when a file was
// renamed but the human-readable name should stay the same as before).
Dictionary<string, string> displayNames = new()
{
    ["AmericanFlagSort"] = "American Flag Sort",
    ["BalancedBinaryTreeSort"] = "Binary Tree Sort (AVL)",
    ["BatcherOddEvenMergeSort"] = "Batcher Odd-Even Merge Sort",
    ["BidirectionalStableQuickSort"] = "Quick Sort (Bidirectional Stable)",
    ["BinaryTreeSort"] = "Binary Tree Sort (BST)",
    ["BitonicSort"] = "Bitonic Sort",
    ["BlockMergeSort"] = "Block Merge Sort",
    ["BlockQuickSort"] = "Block Quick Sort",
    ["BottomupHeapSort"] = "Bottom-Up Heap Sort",
    ["BottomupMergeSort"] = "Bottom-Up Merge Sort",
    ["DestswapStableQuickSort"] = "Quick Sort (Destswap Stable)",
    ["DropMergeSort"] = "Drop-Merge Sort",
    ["DualPivotQuickSort"] = "Quick Sort (Dual Pivot)",
    ["FlatStableSort"] = "Flat Stable Sort",
    ["Glidesort"] = "Glidesort",
    ["IntroSortDotnet"] = "Intro Sort (Dotnet)",
    ["MinHeapSort"] = "Min-Heap Sort",
    ["OddEvenSort"] = "Odd-Even Sort",
    ["PDQSort"] = "Pattern-Defeating Quick Sort",
    ["PingpongMergeSort"] = "Pingpong Merge Sort",
    ["QuickSort3way"] = "Quick Sort (3-Way)",
    ["QuickSortMedian3"] = "Quick Sort (Median of 3)",
    ["QuickSortMedian9"] = "Quick Sort (Median of 9)",
    ["RadixLSD4Sort"] = "Radix LSD Sort (Base 4)",
    ["RadixLSD10Sort"] = "Radix LSD Sort (Base 10)",
    ["RadixLSD256Sort"] = "Radix LSD Sort (Base 256)",
    ["RadixMSD4Sort"] = "Radix MSD Sort (Base 4)",
    ["RadixMSD10Sort"] = "Radix MSD Sort (Base 10)",
    ["RotateMergeSort"] = "Rotate Merge Sort",
    ["SpinSortVariant"] = "Spin Sort (Boost)",
    ["StableQuickSort"] = "Quick Sort (Stable)",
    ["StdSort"] = "std::sort (LLVM)",
    ["StdStableSort"] = "std::stable_sort (LLVM)",
    ["SymMergeSort"] = "SymMerge Sort",
    ["TernaryHeapSort"] = "Ternary Heap Sort",
    ["WeakHeapSort"] = "Weak Heap Sort",
};

// Sub-items (algorithm variants) rendered as indented bullets under their parent entry.
Dictionary<string, string[]> subItems = new()
{
    ["BitonicSort"] = ["Iterative", "Recursive"],
    ["RotateMergeSort"] = ["Iterative", "Recursive"],
    ["ShellSort"] = ["Knuth1973", "Sedgewick1986", "Tokuda1992", "Ciura2001", "Lee2021"],
};

UpdateReadme();
return 0;

// ── helpers ─────────────────────────────────────────────────────────────────

static string FindRepoRoot()
{
    // Walk up from the script's directory until we find the .git folder or the slnx file.
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir is not null)
    {
        if (dir.GetFiles("*.slnx").Length > 0 || dir.GetDirectories(".git").Length > 0)
            return dir.FullName;
        dir = dir.Parent;
    }
    // Fallback: current working directory
    return Directory.GetCurrentDirectory();
}

string GetDisplayName(string stem)
{
    if (displayNames.TryGetValue(stem, out var name))
        return name;
    return CamelToWords(stem);
}

static string CamelToWords(string name)
{
    // Insert space before a capital that follows a lower-case letter or digit
    var s = Regex.Replace(name, @"([a-z\d])([A-Z])", "$1 $2");
    // Insert space between a run of capitals and the start of a new word
    s = Regex.Replace(s, @"([A-Z]+)([A-Z][a-z])", "$1 $2");
    return s;
}

string GenerateSection()
{
    var sb = new StringBuilder();
    bool firstCategory = true;

    foreach (var category in categoryOrder)
    {
        var categoryDir = Path.Combine(algorithmsDir, category);
        if (!Directory.Exists(categoryDir))
            continue;

        var stems = Directory.GetFiles(categoryDir, "*.cs")
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Where(stem => !excludedFiles.Contains(stem))
            .OrderBy(stem => stem, StringComparer.Ordinal)
            .ToArray();

        if (stems.Length == 0)
            continue;

        if (!firstCategory)
            sb.AppendLine();
        firstCategory = false;

        sb.AppendLine($"### {category}");
        foreach (var stem in stems)
        {
            var relPath = $"./src/SortAlgorithm/Algorithms/{category}/{stem}.cs";
            sb.AppendLine($"- [{GetDisplayName(stem)}]({relPath})");
            if (subItems.TryGetValue(stem, out var subs))
            {
                foreach (var sub in subs)
                    sb.AppendLine($"  - {sub}");
            }
        }
    }

    // Remove trailing newline — the caller adds its own newline before the end marker
    return sb.ToString().TrimEnd('\r', '\n');
}

bool UpdateReadme()
{
    var content = File.ReadAllText(readmePath, Encoding.UTF8);

    var startIdx = content.IndexOf(StartMarker, StringComparison.Ordinal);
    var endIdx = content.IndexOf(EndMarker, StringComparison.Ordinal);

    if (startIdx < 0 || endIdx < 0)
    {
        Console.Error.WriteLine($"error: markers not found in {readmePath}");
        Console.Error.WriteLine($"  expected: {StartMarker}");
        Console.Error.WriteLine($"  expected: {EndMarker}");
        Environment.Exit(1);
    }

    var section = GenerateSection();
    var newContent = string.Concat(
        content[..(startIdx + StartMarker.Length)],
        "\n",
        section,
        "\n",
        content[endIdx..]);

    if (newContent == content)
    {
        Console.WriteLine("README.md is already up to date.");
        return false;
    }

    File.WriteAllText(readmePath, newContent, Encoding.UTF8);
    Console.WriteLine("README.md updated.");
    return true;
}
