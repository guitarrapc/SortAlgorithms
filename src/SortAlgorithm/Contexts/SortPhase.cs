namespace SortAlgorithm.Contexts;

/// <summary>
/// Enumeration representing the current phase of the algorithm.
/// Provides type information for the tutorial side to construct phase text.
/// </summary>
/// <remarks>
/// New members are appended at the end of the enum, not placed in their family section: the numeric values are part of
/// the recorded event stream (PreparedFloatingSortEventTests hashes them), so inserting a member renumbers every later phase.
/// <para/>
/// Parameter meanings for each phase:
/// <list type="table">
///   <listheader><term>Phase</term><description>param1 / param2 / param3</description></listheader>
///   <item><term>BubblePass</term><description>pass (current pass number) / totalPasses (total passes) / boundary (right boundary position)</description></item>
///   <item><term>CircleSortPass</term><description>pass (current pass number, 1-based)</description></item>
///   <item><term>CircleSortInterval</term><description>lo (interval start index) / hi (interval end index) / depth (recursion depth, 0-based)</description></item>
///   <item><term>SelectionFindMin</term><description>i (sorted boundary) / last (last index)</description></item>
///   <item><term>TournamentBuild</term><description>n (element count) / size (leaf count = next power of 2 ≥ n)</description></item>
///   <item><term>TournamentExtract</term><description>sortedCount+1 (1-based extraction step) / n (total elements)</description></item>
///   <item><term>BTreeInsert</term><description>i (index of element being inserted) / last (array last index) / maxKeys (node capacity)</description></item>
///   <item><term>BTreeExtract</term><description>0 / 0 / maxKeys (node capacity)</description></item>
///   <item><term>BPlusTreeInsert</term><description>i (index of element being inserted) / last (array last index) / maxKeys (node capacity)</description></item>
///   <item><term>BPlusTreeScan</term><description>0 / 0 / maxKeys (node capacity)</description></item>
///   <item><term>BinomialHeapInsert</term><description>i (index of element being inserted) / last (array last index)</description></item>
///   <item><term>BinomialHeapExtract</term><description>step (1-based extraction step) / n (total elements)</description></item>
///   <item><term>PairingHeapInsert</term><description>i (index of element being inserted) / last (array last index)</description></item>
///   <item><term>PairingHeapExtract</term><description>step (1-based extraction step) / n (total elements)</description></item>
///   <item><term>DoubleSelectionFindMinMax</term><description>left (left boundary) / right (right boundary)</description></item>
///   <item><term>CycleSortCycle</term><description>cycleStart (cycle start index) / last (last index)</description></item>
///   <item><term>PancakeFindMax</term><description>first (head index) / currentSize (current subarray last index, inclusive)</description></item>
///   <item><term>BingoFindMax</term><description>first (range start index) / end (range last index, inclusive)</description></item>
///   <item><term>BingoPass</term><description>first (range start index) / end (unsorted region last index, inclusive)</description></item>
///   <item><term>CocktailForwardPass</term><description>pass (current pass number) / min (left boundary) / max (right boundary)</description></item>
///   <item><term>CocktailBackwardPass</term><description>pass (current pass number) / min (left boundary) / max (right boundary)</description></item>
///   <item><term>CombGapPass</term><description>gap (current gap value) / len (array length)</description></item>
///   <item><term>CombBubblePass</term><description>bubbleEnd (right boundary position of bubble phase)</description></item>
///   <item><term>OddEvenOddPhase</term><description>pass (current pass number)</description></item>
///   <item><term>OddEvenEvenPhase</term><description>pass (current pass number)</description></item>
///   <item><term>QuickSortPartition</term><description>left / right / pivotIndex</description></item>
///   <item><term>SampleSortSample</term><description>left (inclusive) / right (inclusive) / sampleCount</description></item>
///   <item><term>HybridToInsertionSort</term><description>left (inclusive) / right (inclusive) / threshold</description></item>
///   <item><term>HybridToHeapSort</term><description>left (inclusive) / right (inclusive)</description></item>
///   <item><term>HybridToMergeSort</term><description>left (inclusive) / right (inclusive) / threshold (SMALL_SORT when small; 0 when recursion budget exhausted)</description></item>
///   <item><term>IpnsortSmallSort</term><description>start (inclusive) / end-1 (inclusive) / threshold</description></item>
///   <item><term>PDQPartialInsertionSort</term><description>begin (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>PDQPatternShuffle</term><description>begin (inclusive) / end-1 (inclusive) / badAllowed remaining</description></item>
///   <item><term>OddEvenMergeSortPass</term><description>p (merge group size, power of 2) / count</description></item>
///   <item><term>OddEvenMergeSortStage</term><description>k (comparison distance within merge) / p (current merge group size) / count</description></item>
///   <item><term>PairwisePairLayer</term><description>distance (comparison distance, power of 2) / count</description></item>
///   <item><term>PairwiseSortedPairsLayer</term><description>distance (comparison distance, stride × (2s − 1)) / stride (subsequence stride, power of 2) / count</description></item>
///   <item><term>Reverse</term><description>lo (start index, inclusive) / hi (end index, inclusive)</description></item>
///   <item><term>StdStableSortSort</term><description>left (abs start of buffer's left half) / mid (abs start of buffer's right half) / right (abs end of buffer, inclusive)</description></item>
///   <item><term>StdStableSortMove</term><description>left (abs start of span's left half) / mid (abs start of span's right half) / right (abs end of span, inclusive)</description></item>
///   <item><term>GlidesortQuicksort</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>GlidesortPhysicalMerge</term><description>start (inclusive) / mid-1 (inclusive end of left half) / end-1 (inclusive end)</description></item>
///   <item><term>DriftsortQuicksort</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>DriftsortPhysicalMerge</term><description>start (inclusive) / mid-1 (inclusive end of left half) / end-1 (inclusive end)</description></item>
///   <item><term>DriftsortSmallSort</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>BlockMergeSortNetwork</term><description>n (total array length)</description></item>
///   <item><term>BlockMergeSortLevel</term><description>levelWidth (current merge run size) / passNum (1-based) / blockSize (√levelWidth)</description></item>
///   <item><term>QuadSortTailSwap</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>QuadSortQuadSwap</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>QuadSortQuadMerge</term><description>start (inclusive) / end-1 (inclusive) / block</description></item>
///   <item><term>QuadSortTailMerge</term><description>start (inclusive) / end-1 (inclusive) / block</description></item>
///   <item><term>QuadSortRotateMerge</term><description>start (inclusive) / end-1 (inclusive) / block</description></item>
///   <item><term>FluxSortAnalyze</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>FluxSortPartition</term><description>start (inclusive) / end-1 (inclusive) / source buffer</description></item>
///   <item><term>FluxSortReversePartition</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>FluxSortPivotSample</term><description>start (inclusive) / end-1 (inclusive) / sampleCount</description></item>
///   <item><term>FluxSortQuadsort</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>FluxSortMerge</term><description>start (inclusive) / end of first half (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>SimdQuickSortBitonicSort</term><description>start (inclusive) / end-1 (inclusive) / lanes per vector</description></item>
///   <item><term>CrumSortAnalyze, CrumSortPartition, CrumSortReversePartition, CrumSortQuadsort</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>CrumSortPivotSample</term><description>start (inclusive) / end-1 (inclusive) / sampleCount</description></item>
///   <item><term>CrumSortMerge</term><description>start (inclusive) / end of first half (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>BlitSortAnalyze, BlitSortPartition, BlitSortReversePartition, BlitSortQuadsort</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>BlitSortPivotSample</term><description>start (inclusive) / end-1 (inclusive) / sampleCount</description></item>
///   <item><term>BlitSortMerge</term><description>start (inclusive) / end of first half (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>SkaSortAmericanFlag</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>SkaSortRound</term><description>start (inclusive) / end-1 (inclusive) / buckets listed at the start of the round</description></item>
///   <item><term>SkaSortStdSort</term><description>start (inclusive) / end-1 (inclusive) / threshold</description></item>
///   <item><term>Ips4oSimpleCases, Ips4oBaseCase</term><description>start (inclusive) / end-1 (inclusive)</description></item>
///   <item><term>Ips4oSampling</term><description>start (inclusive) / end-1 (inclusive) / sample size</description></item>
///   <item><term>Ips4oClassification</term><description>start (inclusive) / end-1 (inclusive) / bucket count</description></item>
///   <item><term>Ips4oBlockPermutation</term><description>start (inclusive) / end-1 (inclusive) / block size</description></item>
///   <item><term>Ips4oCleanup</term><description>bucket start (inclusive) / bucket end-1 (inclusive) / bucket index</description></item>
///   <item><term>SimdMergeSortInRegisterSort</term><description>block start (inclusive) / block end-1 (inclusive) / lanes per vector</description></item>
///   <item><term>SimdMergeSortBlockMerge, SimdMergeSortMerge</term><description>start (inclusive) / end-1 (inclusive) / length of the merged sequences</description></item>
/// </list>
/// </remarks>
public enum SortPhase
{
    /// <summary>Phase not set (hides tutorial phase bar)</summary>
    None = 0,

