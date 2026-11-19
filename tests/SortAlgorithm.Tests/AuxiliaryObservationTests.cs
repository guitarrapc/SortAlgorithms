using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;

namespace SortAlgorithm.Tests;

public class AuxiliaryObservationTests
{
    [Test]
    public void PingpongWritesIncludeBootstrapAndEveryMergeLevel()
    {
        foreach (var n in new[] { 2, 3, 4, 5, 8, 9, 16, 33 })
        foreach (var sorted in new[] { false, true })
        {
            var input = Enumerable.Range(0, n).ToArray();
            if (!sorted) Array.Reverse(input);
            var context = new StatisticsContext();
            PingpongMergeSort.Sort(input.AsSpan(), context);
            // The bootstrap writes n, and every merge node writes its whole range, merged or copied: n(h + 1) - 2^h.
            // Leaves write nothing, because the bootstrap already put their element in both buffers.
            var height = BitOperations.Log2((uint)(n - 1)) + 1;
            var expected = (ulong)(n * (height + 2) - (1 << height));
            if (context.IndexWriteCount != expected)
                throw new InvalidOperationException("Pingpong structural writes were omitted.");
        }
    }

    [Test]
    [Arguments("PingpongMergeSort")]
    [Arguments("LibrarySort")]
    [Arguments("MergeInsertionSort")]
    public void AuxiliaryOperationsCanBeReplayed(string algorithm)
    {
        foreach (var n in new[] { 0, 1, 2, 3, 4, 7, 8, 31, 32, 33, 65, 129, 257 })
        foreach (var pattern in new[] { "sorted", "reverse", "duplicates", "random", "equal" })
        {
            var rng = new Random(42);
            var input = Enumerable.Range(0, n).Select(i => new Item(pattern switch
            {
                "sorted" => i, "reverse" => n - i, "duplicates" => (n - i) % 4,
                "equal" => 1, _ => rng.Next(-20, 21)
            }, i)).ToArray();
            Verify(algorithm, input, default(ItemComparer));
            IComparer<Item> boxed = new ItemComparer();
            Verify(algorithm, input, boxed);
            Verify(algorithm, input, Comparer<Item>.Create((x, y) => y.Value.CompareTo(x.Value)));
        }
    }

    [Test]
    [Arguments("PingpongMergeSort")]
    [Arguments("LibrarySort")]
    [Arguments("MergeInsertionSort")]
    public void FloatingAuxiliaryValuesRetainTheirBits(string algorithm)
    {
        VerifyFloating<Half>(algorithm);
        VerifyFloating<float>(algorithm);
        VerifyFloating<double>(algorithm);
    }

    static void VerifyFloating<T>(string algorithm) where T : unmanaged, IFloatingPointIeee754<T>
    {
        foreach (var n in new[] { 0, 1, 3, 4, 8, 33, 65, 129 })
        foreach (var allNaN in new[] { false, true })
        {
            var input = Enumerable.Range(0, n).Select(i => allNaN || i % 3 == 0
                ? PreparedFloatingSortCorrectnessTests.NaN<T>(i % 4) : i % 3 == 1 ? -T.Zero : T.Zero).ToArray();
            Verify(algorithm, input, Comparer<T>.Default);
            Verify(algorithm, input, Comparer<T>.Create((x, y) => y.CompareTo(x)));
            Verify(algorithm, input, Comparer<T>.Create((x, y) => PreparedFloatingSortCorrectnessTests.Bits(x).CompareTo(PreparedFloatingSortCorrectnessTests.Bits(y))));
        }
    }

