using System.Text;
using System.Text.RegularExpressions;

if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: UpdateBenchmarkReadme.cs <artifacts-dir> <readme-path> <workflow-run-url>");
    Environment.Exit(1);
}

var artifactsDir = args[0];
var readmePath = args[1];
var runUrl = args[2];

const string StartMarker = "<!-- BENCHMARK_START -->";
const string EndMarker = "<!-- BENCHMARK_END -->";

var section = BuildBenchmarkSection(artifactsDir, runUrl);
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
    var machineInfo = lines
        .Take(9)
        .Where(line => !string.IsNullOrWhiteSpace(line))
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