    // Exchange family

    /// <summary>
    /// Bubble Sort pass.
    /// param1=pass (current pass number, 1-based), param2=totalPasses (total passes), param3=boundary (right boundary position)
    /// </summary>
    BubblePass,

    /// <summary>
    /// Circle Sort pass (outer iteration until no swaps).
    /// param1=pass (current pass number, 1-based)
    /// </summary>
    CircleSortPass,

    /// <summary>
    /// Circle Sort recursive interval: mirrored compare/swap over [lo, hi].
    /// param1=lo (interval start index), param2=hi (interval end index), param3=depth (recursion depth, 0-based)
    /// </summary>
    CircleSortInterval,

    /// <summary>
    /// Cocktail Shaker Sort forward pass (left to right, moving maximum to the right).
    /// param1=pass (current pass number, 1-based), param2=min (left boundary), param3=max (right boundary)
    /// </summary>
    CocktailForwardPass,

    /// <summary>
    /// Cocktail Shaker Sort backward pass (right to left, moving minimum to the left).
    /// param1=pass (current pass number, 1-based), param2=min (left boundary), param3=max (right boundary)
    /// </summary>
    CocktailBackwardPass,

    /// <summary>
    /// Comb Sort gap pass.
    /// param1=gap (current gap value), param2=len (array length)
    /// </summary>
    CombGapPass,

    /// <summary>
    /// Comb Sort bubble phase (gap=1).
    /// param1=bubbleEnd (right boundary position of bubble phase)
    /// </summary>
    CombBubblePass,

    /// <summary>
    /// Odd-Even Sort odd-even phase (pairs starting at even indices: (0,1), (2,3), ...).
    /// param1=pass (current pass number, 1-based)
    /// </summary>
    OddEvenOddPhase,

    /// <summary>
    /// Odd-Even Sort even-odd phase (pairs starting at odd indices: (1,2), (3,4), ...).
    /// param1=pass (current pass number, 1-based)
    /// </summary>
    OddEvenEvenPhase,

    // Selection family

    /// <summary>
    /// Selection Sort minimum value search.
    /// param1=i (sorted boundary index), param2=last (search end index, inclusive)
    /// </summary>
    SelectionFindMin,

    /// <summary>
    /// Double Selection Sort simultaneous min/max search.
    /// param1=left (left boundary index), param2=right (right boundary index)
    /// </summary>
    DoubleSelectionFindMinMax,