    static void Verify<T, TComparer>(string algorithm, T[] input, TComparer comparer) where TComparer : IComparer<T>
    {
        var expected = input.OrderBy(x => x, comparer).ToArray();
        var replay = new Replay<T>(algorithm, input);
        var observed = input.ToArray();
        Run(algorithm, observed.AsSpan(), comparer, replay);
        replay.Verify(observed);
        var stats = new StatisticsContext();
        var counted = input.ToArray();
        Run(algorithm, counted.AsSpan(), comparer, stats);
        if (stats.CompareCount != replay.Comparisons || stats.IndexReadCount != replay.Reads
            || stats.IndexWriteCount != replay.Writes || stats.SwapCount != replay.Swaps)
            throw new InvalidOperationException("Statistics do not match the replayed operations.");
        var sentinel = typeof(T) == typeof(Item) ? (T)(object)new Item(int.MinValue, -1)
            : typeof(T) == typeof(Half) ? (T)(object)(Half)987
            : typeof(T) == typeof(float) ? (T)(object)987f
            : typeof(T) == typeof(double) ? (T)(object)987d
            : throw new InvalidOperationException("Missing test sentinel.");
        var guarded = Enumerable.Repeat(sentinel, input.Length + 4).ToArray();
        input.CopyTo(guarded, 2);
        var before = guarded.ToArray();
        Run(algorithm, guarded.AsSpan(2, input.Length), comparer, NullContext.Default);
        if (!Same(guarded[0], before[0]) || !Same(guarded[1], before[1]) || !Same(guarded[^2], before[^2]) || !Same(guarded[^1], before[^1]))
            throw new InvalidOperationException("A sort wrote outside the requested span.");
        var plain = guarded.AsSpan(2, input.Length).ToArray();
        for (var i = 0; i < input.Length; i++)
        {
            if (comparer.Compare(expected[i], observed[i]) != 0 || !Same(observed[i], counted[i]) || !Same(observed[i], plain[i]))
                throw new InvalidOperationException("Wrong order or context-dependent output.");
            if (algorithm != "MergeInsertionSort" && !Same(expected[i], observed[i]))
                throw new InvalidOperationException("Stable order was lost.");
        }
        var multiplicity = new Dictionary<object, int>();
        foreach (var item in input) { var key = Key(item); multiplicity[key] = multiplicity.GetValueOrDefault(key) + 1; }
        foreach (var item in observed) { var key = Key(item); if (!multiplicity.TryGetValue(key, out var count) || count == 0) throw new InvalidOperationException("Element bits or multiplicity changed."); multiplicity[key] = count - 1; }
    }

    static object Key<T>(T value) => value is null ? "<null>" : typeof(T) == typeof(Half)
        ? (object)unchecked((ushort)BitConverter.HalfToInt16Bits((Half)(object)value))
        : typeof(T) == typeof(float) ? unchecked((uint)BitConverter.SingleToInt32Bits((float)(object)value))
        : typeof(T) == typeof(double) ? unchecked((ulong)BitConverter.DoubleToInt64Bits((double)(object)value)) : value;

    static bool Same<T>(T left, T right) => Equals(Key(left), Key(right));

