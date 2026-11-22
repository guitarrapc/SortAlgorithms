using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

// Builds the data that the benchmark site reads.
//
//   convert        BenchmarkDotNet *-report-full*.json  ->  one run file
//   import-readme  README benchmark sections in git history  ->  one run file per past run
//   index          runs/*.json  ->  index.json + history.json
//
// The run file format is described in .github/docs/references/benchmark_site.md.

const int Schema = 1;
const string ReferenceMethod = "DotnetSort";
var inv = CultureInfo.InvariantCulture;

if (args.Length == 0)
{
    Usage();
    return 1;
}

var opts = ParseOptions(args.Skip(1).ToArray());
switch (args[0])
{
    case "convert":
        return ConvertReports(opts);
    case "import-readme":
        return ImportReadme(opts);
    case "index":
        return BuildIndex(opts);
    default:
        Usage();
        return 1;
}

static void Usage()
{
    Console.Error.WriteLine("""
        Usage:
          BenchmarkData.cs convert --artifacts <dir> --repo <dir> --out <file> [--run-url <url>] [--commit <sha>] [--timestamp <iso8601>]
          BenchmarkData.cs import-readme --repo <dir> --out-dir <dir>
          BenchmarkData.cs index --data <dir>
        """);
}

static Dictionary<string, string> ParseOptions(string[] a)
{
    var d = new Dictionary<string, string>(StringComparer.Ordinal);
    for (var i = 0; i < a.Length; i++)
    {
        if (!a[i].StartsWith("--", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Unexpected argument: {a[i]}");
        }
        d[a[i][2..]] = i + 1 < a.Length && !a[i + 1].StartsWith("--", StringComparison.Ordinal) ? a[++i] : "true";
    }
    return d;
}

static string Require(Dictionary<string, string> o, string key) =>
    o.TryGetValue(key, out var v) ? v : throw new ArgumentException($"Missing --{key}");

// ---------------------------------------------------------------- convert

int ConvertReports(Dictionary<string, string> o)
{
    var artifacts = Require(o, "artifacts");
    var repo = Require(o, "repo");
    var outPath = Require(o, "out");
    var files = Directory.EnumerateFiles(artifacts, "*-report-full*.json", SearchOption.AllDirectories)
        .Where(static f => !Path.GetFileName(f).Contains("SortAudit", StringComparison.Ordinal))
        .OrderBy(static f => f, StringComparer.Ordinal)
        .ToList();
    if (files.Count == 0)
    {
        Console.Error.WriteLine($"No *-report-full*.json under {artifacts}");
        return 1;
    }

    var cases = new List<Case>();
    var env = new Dictionary<string, string>(StringComparer.Ordinal);
    foreach (var file in files)
    {
        using var doc = JsonDocument.Parse(File.ReadAllBytes(file));
        var root = doc.RootElement;
        if (env.Count == 0 && root.TryGetProperty("HostEnvironmentInfo", out var host))
        {
            env["cpu"] = Str(host, "ProcessorName");
            env["cores"] = $"{Num(host, "PhysicalProcessorCount")} CPU, {Num(host, "LogicalCoreCount")} logical and {Num(host, "PhysicalCoreCount")} physical cores";
            env["os"] = Str(host, "OsVersion");
            env["runtime"] = Str(host, "RuntimeVersion");
            env["sdk"] = Str(host, "DotNetCliVersion");
            env["benchmarkDotNet"] = Str(host, "BenchmarkDotNetVersion");
        }

        foreach (var b in root.GetProperty("Benchmarks").EnumerateArray())
        {
            var type = Str(b, "Type");
            var method = Str(b, "Method");
            var p = ParseParameters(Str(b, "Parameters"));
            if (!p.TryGetValue("Size", out var sizeText) || !p.TryGetValue("Pattern", out var pattern))
            {
                continue; // not a Size x Pattern benchmark
            }
            if (!b.TryGetProperty("Statistics", out var st) || st.ValueKind != JsonValueKind.Object)
            {
                Console.Error.WriteLine($"skip {type}.{method} {sizeText}/{pattern}: no statistics (failed run?)");
                continue;
            }

            var measurements = new List<double>();
            if (b.TryGetProperty("Measurements", out var ms))
            {
                foreach (var m in ms.EnumerateArray())
                {
                    if (Str(m, "IterationMode") == "Workload" && Str(m, "IterationStage") == "Actual")
                    {
                        var ops = m.GetProperty("Operations").GetDouble();
                        measurements.Add(Round(m.GetProperty("Nanoseconds").GetDouble() / ops, 1));
                    }
                }
            }

            double? allocated = null;
            if (b.TryGetProperty("Memory", out var mem) && mem.ValueKind == JsonValueKind.Object && mem.TryGetProperty("BytesAllocatedPerOperation", out var bytes))
            {
                allocated = bytes.GetDouble();
            }

            cases.Add(new Case(
                Category: type.EndsWith("Benchmark", StringComparison.Ordinal) ? type[..^"Benchmark".Length] : type,
                Method: method,
                Size: int.Parse(sizeText, inv),
                Pattern: pattern,
                Mean: st.GetProperty("Mean").GetDouble(),
                Median: st.GetProperty("Median").GetDouble(),
                StdDev: st.GetProperty("StandardDeviation").GetDouble(),
                Error: st.TryGetProperty("ConfidenceInterval", out var ci) ? ci.GetProperty("Margin").GetDouble() : null,
                N: st.GetProperty("N").GetInt32(),
                Allocated: allocated,
                Measurements: measurements));
        }
    }

    var timestamp = o.TryGetValue("timestamp", out var ts) ? DateTimeOffset.Parse(ts, inv).ToUniversalTime() : DateTimeOffset.UtcNow;
    o.TryGetValue("commit", out var commit);
    o.TryGetValue("run-url", out var runUrl);
    var run = new Run(
        Id: MakeId(timestamp, commit is null ? "local" : commit[..Math.Min(7, commit.Length)]),
        Source: "ci",
        Timestamp: timestamp,
        Commit: commit,
        RunUrl: runUrl,
        Environment: env,
        Algorithms: ReadAlgorithms(repo, cases.Select(static c => c.Method)),
        Cases: cases);
    WriteRun(run, outPath);
    Console.WriteLine($"{outPath}: {cases.Count} cases from {files.Count} report(s)");
    return 0;
}

static Dictionary<string, string> ParseParameters(string text) =>
    text.Split('&', StringSplitOptions.RemoveEmptyEntries)
        .Select(static kv => kv.Split('=', 2))
        .Where(static kv => kv.Length == 2)
        .ToDictionary(static kv => kv[0], static kv => kv[1], StringComparer.Ordinal);

static string Str(JsonElement e, string name) =>
    e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() ?? "" : "";

static string Num(JsonElement e, string name) =>
    e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetRawText() : "?";

// ---------------------------------------------------------------- import-readme

int ImportReadme(Dictionary<string, string> o)
{
    var repo = Require(o, "repo");
    var outDir = Require(o, "out-dir");
    Directory.CreateDirectory(outDir);
    var commits = Git(repo, "log", "--format=%H", "--", "README.md")
        .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Reverse()
        .ToList();
    var seen = new HashSet<string>(StringComparer.Ordinal);
    var written = 0;
    foreach (var sha in commits)
    {
        string text;
        try
        {
            text = Git(repo, "show", $"{sha}:README.md");
        }
        catch (InvalidOperationException)
        {
            continue;
        }
        var parsed = ParseReadmeSection(text);
        if (parsed is null || parsed.Value.Cases.Count == 0 || !seen.Add(parsed.Value.Stamp))
        {
            continue;
        }
        var (stamp, cpu, url, cases, envLines) = parsed.Value;
        var timestamp = DateTimeOffset.ParseExact(stamp, "yyyy-MM-dd HH:mm 'UTC'", inv, DateTimeStyles.AssumeUniversal);
        var env = new Dictionary<string, string>(StringComparer.Ordinal) { ["cpu"] = cpu };
        foreach (var line in envLines)
        {
            if (line.StartsWith("BenchmarkDotNet v", StringComparison.Ordinal))
            {
                var parts = line["BenchmarkDotNet v".Length..].Split(',', 2);
                env["benchmarkDotNet"] = parts[0].Trim();
                if (parts.Length > 1) env["os"] = parts[1].Trim();
            }
            else if (line.StartsWith(".NET SDK", StringComparison.Ordinal)) env["sdk"] = line[".NET SDK".Length..].Trim();
            else if (line.StartsWith("[Host]", StringComparison.Ordinal)) env["runtime"] = line[(line.IndexOf(':') + 1)..].Trim();
            else if (line.Contains("physical cores", StringComparison.Ordinal)) env["cores"] = line[(line.IndexOf(',') + 1)..].Trim();
        }
        var run = new Run(
            Id: MakeId(timestamp, "readme"),
            Source: "readme",
            Timestamp: timestamp,
            Commit: null,
            RunUrl: url,
            Environment: env,
            Algorithms: ReadAlgorithms(repo, cases.Select(static c => c.Method)),
            Cases: cases);
        WriteRun(run, Path.Combine(outDir, run.Id + ".json"));
        written++;
    }
    Console.WriteLine($"{outDir}: imported {written} run(s) from {commits.Count} README commit(s)");
    return 0;
}

(string Stamp, string Cpu, string? Url, List<Case> Cases, List<string> EnvLines)? ParseReadmeSection(string text)
{
    var lines = text.Replace("\r\n", "\n").Split('\n');
    var start = Array.FindIndex(lines, static l => l.Contains("<!-- BENCHMARK_START -->", StringComparison.Ordinal));
    var end = Array.FindIndex(lines, static l => l.Contains("<!-- BENCHMARK_END -->", StringComparison.Ordinal));
    if (start < 0 || end < start)
    {
        return null;
    }
    string? stamp = null, cpu = null, url = null, category = null;
    Dictionary<string, int>? cols = null;
    var cases = new List<Case>();
    var envLines = new List<string>();
    var inFence = false;
    for (var i = start; i < end; i++)
    {
        var l = lines[i];
        var m = Regex.Match(l, @"Benchmark results \((\d{4}-\d\d-\d\d \d\d:\d\d UTC)\)");
        if (m.Success) { stamp = m.Groups[1].Value; continue; }
        if (l.StartsWith("Workflow run:", StringComparison.Ordinal)) { url = l["Workflow run:".Length..].Trim(); continue; }
        if (l.StartsWith("```", StringComparison.Ordinal)) { inFence = !inFence; continue; }
        if (inFence)
        {
            if (cpu is null && Regex.IsMatch(l, "^(AMD|Intel|Apple|ARM)")) cpu = l.Split(',')[0].Trim();
            if (category is not null && envLines.Count < 12) envLines.Add(l.Trim());
            continue;
        }
        if (l.StartsWith("### ", StringComparison.Ordinal))
        {
            category = l[4..].Trim();
            if (category.EndsWith("Benchmark", StringComparison.Ordinal)) category = category[..^"Benchmark".Length];
            cols = null;
            continue;
        }
        if (!l.StartsWith('|') || category is null) continue;
        var cells = l.Trim().Trim('|').Split('|').Select(static c => c.Replace("**", "").Trim()).ToArray();
        if (cells[0] == "Method")
        {
            cols = cells.Select(static (c, idx) => (c, idx)).ToDictionary(static t => t.c, static t => t.idx);
            continue;
        }
        if (cols is null || cells[0].Length == 0 || cells[0].StartsWith('-') || cells[0].StartsWith(':')) continue;
        if (!cols.TryGetValue("Size", out var si) || !cols.TryGetValue("Pattern", out var pi)) continue;
        var mean = ParseTime(cells[cols["Mean"]]);
        if (mean is null || !int.TryParse(cells[si], NumberStyles.Integer, inv, out var size)) continue;
        cases.Add(new Case(
            Category: category,
            Method: cells[cols["Method"]],
            Size: size,
            Pattern: cells[pi],
            Mean: mean.Value,
            Median: null,
            StdDev: ParseTime(cells[cols["StdDev"]]) ?? 0,
            Error: ParseTime(cells[cols["Error"]]),
            N: null,
            Allocated: cols.TryGetValue("Allocated", out var ai) ? ParseBytes(cells[ai]) : null,
            Measurements: []));
    }
    return stamp is null ? null : (stamp, cpu ?? "unknown", url, cases, envLines);
}

double? ParseTime(string s)
{
    var m = Regex.Match(s.Trim(), @"^([\d,\.]+)\s*(ns|μs|us|ms|s)$");
    if (!m.Success) return null;
    var v = double.Parse(m.Groups[1].Value.Replace(",", ""), inv);
    return m.Groups[2].Value switch { "ns" => v, "μs" or "us" => v * 1e3, "ms" => v * 1e6, _ => v * 1e9 };
}

double? ParseBytes(string s)
{
    s = s.Trim();
    if (s is "-" or "0 B") return 0;
    var m = Regex.Match(s, @"^([\d,\.]+)\s*(B|KB|MB|GB)$");
    if (!m.Success) return null;
    var v = double.Parse(m.Groups[1].Value.Replace(",", ""), inv);
    return m.Groups[2].Value switch { "B" => v, "KB" => v * 1024, "MB" => v * 1024 * 1024, _ => v * 1024 * 1024 * 1024 };
}

static string Git(string repo, params string[] args)
{
    var psi = new ProcessStartInfo("git") { WorkingDirectory = repo, RedirectStandardOutput = true, RedirectStandardError = true, StandardOutputEncoding = Encoding.UTF8 };
    foreach (var a in args) psi.ArgumentList.Add(a);
    using var p = Process.Start(psi)!;
    var output = p.StandardOutput.ReadToEnd();
    p.WaitForExit();
    return p.ExitCode == 0 ? output : throw new InvalidOperationException($"git {string.Join(' ', args)} failed: {p.StandardError.ReadToEnd()}");
}

// ---------------------------------------------------------------- algorithms (benchmark method -> class -> XML doc)

static Dictionary<string, Algo> ReadAlgorithms(string repo, IEnumerable<string> methods)
{
    var benchDir = Path.Combine(repo, "src", "SortAlgorithm.Benchmark");
    var algoDir = Path.Combine(repo, "src", "SortAlgorithm", "Algorithms");
    var typeOf = new Dictionary<string, string>(StringComparer.Ordinal);
    foreach (var file in Directory.EnumerateFiles(benchDir, "*Benchmark.cs"))
    {
        var text = File.ReadAllText(file);
        foreach (Match m in Regex.Matches(text, @"public void (\w+)\(\)\s*\{([^}]*)\}"))
        {
            var call = Regex.Match(m.Groups[2].Value, @"SortAlgorithm\.Algorithms\.(\w+)\.");
            if (call.Success) typeOf.TryAdd(m.Groups[1].Value, call.Groups[1].Value);
        }
    }

    var docs = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
    var files = new Dictionary<string, string>(StringComparer.Ordinal);
    foreach (var file in Directory.EnumerateFiles(algoDir, "*.cs", SearchOption.AllDirectories))
    {
        var doc = new List<string>();
        foreach (var raw in File.ReadLines(file))
        {
            var t = raw.Trim();
            if (t.StartsWith("///", StringComparison.Ordinal)) { doc.Add(t); continue; }
            var m = Regex.Match(t, @"^public\s+(?:static\s+)?(?:sealed\s+)?(?:partial\s+)?class\s+(\w+)");
            if (m.Success && doc.Count > 0)
            {
                var rows = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var d in doc)
                {
                    var dm = Regex.Match(d, @"<description>\s*([^:<]+?)\s*:\s*(.+?)</description>");
                    if (dm.Success) rows.TryAdd(dm.Groups[1].Value.Trim(), CleanXml(dm.Groups[2].Value));
                }
                if (rows.ContainsKey("Best case"))
                {
                    docs.TryAdd(m.Groups[1].Value, rows);
                    files.TryAdd(m.Groups[1].Value, Path.GetRelativePath(Path.Combine(repo), file).Replace('\\', '/'));
                }
            }
            if (!t.StartsWith('[')) doc.Clear();
        }
    }

    var result = new Dictionary<string, Algo>(StringComparer.Ordinal);
    foreach (var method in methods.Distinct(StringComparer.Ordinal))
    {
        if (method == ReferenceMethod)
        {
            result[method] = new Algo(null, null, new Dictionary<string, string> { ["Family"] = ".NET MemoryExtensions.Sort (reference)" }, true);
            continue;
        }
        if (typeOf.TryGetValue(method, out var type) && docs.TryGetValue(type, out var rows))
        {
            result[method] = new Algo(type, files[type], rows, false);
        }
        else
        {
            result[method] = new Algo(typeOf.GetValueOrDefault(method), null, new Dictionary<string, string>(), false);
        }
    }
    return result;
}