    /// <summary>
    /// Cycle Sort cycle processing.
    /// param1=cycleStart (cycle start index), param2=last (array last index)
    /// </summary>
    CycleSortCycle,

    /// <summary>
    /// Pancake Sort max element search.
    /// param1=first (head index), param2=currentSize-1 (current subarray last index, inclusive)
    /// </summary>
    PancakeFindMax,

    // Insertion family

    /// <summary>
    /// Insertion Sort pass: inserting element at position i into sorted region.
    /// param1=i (element being inserted), param2=first, param3=last-1
    /// </summary>
    InsertionPass,

    /// <summary>
    /// Binary Insertion Sort pass: binary-search insertion of element at position i.
    /// param1=i (element being inserted), param2=first, param3=last-1
    /// </summary>
    BinaryInsertionPass,

    /// <summary>
    /// Gnome Sort current position check.
    /// param1=i (current position), param2=last-1
    /// </summary>
    GnomePass,

    /// <summary>
    /// Shell Sort gap pass (h-insertion sort with gap h).
    /// param1=gap (current gap value), param2=gapIndex+1 (1-based), param3=totalGaps
    /// </summary>
    ShellGapPass,

    /// <summary>
    /// Pair Insertion Sort pair processing.
    /// param1=i (first index of the pair), param2=last-1
    /// </summary>
    PairInsertionPass,

    /// <summary>
    /// Merge Insertion Sort (Ford-Johnson): pairing elements and comparing each pair.
    /// param1=0, param2=pairs-1 (number of pairs)
    /// Emitted once per sort, for the outermost level only: the counts describe the whole input,
    /// and nested levels stay silent so this scope phase is not overwritten mid-recursion.
    /// </summary>
    MergeInsertionPairing,

    /// <summary>
    /// Merge Insertion Sort (Ford-Johnson): sorting the larger elements from each pair.
    /// param1=0, param2=pairs-1
    /// Spans the entire recursion, and is skipped when there is only one larger element to sort.
    /// </summary>
    MergeInsertionSortLarger,

    /// <summary>
    /// Merge Insertion Sort (Ford-Johnson): inserting pended elements using Jacobsthal sequence.
    /// param1=0, param2=pendCount-1
    /// Outermost level only, matching <see cref="MergeInsertionPairing"/>.
    /// </summary>
    MergeInsertionInsertPend,

    /// <summary>
    /// Merge Insertion Sort (Ford-Johnson): rearranging the original array based on sorted indices.
    /// param1=0, param2=n-1
    /// </summary>
    MergeInsertionRearrange,

    /// <summary>
    /// Library Sort phase marker.
    /// param1=phase (1=initial sort, 2=insert remaining, 3=extract)
    /// </summary>
    LibrarySortPhase,

    /// <summary>
    /// Patience Sort pile-building (dealing) phase: distributing elements into sorted piles.
    /// No parameters.
    /// </summary>
    PatienceSortDeal,

    /// <summary>
    /// Patience Sort k-way merge phase: merging all piles using a min-heap.
    /// param1=pileCount (total number of piles to merge)
    /// </summary>
    PatienceSortMerge,

    /// <summary>
    /// Strand Sort extract phase: scanning remaining elements and pulling out a sorted strand.
    /// param1=strandPass (1-based pass number), param2=remainingCount (elements before extraction)
    /// </summary>
    StrandSortExtract,

    /// <summary>
    /// Strand Sort merge phase: merging the extracted strand into the accumulated sorted result.
    /// param1=strandPass (1-based pass number), param2=strandLen, param3=resultLen
    /// </summary>
    StrandSortMerge,

    // Tree family

    /// <summary>
    /// Tree Sort insertion phase: inserting element at index i into the BST.
    /// param1=i (index of element being inserted), param2=last (array last index)
    /// </summary>
    TreeSortInsert,

    /// <summary>
    /// Tree Sort extraction phase: in-order traversal writing sorted elements back.
    /// No parameters used.
    /// </summary>
    TreeSortExtract,

    /// <summary>
    /// Cartesian Tree Sort build phase: appending the element at index i to the tree by popping the
    /// right spine. Distinct from <see cref="TreeSortInsert"/>, which descends from the root to find an
    /// insertion point: nothing is searched here, and the nodes touched are the tail of the previous
    /// step's spine rather than a root-to-leaf path.
    /// param1=i (index of element being appended), param2=last (array last index)
    /// </summary>
    CartesianTreeBuild,

    /// <summary>
    /// Cartesian Tree Sort extraction phase: extracting the smallest available node and making its
    /// children available. Distinct from <see cref="TreeSortExtract"/>, which is a single in-order
    /// traversal: this phase reports per-extraction progress because the order is decided by a priority
    /// queue rather than by the tree shape alone.
    /// param1=step (1-based extraction step), param2=n (total elements)
    /// </summary>
    CartesianTreeExtract,

    /// <summary>
    /// B-Tree Sort insertion phase: inserting the element at index i into the B-tree. Distinct from
    /// <see cref="TreeSortInsert"/>, which descends a binary search tree one key at a time: here a node
    /// holds many keys, the descent searches inside each node before choosing a child, and a node that is
    /// already full is split on the way down rather than after the fact.
    /// <para>
    /// param3 carries the node capacity because it is the one thing about a B-tree that its own theory
    /// leaves free, and an observer needs it to know which keys belong to the same node — the key slots of
    /// a node are a block of param3 consecutive indices in the tree buffer. Hard-coding it on the consumer
    /// side would silently draw a different tree the moment the parameter is retuned.
    /// </para>
    /// param1=i (index of element being inserted), param2=last (array last index), param3=maxKeys (node capacity)
    /// </summary>
    BTreeInsert,

