namespace SortAlgorithm.Utils;

public static class TimsortAdversaryGenerator
{
    /// <summary>
    /// Generate an array that causes TimSort to perform poorly by creating many runs that trigger worst-case merging behavior.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="minRun"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static int[] Generate(int size, int minRun)
    {
        if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
        if (minRun <= 0) throw new ArgumentOutOfRangeException(nameof(minRun));
        if (size == 0) return Array.Empty<int>();
        if (size <= minRun) return GenerateTiny(size);

        var runs = BuildTimSortBadRuns(size, minRun);
        var tree = BuildExpectedMergeTree(runs);
        return MaterializeRunsFromTree(runs, tree);
    }

    private static int[] GenerateTiny(int size)
    {
        var a = new int[size];
        for (int i = 0; i < size; i++)
            a[i] = (i & 1) == 0 ? i : size + i;
        return a;
    }

    // ----------------------------------------------------------------
    // Run-length generation
    // ----------------------------------------------------------------

    private static int[] BuildTimSortBadRuns(int size, int minRun)
    {
        var rev = new List<int>();
        int sum = 0;

        // Seed.
        rev.Add(minRun);
        rev.Add(minRun + 1);
        sum += rev[0] + rev[1];

        // Build a reverse-Fibonacci-ish suffix to stress MergeCollapse.
        while (true)
        {
            int next = rev[^1] + rev[^2] + 1;
            if (sum + next > size * 3 / 5)
                break;

            rev.Add(next);
            sum += next;
        }

        // Spend the rest on many minRun-ish runs.
        while (sum + minRun <= size)
        {
            int len = minRun + ((rev.Count & 1) == 0 ? 0 : 1);
            if (sum + len > size) break;

            rev.Add(len);
            sum += len;
        }

        int rem = size - sum;
        if (rem > 0)
        {
            if (rev.Count > 0 && rem < minRun)
                rev[^1] += rem;
            else
                rev.Add(rem);
        }

        rev.Reverse();
        NormalizeRuns(rev, minRun, size);
        return rev.ToArray();
    }

    private static void NormalizeRuns(List<int> runs, int minRun, int total)
    {
        for (int i = runs.Count - 2; i >= 0; i--)
        {
            if (runs[i] < minRun)
            {
                runs[i + 1] += runs[i];
                runs.RemoveAt(i);
            }
        }

        int sum = runs.Sum();
        if (sum != total)
            throw new InvalidOperationException($"Run normalization changed total: {sum} != {total}");
    }

    // ----------------------------------------------------------------
    // Expected merge tree
    // ----------------------------------------------------------------

    private sealed class Node
    {
        public int StartLeaf;
        public int EndLeaf; // exclusive
        public int Length;
        public Node? Left;
        public Node? Right;
        public bool IsLeaf => Left is null && Right is null;
    }

    private static Node BuildExpectedMergeTree(int[] runs)
    {
        var stack = new List<Node>();

        foreach (var (len, idx) in runs.Select((len, idx) => (len, idx)))
        {
            stack.Add(new Node
            {
                StartLeaf = idx,
                EndLeaf = idx + 1,
                Length = len
            });

            Collapse(stack);
        }

        ForceCollapse(stack);
        return stack[0];
    }

    private static void Collapse(List<Node> stack)
    {
        while (stack.Count > 1)
        {
            int n = stack.Count - 2;

            int lenNm1 = n - 1 >= 0 ? stack[n - 1].Length : 0;
            int lenN = stack[n].Length;
            int lenNp1 = stack[n + 1].Length;
            int lenNm2 = n - 2 >= 0 ? stack[n - 2].Length : 0;

            if ((n > 0 && lenNm1 <= lenN + lenNp1) ||
                (n > 1 && lenNm2 <= lenNm1 + lenN))
            {
                if (lenNm1 < lenNp1)
                    n--;

                MergeAt(stack, n);
            }
            else if (lenN <= lenNp1)
            {
                MergeAt(stack, n);
            }
            else
            {
                break;
            }
        }
    }

    private static void ForceCollapse(List<Node> stack)
    {
        while (stack.Count > 1)
        {
            int n = stack.Count - 2;
            if (n > 0 && stack[n - 1].Length < stack[n + 1].Length)
                n--;
            MergeAt(stack, n);
        }
    }

    private static void MergeAt(List<Node> stack, int i)
    {
        var left = stack[i];
        var right = stack[i + 1];

        stack[i] = new Node
        {
            StartLeaf = left.StartLeaf,
            EndLeaf = right.EndLeaf,
            Length = left.Length + right.Length,
            Left = left,
            Right = right
        };
        stack.RemoveAt(i + 1);
    }

    // ----------------------------------------------------------------
    // Materialization
    // ----------------------------------------------------------------

    private readonly struct Slot
    {
        public readonly int Leaf;
        public readonly int Offset;

        public Slot(int leaf, int offset)
        {
            Leaf = leaf;
            Offset = offset;
        }
    }

