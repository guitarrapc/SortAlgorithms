#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0

var big = Enumerable.Range(1, 100).ToArray();
var src = 75; var dst = 10;
var elem = big[src];
Array.Copy(big, dst, big, dst + 1, src - dst);
big[dst] = elem;
Console.WriteLine($"SingleElementMoved n=100: moved value {elem} from index {src} to index {dst}");
Console.WriteLine();
var pos = 0;
var runs = new List<(int start, int end, string kind)>();
while (pos < big.Length)
{
    var s2 = pos; var e2 = pos + 1;
    if (e2 >= big.Length) { runs.Add((s2, big.Length, "single")); break; }
    if (big[e2 - 1] > big[e2]) { while (e2 < big.Length && big[e2 - 1] > big[e2]) e2++; runs.Add((s2, e2, "DESC")); }
    else { while (e2 < big.Length && big[e2 - 1] <= big[e2]) e2++; runs.Add((s2, e2, "asc")); }
    pos = e2;
}
Console.WriteLine("Natural runs:");
foreach (var (s, e, k) in runs) Console.WriteLine($"  [{s}..{e}) len={e-s} {k} vals=[{big[s]}..{big[e-1]}]");
Console.WriteLine();
if (runs.Count >= 2)
{
    var r1s = runs[0].start; var r1e = runs[0].end; var r1len = r1e - r1s;
    var r2s = runs[1].start; var r2e = runs[1].end; var r2len = r2e - r2s;
    Console.WriteLine($"=== Merge run0 [{r1s}..{r1e}) len={r1len} + run1 [{r2s}..{r2e}) len={r2len} ===");
    var key2 = big[r2s];
    var ip = Array.BinarySearch(big, r1s, r1len, key2);
    if (ip < 0) ip = ~ip;
    var trim = ip - r1s;
    var nb1 = r1s + trim; var nl1 = r1len - trim;
    Console.WriteLine($"TimSort GallopRight: skip {trim} from left (run2[0]={key2})");
    if (nl1 > 0)
    {
        var last = big[nb1 + nl1 - 1];
        ip = Array.BinarySearch(big, r2s, r2len, last);
        if (ip < 0) ip = ~ip;
        var nl2 = ip - r2s;
        Console.WriteLine($"TimSort GallopLeft:  trim run2 to len={nl2} (last={last})");
        Console.WriteLine($"TimSort actual merge: {nl1}+{nl2}={nl1+nl2} elements (buffer={Math.Min(nl1,nl2)})");
        if (nl1 == 0 || nl2 == 0) Console.WriteLine("  -> EARLY RETURN, no buffer!");
    }
    else Console.WriteLine("TimSort: len1=0 -> EARLY RETURN!");
    Console.WriteLine();
    Console.WriteLine($"PowerSort (no trim): buffer={Math.Min(r1len,r2len)}, merge all {r1len+r2len} elements");
}