    /// <summary>
    /// B-Tree Sort extraction phase: in-order traversal of the multiway tree writing sorted elements back.
    /// Distinct from <see cref="TreeSortExtract"/> only in that the traversal alternates children and keys
    /// within each node; it is announced separately so that param3 keeps naming the node capacity for the
    /// whole run.
    /// param3=maxKeys (node capacity)
    /// </summary>
    BTreeExtract,

    /// <summary>
    /// B+Tree Sort insertion phase: inserting the element at index i into the B+ tree. Distinct from
    /// <see cref="BTreeInsert"/> because the keys an internal node holds are separator <em>copies</em>
    /// rather than elements: an observer that counts keys in the tree buffer would otherwise conclude the
    /// sort had lost or duplicated elements.
    /// param1=i (index of element being inserted), param2=last (array last index), param3=maxKeys (node capacity)
    /// </summary>
    BPlusTreeInsert,

    /// <summary>
    /// B+Tree Sort output phase: walking the linked list of leaves and writing the sorted elements back.
    /// Distinct from <see cref="BTreeExtract"/> because nothing is traversed — every element lives in a
    /// leaf and the leaves are already chained, so the phase is a linear scan with no stack and no
    /// revisiting of internal nodes.
    /// param3=maxKeys (node capacity)
    /// </summary>
    BPlusTreeScan,

    // Heap family

    /// <summary>
    /// Heap Sort build phase: constructing the max-heap from the input array.
    /// param1=first (start index), param2=last-1 (end index)
    /// </summary>
    HeapBuild,

    /// <summary>
    /// Heap Sort extract phase: extracting the max element and shrinking the heap.
    /// param1=current extraction step (1-based), param2=total extractions (n-1)
    /// </summary>
    HeapExtract,

    /// <summary>
    /// Tournament Sort build phase: constructing the winner tree from the input array.
    /// param1=n (element count), param2=size (leaf count = next power of 2 ≥ n)
    /// </summary>
    TournamentBuild,

    /// <summary>
    /// Tournament Sort extract phase: extracting the current minimum and replaying the tournament.
    /// param1=sortedCount+1 (1-based extraction step), param2=n (total elements)
    /// </summary>
    TournamentExtract,

    /// <summary>
    /// Binomial Heap Sort build phase: inserting the element at index i into the binomial heap.
    /// Distinct from <see cref="HeapBuild"/>, which heapifies the array in place in a single bottom-up pass:
    /// this phase reports per-element progress because the heap is grown one insert at a time, and each insert
    /// carries through the root list like a binary increment.
    /// param1=i (index of element being inserted), param2=last (array last index)
    /// </summary>
    BinomialHeapInsert,

    /// <summary>
    /// Binomial Heap Sort extract phase: removing the minimum root and unioning its children back into the heap.
    /// param1=step (1-based extraction step), param2=n (total elements)
    /// </summary>
    BinomialHeapExtract,

    /// <summary>
    /// Pairing Heap Sort build phase: melding the element at index i into the heap as a single-node heap.
    /// param1=i (index of element being inserted), param2=last (array last index)
    /// </summary>
    PairingHeapInsert,

    /// <summary>
    /// Pairing Heap Sort extract phase: removing the root and rebuilding one heap from its children
    /// with the two pairing passes.
    /// param1=step (1-based extraction step), param2=n (total elements)
    /// </summary>
    PairingHeapExtract,

    /// <summary>
    /// In-place reversal of a subrange (e.g. MinHeapSort final step).
    /// param1=lo (start index, inclusive), param2=hi (end index, inclusive)
    /// </summary>
    Reverse,

    // Merge family

    /// <summary>
    /// Merge Sort merge step: merging two sorted halves [left..mid] and [mid+1..right].
    /// param1=left (inclusive start), param2=mid (inclusive end of left half), param3=right (inclusive end)
    /// </summary>
    MergeSortMerge,

    /// <summary>
    /// Bottom-up merge pass: one sweep merging pairs of runs with a given width.
    /// param1=width (run size being merged), param2=pass number (1-based)
    /// </summary>
    MergePass,

    /// <summary>
    /// Initial insertion sort phase used by hybrid merge sorts (e.g., RotateMerge, SymMerge).
    /// param1=blockSize (insertion sort threshold)
    /// </summary>
    MergeInitSort,

    /// <summary>
    /// Natural run detection phase (ShiftSort, TimSort, PowerSort).
    /// No parameters.
    /// </summary>
    MergeRunDetect,

    /// <summary>
    /// Final run collapse/merge phase (TimSort MergeForceCollapse, PowerSort final flush).
    /// param1=remaining run count on the stack
    /// </summary>
    MergeRunCollapse,

    /// <summary>
    /// StdStableSort StableSort merge step: merging two sorted halves of the auxiliary buffer back into the main span (MergeIntoS).
    /// param1=left (absolute start of buffer's left half, inclusive), param2=mid (absolute start of buffer's right half, inclusive), param3=right (absolute end of buffer, inclusive)
    /// </summary>
    StdStableSortSort,

    /// <summary>
    /// StdStableSort StableSortMove merge step: merging two sorted halves of the main span into the auxiliary buffer (MergeIntoB).
    /// param1=left (absolute start of main span's left half, inclusive), param2=mid (absolute start of main span's right half, inclusive), param3=right (absolute end of main span, inclusive)
    /// </summary>
    StdStableSortMove,

    /// <summary>
    /// Glidesort stable bidirectional quicksort on an Unsorted logical block.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    GlidesortQuicksort,

    /// <summary>
    /// Glidesort physical merge of two adjacent sorted runs [start..mid) and [mid..end).
    /// param1=start (inclusive), param2=mid-1 (inclusive end of left half), param3=end-1 (inclusive end)
    /// </summary>
    GlidesortPhysicalMerge,

