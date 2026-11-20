using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// PDQSort and PDQSortBranchless are ports of orlp's pdqsort, so pdqsort.h defines what they must do. Each row holds the comparison count
/// and a hash of the compared pairs that pdqsort.h produces for the same input. A change to which elements are compared changes that hash
/// even when the output stays sorted. The hash is a sum, so it does not depend on the order in which the two sides of a partition are
/// sorted: PDQSort recurses on the smaller side, pdqsort on the left. Each key carries its position in its low bits, so the hash also tells
/// equal keys apart. sandbox/PdqReferenceTrace/trace.cpp prints the rows.
/// </summary>
public class PdqReferenceTraceTests
{
    const int KeyShift = 28;

    enum Sorter { Pdq, PdqBranchless }

    static readonly (Sorter Sorter, int Shape, int N, ulong Count, ulong Hash)[] Rows =
    [
        (Sorter.Pdq, 0, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.Pdq, 0, 3, 3UL, 0xFD14E387E1D79550UL),
        (Sorter.Pdq, 0, 23, 178UL, 0x9D17BC209A779A88UL),
        (Sorter.Pdq, 0, 24, 119UL, 0x8099851400A60713UL),
        (Sorter.Pdq, 0, 25, 149UL, 0xC7566730A8DDF272UL),
        (Sorter.Pdq, 0, 128, 1013UL, 0x59047733C6489BFFUL),
        (Sorter.Pdq, 0, 129, 1053UL, 0xD33BA1D15660E6C7UL),
        (Sorter.Pdq, 0, 1000, 11510UL, 0x20EC30AF0CC5F8DAUL),
        (Sorter.Pdq, 0, 10000, 151991UL, 0x1A65AA67D763590BUL),
        (Sorter.Pdq, 0, 100000, 1839958UL, 0xFC53137C2861F9C8UL),
        (Sorter.Pdq, 1, 2, 1UL, 0x6177136503314586UL),
        (Sorter.Pdq, 1, 3, 3UL, 0xFA6A7B30E9C4C922UL),
        (Sorter.Pdq, 1, 23, 138UL, 0x4F2A28E730E5E25DUL),
        (Sorter.Pdq, 1, 24, 138UL, 0x32003AC079297A60UL),
        (Sorter.Pdq, 1, 25, 123UL, 0x6FB23734DBB802D9UL),
        (Sorter.Pdq, 1, 128, 943UL, 0x285229E3D3B079C3UL),
        (Sorter.Pdq, 1, 129, 840UL, 0xA58D05C414183A1AUL),
        (Sorter.Pdq, 1, 1000, 5493UL, 0x8075347C871A9D7AUL),
        (Sorter.Pdq, 1, 10000, 54776UL, 0x0B3738C2CEF38DF8UL),
        (Sorter.Pdq, 1, 100000, 525401UL, 0x9780C3D072B72EBBUL),
        (Sorter.Pdq, 2, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.Pdq, 2, 3, 2UL, 0xA21ECBF5BF1B15EBUL),
        (Sorter.Pdq, 2, 23, 91UL, 0xC76372944BFF8C8EUL),
        (Sorter.Pdq, 2, 24, 100UL, 0x4E070A6523D90561UL),
        (Sorter.Pdq, 2, 25, 51UL, 0x64AAFA9351E14F04UL),
        (Sorter.Pdq, 2, 128, 333UL, 0x5531BB82B007DFB7UL),
        (Sorter.Pdq, 2, 129, 409UL, 0xB8FEB1712CECD7F0UL),
        (Sorter.Pdq, 2, 1000, 2549UL, 0xE3DD920DE0611B29UL),
        (Sorter.Pdq, 2, 10000, 30049UL, 0x2B766DC6BE42FEE8UL),
        (Sorter.Pdq, 2, 100000, 250049UL, 0x8AE52A3D4170EE13UL),
        (Sorter.Pdq, 3, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.Pdq, 3, 3, 2UL, 0x82558DF17090A640UL),
        (Sorter.Pdq, 3, 23, 22UL, 0xB922B7193BA46850UL),
        (Sorter.Pdq, 3, 24, 49UL, 0x232FB29FFCCD606BUL),
        (Sorter.Pdq, 3, 25, 51UL, 0x21D7A2CB246CEDFFUL),
        (Sorter.Pdq, 3, 128, 257UL, 0xB1DB5635915A0595UL),
        (Sorter.Pdq, 3, 129, 268UL, 0xC5A7DB9C3CFC46ADUL),
        (Sorter.Pdq, 3, 1000, 2010UL, 0x0FA3AC6CFC31B9EAUL),
        (Sorter.Pdq, 3, 10000, 20010UL, 0x83D8A0F54041B1AFUL),
        (Sorter.Pdq, 3, 100000, 200010UL, 0x1E9A1DB037BACF85UL),
        (Sorter.Pdq, 4, 2, 1UL, 0x727E694B1A1E85A4UL),
        (Sorter.Pdq, 4, 3, 3UL, 0x5CA4B87E1E8054D3UL),
        (Sorter.Pdq, 4, 23, 253UL, 0x6EF5BA549AA1C1CBUL),
        (Sorter.Pdq, 4, 24, 49UL, 0xD21B2C3169A65859UL),
        (Sorter.Pdq, 4, 25, 51UL, 0x17E79C2BF8AA3AE7UL),
        (Sorter.Pdq, 4, 128, 389UL, 0xD73620B919ED281CUL),
        (Sorter.Pdq, 4, 129, 401UL, 0x9C213E1E6A9B7D40UL),
        (Sorter.Pdq, 4, 1000, 3032UL, 0x4D3560CAB999E637UL),
        (Sorter.Pdq, 4, 10000, 30032UL, 0x52A743512CFB7F8FUL),
        (Sorter.Pdq, 4, 100000, 300032UL, 0x61D50B8F16FE97FDUL),
        (Sorter.Pdq, 5, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.Pdq, 5, 3, 3UL, 0xFAC73A5BE0A92205UL),
        (Sorter.Pdq, 5, 23, 133UL, 0x0541DF74515CF40BUL),
        (Sorter.Pdq, 5, 24, 149UL, 0x128890CAAE1B804CUL),
        (Sorter.Pdq, 5, 25, 161UL, 0xBAC121EEEB4C586FUL),
        (Sorter.Pdq, 5, 128, 1293UL, 0x9B35C6FCF6CF8B66UL),
        (Sorter.Pdq, 5, 129, 1239UL, 0x38E58F0E42AE9E08UL),
        (Sorter.Pdq, 5, 1000, 13838UL, 0x3F3404C2F694157FUL),
        (Sorter.Pdq, 5, 10000, 198237UL, 0x769FF60CA5B471B7UL),
        (Sorter.Pdq, 5, 100000, 2578937UL, 0xF26088DDC43683D6UL),
        (Sorter.Pdq, 6, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.Pdq, 6, 3, 2UL, 0x82558DF17090A640UL),
        (Sorter.Pdq, 6, 23, 22UL, 0xB922B7193BA46850UL),
        (Sorter.Pdq, 6, 24, 49UL, 0x232FB29FFCCD606BUL),
        (Sorter.Pdq, 6, 25, 51UL, 0x21D7A2CB246CEDFFUL),
        (Sorter.Pdq, 6, 128, 1000UL, 0x5211CA107EF721F4UL),
        (Sorter.Pdq, 6, 129, 1029UL, 0x47257DA4BC901861UL),
        (Sorter.Pdq, 6, 1000, 7361UL, 0x0CD3CF83AE0CBAA1UL),
        (Sorter.Pdq, 6, 10000, 69834UL, 0xF2227CD6A3FC1CB7UL),
        (Sorter.Pdq, 6, 100000, 684715UL, 0xBEE95E6593438FFAUL),
        (Sorter.Pdq, 7, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.Pdq, 7, 3, 2UL, 0x82558DF17090A640UL),
        (Sorter.Pdq, 7, 23, 25UL, 0x50A087DD819AE0D6UL),
        (Sorter.Pdq, 7, 24, 49UL, 0x5AD796E075DEEAF4UL),
        (Sorter.Pdq, 7, 25, 74UL, 0x5506A4082929F437UL),
        (Sorter.Pdq, 7, 128, 460UL, 0xC450B0F1CEAC94DFUL),
        (Sorter.Pdq, 7, 129, 491UL, 0x1FB12D07E72F0F2EUL),
        (Sorter.Pdq, 7, 1000, 6337UL, 0xD6C79D9E685C4441UL),
        (Sorter.Pdq, 7, 10000, 99727UL, 0xBCA197C752C08DD6UL),
        (Sorter.Pdq, 7, 100000, 1325340UL, 0xAE41CF13F68CE64FUL),
        (Sorter.Pdq, 8, 2, 1UL, 0xB5010A203C179EAEUL),
        (Sorter.Pdq, 8, 3, 2UL, 0x44AE10B06A7B2899UL),
        (Sorter.Pdq, 8, 23, 22UL, 0x3C18F532DD653FE2UL),
        (Sorter.Pdq, 8, 24, 49UL, 0x96EE9663B736C82FUL),
        (Sorter.Pdq, 8, 25, 56UL, 0xA1282EE95CC1D868UL),
        (Sorter.Pdq, 8, 128, 262UL, 0x9FC3B723E6B75BC3UL),
        (Sorter.Pdq, 8, 129, 273UL, 0x5E79F2E0526538ABUL),
        (Sorter.Pdq, 8, 1000, 2024UL, 0x847C17C000AD722FUL),
        (Sorter.Pdq, 8, 10000, 20024UL, 0xF43BDE45D0A286BEUL),
        (Sorter.Pdq, 8, 100000, 200024UL, 0x10720941B6E7B111UL),
        (Sorter.PdqBranchless, 0, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.PdqBranchless, 0, 3, 3UL, 0xFD14E387E1D79550UL),
        (Sorter.PdqBranchless, 0, 23, 178UL, 0x9D17BC209A779A88UL),
        (Sorter.PdqBranchless, 0, 24, 117UL, 0xC6D560159C45B8DFUL),
        (Sorter.PdqBranchless, 0, 25, 132UL, 0xC16E710186AAEEB5UL),
        (Sorter.PdqBranchless, 0, 128, 1050UL, 0xCE637FAD93D57A4CUL),
        (Sorter.PdqBranchless, 0, 129, 1096UL, 0xEE120661B867BE6DUL),
        (Sorter.PdqBranchless, 0, 1000, 11269UL, 0x4606569C9149F760UL),
        (Sorter.PdqBranchless, 0, 10000, 150104UL, 0x2B07C3594315696FUL),
        (Sorter.PdqBranchless, 0, 100000, 1849709UL, 0x6FD622A09CE921F2UL),
        (Sorter.PdqBranchless, 1, 2, 1UL, 0x6177136503314586UL),
        (Sorter.PdqBranchless, 1, 3, 3UL, 0xFA6A7B30E9C4C922UL),
        (Sorter.PdqBranchless, 1, 23, 138UL, 0x4F2A28E730E5E25DUL),
        (Sorter.PdqBranchless, 1, 24, 136UL, 0x423BFB2E5DE61458UL),
        (Sorter.PdqBranchless, 1, 25, 127UL, 0x4BE9888CE424EDAFUL),
        (Sorter.PdqBranchless, 1, 128, 820UL, 0x92E7DD4BD687ADC4UL),
        (Sorter.PdqBranchless, 1, 129, 916UL, 0x7948F2768659C95FUL),
        (Sorter.PdqBranchless, 1, 1000, 5480UL, 0x94FD3C067AAE6BD2UL),
        (Sorter.PdqBranchless, 1, 10000, 55378UL, 0xEA916C9FE53054F1UL),
        (Sorter.PdqBranchless, 1, 100000, 531631UL, 0xA954E65A2E9015C7UL),
        (Sorter.PdqBranchless, 2, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.PdqBranchless, 2, 3, 2UL, 0xA21ECBF5BF1B15EBUL),
        (Sorter.PdqBranchless, 2, 23, 91UL, 0xC76372944BFF8C8EUL),
        (Sorter.PdqBranchless, 2, 24, 100UL, 0x4E070A6523D90561UL),
        (Sorter.PdqBranchless, 2, 25, 49UL, 0x1604AF2EEDCD5B0FUL),
        (Sorter.PdqBranchless, 2, 128, 331UL, 0xE98DD7F81E665592UL),
        (Sorter.PdqBranchless, 2, 129, 409UL, 0xB8FEB1712CECD7F0UL),
        (Sorter.PdqBranchless, 2, 1000, 2547UL, 0x192A37449ECCC641UL),
        (Sorter.PdqBranchless, 2, 10000, 30047UL, 0x8E58A888A640E412UL),
        (Sorter.PdqBranchless, 2, 100000, 250047UL, 0x580998419EF70F9FUL),
        (Sorter.PdqBranchless, 3, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.PdqBranchless, 3, 3, 2UL, 0x82558DF17090A640UL),
        (Sorter.PdqBranchless, 3, 23, 22UL, 0xB922B7193BA46850UL),
        (Sorter.PdqBranchless, 3, 24, 49UL, 0x232FB29FFCCD606BUL),
        (Sorter.PdqBranchless, 3, 25, 51UL, 0x21D7A2CB246CEDFFUL),
        (Sorter.PdqBranchless, 3, 128, 257UL, 0xB1DB5635915A0595UL),
        (Sorter.PdqBranchless, 3, 129, 268UL, 0xC5A7DB9C3CFC46ADUL),
        (Sorter.PdqBranchless, 3, 1000, 2010UL, 0x0FA3AC6CFC31B9EAUL),
        (Sorter.PdqBranchless, 3, 10000, 20010UL, 0x83D8A0F54041B1AFUL),
        (Sorter.PdqBranchless, 3, 100000, 200010UL, 0x1E9A1DB037BACF85UL),
        (Sorter.PdqBranchless, 4, 2, 1UL, 0x727E694B1A1E85A4UL),
        (Sorter.PdqBranchless, 4, 3, 3UL, 0x5CA4B87E1E8054D3UL),
        (Sorter.PdqBranchless, 4, 23, 253UL, 0x6EF5BA549AA1C1CBUL),
        (Sorter.PdqBranchless, 4, 24, 47UL, 0x954DDECEBECF3F24UL),
        (Sorter.PdqBranchless, 4, 25, 58UL, 0xAEAD065F5B6FD468UL),
        (Sorter.PdqBranchless, 4, 128, 387UL, 0x525E7A1BB3E1E597UL),
        (Sorter.PdqBranchless, 4, 129, 610UL, 0xD892AD013EB1E23CUL),
        (Sorter.PdqBranchless, 4, 1000, 3030UL, 0xAE5A507EC1B10ABBUL),
        (Sorter.PdqBranchless, 4, 10000, 30030UL, 0xB433BB03C3E87C2FUL),
        (Sorter.PdqBranchless, 4, 100000, 300030UL, 0x0EF7A3A04C160ADBUL),
        (Sorter.PdqBranchless, 5, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.PdqBranchless, 5, 3, 3UL, 0xFAC73A5BE0A92205UL),
        (Sorter.PdqBranchless, 5, 23, 133UL, 0x0541DF74515CF40BUL),
        (Sorter.PdqBranchless, 5, 24, 147UL, 0x3C38EBB305B18711UL),
        (Sorter.PdqBranchless, 5, 25, 159UL, 0x3C6760775E162D5FUL),
        (Sorter.PdqBranchless, 5, 128, 1230UL, 0x577C6A9667F9229AUL),
        (Sorter.PdqBranchless, 5, 129, 1268UL, 0x6C7F711646686722UL),
        (Sorter.PdqBranchless, 5, 1000, 13511UL, 0x6593FDA378EA0568UL),
        (Sorter.PdqBranchless, 5, 10000, 192931UL, 0x63E7917EB4E52500UL),
        (Sorter.PdqBranchless, 5, 100000, 2521361UL, 0x8DD89276A4776B4DUL),
        (Sorter.PdqBranchless, 6, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.PdqBranchless, 6, 3, 2UL, 0x82558DF17090A640UL),
        (Sorter.PdqBranchless, 6, 23, 22UL, 0xB922B7193BA46850UL),
        (Sorter.PdqBranchless, 6, 24, 49UL, 0x232FB29FFCCD606BUL),
        (Sorter.PdqBranchless, 6, 25, 51UL, 0x21D7A2CB246CEDFFUL),
        (Sorter.PdqBranchless, 6, 128, 1025UL, 0xE521F59390000D02UL),
        (Sorter.PdqBranchless, 6, 129, 996UL, 0x0B52B228A32A3119UL),
        (Sorter.PdqBranchless, 6, 1000, 7363UL, 0xB4C239CBE56A4574UL),
        (Sorter.PdqBranchless, 6, 10000, 70053UL, 0x48049CEF8A346400UL),
        (Sorter.PdqBranchless, 6, 100000, 673843UL, 0x283B6404A019EC11UL),
        (Sorter.PdqBranchless, 7, 2, 1UL, 0x912695CC25D04BFCUL),
        (Sorter.PdqBranchless, 7, 3, 2UL, 0x82558DF17090A640UL),
        (Sorter.PdqBranchless, 7, 23, 25UL, 0x50A087DD819AE0D6UL),
        (Sorter.PdqBranchless, 7, 24, 49UL, 0x5AD796E075DEEAF4UL),
        (Sorter.PdqBranchless, 7, 25, 74UL, 0x5506A4082929F437UL),
        (Sorter.PdqBranchless, 7, 128, 456UL, 0x0FFAE54BB96EDF6FUL),
        (Sorter.PdqBranchless, 7, 129, 487UL, 0x204B084662455179UL),
        (Sorter.PdqBranchless, 7, 1000, 6282UL, 0xF51B7F4714F40A0DUL),
        (Sorter.PdqBranchless, 7, 10000, 99904UL, 0x7FB968F2205321E4UL),
        (Sorter.PdqBranchless, 7, 100000, 1323743UL, 0xFE77AA9FFBBBA992UL),
        (Sorter.PdqBranchless, 8, 2, 1UL, 0xB5010A203C179EAEUL),
        (Sorter.PdqBranchless, 8, 3, 2UL, 0x44AE10B06A7B2899UL),
        (Sorter.PdqBranchless, 8, 23, 22UL, 0x3C18F532DD653FE2UL),
        (Sorter.PdqBranchless, 8, 24, 49UL, 0x96EE9663B736C82FUL),
        (Sorter.PdqBranchless, 8, 25, 56UL, 0xA1282EE95CC1D868UL),
        (Sorter.PdqBranchless, 8, 128, 262UL, 0x9FC3B723E6B75BC3UL),
        (Sorter.PdqBranchless, 8, 129, 273UL, 0x5E79F2E0526538ABUL),
        (Sorter.PdqBranchless, 8, 1000, 2024UL, 0x847C17C000AD722FUL),
        (Sorter.PdqBranchless, 8, 10000, 20024UL, 0xF43BDE45D0A286BEUL),
        (Sorter.PdqBranchless, 8, 100000, 200024UL, 0x10720941B6E7B111UL),
    ];

