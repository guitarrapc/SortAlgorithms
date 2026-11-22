using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

if (args.Length < 3)
{
    Console.Error.WriteLine("Usage: UpdateBenchmarkReadme.cs <artifacts-dir> <readme-path> <workflow-run-url> [--run <run.json> --image-dir <dir> --site-url <url>]");
    Environment.Exit(1);
}

var artifactsDir = args[0];
var readmePath = args[1];
var runUrl = args[2];
var options = new Dictionary<string, string>(StringComparer.Ordinal);
for (var i = 3; i + 1 < args.Length; i += 2)
{
    options[args[i].TrimStart('-')] = args[i + 1];
}

const string StartMarker = "<!-- BENCHMARK_START -->";
const string EndMarker = "<!-- BENCHMARK_END -->";

var summary = "";
if (options.TryGetValue("run", out var runJson))
{
    var imageDir = options.GetValueOrDefault("image-dir", ".github/images/benchmark");
    var siteUrl = options.GetValueOrDefault("site-url", "");
    var readmeDir = Path.GetDirectoryName(Path.GetFullPath(readmePath))!;
    summary = BenchmarkSummary.Build(runJson, imageDir, Path.GetRelativePath(readmeDir, Path.GetFullPath(imageDir)).Replace('\\', '/'), siteUrl);
}

