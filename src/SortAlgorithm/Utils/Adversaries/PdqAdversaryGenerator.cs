namespace SortAlgorithm.Utils;

public static class PdqAdversaryGenerator
{
    // Match your PDQSort constants.
    private const int InsertionSortThreshold = 24;
    private const int NintherThreshold = 128;

    public static int[] Generate(int length)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

        var a = new int[length];
        Array.Fill(a, -1);

        int low = 0;
        int high = length - 1;

        PoisonSegment(a, 0, length, ref low, ref high);

        // Fill all remaining holes with descending values.
        // This helps avoid "already partitioned" / partial insertion style easy cases.
        for (int i = 0; i < length; i++)
        {
            if (a[i] < 0)
                a[i] = high--;
        }

        return a;
    }

    private static void PoisonSegment(int[] a, int begin, int end, ref int low, ref int high)
    {
        int size = end - begin;
        if (size < InsertionSortThreshold || low > high)
            return;

        if (size > NintherThreshold)
        {
            PoisonNinther(a, begin, end, ref low, ref high);

            // In this construction, the chosen pivot is intended to be very small
            // (roughly rank 4 within the segment), so recurse into the large right side.
            int virtualPivotRank = 4;
            int nextBegin = Math.Min(end, begin + virtualPivotRank + 1);
            PoisonSegment(a, nextBegin, end, ref low, ref high);
        }
        else
        {
            PoisonMedianOf3(a, begin, end, ref low, ref high);

            // Pivot intended to be roughly rank 1 within the segment.
            int virtualPivotRank = 1;
            int nextBegin = Math.Min(end, begin + virtualPivotRank + 1);
            PoisonSegment(a, nextBegin, end, ref low, ref high);
        }
    }

    private static void PoisonMedianOf3(int[] a, int begin, int end, ref int low, ref int high)
    {
        int mid = begin + ((end - begin) / 2);

        // PDQSort calls: Sort3(mid, begin, end - 1)
        // After that, value at begin becomes the median of:
        //   positions mid, begin, end-1
        //
        // So put:
        //   small, second-small, huge
        // into those three slots, forcing pivot near the minimum.
        AssignIfEmpty(a, mid, low++, ref high);
        AssignIfEmpty(a, begin, low++, ref high);
        AssignIfEmpty(a, end - 1, high--, ref high);
    }

    private static void PoisonNinther(int[] a, int begin, int end, ref int low, ref int high)
    {
        int s2 = (end - begin) / 2;
        int mid = begin + s2;

        // PDQSort calls:
        //   Sort3(begin,   mid,     end - 1)   -> median ends at mid
        //   Sort3(begin+1, mid - 1, end - 2)   -> median ends at mid - 1
        //   Sort3(begin+2, mid + 1, end - 3)   -> median ends at mid + 1
        //   Sort3(mid - 1, mid, mid + 1)       -> median-of-medians ends at mid
        //   Swap(begin, mid)                   -> pivot moves to begin
        //
        // We want medians at [mid-1, mid, mid+1] to be something like:
        //   2, 4, 8
        // so that final pivot is 4: still very small.
        //
        // One valid pattern:
        //   triple1 = {0, 2, huge} -> median 2
        //   triple2 = {1, 4, huge} -> median 4
        //   triple3 = {3, 8, huge} -> median 8
        //
        // That costs 6 lows + 3 highs.

        int l0 = low++;
        int l1 = low++;
        int l2 = low++;
        int l3 = low++;
        int l4 = low++;
        int l5 = low++;

        int h0 = high--;
        int h1 = high--;
        int h2 = high--;

        // triple1: Sort3(begin, mid, end - 1) -> median at mid
        AssignIfEmpty(a, begin, l0, ref high);
        AssignIfEmpty(a, mid, l2, ref high);
        AssignIfEmpty(a, end - 1, h0, ref high);

        // triple2: Sort3(begin + 1, mid - 1, end - 2) -> median at mid - 1
        AssignIfEmpty(a, begin + 1, l1, ref high);
        AssignIfEmpty(a, mid - 1, l4, ref high);
        AssignIfEmpty(a, end - 2, h1, ref high);

        // triple3: Sort3(begin + 2, mid + 1, end - 3) -> median at mid + 1
        AssignIfEmpty(a, begin + 2, l3, ref high);
        AssignIfEmpty(a, mid + 1, l5, ref high);
        AssignIfEmpty(a, end - 3, h2, ref high);
    }

    private static void AssignIfEmpty(int[] a, int index, int value, ref int high)
    {
        if ((uint)index >= (uint)a.Length) return;

        if (a[index] < 0)
            a[index] = value;
    }
}