    /// <summary>
    /// Driftsort stable partition quicksort on an unsorted logical run.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    DriftsortQuicksort,

    /// <summary>
    /// Driftsort physical merge of two adjacent sorted runs [start..mid) and [mid..end).
    /// param1=start (inclusive), param2=mid-1 (inclusive end of left half), param3=end-1 (inclusive end)
    /// </summary>
    DriftsortPhysicalMerge,

    /// <summary>
    /// Driftsort small sort of at most SMALL_SORT_THRESHOLD elements (sorting network seed,
    /// guarded insertion extension, bidirectional merge back into place). Raised from both
    /// call sites: eager logical-run creation and the stable quicksort base case.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    DriftsortSmallSort,

    /// <summary>
    /// Block Sort (WikiSort) Phase 1: seeding sorted groups of 4–8 elements via sorting networks.
    /// param1=n (total array length)
    /// </summary>
    BlockMergeSortNetwork,

    /// <summary>
    /// Block Sort (WikiSort) large-level pass: in-place block merging using internal buffers.
    /// Used when the current merge level exceeds the cache threshold.
    /// param1=levelWidth (current merge run size), param2=passNum (1-based), param3=blockSize (≈√levelWidth)
    /// </summary>
    BlockMergeSortLevel,

    /// <summary>
    /// FlatStableSort small-group sort: directly sorting a group of fewer than 5 blocks.
    /// param1=start (absolute, inclusive), param2=end (absolute, inclusive), param3=nblock
    /// </summary>
    FlatStableSortSmall,

    /// <summary>
    /// FlatStableSort block-index merge: merging two sorted block groups via circular scratch buffer.
    /// param1=leftCount (number of blocks in left group), param2=rightCount (number of blocks in right group), param3=total blocks
    /// </summary>
    FlatStableSortMergeBlocks,

    /// <summary>
    /// FlatStableSort rearrange: materializing the deferred block permutation in-place (cycle-following).
    /// param1=start (absolute, inclusive), param2=end (absolute, inclusive), param3=nblock
    /// </summary>
    FlatStableSortRearrange,

    /// <summary>
    /// FlatStableSort pre-sorted forward fast path: sorted prefix detected, only unsorted tail processed.
    /// param1=start (absolute, inclusive), param2=end (absolute, inclusive), param3=nsorted (leading sorted element count)
    /// </summary>
    FlatStableSortPreSortedForward,

    /// <summary>
    /// FlatStableSort pre-sorted backward fast path: sorted suffix detected, only unsorted prefix processed.
    /// param1=start (absolute, inclusive), param2=end (absolute, inclusive), param3=nsorted (trailing sorted element count)
    /// </summary>
    FlatStableSortPreSortedBackward,

    // Merge family: sorts that take sorted runs out of the input and merge them (Drop-Merge, Patience, Strand)

    /// <summary>
    /// Drop Merge Sort: LNS (Longest Nondecreasing Subsequence) detection scan.
    /// No parameters.
    /// </summary>
    DropMergeDetect,

    /// <summary>
    /// Drop Merge Sort: sorting the dropped elements.
    /// param1=droppedCount
    /// </summary>
    DropMergeSort,

    /// <summary>
    /// Drop Merge Sort: merging the LNS and sorted dropped elements back.
    /// param1=droppedCount, param2=total length
    /// </summary>
    DropMergeMerge,

    // Network family

    /// <summary>
    /// Bitonic Sort level: building a bitonic sequence of size k.
    /// param1=k (bitonic sequence size), param2=count
    /// </summary>
    BitonicLevel,

    /// <summary>
    /// Bitonic Sort stage: comparison-swap pass at distance j within level k.
    /// param1=j (comparison distance), param2=k (current level), param3=count
    /// </summary>
    BitonicStage,

    /// <summary>
    /// Batcher Odd-Even Merge Sort merge pass: merging blocks of size p.
    /// param1=p (merge group size, power of 2), param2=count
    /// </summary>
    OddEvenMergeSortPass,

    /// <summary>
    /// Batcher Odd-Even Merge Sort stage: comparison-swap pass at distance k within merge group p.
    /// param1=k (comparison distance), param2=p (current merge group size), param3=count
    /// </summary>
    OddEvenMergeSortStage,

    // Joke family

    /// <summary>
    /// Bogo Sort shuffle attempt.
    /// param1=attempt number (1-based)
    /// </summary>
    BogoShuffle,

    /// <summary>
    /// Slow Sort: settling the maximum of [start..end] at position end.
    /// param1=start (subrange start), param2=end (subrange end, receives the settled max)
    /// </summary>
    SlowSortSettle,

    /// <summary>
    /// Stooge Sort: one of the three recursive 2/3-passes.
    /// param1=start, param2=end (of the current sub-pass range), param3=pass (1=first 2/3, 2=last 2/3, 3=repeat first 2/3)
    /// </summary>
    StoogeSortPass,

    // Distribution family

    /// <summary>
    /// Radix Sort pass over one digit position.
    /// param1=digit (0-based, LSD: 0=least significant), param2=totalDigits
    /// </summary>
    RadixPass,

    /// <summary>
    /// Distribution Sort counting phase (count occurrences / build count array).
    /// No parameters.
    /// </summary>
    DistributionCount,

    /// <summary>
    /// A pass over the input that only measures the keys — finding min/max, or the maximum digit count — to
    /// decide how much work the following passes need. No element is read for its own sake and none is moved.
    /// Distinct from <see cref="DistributionCount"/>, which tallies per-value occurrences: describing a
    /// min/max scan as "counting occurrences" tells a consumer the wrong thing about what it is watching.
    /// No parameters.
    /// </summary>
    KeyRangeScan,

