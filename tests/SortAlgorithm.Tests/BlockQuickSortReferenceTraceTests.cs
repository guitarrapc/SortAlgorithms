using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// BlockQuickSort is a port of Edelkamp and Weiß's blocked_double_pivot_check_mosqrt, so its C++ source defines what it must do. Each row
/// holds the comparison count and a hash of the compared pairs that the C++ source produces for the same input. A change to which elements
/// are compared changes that hash even when the output stays sorted. The hash is a sum, so it does not depend on the order in which ranges
/// are sorted. Each key carries its position in its low bits, so the hash also tells equal keys apart. The cases stop at 20,000 elements,
/// above which the pivot sample is selected by std::nth_element, and leave out the inputs that reach std::partial_sort: both are defined
/// by the C++ library. sandbox/BlockQuickSortReferenceTrace/trace.cpp prints the rows.
/// </summary>
public class BlockQuickSortReferenceTraceTests
{
    const int KeyShift = 28;

    static readonly (int Shape, int N, ulong Count, ulong Hash)[] Rows =
    [
        (0, 2, 2UL, 0x224D2B984BA097F8UL),
        (0, 3, 2UL, 0x005CF14DED2B68B4UL),
        (0, 20, 94UL, 0x1A12E3CC2DB90C2EUL),
        (0, 21, 95UL, 0x3C8D5688D3D10AD2UL),
        (0, 22, 109UL, 0xD93920BE4ABA14FAUL),
        (0, 49, 313UL, 0xD873A485BF61277FUL),
        (0, 50, 271UL, 0x2C39D18AD9A232DCUL),
        (0, 51, 296UL, 0x826C659F2D4A7BB1UL),
        (0, 100, 735UL, 0xDD3978E495485F87UL),
        (0, 101, 771UL, 0x4F095607D688844EUL),
        (0, 257, 2184UL, 0x7A4805CA45A13FAAUL),
        (0, 300, 2604UL, 0x72847551A08C1D9DUL),
        (0, 500, 5017UL, 0xBDB7BAFDEA2F962EUL),
        (0, 800, 8672UL, 0xB4C53D4F8AA62CF3UL),
        (0, 801, 8240UL, 0xB055733B9B7F7405UL),
        (0, 1000, 10964UL, 0xACE9036675379C25UL),
        (0, 4096, 52565UL, 0x2D7D3224741D7ACFUL),
        (0, 10000, 143399UL, 0xD21C86BDEFDD80B0UL),
        (0, 20000, 309641UL, 0x154975F94EC46304UL),
        (1, 2, 1UL, 0x6177136503314586UL),
        (1, 3, 4UL, 0xDD30FFBDC2B9003CUL),
        (1, 20, 126UL, 0xEB8B8DFFB7BEFBEDUL),
        (1, 21, 83UL, 0x796788075B6CE8A0UL),
        (1, 22, 137UL, 0x8CC0013E7DAE47FAUL),
        (1, 49, 281UL, 0xA2170FB24E6546EFUL),
        (1, 50, 294UL, 0x0267E2533DE9C352UL),
        (1, 51, 386UL, 0x388E1412B20F241AUL),
        (1, 100, 684UL, 0xD737EAAD110332FAUL),
        (1, 101, 730UL, 0x1DFE4E6464A8A39FUL),
        (1, 257, 1981UL, 0x9CF634D37848B9A1UL),
        (1, 300, 2187UL, 0x8B7F9ADBEC3A086BUL),
        (1, 500, 3604UL, 0xEB680369AD662213UL),
        (1, 800, 6007UL, 0x903B83193E878DB5UL),
        (1, 801, 5762UL, 0x61D4406082130D28UL),
        (1, 1000, 7377UL, 0xFD6BC70CF29EDD79UL),
        (1, 4096, 30120UL, 0xEBB065BA85F5F4F8UL),
        (1, 10000, 82004UL, 0x39E98C6FD779D772UL),
        (1, 20000, 166962UL, 0x72B580A20BF65FD1UL),
        (2, 2, 2UL, 0x224D2B984BA097F8UL),
        (2, 3, 4UL, 0xB1F5F095EF811798UL),
        (2, 20, 100UL, 0xB5C79DA1F301B407UL),
        (2, 21, 63UL, 0x96FFB69505C08FE5UL),
        (2, 22, 64UL, 0xDF3340BA116AA094UL),
        (2, 49, 166UL, 0x63152D0976062E72UL),
        (2, 50, 172UL, 0x19C578D73A402B7AUL),
        (2, 51, 186UL, 0x97A7EFBEA9FA2D65UL),
        (2, 100, 372UL, 0x0C53A706DF8078EAUL),
        (2, 101, 385UL, 0xB1F606346598DC58UL),
        (2, 257, 1154UL, 0x1291AC6333198F08UL),
        (2, 300, 1362UL, 0x36F4242C9399AD5CUL),
        (2, 500, 2244UL, 0xE31A077CCEBB71DFUL),
        (2, 800, 3128UL, 0x1416D37F81DCF0A3UL),
        (2, 801, 4106UL, 0xCFB4B1A8FED91B68UL),
        (2, 1000, 4549UL, 0x19BE824657B322B3UL),
        (2, 4096, 21303UL, 0x06D4CDF83AC34E68UL),
        (2, 10000, 41935UL, 0xEA73F01C9754DDE0UL),
        (2, 20000, 88688UL, 0x1B7C4AFB286E6C42UL),
        (3, 2, 2UL, 0x224D2B984BA097F8UL),
        (3, 3, 4UL, 0x35C94F4DE2018A34UL),
        (3, 20, 38UL, 0xD38BE1B66F7B6821UL),
        (3, 21, 59UL, 0x921DC86990227B58UL),
        (3, 22, 62UL, 0xFC151224044FDD2BUL),
        (3, 49, 187UL, 0x95873378BFB95EBAUL),
        (3, 50, 191UL, 0x6D5653B765EB409DUL),
        (3, 51, 195UL, 0x4E04D3E6766A7257UL),
        (3, 100, 480UL, 0x43B8DD7665B5F563UL),
        (3, 101, 494UL, 0xE02F54A19862C091UL),
        (3, 257, 1526UL, 0xAFD0A8D8C876BE4EUL),
        (3, 300, 1784UL, 0x9FB2F6E2211D571CUL),
        (3, 500, 3473UL, 0x5CF91EB0100BC28BUL),
        (3, 800, 6298UL, 0x120425786DB0058BUL),
        (3, 801, 6336UL, 0x145721415ECA9FA4UL),
        (3, 1000, 7980UL, 0x87CFDF9BC6D52558UL),
        (3, 4096, 40978UL, 0x6C92A1F0950CA71FUL),
        (3, 10000, 110067UL, 0x13E4666B715BBC85UL),
        (3, 20000, 240164UL, 0x7FA5E1FCBA16D504UL),
        (4, 2, 1UL, 0x727E694B1A1E85A4UL),
        (4, 3, 2UL, 0x02DB388F13ED6BF7UL),
        (4, 20, 19UL, 0x7276D0DA4EDAF264UL),
        (4, 21, 67UL, 0xB73A1ED2AB20C02CUL),
        (4, 22, 70UL, 0xEB80F5CB3E1DC7CAUL),
        (4, 49, 205UL, 0x3DFAFD36C742A630UL),
        (4, 50, 209UL, 0x00A44D2B5C364CA3UL),
        (4, 51, 214UL, 0xE502BD12FADD6217UL),
        (4, 100, 557UL, 0xC728D35925120B5AUL),
        (4, 101, 520UL, 0x9E189854B25E523CUL),
        (4, 257, 1659UL, 0x924258018B49F647UL),
        (4, 300, 1943UL, 0x8751010FF83366FFUL),
        (4, 500, 3732UL, 0xE15DCC311E1222A2UL),
        (4, 800, 6830UL, 0xDE2E274808F32A13UL),
        (4, 801, 7064UL, 0x9E909B54B7F3EB01UL),
        (4, 1000, 8865UL, 0xA8911A69B99B94A1UL),
        (4, 4096, 43136UL, 0xB12623D9405A7725UL),
        (4, 10000, 115502UL, 0x622F5CCD090A5C77UL),
        (4, 20000, 251040UL, 0xF2E0349C0DB5D63CUL),
        (5, 2, 2UL, 0x224D2B984BA097F8UL),
        (5, 3, 5UL, 0xAE3AFBBA562C0979UL),
        (5, 20, 119UL, 0x0AAFB7B68027BDC9UL),
        (5, 21, 114UL, 0x20751E7D913720AAUL),
        (5, 22, 125UL, 0xF884E3656C56FB33UL),
        (5, 49, 445UL, 0xCC47FEDD9AFD9B53UL),
        (5, 50, 399UL, 0xC207D3169BD33D6EUL),
        (5, 51, 428UL, 0xFBD29FC11341954AUL),
        (5, 100, 1117UL, 0xC3D56748094BEEE6UL),
        (5, 101, 1088UL, 0xDFD0BA5A4D0161D7UL),
        (5, 257, 3225UL, 0xEAD6F1C242EEBAE7UL),
        (5, 300, 3875UL, 0x306FD9D3A9CF9D16UL),
        (5, 500, 7547UL, 0xEEBCE61FD489679DUL),
        // shape 5, n = 800 reaches std::partial_sort
        (5, 801, 13237UL, 0x12FF2991EBA86854UL),
        (5, 1000, 16920UL, 0xA70B85620D62D7B9UL),
        // shape 5, n = 4096 reaches std::partial_sort
        (5, 10000, 191139UL, 0x21F11D4E9F5ECB52UL),
        (5, 20000, 435430UL, 0xB0636E28BFB87908UL),
        (6, 2, 2UL, 0x224D2B984BA097F8UL),
        (6, 3, 4UL, 0x35C94F4DE2018A34UL),
        (6, 20, 38UL, 0xD38BE1B66F7B6821UL),
        (6, 21, 59UL, 0x921DC86990227B58UL),
        (6, 22, 62UL, 0xFC151224044FDD2BUL),
        (6, 49, 249UL, 0x4899184EC4772B67UL),
        (6, 50, 260UL, 0xC9DF4350D338C966UL),
        (6, 51, 328UL, 0xC42A1257D8C415FCUL),
        (6, 100, 742UL, 0xFA4017EED3068111UL),
        (6, 101, 702UL, 0x5C6ED490A854C669UL),
        (6, 257, 2190UL, 0x21D5C9879CD97571UL),
        (6, 300, 2795UL, 0xB69AEF81FA70913AUL),
        (6, 500, 4452UL, 0xAAF65ED054045748UL),
        (6, 800, 7384UL, 0x9529B02C6DC05830UL),
        (6, 801, 7290UL, 0x2073CC19B33777F2UL),
        (6, 1000, 8906UL, 0x58CB414BD5B8AD7CUL),
        (6, 4096, 36489UL, 0x082D7B7262060AF5UL),
        (6, 10000, 90972UL, 0x87299E4260D4405BUL),
        (6, 20000, 187003UL, 0x14F3D59572FB913CUL),
        (7, 2, 2UL, 0x224D2B984BA097F8UL),
        (7, 3, 4UL, 0x35C94F4DE2018A34UL),
        (7, 20, 71UL, 0x716A0057CC585BE2UL),
        (7, 21, 65UL, 0xA342BCFBFDC134E6UL),
        (7, 22, 69UL, 0xDEB0B4762F814B4EUL),
        (7, 49, 187UL, 0xC5C3C69CC3FF3A8AUL),
        (7, 50, 213UL, 0x58E0BCE6557AA3BDUL),
        (7, 51, 216UL, 0x600A7A958EA6EFC0UL),
        (7, 100, 489UL, 0xC7C0505FA4819D03UL),
        (7, 101, 521UL, 0x5E76715CD0ABDC6CUL),
        (7, 257, 1562UL, 0xFD09C25E87C62B76UL),
        (7, 300, 1805UL, 0xCC0918E698BEB73AUL),
        (7, 500, 3510UL, 0x01B5619D814E50B3UL),
        (7, 800, 6341UL, 0x660E5867ABA84CC2UL),
        (7, 801, 6376UL, 0xB1094A4903CE4413UL),
        (7, 1000, 8171UL, 0x895CA523A3826A01UL),
        (7, 4096, 41361UL, 0x81B9BD625F27AF3BUL),
        (7, 10000, 112685UL, 0x46F95797BE4D869CUL),
        (7, 20000, 244930UL, 0x1E805F011FC546D4UL),
        (8, 2, 2UL, 0x6A021440782F3D5CUL),
        (8, 3, 4UL, 0xEF57DD90810FC351UL),
        (8, 20, 38UL, 0x8B626139529AF081UL),
        (8, 21, 58UL, 0x10415B262EBC9C36UL),
        (8, 22, 52UL, 0x990461A8801C80A0UL),
        (8, 49, 164UL, 0x5F6A49A7303FA531UL),
        (8, 50, 133UL, 0x21E5FF19F34D23D7UL),
        (8, 51, 192UL, 0x1D68ED4A26128AB7UL),
        (8, 100, 315UL, 0xABA5184CDBE9C27CUL),
        (8, 101, 377UL, 0x0B84D419BBCD4369UL),
        (8, 257, 1151UL, 0x5F0555CE4E4D6197UL),
        (8, 300, 1017UL, 0x0042910C44950A16UL),
        (8, 500, 1823UL, 0x142C316D74A23E62UL),
        (8, 800, 3749UL, 0xE33E2F5657666B94UL),
        (8, 801, 4191UL, 0x9C7BF886FE32D028UL),
        (8, 1000, 4175UL, 0x32E805A6AB36D0E6UL),
        (8, 4096, 24473UL, 0x7A67B764AC7AE2FBUL),
        (8, 10000, 51473UL, 0x6DA8E927D5948D4DUL),
        (8, 20000, 112945UL, 0xA9B210D1E0109B96UL),
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
            BlockQuickSort.Sort(data.AsSpan(), new TraceComparer(trace), NullContext.Default);

            var label = $"shape={row.Shape} n={row.N}";
            await Assert.That(trace.Count).IsEqualTo(row.Count).Because(label);
            await Assert.That(trace.Hash).IsEqualTo(row.Hash).Because(label);
        }
    }
}
