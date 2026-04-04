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
            a[i] = ((i & 1) == 0) ? i : size + i;
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
            if (sum + next > size * 2 / 3) break;
            rev.Add(next);
            sum += next;
        }

        while (sum + minRun <= size)
        {
            rev.Add(minRun + ((rev.Count & 1) == 0 ? 0 : 1));
            sum += rev[^1];
        }

        int rem = size - sum;
        if (rem > 0)
        {
            if (rem < minRun && rev.Count > 0)
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
        int pos = 0;
        for (int i = 0; i < runs.Length; i++)
        {
            runOffsets[i] = pos;
            pos += runs[i];
        }

        // 最終ソート順に相当する「スロット順序」を構築
        var slots = BuildSlotOrder(root);

        if (slots.Count != total)
            throw new InvalidOperationException($"Slot count mismatch: {slots.Count} != {total}");

        // その順に rank を割り当てる
        for (int rank = 0; rank < total; rank++)
        {
            var slot = slots[rank];
            result[runOffsets[slot.Leaf] + slot.Offset] = rank;
        }

        // 自然 run 境界の確認
        for (int i = 0, start = 0; i < runs.Length - 1; i++)
        {
            int len = runs[i];
            int nextStart = start + len;
            if (result[start + len - 1] <= result[nextStart])
            {
                // 境界が降下していないと run がつながるので、末尾と先頭を軽く調整
                int tmp = result[start + len - 1];
                result[start + len - 1] = result[nextStart];
                result[nextStart] = tmp;
            }
            start = nextStart;
        }

        return result;
    }

    private static List<Slot> BuildSlotOrder(Node node)
    {
        if (node.IsLeaf)
        {
            var list = new List<Slot>(node.Length);
            for (int i = 0; i < node.Length; i++)
                list.Add(new Slot(node.StartLeaf, i));
            return list;
        }

        var left = BuildSlotOrder(node.Left!);
        var right = BuildSlotOrder(node.Right!);
        return WeightedInterleave(left, right);
    }

    private static List<Slot> WeightedInterleave(List<Slot> left, List<Slot> right)
    {
        int a = left.Count;
        int b = right.Count;
        int i = 0, j = 0;

        var merged = new List<Slot>(a + b);

        while (i < a || j < b)
        {
            if (i >= a)
            {
                merged.Add(right[j++]);
                continue;
            }
            if (j >= b)
            {
                merged.Add(left[i++]);
                continue;
            }

            // それぞれの「次の要素の理想位置」を比較して、小さい方を先に出す
            long leftKey = ((long)(2 * i + 1)) * b;
            long rightKey = ((long)(2 * j + 1)) * a;

            if (leftKey <= rightKey)
                merged.Add(left[i++]);
            else
                merged.Add(right[j++]);
        }

        return merged;
    }
}