    /// <summary>
    /// Distribution Sort accumulate phase (prefix sum / compute offsets).
    /// No parameters.
    /// </summary>
    DistributionAccumulate,

    /// <summary>
    /// Distribution Sort write/collect phase (scatter to output / collect buckets).
    /// No parameters.
    /// </summary>
    DistributionWrite,

    /// <summary>
    /// The span one non-empty bucket occupies, reported once per bucket as soon as the offsets are known
    /// and before any element moves. Purely structural: it states where a bucket will lie, and carries no
    /// progress meaning, so a consumer that only renders progress can ignore it.
    /// param1 = start index of the bucket (absolute, in whichever buffer the following writes target),
    /// param2 = number of elements in the bucket, param3 = bucket label (the digit value for radix sorts;
    /// the meaning is algorithm-specific).
    /// <para>
    /// Exists so consumers do not have to re-derive bucket boundaries. Doing that requires reimplementing the
    /// key mapping, the range normalization, the digit width and the digit count — four things that live in
    /// this library and change with it. Every one of those has already drifted out of sync with a consumer at
    /// least once, silently, because a wrong reconstruction still produces a plausible-looking partition.
    /// The counts and offsets reported here are the ones the sort itself computed, so there is nothing to
    /// keep in sync and nothing extra to compute; empty buckets are skipped, which bounds the reports per
    /// distribution to min(radix, length).
    /// </para>
    /// </summary>
    DistributionBucket,

    // Partition family

    /// <summary>
    /// Quick Sort partition step: partitioning [left..right] around a pivot.
    /// param1=left, param2=right, param3=pivotIndex
    /// </summary>
    QuickSortPartition,

    /// <summary>
    /// Quick Sort 3-way (Dutch National Flag) partition step: partitioning [left..right] around a pivot value.
    /// After median-of-3 selection the pivot is moved to <c>left</c>.
    /// param1=left, param2=right, param3=pivotIndex (== left)
    /// </summary>
    QuickSort3wayPartition,

    /// <summary>
    /// Hybrid sort (IntroSort / PDQSort / StdSort / BlockQuickSort) switching to InsertionSort
    /// because the partition size is at or below the threshold.
    /// param1=left (inclusive), param2=right (inclusive), param3=threshold
    /// </summary>
    HybridToInsertionSort,

    /// <summary>
    /// Hybrid sort switching to HeapSort because the recursion depth limit (or bad-partition
    /// counter for PDQSort) has been exceeded, guaranteeing O(n log n) worst-case.
    /// param1=left (inclusive), param2=right (inclusive)
    /// </summary>
    HybridToHeapSort,

    /// <summary>
    /// Hybrid sort switching to MergeSort for a range.
    /// param1=left (inclusive), param2=right (inclusive), param3=threshold (0 when a recursion budget was exhausted)
    /// No algorithm announces it since DestswapStableQuickSort stopped falling back to merge sort; it stays so later phases keep their values.
    /// </summary>
    HybridToMergeSort,

    /// <summary>
    /// Ipnsort small sort of at most SmallSortThreshold elements (sorting network seed, guarded
    /// insertion extension, and for the longer inputs a bidirectional merge through scratch).
    /// Raised whenever a slice is handed to the small sort instead of being partitioned further.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=threshold
    /// </summary>
    /// <remarks>
    /// Reported for the same reason as <see cref="HybridToInsertionSort"/>: a hybrid that hands a
    /// range to another algorithm has to say so, or a consumer reconstructing the divide-and-conquer
    /// structure sees the recursion stop with no explanation. It is a separate phase because the
    /// small sort is not an insertion sort — for word-sized elements it is a sorting network — and
    /// naming it InsertionSort would put a wrong label on the leaf.
    /// </remarks>
    IpnsortSmallSort,

    /// <summary>
    /// PDQSort: the partition appears already sorted; attempting PartialInsertionSort
    /// (up to PartialInsertionSortLimit moves) before deciding whether to recurse.
    /// param1=begin (inclusive), param2=end-1 (inclusive)
    /// </summary>
    PDQPartialInsertionSort,

    /// <summary>
    /// PDQSort: a highly unbalanced partition was detected; shuffling elements to break
    /// adversarial patterns before re-partitioning.
    /// param1=begin (inclusive), param2=end-1 (inclusive), param3=badAllowed remaining
    /// </summary>
    PDQPatternShuffle,

    // Selection family (appended rather than placed in the section above: inserting a member there
    // would renumber every later phase, and the numeric values are part of the recorded event stream)

    /// <summary>
    /// Bingo Sort first pass: find the maximum of the range without moving anything.
    /// param1=first (range start index), param2=end (range last index, inclusive)
    /// </summary>
    BingoFindMax,

    /// <summary>
    /// Bingo Sort pass: move every copy of the previous maximum to the end of the unsorted region while finding the next maximum.
    /// param1=first (range start index), param2=end (unsorted region last index, inclusive)
    /// </summary>
    BingoPass,

    // Joke family (appended rather than placed in the section above: inserting a member there
    // would renumber every later phase, and the numeric values are part of the recorded event stream)

    /// <summary>
    /// Bozo Sort random transposition, announced before the swap.
    /// param1=attempt number (1-based), param2=i, param3=j (the two distinct positions about to be swapped)
    /// </summary>
    BozoSwap,

    // Network family (appended rather than placed in the section above: inserting a member there
    // would renumber every later phase, and the numeric values are part of the recorded event stream)

    /// <summary>
    /// Pairwise Sorting Network pair layer: sorting the pairs of positions that are distance apart (Parberry's Step 1 at stride = distance).
    /// param1=distance (comparison distance, power of 2), param2=count
    /// </summary>
    PairwisePairLayer,

