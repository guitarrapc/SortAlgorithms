namespace SortAlgorithm.Benchmark;

public static class SortAuditVerification
{
    public static void Run()
    {
        var declared = typeof(Algorithms.InsertionSort).Assembly.GetTypes()
            .Where(t => t.Namespace == "SortAlgorithm.Algorithms" && t.GetProperty("IsStable") is not null)
            .Select(t => t.Name).Order().ToArray();
        var registered = SortAuditAlgorithms.All.Select(a => a.Name).Order().ToArray();
        if (!declared.SequenceEqual(registered))
            throw new InvalidOperationException("Audit registry differs from public sorting implementations.");
        var total = 0;
        foreach (var (name, sort) in SortAuditAlgorithms.All)
        {
            var count = 0;
            foreach (var size in new[] { 0, 1, 2, 3, 5, 7, 8, 16, 31, 32, 33, 64, 255, 256, 257, 999, 1000, 1001 })
            {
                if (name is "BogoSort" or "BozoSort" && size > 7 || name == "SlowSort" && size > 32 || name == "StoogeSort" && size > 64)
                    continue;
                foreach (var seed in new[] { 42, 123, 2026 })
                foreach (var pattern in Enum.GetValues<DataPattern>())
                {
                    var random = new Random(seed);
                    var values = pattern == DataPattern.Random
                        ? Enumerable.Range(0, size).Select(_ => random.Next(-1000, 1001)).ToArray()
                        : BenchmarkData.GenerateIntArray(size, pattern);
                    Verify(name, sort, values);
                    count++;
                }
                Verify(name, sort, Enumerable.Repeat(0, size).ToArray());
                Verify(name, sort, Enumerable.Range(0, size).Select(i => i % 2 == 0 ? -1 : 1).ToArray());
                count += 2;
            }
            total += count;
            Console.WriteLine($"{name}: PASS ({count} cases)");
        }
        Console.WriteLine($"TOTAL: {SortAuditAlgorithms.All.Length} implementations, {total} cases, PASS");
    }

    static void Verify(string name, AuditSort sort, int[] values)
    {
        var unsupportedLength = name is "BitonicSort" or "BitonicSortNonOptimized" && values.Length > 1 && !int.IsPow2(values.Length);
        var unsupportedRange = name is "CountingSortInteger" or "PigeonholeSortInteger" && values.Length > 1
            && (long)values.Max() - values.Min() + 1 > (long)values.Length * 32;
        if (unsupportedLength || unsupportedRange)
        {
            try { sort(values); }
            catch (ArgumentException) { return; }
            throw new InvalidOperationException($"{name} accepted unsupported input (length {values.Length}).");
        }
        var expected = values.Order().ToArray();
        var guarded = new int[values.Length + 2];
        guarded[0] = 0x12345678;
        guarded[^1] = 0x76543210;
        values.CopyTo(guarded, 1);
        sort(guarded.AsSpan(1, values.Length));
        if (!guarded.AsSpan(1, values.Length).SequenceEqual(expected)
            || guarded[0] != 0x12345678 || guarded[^1] != 0x76543210)
            throw new InvalidOperationException($"{name} failed length {values.Length}.");
    }
}