    static void Run<T, TComparer, TContext>(string algorithm, Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T> where TContext : ISortContext
    {
        switch (algorithm)
        {
            case "PingpongMergeSort": PingpongMergeSort.Sort(span, comparer, context); break;
            case "LibrarySort": LibrarySort.Sort(span, comparer, context); break;
            case "MergeInsertionSort": MergeInsertionSort.Sort(span, comparer, context); break;
            default: throw new ArgumentOutOfRangeException(nameof(algorithm));
        }
    }

    public sealed record Item(int Value, int OriginalIndex) : IComparable<Item>
    {
        public int CompareTo(Item? other) => Value.CompareTo(other!.Value);
    }

    readonly struct ItemComparer : IComparer<Item>
    {
        public int Compare(Item? x, Item? y) => x!.Value.CompareTo(y!.Value);
    }

    sealed class Replay<T>(string algorithm, T[] input) : ISortContext
    {
        readonly Dictionary<(int Buffer, int Index), T> values = input.Select((x, i) => (x, i)).ToDictionary(x => (0, x.i), x => x.x);
        readonly Dictionary<int, int> auxReads = [];
        readonly Dictionary<int, int> auxWrites = [];
        bool bootstrap;
        public ulong Reads { get; private set; }
        public ulong Writes { get; private set; }
        public ulong Comparisons { get; private set; }
        public ulong Swaps { get; private set; }

        void Position(int index, int buffer)
        {
            var capacity = buffer == 1 && algorithm == "LibrarySort" ? 2 * input.Length : input.Length;
            if (buffer < 0 || buffer > (algorithm == "PingpongMergeSort" ? 1 : 2) || (uint)index >= (uint)capacity)
                throw new InvalidOperationException($"Invalid buffer coordinate: {algorithm}/{buffer}/{index}.");
        }

        T Read(int index, int buffer)
        {
            Position(index, buffer);
            if (!values.TryGetValue((buffer, index), out var value)) throw new InvalidOperationException($"Read before observed initialization: {algorithm}/{buffer}/{index}.");
            Reads++;
            if (buffer > 0) auxReads[buffer] = auxReads.GetValueOrDefault(buffer) + 1;
            return value;
        }

        void Write(int index, int buffer, T value)
        {
            Position(index, buffer);
            values[(buffer, index)] = value;
            Writes++;
            if (buffer > 0) auxWrites[buffer] = auxWrites.GetValueOrDefault(buffer) + 1;
        }

        public void OnIndexRead(int index, int bufferId) => Read(index, bufferId);
        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { Comparisons++; }
        public void OnSwap(int i, int j, int bufferId) { var left = Read(i, bufferId); var right = Read(j, bufferId); Write(i, bufferId, right); Write(j, bufferId, left); Swaps++; }
        public void OnIndexWrite(int index, int bufferId) => throw new InvalidOperationException("Element write without a value.");
        public void OnIndexWrite<TValue>(int index, int bufferId, TValue value)
        {
            if (typeof(TValue) != typeof(T)) throw new InvalidOperationException("Metadata reported as an element.");
            Write(index, bufferId, (T)(object)value!);
        }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId)
            => throw new InvalidOperationException("Copy without element values.");
        public void OnRangeCopy<TValue>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<TValue> copied)
        {
            if (typeof(TValue) != typeof(T) || copied.Length != length) throw new InvalidOperationException("Wrong copy payload.");
            var snapshot = new T[length];
            for (var i = 0; i < length; i++) { snapshot[i] = Read(sourceIndex + i, sourceBufferId); if (!Same(snapshot[i], (T)(object)copied[i]!)) throw new InvalidOperationException("Copy payload differs from replay."); }
            for (var i = 0; i < length; i++) Write(destinationIndex + i, destinationBufferId, snapshot[i]);
            if (sourceBufferId == 0 && destinationBufferId == 1 && sourceIndex == 0 && destinationIndex == 0 && length == input.Length) bootstrap = true;
        }
        public void OnPhase(SortPhase phase, int a = -1, int b = -1, int c = -1) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
        public void OnLink(int parentIndex, int childIndex, int bufferId, LinkSide side) => throw new InvalidOperationException("Unexpected tree metadata.");

        public void Verify(T[] actual)
        {
            for (var i = 0; i < actual.Length; i++) if (!Same(values[(0, i)], actual[i])) throw new InvalidOperationException("Replayed main buffer differs from actual output.");
            if (algorithm == "PingpongMergeSort" && input.Length > 1 && !bootstrap) throw new InvalidOperationException("Missing bootstrap copy.");
            if ((algorithm == "MergeInsertionSort" && input.Length > 1 || algorithm == "LibrarySort" && input.Length >= 129)
                && (auxReads.GetValueOrDefault(2) == 0 || auxWrites.GetValueOrDefault(2) == 0))
                throw new InvalidOperationException("Missing snapshot/staging buffer observations.");
        }
    }
}
