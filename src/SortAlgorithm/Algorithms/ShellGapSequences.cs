using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

internal static class ShellGapSequences
{
    internal static ReadOnlySpan<int> Knuth => [1, 4, 13, 40, 121, 364, 1093, 3280, 9841, 29524, 88573, 265720, 797161, 2391484, 7174453, 21523360, 64570081, 193710244, 581130733];

    internal static ReadOnlySpan<int> Sedgewick => [1, 5, 19, 41, 109, 209, 505, 929, 2161, 3905, 8929, 16001, 36289, 64769, 146305, 260609, 587521, 1045505, 2354689, 4188161, 9427969, 16764929, 37730305, 67084289, 150958081, 268386305, 603906049, 1073643521];

    internal static ReadOnlySpan<int> Tokuda => [1, 3, 8, 19, 45, 102, 232, 524, 1181, 2659, 5984, 13466, 30300, 68177, 153400, 345151, 776590, 1747330, 3931495, 8845865, 19903197, 44782195, 100759939, 226709865, 510097199];

    internal static ReadOnlySpan<int> Ciura => [1, 4, 10, 23, 57, 132, 301, 701, 1750, 3937, 8858, 19930, 44842, 100894, 227011, 510774, 1149241, 2585792, 5818032, 13090572, 29453787, 66271020, 149109795, 335497038, 754868335];

    internal static ReadOnlySpan<int> Lee => [1, 4, 9, 20, 45, 102, 230, 516, 1158, 2599, 5831, 13082, 29351, 65853, 147748, 331490, 743735, 1668650, 3743800, 8399623, 18845471, 42281871, 94863989, 212837706, 477524607, 1071378536];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void FindLast(ReadOnlySpan<int> gaps, int length, ref int index)
    {
        while (index >= 0 && gaps[index] > length / 2) index--;
    }
}