    /// <summary>
    /// Pairwise Sorting Network sorted-pairs layer: comparing each pair's larger element with the smaller element of the pair s ahead,
    /// within the interleaved subsequences of the given stride (Parberry's Step 3). distance = stride × (2s − 1).
    /// param1=distance (comparison distance), param2=stride (subsequence stride, power of 2), param3=count
    /// </summary>
    PairwiseSortedPairsLayer,

    // Partition family (appended rather than placed in the section above: inserting a member there
    // would renumber every later phase, and the numeric values are part of the recorded event stream)

    /// <summary>
    /// Sample Sort drawing a random sample of a range without a presorted sample: the drawn elements are swapped to the front
    /// of the range, which is then sorted as its own range before any of its elements serves as a pivot.
    /// Announced only when the draw moves an element, so the phase always covers at least one swap.
    /// param1=left (range start, inclusive), param2=right (range end, inclusive), param3=sampleCount (2^k − 1)
    /// </summary>
    SampleSortSample,

    // Joke family (appended rather than placed in the section above: inserting a member there
    // would renumber every later phase, and the numeric values are part of the recorded event stream)

    /// <summary>
    /// Fun-Sort phase i: repeatedly binary-searching the whole, possibly unsorted, array for the content of cell i and swapping the
    /// out-of-order pair each failed search identifies, until a search lands on cell i. Cells 0..i-1 are in order among themselves when it starts.
    /// param1=i (cell being settled), param2=n-1 (last index)
    /// </summary>
    FunSortPhase,

    // Merge family (appended rather than placed in the section above: inserting a member there
    // would renumber every later phase, and the numeric values are part of the recorded event stream)

    /// <summary>
    /// Quadsort tail swap: sorting a short range (fewer than 32 elements at the top level, up to 96 when FluxSort hands one over)
    /// by sorting its four quarters recursively and parity merging them, with fixed parity swap networks below eight elements.
    /// Detail phase: FluxSort delegates to quadsort.
    /// param1=start (inclusive), param2=end-1 (inclusive), in the coordinates of the buffer that holds the range
    /// </summary>
    QuadSortTailSwap,

    /// <summary>
    /// Quadsort quad swap: classifying each group of eight as ordered, strictly descending or neither, reversing descending stretches
    /// and sorting the rest into blocks of eight, then parity merging those into blocks of 32. Detail phase.
    /// param1=start (inclusive), param2=end-1 (inclusive), in the coordinates of the buffer that holds the range
    /// </summary>
    QuadSortQuadSwap,

    /// <summary>
    /// Quadsort quad merge level: merging four adjacent sorted blocks at a time, pairwise into swap memory and back as one. Detail phase.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=block (length of the blocks being merged)
    /// </summary>
    QuadSortQuadMerge,

    /// <summary>
    /// Quadsort tail merge level: merging adjacent runs pairwise, backwards with the right run in swap memory, or by a cross merge through
    /// swap memory when both runs fit there and the right run has at least 64 elements. Detail phase.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=block (length of the left runs being merged)
    /// </summary>
    QuadSortTailMerge,

    /// <summary>
    /// Quadsort rotate merge level: merging adjacent runs in place with rotations and binary searches once the runs outgrow swap memory.
    /// Detail phase.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=block (length of the left runs being merged)
    /// </summary>
    QuadSortRotateMerge,

    // Partition family (appended rather than placed in the section above: inserting a member there
    // would renumber every later phase, and the numeric values are part of the recorded event stream)

    /// <summary>
    /// FluxSort analysis: counting descending pairs and streaks in each quarter, detecting sorted input, and reversing strictly descending quarters.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    FluxSortAnalyze,

    /// <summary>
    /// FluxSort partition: copying the elements not greater than the pivot to the front of the range in buffer 0 and the greater ones to
    /// swap memory, both in scan order. The pivot is on the stack in swap memory and carries <see cref="RoleType.Pivot"/>.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=source buffer (0 = main, 1 = swap memory)
    /// </summary>
    FluxSortPartition,

    /// <summary>
    /// FluxSort reverse partition: separating the elements less than the pivot from those equal to it, which are then in their final place.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    FluxSortReversePartition,

    /// <summary>
    /// FluxSort pivot sample: choosing the pivot from an evenly spaced sample of the range. Nine candidates are compared directly. A larger
    /// sample is copied into the other buffer, its two halves are sorted with quadsort, and their median becomes the pivot. The phase is
    /// announced again after those quadsort calls, so their detail phase does not cover the median search.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=sampleCount
    /// </summary>
    FluxSortPivotSample,

    /// <summary>
    /// FluxSort handing a range to quadsort: a small input, a streaky quarter, a short or unbalanced partition, both sides of a partition
    /// that began with a long run of elements not greater than the pivot, or low-cardinality data.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    FluxSortQuadsort,

    /// <summary>
    /// FluxSort merging the sorted quarters: pairwise into swap memory and back as one, skipping merges whose boundary is in order.
    /// param1=start (inclusive), param2=start of the second half - 1 (inclusive end of the first half), param3=end-1 (inclusive)
    /// </summary>
    FluxSortMerge,

    /// <summary>
    /// SimdQuickSort small sort: loading a range of at most sixteen vectors, padding the last with the largest key, sorting them with
    /// the bitonic network in registers, and storing them back.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=lanes per vector (1 on the scalar path)
    /// </summary>
    SimdQuickSortBitonicSort,

    /// <summary>
    /// CrumSort analysis: counting descending pairs and streaks in each quarter, detecting sorted input, and reversing strictly descending quarters.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    CrumSortAnalyze,

    /// <summary>
    /// CrumSort fulcrum partition: the first and last 32 elements are held in swap memory, and every element after them is written to the
    /// next free slot at both ends of the range, the end it belongs to keeping it. The pivot is held aside and then put at the boundary.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    CrumSortPartition,