var section = summary + BuildBenchmarkSection(artifactsDir, runUrl);
var readme = File.ReadAllText(readmePath);
var updated = ReplaceMarkedSection(readme, StartMarker, EndMarker, section);
File.WriteAllText(readmePath, updated, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

static string BuildBenchmarkSection(string artifactsDir, string runUrl)
{
    var reportFiles = Directory
        .EnumerateFiles(artifactsDir, "*-report-*.md", SearchOption.AllDirectories)
        .Select(path => new ReportFile(path))
        .Where(report => report.Key is not null)
        .GroupBy(report => report.Key!)
        .Select(group => group.OrderBy(report => report.PreferGitHub).First())
        .OrderBy(report => report.Key, StringComparer.Ordinal)
        .ToList();

    var builder = new StringBuilder();
    builder.AppendLine("<details>");
    builder.AppendLine($"<summary>Benchmark results ({DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC)</summary>");
    builder.AppendLine();
    builder.AppendLine($"Workflow run: {runUrl}");
    builder.AppendLine();

    if (reportFiles.Count == 0)
    {
        builder.AppendLine("_No benchmark report files found in artifacts._");
    }
    else
    {
        foreach (var report in reportFiles)
        {
            // Its table alone would push the README past the size GitHub renders.
            if (report.Key == "ScalingBenchmark")
            {
                builder.AppendLine($"### {report.Title}");
                builder.AppendLine();
                builder.AppendLine("The table is not listed here because of its length. The results are on the benchmark site (category Scaling) and in the artifacts of the workflow run.");
                builder.AppendLine();
                continue;
            }
            AppendReport(builder, report);
        }
    }

    builder.AppendLine("</details>");
    return builder.ToString().TrimEnd() + Environment.NewLine;
}

static void AppendReport(StringBuilder builder, ReportFile report)
{
    builder.AppendLine($"### {report.Title}");
    builder.AppendLine();

    var lines = File.ReadAllLines(report.FilePath);
    // The header ends where the result table begins. Its line count varies with the
    // BenchmarkDotNet version (0.16 added a "Memory:" line) and with the job count.
    var machineInfo = lines
        .TakeWhile(static line => !line.Contains('|', StringComparison.Ordinal))
        .Where(static line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("```", StringComparison.Ordinal))
        .ToList();

    if (machineInfo.Count > 0)
    {
        builder.AppendLine("```");
        foreach (var line in machineInfo)
        {
            builder.AppendLine(line);
        }
        builder.AppendLine("```");
        builder.AppendLine();
    }

    foreach (var tableLine in ExtractTableLines(lines))
    {
        builder.AppendLine(tableLine);
    }

    builder.AppendLine();
}

static IEnumerable<string> ExtractTableLines(string[] lines)
{
    if (lines.Any(static line => line.StartsWith('|')))
    {
        return lines.Where(static line => line.StartsWith('|'));
    }

    return lines
        .Select(static line => line.TrimStart())
        .Where(static line => line.Contains('|', StringComparison.Ordinal))
        .Select(static line => line.StartsWith('|') ? line : "| " + line)
        .Select(static line => line.StartsWith("| |", StringComparison.Ordinal) ? "|" + line[2..] : line);
}

static string ReplaceMarkedSection(string readme, string startMarker, string endMarker, string section)
{
    var start = readme.IndexOf(startMarker, StringComparison.Ordinal);
    var end = readme.IndexOf(endMarker, StringComparison.Ordinal);

    if (start < 0 || end < 0 || end < start)
    {
        throw new InvalidOperationException($"README markers not found: {startMarker} ... {endMarker}");
    }

    var before = readme[..(start + startMarker.Length)];
    var after = readme[end..];
    return before + Environment.NewLine + section + Environment.NewLine + after;
}

internal sealed partial class ReportFile(string filePath)
{
    private static readonly Regex NameRegex = new(
        @"SortAlgorithm\.Benchmark\.([^/\\]+)-report-(github|default)\.md$",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    public string FilePath { get; } = filePath;

    public string? Key { get; } = ExtractKey(filePath);

    public bool PreferGitHub { get; } = filePath.Contains("-report-github.md", StringComparison.Ordinal);

    public string Title => Key ?? System.IO.Path.GetFileName(FilePath);

    private static string? ExtractKey(string filePath)
    {
        var fileName = System.IO.Path.GetFileName(filePath);
        var match = NameRegex.Match(fileName);
        return match.Success ? match.Groups[1].Value : null;
    }
}

// Summary placed above the full tables: a speed-versus-stability chart (SVG, light and dark) and the
// fastest algorithm per category and input pattern. Reads the run file written by BenchmarkData.cs.
internal static class BenchmarkSummary
{
    private static readonly string[] Patterns = ["Random", "SingleElementMoved", "Sorted", "Reversed", "PipeOrgan", "ManyDuplicates"];
    private static readonly Dictionary<string, string> PatternShort = new(StringComparer.Ordinal)
    {
        ["Random"] = "Random", ["SingleElementMoved"] = "1-Moved", ["Sorted"] = "Sorted",
        ["Reversed"] = "Reversed", ["PipeOrgan"] = "PipeOrgan", ["ManyDuplicates"] = "ManyDup",
    };
    private static readonly string[] CategoryOrder = ["Exchange", "Selection", "Insertion", "Merge", "MergeSlow", "Heap", "Partition", "Distribution", "Network", "Tree"];
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    private sealed record Row(string Category, string Method, int Size, string Pattern, double Mean, double Error, double StdDev)
    {
        public double Cv => Mean > 0 ? StdDev / Mean : 0;
    }

    private sealed record Theme(string Name, string Ink, string Ink2, string Muted, string Grid, string Accent, string Halo);

    private static readonly Theme Light = new("light", "#0b0b0b", "#52514e", "#898781", "#e1e0d9", "#2a78d6", "#ffffff");
    private static readonly Theme Dark = new("dark", "#f0efec", "#c3c2b7", "#898781", "#2c2c2a", "#3987e5", "#0d1117");

    public static string Build(string runPath, string imageDir, string imageRel, string siteUrl)
    {
        using var doc = JsonDocument.Parse(File.ReadAllBytes(runPath));
        var root = doc.RootElement;
        var reference = root.TryGetProperty("reference", out var refEl) ? refEl.GetString() ?? "DotnetSort" : "DotnetSort";
        var rows = root.GetProperty("cases").EnumerateArray().Select(static c => new Row(
            c.GetProperty("category").GetString()!,
            c.GetProperty("method").GetString()!,
            c.GetProperty("size").GetInt32(),
            c.GetProperty("pattern").GetString()!,
            c.GetProperty("mean").GetDouble(),
            c.TryGetProperty("error", out var e) && e.ValueKind == JsonValueKind.Number ? e.GetDouble() : 0,
            c.GetProperty("stdDev").GetDouble())).ToList();

        var categories = rows.Select(static r => r.Category).Distinct().OrderBy(c => Array.IndexOf(CategoryOrder, c) is var i && i < 0 ? int.MaxValue : i).ThenBy(static c => c, StringComparer.Ordinal).ToList();
        var leagueSize = rows.GroupBy(static r => r.Size).Where(g => g.Select(static r => r.Category).Distinct().Count() >= 2).Select(static g => g.Key).DefaultIfEmpty(rows.Max(static r => r.Size)).Max();

        Directory.CreateDirectory(imageDir);
        var points = rows.Where(r => r.Size == leagueSize && r.Pattern == "Random").ToList();
        var utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        File.WriteAllText(Path.Combine(imageDir, "pareto-light.svg"), ParetoSvg(points, leagueSize, reference, Light), utf8);
        File.WriteAllText(Path.Combine(imageDir, "pareto-dark.svg"), ParetoSvg(points, leagueSize, reference, Dark), utf8);

        var sb = new StringBuilder();
        if (siteUrl.Length > 0)
        {
            sb.AppendLine($"Interactive results: {siteUrl} (rankings per size, scaling exponents, run-to-run stability, history).");
            sb.AppendLine();
        }
        sb.AppendLine("<picture>");
        sb.AppendLine($"  <source media=\"(prefers-color-scheme: dark)\" srcset=\"{imageRel}/pareto-dark.svg\">");
        sb.AppendLine($"  <img alt=\"Mean time against run-to-run variation (CV) for every algorithm measured at n = {leagueSize}, Random input\" src=\"{imageRel}/pareto-light.svg\">");
        sb.AppendLine("</picture>");
        sb.AppendLine();
        sb.AppendLine("#### Fastest per category and input pattern");
        sb.AppendLine();
        sb.AppendLine($"Measured at each category's largest size. \"≈ 2nd\" means the 99.9% confidence intervals of the two fastest overlap. 🟢 🟡 🔴 mark a CV below 2%, below 5%, and 5% or more. {reference} is a shared reference and is not ranked.");
        sb.AppendLine();
        sb.AppendLine($"| Category | n | {string.Join(" | ", Patterns.Select(static p => PatternShort[p]))} |");
        sb.AppendLine($"|---|--:|{string.Concat(Enumerable.Repeat("---|", Patterns.Length))}");
        foreach (var category in categories)
        {
            var n = rows.Where(r => r.Category == category).Max(static r => r.Size);
            sb.Append($"| {category} | {n} |");
            foreach (var pattern in Patterns)
            {
                var ranked = rows.Where(r => r.Category == category && r.Size == n && r.Pattern == pattern && r.Method != reference).OrderBy(static r => r.Mean).ToList();
                if (ranked.Count == 0)
                {
                    sb.Append(" — |");
                    continue;
                }
                var first = ranked[0];
                var note = ranked.Count < 2 ? "only one"
                    : first.Mean + first.Error >= ranked[1].Mean - ranked[1].Error ? "≈ 2nd"
                    : $"×{(ranked[1].Mean / first.Mean).ToString("0.00", Inv)} vs 2nd";
                sb.Append($" **{first.Method}**<br>{note} {CvMark(first.Cv)} |");
            }
            sb.AppendLine();
        }
        sb.AppendLine();
        return sb.ToString();
    }

    private static string CvMark(double cv) => cv < 0.02 ? "🟢" : cv < 0.05 ? "🟡" : "🔴";

    private static string ParetoSvg(List<Row> all, int size, string reference, Theme t)
    {
        const double W = 900, H = 440, L = 58, R = 190, T = 40, B = 44;
        var list = all.Where(r => r.Method != reference).ToList();
        var refRow = all.Where(r => r.Method == reference).OrderBy(static r => r.Category == "Partition" ? 0 : 1).FirstOrDefault();
        List<Row> pts = refRow is null ? list : [.. list, refRow];
        var lo = Math.Pow(10, Math.Floor(Math.Log10(pts.Min(static r => r.Mean))));
        var hi = Math.Pow(10, Math.Ceiling(Math.Log10(pts.Max(static r => r.Mean))));
        var cvMax = Math.Max(0.03, pts.Max(static r => r.Cv) * 1.05);
        double X(double v) => L + (Math.Log10(v) - Math.Log10(lo)) / (Math.Log10(hi) - Math.Log10(lo)) * (W - L - R);
        // Square-root scale: one noisy case must not flatten the sub-percent CVs of everything else.
        double Y(double v) => H - B - Math.Sqrt(v / cvMax) * (H - T - B);
        string F(double v) => v.ToString("0.#", Inv);

        var frontier = new List<Row>();
        var best = double.PositiveInfinity;
        foreach (var r in list.OrderBy(static r => r.Mean))
        {
            if (r.Cv < best)
            {
                frontier.Add(r);
                best = r.Cv;
            }
        }

        var s = new StringBuilder();
        s.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {W} {H}\" width=\"{W}\" height=\"{H}\" font-family=\"-apple-system, 'Segoe UI', Helvetica, Arial, sans-serif\" font-size=\"12\">");
        s.AppendLine($"<text x=\"{L}\" y=\"18\" fill=\"{t.Ink}\" font-size=\"14\" font-weight=\"600\">Speed and run-to-run variation · n = {size} · Random input</text>");
        s.AppendLine($"<text x=\"{L}\" y=\"34\" fill=\"{t.Muted}\">Each dot is one algorithm. Named dots are not beaten on both mean time and CV by any other algorithm.</text>");
        var ticks = new List<double>();
        for (var e = Math.Log10(lo); e <= Math.Log10(hi) + 1e-9; e++)
        {
            foreach (var m in new[] { 1.0, 2.0, 5.0 })
            {
                var v = m * Math.Pow(10, e);
                if (v >= lo * 0.999 && v <= hi * 1.001) ticks.Add(v);
            }
        }
        if (ticks.Count > 9) ticks = ticks.Where(v => Math.Abs(Math.Log10(v) - Math.Round(Math.Log10(v))) < 1e-9).ToList();
        foreach (var v in ticks)
        {
            s.AppendLine($"<text x=\"{F(X(v))}\" y=\"{F(H - B + 16)}\" fill=\"{t.Muted}\" text-anchor=\"middle\" font-size=\"11\">{FormatTime(v)}</text>");
        }
        foreach (var v in new[] { 0, 0.005, 0.01, 0.02, 0.05, 0.1, 0.2, 0.5 }.Where(v => v <= cvMax))
        {
            s.AppendLine($"<line x1=\"{L}\" x2=\"{F(W - R)}\" y1=\"{F(Y(v))}\" y2=\"{F(Y(v))}\" stroke=\"{t.Grid}\"/>");
            s.AppendLine($"<text x=\"{L - 8}\" y=\"{F(Y(v) + 4)}\" fill=\"{t.Muted}\" text-anchor=\"end\" font-size=\"11\">{(v * 100).ToString("0.#", Inv)}%</text>");
        }
        s.AppendLine($"<text x=\"{F(W - R)}\" y=\"{H - 8}\" fill=\"{t.Muted}\" text-anchor=\"end\">Mean time (log scale) → slower</text>");
        s.AppendLine($"<text x=\"{L + 4}\" y=\"{T + 10}\" fill=\"{t.Muted}\">↑ CV, square-root scale (less stable)</text>");

        var path = new StringBuilder();
        for (var i = 0; i < frontier.Count; i++)
        {
            path.Append(i == 0 ? 'M' : 'L').Append(F(X(frontier[i].Mean))).Append(',').Append(F(Y(frontier[i].Cv)));
            if (i + 1 < frontier.Count) path.Append('L').Append(F(X(frontier[i + 1].Mean))).Append(',').Append(F(Y(frontier[i].Cv)));
        }
        if (frontier.Count > 1) s.AppendLine($"<path d=\"{path}\" fill=\"none\" stroke=\"{t.Ink2}\" stroke-width=\"1.5\"/>");
        foreach (var r in list.Where(r => !frontier.Contains(r)))
        {
            s.AppendLine($"<circle cx=\"{F(X(r.Mean))}\" cy=\"{F(Y(r.Cv))}\" r=\"3.6\" fill=\"{t.Muted}\" fill-opacity=\"0.5\"/>");
        }
        foreach (var r in frontier)
        {
            s.AppendLine($"<circle cx=\"{F(X(r.Mean))}\" cy=\"{F(Y(r.Cv))}\" r=\"5\" fill=\"{t.Accent}\" stroke=\"{t.Halo}\" stroke-width=\"2\"/>");
        }
        // Labels go above-right of their dot, clear of the frontier's horizontal steps, and move up past
        // earlier labels they would overlap; a leader line connects a moved label to its dot.
        var placed = new List<(double X0, double X1, double Y)>();
        void Label(double px, double py, string text, string fill)
        {
            var x0 = px + 7;
            var x1 = x0 + text.Length * 6.6;
            var y0 = Math.Min(py - 9, H - B - 6);
            var y = y0;
            while (placed.Any(p => p.X0 < x1 && x0 < p.X1 && Math.Abs(p.Y - y) < 13) && y > T + 24) y -= 13;
            placed.Add((x0, x1, y));
            if (y0 - y > 3) s.AppendLine($"<line x1=\"{F(px + 2)}\" y1=\"{F(py - 4)}\" x2=\"{F(x0 - 1)}\" y2=\"{F(y + 2)}\" stroke=\"{t.Muted}\" stroke-width=\"0.8\"/>");
            s.AppendLine($"<text x=\"{F(x0)}\" y=\"{F(y + 4)}\" fill=\"{fill}\" font-size=\"11.5\">{Escape(text)}</text>");
        }
        if (refRow is not null)
        {
            s.AppendLine($"<circle cx=\"{F(X(refRow.Mean))}\" cy=\"{F(Y(refRow.Cv))}\" r=\"5.5\" fill=\"none\" stroke=\"{t.Ink}\" stroke-width=\"1.5\"/>");
        }
        foreach (var r in frontier)
        {
            Label(X(r.Mean), Y(r.Cv), r.Method, t.Ink);
        }
        if (refRow is not null)
        {
            Label(X(refRow.Mean), Y(refRow.Cv), $"{reference} (reference)", t.Muted);
        }
        s.AppendLine("</svg>");
        return s.ToString();
    }

    private static string FormatTime(double ns) =>
        ns < 1e3 ? $"{ns.ToString("0.##", Inv)} ns" : ns < 1e6 ? $"{(ns / 1e3).ToString("0.##", Inv)} μs" : ns < 1e9 ? $"{(ns / 1e6).ToString("0.##", Inv)} ms" : $"{(ns / 1e9).ToString("0.##", Inv)} s";

    private static string Escape(string s) => s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}