static string CleanXml(string s)
{
    s = Regex.Replace(s, @"<see cref=""(?:[A-Z]:)?([^""]+)""\s*/>", "$1");
    s = Regex.Replace(s, "<[^>]+>", "");
    return WebUtility.HtmlDecode(s).Trim();
}

// ---------------------------------------------------------------- index

int BuildIndex(Dictionary<string, string> o)
{
    var dataDir = Require(o, "data");
    var runDir = Path.Combine(dataDir, "runs");
    var runs = Directory.EnumerateFiles(runDir, "*.json")
        .Select(f => (File: f, Doc: JsonDocument.Parse(File.ReadAllBytes(f))))
        .Select(t => (t.File, t.Doc, Time: DateTimeOffset.Parse(t.Doc.RootElement.GetProperty("timestamp").GetString()!, inv)))
        .OrderBy(static t => t.Time)
        .ToList();
    try
    {
        var seriesMap = new SortedDictionary<string, List<double[]>>(StringComparer.Ordinal);
        using (var fs = File.Create(Path.Combine(dataDir, "index.json")))
        using (var w = new Utf8JsonWriter(fs, new JsonWriterOptions { Indented = true }))
        {
            w.WriteStartObject();
            w.WriteNumber("schema", Schema);
            w.WriteString("generated", DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", inv));
            w.WriteString("latest", runs.Count > 0 ? runs[^1].Doc.RootElement.GetProperty("id").GetString() : null);
            w.WriteStartArray("runs");
            for (var i = 0; i < runs.Count; i++)
            {
                var r = runs[i].Doc.RootElement;
                var cases = r.GetProperty("cases");
                w.WriteStartObject();
                foreach (var key in new[] { "id", "source", "timestamp", "commit", "runUrl" })
                {
                    w.WritePropertyName(key);
                    r.GetProperty(key).WriteTo(w);
                }
                w.WriteString("cpu", r.GetProperty("environment").TryGetProperty("cpu", out var cpu) ? cpu.GetString() : null);
                w.WriteString("file", "runs/" + Path.GetFileName(runs[i].File));
                w.WriteNumber("cases", cases.GetArrayLength());
                w.WriteEndObject();
                foreach (var c in cases.EnumerateArray())
                {
                    var key = $"{c.GetProperty("category").GetString()}|{c.GetProperty("method").GetString()}|{c.GetProperty("size").GetInt32()}|{c.GetProperty("pattern").GetString()}";
                    if (!seriesMap.TryGetValue(key, out var list)) seriesMap[key] = list = [];
                    list.Add([i, c.GetProperty("mean").GetDouble(), c.TryGetProperty("error", out var e) && e.ValueKind == JsonValueKind.Number ? e.GetDouble() : double.NaN, c.GetProperty("stdDev").GetDouble()]);
                }
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }

        using (var fs = File.Create(Path.Combine(dataDir, "history.json")))
        using (var w = new Utf8JsonWriter(fs))
        {
            w.WriteStartObject();
            w.WriteNumber("schema", Schema);
            w.WriteStartArray("columns");
            foreach (var col in new[] { "run", "mean", "error", "stdDev" }) w.WriteStringValue(col);
            w.WriteEndArray();
            w.WriteStartObject("series");
            foreach (var (key, list) in seriesMap)
            {
                w.WriteStartArray(key);
                foreach (var v in list)
                {
                    w.WriteStartArray();
                    w.WriteNumberValue((int)v[0]);
                    w.WriteNumberValue(Round(v[1], 1));
                    if (double.IsNaN(v[2])) w.WriteNullValue(); else w.WriteNumberValue(Round(v[2], 2));
                    w.WriteNumberValue(Round(v[3], 2));
                    w.WriteEndArray();
                }
                w.WriteEndArray();
            }
            w.WriteEndObject();
            w.WriteEndObject();
        }
        Console.WriteLine($"{dataDir}: indexed {runs.Count} run(s), {seriesMap.Count} series");
    }
    finally
    {
        foreach (var r in runs) r.Doc.Dispose();
    }
    return 0;
}

// ---------------------------------------------------------------- run file

static string MakeId(DateTimeOffset t, string suffix) => $"{t.UtcDateTime:yyyyMMdd'T'HHmm'Z'}-{suffix}";

static double Round(double v, int digits) => Math.Round(v, digits, MidpointRounding.AwayFromZero);

static void WriteRun(Run run, string path)
{
    Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
    using var fs = File.Create(path);
    using var w = new Utf8JsonWriter(fs);
    w.WriteStartObject();
    w.WriteNumber("schema", Schema);
    w.WriteString("id", run.Id);
    w.WriteString("source", run.Source);
    w.WriteString("timestamp", run.Timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
    w.WriteString("commit", run.Commit);
    w.WriteString("runUrl", run.RunUrl);
    w.WriteString("reference", ReferenceMethod);
    w.WriteStartObject("environment");
    foreach (var (k, v) in run.Environment) w.WriteString(k, v);
    w.WriteEndObject();
    w.WriteStartObject("algorithms");
    foreach (var (method, a) in run.Algorithms.OrderBy(static kv => kv.Key, StringComparer.Ordinal))
    {
        w.WriteStartObject(method);
        w.WriteString("type", a.Type);
        w.WriteString("file", a.File);
        if (a.Reference) w.WriteBoolean("reference", true);
        foreach (var (label, key) in new[] { ("Family", "family"), ("Stable", "stable"), ("In-place", "inPlace"), ("Best case", "best"), ("Average case", "average"), ("Worst case", "worst") })
        {
            if (a.Doc.TryGetValue(label, out var v)) w.WriteString(key, v);
        }
        w.WriteEndObject();
    }
    w.WriteEndObject();
    w.WriteStartArray("cases");
    foreach (var c in run.Cases)
    {
        w.WriteStartObject();
        w.WriteString("category", c.Category);
        w.WriteString("method", c.Method);
        w.WriteNumber("size", c.Size);
        w.WriteString("pattern", c.Pattern);
        w.WriteNumber("mean", Round(c.Mean, 2));
        WriteNullable(w, "median", c.Median, 2);
        w.WriteNumber("stdDev", Round(c.StdDev, 3));
        WriteNullable(w, "error", c.Error, 3);
        if (c.N is null) w.WriteNull("n"); else w.WriteNumber("n", c.N.Value);
        WriteNullable(w, "allocated", c.Allocated, 0);
        w.WriteStartArray("measurements");
        foreach (var m in c.Measurements) w.WriteNumberValue(m);
        w.WriteEndArray();
        w.WriteEndObject();
    }
    w.WriteEndArray();
    w.WriteEndObject();
}

static void WriteNullable(Utf8JsonWriter w, string name, double? v, int digits)
{
    if (v is null) w.WriteNull(name); else w.WriteNumber(name, Round(v.Value, digits));
}

record Case(string Category, string Method, int Size, string Pattern, double Mean, double? Median, double StdDev, double? Error, int? N, double? Allocated, List<double> Measurements);
record Algo(string? Type, string? File, Dictionary<string, string> Doc, bool Reference);
record Run(string Id, string Source, DateTimeOffset Timestamp, string? Commit, string? RunUrl, Dictionary<string, string> Environment, Dictionary<string, Algo> Algorithms, List<Case> Cases);