    /// <summary>
    /// CrumSort reverse partition: separating the elements less than the previous pivot from those equal to it, which are then in their final place.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    CrumSortReversePartition,

    /// <summary>
    /// CrumSort pivot sample: the median of three medians of three for up to 2,048 elements; above that, an evenly spaced sample of about the
    /// cube root of the range, exchanged to its front and sorted there in two halves by quadsort. Announced again after those quadsort calls.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=sampleCount
    /// </summary>
    CrumSortPivotSample,

    /// <summary>
    /// CrumSort handing a range to quadsort with its swap memory: a small input, a streaky quarter, a short or unbalanced partition, or a
    /// range whose pivot sample marks low-cardinality data.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    CrumSortQuadsort,

    /// <summary>
    /// CrumSort merging the sorted quarters in place with rotations, skipping merges whose boundary is in order.
    /// param1=start (inclusive), param2=start of the second half - 1 (inclusive end of the first half), param3=end-1 (inclusive)
    /// </summary>
    CrumSortMerge,

    /// <summary>
    /// BlitSort analysis: counting descending pairs and streaks in each quarter, detecting sorted input, and reversing strictly descending quarters.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    BlitSortAnalyze,

    /// <summary>
    /// BlitSort partition: stably splitting the range into the elements not greater than the pivot and the greater ones. A piece that fits in
    /// swap memory moves its greater elements there and back; a longer range is split in halves, each partitioned, and the middle rotated.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    BlitSortPartition,

    /// <summary>
    /// BlitSort reverse partition: stably separating the elements less than the previous pivot from those equal to it.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    BlitSortReversePartition,

    /// <summary>
    /// BlitSort pivot sample: the pseudomedian of nine for up to 2,048 elements; above that, an evenly spaced sample of about the cube root of
    /// the range copied into swap memory, trimmed to its middle half and sorted in two halves by quadsort. Announced again after those calls.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=sampleCount
    /// </summary>
    BlitSortPivotSample,

    /// <summary>
    /// BlitSort handing a range to quadsort: a small input, a streaky quarter, a short or unbalanced partition, or a range whose pivot sample
    /// marks low-cardinality data.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    BlitSortQuadsort,

    /// <summary>
    /// BlitSort merging the sorted quarters in place with rotations, skipping merges whose boundary is in order.
    /// param1=start (inclusive), param2=start of the second half - 1 (inclusive end of the first half), param3=end-1 (inclusive)
    /// </summary>
    BlitSortMerge,

    /// <summary>
    /// SkaSort permuting a range of 128 to 1,023 elements by American flag sort: the buckets are filled in order, and an element that
    /// does not belong to the bucket being filled is exchanged with the next free slot of its own bucket.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    SkaSortAmericanFlag,

    /// <summary>
    /// SkaSort round over a range of 1,024 or more elements: every unfilled slot of each listed bucket is exchanged with the next free
    /// slot of its element's bucket, which is the slot itself when the element is home. Buckets that are full afterwards leave the list.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=buckets listed at the start of the round
    /// </summary>
    SkaSortRound,

    /// <summary>
    /// SkaSort handing a range of fewer than 128 elements to std::sort (<see cref="SortAlgorithm.Algorithms.StdSort"/>). For six or more
    /// elements StdSort's own phases follow at once.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=threshold
    /// </summary>
    SkaSortStdSort,

    /// <summary>
    /// Ips4oSort checking the whole input once: it returns at once when the input is sorted, and reverses it when it is in
    /// non-increasing order.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    Ips4oSimpleCases,

    /// <summary>
    /// Ips4oSort drawing a random sample to the front of the range and sorting it. The sample's own partitioning steps follow
    /// before the splitters are taken from it.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=sample size
    /// </summary>
    Ips4oSampling,

    /// <summary>
    /// Ips4oSort classifying every element of the range with the branchless decision tree into per-bucket buffer blocks. A full
    /// buffer block is written back to the front of the range.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=bucket count, equality buckets included
    /// </summary>
    Ips4oClassification,

    /// <summary>
    /// Ips4oSort moving the full blocks to their buckets through two swap blocks, reclassifying the first element of each block.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=block size in elements
    /// </summary>
    Ips4oBlockPermutation,

    /// <summary>
    /// Ips4oSort completing one bucket: the elements of a block that crossed its start, the overflow block and its buffer block
    /// fill the bucket's partial blocks.
    /// param1=bucket start (inclusive), param2=bucket end-1 (inclusive), param3=bucket index
    /// </summary>
    Ips4oCleanup,

    /// <summary>
    /// Ips4oSort sorting a range by its own insertion sort: the whole input up to 256 elements, a range up to 32 elements, and every
    /// bucket of a partitioning step on at most 4,096 elements.
    /// param1=start (inclusive), param2=end-1 (inclusive)
    /// </summary>
    Ips4oBaseCase,

    /// <summary>
    /// SimdMergeSort sorting the chunks of a block in registers: K² elements are loaded into K vectors, each lane is sorted by an
    /// odd-even sorting network across the vectors, and a transpose turns every vector into a sorted sequence of K elements.
    /// param1=block start (inclusive), param2=block end-1 (inclusive), param3=K (lanes per vector)
    /// </summary>
    SimdMergeSortInRegisterSort,

    /// <summary>
    /// SimdMergeSort merging pairs of sorted sequences inside a block with bitonic merge networks.
    /// param1=block start (inclusive), param2=block end-1 (inclusive), param3=length of the sequences being merged
    /// </summary>
    SimdMergeSortBlockMerge,

    /// <summary>
    /// SimdMergeSort merging pairs of sorted sequences that are at least a block long, across the whole range.
    /// param1=start (inclusive), param2=end-1 (inclusive), param3=length of the sequences being merged
    /// </summary>
    SimdMergeSortMerge,
}
