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

    private static int[] BuildTimSortBadRuns(int size, int minRun)
    {
        var rev = new List<int>();
        int sum = 0;

        rev.Add(minRun);
        rev.Add(minRun + 1);
        sum += rev[0] + rev[1];

        while (true)
        {
            int next = rev[^1] + rev[^2] + 1;
            if (sum + next > size * 3 / 5)
                break;

            rev.Add(next);
            sum += next;
        }

        // Consume the rest as many minRun-ish runs.
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

    private sealed class Node
    {
        public int StartLeaf;
        public int EndLeaf;
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

        // Ensure every physical run boundary is descending.
        int start = 0;
        for (int r = 0; r < runs.Length - 1; r++)
        {
            int len = runs[r];
            int leftLast = start + len - 1;
            int rightFirst = start + len;

            if (result[leftLast] <= result[rightFirst])
            {
                // Local repair: move a smaller element to the right boundary.
                int swap = rightFirst;
                while (swap + 1 < total && result[leftLast] <= result[swap])
                    swap++;

                if (result[leftLast] <= result[swap])
                {
                    int tmp = result[leftLast];
                    result[leftLast] = result[rightFirst];
                    result[rightFirst] = tmp;
                }
                else
                {
                    int tmp = result[leftLast];
                    result[leftLast] = result[swap];
                    result[swap] = tmp;
                }
            }

            start += len;
        }

        return result;
    }

    private static List<Slot> BuildSlotOrder(Node node)
    {
        if (node.IsLeaf)
        {
            // Important:
            // keep each leaf increasing in physical order,
            // but slightly bias offsets away from trivial monotone alignment
            // to reduce trimming opportunities in upper merges.
            var list = new List<Slot>(node.Length);
            int len = node.Length;

            // First half in order, then second half in order.
            // Still increasing physically after rank assignment,
            // but interacts less trivially with ancestors than plain 0..len-1.
            int mid = len / 2;
            for (int i = 0; i < mid; i++)
                list.Add(new Slot(node.StartLeaf, i));
            for (int i = mid; i < len; i++)
                list.Add(new Slot(node.StartLeaf, i));

            return list;
        }

        var left = BuildSlotOrder(node.Left!);
        var right = BuildSlotOrder(node.Right!);
        return AntiGallopInterleave(left, right);
    }

    private static List<Slot> AntiGallopInterleave(List<Slot> left, List<Slot> right)
    {
        int a = left.Count;
        int b = right.Count;
        int i = 0;
        int j = 0;

        var merged = new List<Slot>(a + b);

        // Phase 1:
        // Alternate as hard as possible to suppress long winning streaks.
        while (i < a && j < b)
        {
            merged.Add(left[i++]);
            merged.Add(right[j++]);
        }

        // Phase 2:
        // Distribute the remainder instead of appending in one block.
        // This keeps upper-level merges from becoming too easy.
        if (i < a)
        {
            SpreadAppend(merged, left, ref i);
        }
        else if (j < b)
        {
            SpreadAppend(merged, right, ref j);
        }

        return merged;
    }

    private static void SpreadAppend(List<Slot> merged, List<Slot> src, ref int index)
    {
        int remain = src.Count - index;
        if (remain <= 0) return;

        // Insert leftovers at roughly even intervals in the current merged list.
        int originalCount = merged.Count;
        if (originalCount == 0)
        {
            while (index < src.Count)
                merged.Add(src[index++]);
            return;
        }

        for (int k = 0; index < src.Count; k++, index++)
        {
            int pos = (int)(((long)(k + 1) * (merged.Count + 1)) / (remain + 1));
            if (pos < 0) pos = 0;
            if (pos > merged.Count) pos = merged.Count;
            merged.Insert(pos, src[index]);
        }
    }
}