    private static int[] MaterializeRunsFromTree(int[] runs, Node root)
    {
        int total = runs.Sum();
        var result = new int[total];

        var runOffsets = new int[runs.Length];
        int p = 0;
        for (int i = 0; i < runs.Length; i++)
        {
            runOffsets[i] = p;
            p += runs[i];
        }

        var slots = BuildSlotOrder(root);

        if (slots.Count != total)
            throw new InvalidOperationException($"Slot count mismatch: {slots.Count} != {total}");

        for (int rank = 0; rank < total; rank++)
        {
            var s = slots[rank];
            result[runOffsets[s.Leaf] + s.Offset] = rank;
        }

        EnsureDescendingRunBoundaries(result, runs);
        return result;
    }

    private static List<Slot> BuildSlotOrder(Node node)
    {
        if (node.IsLeaf)
        {
            // Keep leaf physically ascending by offset.
            var list = new List<Slot>(node.Length);
            for (int i = 0; i < node.Length; i++)
                list.Add(new Slot(node.StartLeaf, i));
            return list;
        }

        var left = BuildSlotOrder(node.Left!);
        var right = BuildSlotOrder(node.Right!);
        return AntiGallopBoundaryInterleave(left, right);
    }

    /// <summary>
    /// Interleave two sorted slot sequences in a way that:
    /// 1. strongly alternates winners to suppress galloping,
    /// 2. places right[0] early in the merged order, so GallopRight trims little,
    /// 3. places left[last] late in the merged order, so GallopLeft trims little,
    /// 4. distributes leftovers instead of appending one block.
    /// </summary>
    private static List<Slot> AntiGallopBoundaryInterleave(List<Slot> left, List<Slot> right)
    {
        int a = left.Count;
        int b = right.Count;

        if (a == 0) return right;
        if (b == 0) return left;

        var merged = new List<Slot>(a + b);

        int li = 0;
        int rj = 0;

        // Reserve special boundary elements:
        // - right first element should appear very early
        // - left last element should appear very late
        var rightFirst = right[rj++];
        Slot? leftLast = null;
        int leftUsable = a;
        if (a >= 2)
        {
            leftLast = left[a - 1];
            leftUsable = a - 1;
        }

        // Put one left element first if possible, then rightFirst immediately.
        // This keeps right[0] from being too large relative to left[0],
        // which helps reduce the initial GallopRight trimming.
        if (li < leftUsable)
            merged.Add(left[li++]);

        merged.Add(rightFirst);

        // Main phase: near-perfect alternation from the remaining usable ranges.
        int leftRemain = leftUsable - li;
        int rightRemain = b - rj;

        while (leftRemain > 0 && rightRemain > 0)
        {
            merged.Add(left[li++]);
            merged.Add(right[rj++]);
            leftRemain--;
            rightRemain--;
        }

        // Spread leftovers rather than appending them as one easy block.
        if (leftRemain > 0)
            SpreadAppendRange(merged, left, li, leftUsable);
        if (rightRemain > 0)
            SpreadAppendRange(merged, right, rj, b);

        // Put leftLast very late.
        if (leftLast.HasValue)
        {
            int pos = merged.Count;
            // Usually append at end. Occasionally one-before-end gives a tiny bit more disorder.
            if (merged.Count >= 2 && ((a + b) & 1) == 0)
                pos = merged.Count - 1;

            merged.Insert(pos, leftLast.Value);
        }

        return merged;
    }

    private static void SpreadAppendRange(List<Slot> merged, List<Slot> src, int start, int end)
    {
        int remain = end - start;
        if (remain <= 0) return;

        if (merged.Count == 0)
        {
            for (int i = start; i < end; i++)
                merged.Add(src[i]);
            return;
        }

        int inserted = 0;
        for (int i = start; i < end; i++, inserted++)
        {
            int pos = (int)(((long)(inserted + 1) * (merged.Count + 1)) / (remain + 1));
            if (pos < 0) pos = 0;
            if (pos > merged.Count) pos = merged.Count;
            merged.Insert(pos, src[i]);
        }
    }

    private static void EnsureDescendingRunBoundaries(int[] result, int[] runs)
    {
        int start = 0;

        for (int r = 0; r < runs.Length - 1; r++)
        {
            int len = runs[r];
            int leftLast = start + len - 1;
            int rightFirst = start + len;

            if (result[leftLast] > result[rightFirst])
            {
                start += len;
                continue;
            }

            // Repair boundary with minimal disturbance:
            // find a later element in the right run smaller than leftLast,
            // or an earlier element in the left run larger than rightFirst.
            int rightRunEnd = rightFirst + runs[r + 1];

            int swapWithRight = -1;
            for (int i = rightFirst + 1; i < rightRunEnd; i++)
            {
                if (result[i] < result[leftLast])
                {
                    swapWithRight = i;
                    break;
                }
            }

            if (swapWithRight >= 0)
            {
                (result[rightFirst], result[swapWithRight]) = (result[swapWithRight], result[rightFirst]);
                if (result[leftLast] > result[rightFirst])
                {
                    start += len;
                    continue;
                }
            }

            int swapWithLeft = -1;
            for (int i = leftLast - 1; i >= start; i--)
            {
                if (result[i] > result[rightFirst])
                {
                    swapWithLeft = i;
                    break;
                }
            }

            if (swapWithLeft >= 0)
            {
                (result[leftLast], result[swapWithLeft]) = (result[swapWithLeft], result[leftLast]);
            }

            if (result[leftLast] <= result[rightFirst])
            {
                // final small fallback
                (result[leftLast], result[rightFirst]) = (result[rightFirst], result[leftLast]);
            }

            start += len;
        }
    }
}
