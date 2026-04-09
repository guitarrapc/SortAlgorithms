#!/usr/bin/env python3
"""Update the algorithm list section in README.md from the source tree.

The script replaces everything between the two HTML comment markers:
    <!-- ALGORITHMS_START -->
    <!-- ALGORITHMS_END -->

Run from anywhere:
    python3 scripts/update-readme-algorithms.py
"""

import re
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent
ALGORITHMS_DIR = REPO_ROOT / "src" / "SortAlgorithm" / "Algorithms"
README_PATH = REPO_ROOT / "README.md"

START_MARKER = "<!-- ALGORITHMS_START -->"
END_MARKER = "<!-- ALGORITHMS_END -->"

# Canonical order of category directories shown in README.
# Each entry must correspond to a subdirectory under ALGORITHMS_DIR.
# When adding a new category, create the directory AND add it here in the desired display order.
CATEGORY_ORDER = [
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
]

# Files inside Algorithms/ that are utilities, not sorting algorithms
EXCLUDED_FILES = {
    "ComparableComparer",
    "FloatingPointUtils",
    "IKeySelector",
    "SortSpan",
}

# Display name overrides for file stems that need special treatment.
# Add an entry here whenever the automatic CamelCase conversion produces an incorrect result
# (e.g. PDQSort → "PDQ Sort" instead of "Pattern-Defeating Quick Sort", or when a file was
# renamed but the human-readable name should stay the same as before).
DISPLAY_NAMES: dict[str, str] = {
    "AmericanFlagSort": "American Flag Sort",
    "BalancedBinaryTreeSort": "Binary Tree Sort (AVL)",
    "BatcherOddEvenMergeSort": "Batcher Odd-Even Merge Sort",
    "BidirectionalStableQuickSort": "Quick Sort (Bidirectional Stable)",
    "BinaryTreeSort": "Binary Tree Sort (BST)",
    "BitonicSort": "Bitonic Sort",
    "BlockMergeSort": "Block Merge Sort",
    "BlockQuickSort": "Block Quick Sort",
    "BottomupHeapSort": "Bottom-Up Heap Sort",
    "BottomupMergeSort": "Bottom-Up Merge Sort",
    "DestswapStableQuickSort": "Quick Sort (Destswap Stable)",
    "DropMergeSort": "Drop-Merge Sort",
    "DualPivotQuickSort": "Quick Sort (Dual Pivot)",
    "FlatStableSort": "Flat Stable Sort",
    "Glidesort": "Glidesort",
    "IntroSortDotnet": "Intro Sort (Dotnet)",
    "MinHeapSort": "Min-Heap Sort",
    "OddEvenSort": "Odd-Even Sort",
    "PDQSort": "Pattern-Defeating Quick Sort",
    "PingpongMergeSort": "Pingpong Merge Sort",
    "QuickSort3way": "Quick Sort (3-Way)",
    "QuickSortMedian3": "Quick Sort (Median of 3)",
    "QuickSortMedian9": "Quick Sort (Median of 9)",
    "RadixLSD4Sort": "Radix LSD Sort (Base 4)",
    "RadixLSD10Sort": "Radix LSD Sort (Base 10)",
    "RadixLSD256Sort": "Radix LSD Sort (Base 256)",
    "RadixMSD4Sort": "Radix MSD Sort (Base 4)",
    "RadixMSD10Sort": "Radix MSD Sort (Base 10)",
    "RotateMergeSort": "Rotate Merge Sort",
    "SpinSortVariant": "Spin Sort (Boost)",
    "StableQuickSort": "Quick Sort (Stable)",
    "StdSort": "std::sort (LLVM)",
    "StdStableSort": "std::stable_sort (LLVM)",
    "SymMergeSort": "SymMerge Sort",
    "TernaryHeapSort": "Ternary Heap Sort",
    "WeakHeapSort": "Weak Heap Sort",
}

# Sub-items (algorithm variants) rendered as indented bullets under their parent entry
SUB_ITEMS: dict[str, list[str]] = {
    "BitonicSort": ["Iterative", "Recursive"],
    "RotateMergeSort": ["Iterative", "Recursive"],
    "ShellSort": ["Knuth1973", "Sedgewick1986", "Tokuda1992", "Ciura2001", "Lee2021"],
}


def camel_to_words(name: str) -> str:
    """Naive CamelCase → 'Camel Case' conversion used as a fallback."""
    # Insert space before a capital that follows a lower/digit
    s = re.sub(r"([a-z\d])([A-Z])", r"\1 \2", name)
    # Insert space between a run of capitals and the start of a new word
    s = re.sub(r"([A-Z]+)([A-Z][a-z])", r"\1 \2", s)
    return s


def display_name(stem: str) -> str:
    return DISPLAY_NAMES.get(stem) or camel_to_words(stem)


def generate_section() -> str:
    lines: list[str] = []

    for category in CATEGORY_ORDER:
        category_dir = ALGORITHMS_DIR / category
        if not category_dir.is_dir():
            continue

        stems = sorted(
            f.stem
            for f in category_dir.glob("*.cs")
            if f.stem not in EXCLUDED_FILES
        )
        if not stems:
            continue

        lines.append(f"### {category}")
        for stem in stems:
            rel_path = f"./src/SortAlgorithm/Algorithms/{category}/{stem}.cs"
            lines.append(f"- [{display_name(stem)}]({rel_path})")
            for sub in SUB_ITEMS.get(stem, []):
                lines.append(f"  - {sub}")
        lines.append("")

    # Remove trailing blank line
    while lines and lines[-1] == "":
        lines.pop()

    return "\n".join(lines)


def update_readme() -> bool:
    """Replace the section between markers in README.md. Returns True if changed."""
    content = README_PATH.read_text(encoding="utf-8")

    start_idx = content.find(START_MARKER)
    end_idx = content.find(END_MARKER)

    if start_idx == -1 or end_idx == -1:
        print(
            f"error: markers not found in {README_PATH}\n"
            f"  expected: {START_MARKER}\n"
            f"  expected: {END_MARKER}",
            file=sys.stderr,
        )
        sys.exit(1)

    section = generate_section()
    new_content = (
        content[: start_idx + len(START_MARKER)]
        + "\n"
        + section
        + "\n"
        + content[end_idx:]
    )

    if new_content == content:
        print("README.md is already up to date.")
        return False

    README_PATH.write_text(new_content, encoding="utf-8")
    print("README.md updated.")
    return True


if __name__ == "__main__":
    update_readme()