    sealed class Trace
    {
        public ulong Count;
        public ulong Hash;
    }

    /// <summary>
    /// Orders by the key above the position bits, and adds a mix of each compared pair to the hash, smaller operand first: asking a &lt; b
    /// or b &gt; a is the same question.
    /// </summary>
    readonly struct TraceComparer(Trace trace) : IComparer<long>
    {
        public int Compare(long a, long b)
        {
            trace.Count++;
            var lo = (ulong)Math.Min(a, b);
            var hi = (ulong)Math.Max(a, b);
            var h = lo * 0x9E3779B97F4A7C15UL + hi;
            h = (h ^ (h >> 31)) * 0xBF58476D1CE4E5B9UL;
            trace.Hash += h ^ (h >> 29);
            return (a >> KeyShift).CompareTo(b >> KeyShift);
        }
    }

    static ulong rng;

    static ulong NextRandom()
    {
        var z = rng += 0x9E3779B97F4A7C15UL;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        return z ^ (z >> 31);
    }

    /// <summary>
    /// Shapes: 0 random distinct, 1 sixteen values, 2 two values, 3 sorted, 4 reversed, 5 pipe organ, 6 sawtooth of period 37,
    /// 7 sorted with n / 64 + 1 random exchanges, 8 all equal.
    /// </summary>
    static long[] MakeInput(int shape, int n)
    {
        rng = 0x2545F4914F6CDD1DUL ^ (ulong)(shape * 1000003 + n);
        var v = new long[n];
        for (var i = 0; i < n; i++)
        {
            v[i] = shape switch
            {
                0 => i,
                1 => i % 16,
                2 => i % 2,
                3 => i,
                4 => n - i,
                5 => i < n / 2 ? i : n - i,
                6 => i % 37,
                7 => i,
                _ => 7,
            };
        }
        if (shape <= 2)
        {
            for (var i = n - 1; i > 0; i--)
            {
                var j = (int)(NextRandom() % (ulong)(i + 1));
                (v[i], v[j]) = (v[j], v[i]);
            }
        }
        if (shape == 7)
        {
            for (var k = 0; k < n / 64 + 1; k++)
            {
                var i = (int)(NextRandom() % (ulong)n);
                var j = (int)(NextRandom() % (ulong)n);
                (v[i], v[j]) = (v[j], v[i]);
            }
        }
        for (var i = 0; i < n; i++) v[i] = (v[i] << KeyShift) | (long)i;
        return v;
    }

    [Test]
    public async Task ComparisonTraceMatchesTheReference()
    {
        foreach (var row in Rows)
        {
            var data = MakeInput(row.Shape, row.N);
            var trace = new Trace();
            var comparer = new TraceComparer(trace);
            if (row.Sorter == Sorter.Pdq) PDQSort.Sort(data.AsSpan(), comparer, NullContext.Default);
            else PDQSortBranchless.Sort(data.AsSpan(), comparer, NullContext.Default);

            var label = $"{row.Sorter} shape={row.Shape} n={row.N}";
            await Assert.That(trace.Count).IsEqualTo(row.Count).Because(label);
            await Assert.That(trace.Hash).IsEqualTo(row.Hash).Because(label);
        }
    }
}
