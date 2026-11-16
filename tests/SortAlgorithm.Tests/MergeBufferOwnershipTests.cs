using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Algorithms;

namespace SortAlgorithm.Tests;

public class MergeBufferOwnershipTests
{
    [Test]
    public void FailedGrowthKeepsTheOriginalLease()
    {
        Failure<int>(false);
        Failure<Box>(false);
        Failure<WithReference>(false);
    }

    [Test]
    public void FailedInitialRentDoesNotCreateAnOwner()
    {
        Failure<int>(true);
        Failure<Box>(true);
        Failure<WithReference>(true);
    }

    static void Failure<T>(bool initiallyEmpty)
    {
        var pool = new TrackingPool<T>();
        T[]? owner = initiallyEmpty ? null : pool.Rent(8);
        var previous = owner;
        var failure = new OutOfMemoryException("Injected Rent failure.");
        pool.RentFailure = failure;
        Exception? caught = null;
        try
        {
            MergeBuffer.EnsureCapacity(ref owner, 16, pool);
        }
        catch (Exception ex)
        {
            caught = ex;
            if (!ReferenceEquals(owner, previous)) throw new InvalidOperationException("Rent failure changed the owner.");
        }
        finally
        {
            if (owner is not null) pool.Return(owner, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
        if (!ReferenceEquals(caught, failure) || pool.Outstanding != 0 || pool.Returns != (initiallyEmpty ? 0 : 1))
            throw new InvalidOperationException("Rent failure or lease cleanup changed.");
        if (!initiallyEmpty && pool.ClearFlags.Single() != RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            throw new InvalidOperationException("Reference clearing changed.");
    }

    [Test]
    public void SuccessfulGrowthPublishesBeforeReturningTheOldLease()
    {
        Success<int>();
        Success<Box>();
        Success<WithReference>();
    }

    static void Success<T>()
    {
        var pool = new TrackingPool<T>();
        T[]? owner = pool.Rent(8);
        var previous = owner;
        pool.OnReturn = array =>
        {
            if (ReferenceEquals(array, previous) && ReferenceEquals(owner, previous))
                throw new InvalidOperationException("The old lease was returned before publishing its replacement.");
        };
        try
        {
            var replacement = MergeBuffer.EnsureCapacity(ref owner, 16, pool);
            if (!ReferenceEquals(owner, replacement) || ReferenceEquals(owner, previous) || owner.Length < 16)
                throw new InvalidOperationException("Incorrect replacement.");
        }
        finally
        {
            if (owner is not null) pool.Return(owner, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
        if (pool.Outstanding != 0 || pool.Rents != 2 || pool.Returns != 2
            || pool.ClearFlags.Any(x => x != RuntimeHelpers.IsReferenceOrContainsReferences<T>()))
            throw new InvalidOperationException("Growth leaked a lease or changed reference clearing.");
    }

    [Test]
    public void CapacityUsesTheRentedArrayLength()
    {
        var pool = new TrackingPool<int> { MinimumArrayLength = 64 };
        int[]? owner = pool.Rent(2);
        var previous = owner;
        try
        {
            foreach (var required in new[] { 1, 2, 32, 64 })
                if (!ReferenceEquals(MergeBuffer.EnsureCapacity(ref owner, required, pool), previous))
                    throw new InvalidOperationException("An adequate rental was replaced.");
            if (pool.Rents != 1 || pool.Returns != 0) throw new InvalidOperationException("The fit path used the pool.");
        }
        finally { pool.Return(owner, false); }
        if (pool.Outstanding != 0) throw new InvalidOperationException("A lease remained outstanding.");
    }

    [Test]
    public void InitialRentCreatesTheOnlyLease()
    {
        Initial<int>(); Initial<Box>(); Initial<WithReference>();
    }

    static void Initial<T>()
    {
        var pool = new TrackingPool<T>();
        T[]? owner = null;
        try
        {
            var result = MergeBuffer.EnsureCapacity(ref owner, 8, pool);
            if (!ReferenceEquals(result, owner) || owner.Length < 8 || pool.Rents != 1 || pool.Returns != 0)
                throw new InvalidOperationException("The first lease was not published.");
        }
        finally
        {
            if (owner is not null) pool.Return(owner, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
        if (pool.Outstanding != 0 || pool.Returns != 1) throw new InvalidOperationException("The first lease leaked.");
    }

    [Test]
    [Arguments("TimSort")]
    [Arguments("PowerSort")]
    public void ObservingExceptionsPropagateFromTemporaryCopies(string algorithm)
    {
        var input = Enumerable.Range(512, 512).Concat(Enumerable.Range(0, 512)).ToArray();
        var failure = new InvalidOperationException("Injected observation failure.");
        var reached = false;
        var context = new SortAlgorithm.Contexts.VisualizationContext(onRangeCopy: (_, _, _, _, destinationBuffer, _) =>
        {
            if (destinationBuffer != 1) return;
            reached = true;
            throw failure;
        });
        Exception? caught = null;
        try
        {
            if (algorithm == "TimSort") TimSort.Sort(input.AsSpan(), context);
            else PowerSort.Sort(input.AsSpan(), context);
        }
        catch (Exception ex) { caught = ex; }
        if (!reached || !ReferenceEquals(caught, failure)) throw new InvalidOperationException("The observer exception was replaced.");
    }

    public sealed record Box(int Value);
    public readonly record struct WithReference(Box Value);

    sealed class TrackingPool<T> : ArrayPool<T>
    {
        readonly HashSet<T[]> leases = [];
        public Exception? RentFailure { get; set; }
        public Action<T[]>? OnReturn { get; set; }
        public int MinimumArrayLength { get; init; }
        public int Rents { get; private set; }
        public int Returns { get; private set; }
        public int Outstanding => leases.Count;
        public List<bool> ClearFlags { get; } = [];
        public override T[] Rent(int minimumLength)
        {
            if (RentFailure is not null) throw RentFailure;
            var array = new T[Math.Max(minimumLength, MinimumArrayLength)];
            leases.Add(array);
            Rents++;
            return array;
        }
        public override void Return(T[] array, bool clearArray = false)
        {
            OnReturn?.Invoke(array);
            if (!leases.Remove(array)) throw new InvalidOperationException("A lease was returned twice.");
            Returns++;
            ClearFlags.Add(clearArray);
            if (clearArray) Array.Clear(array);
        }
    }
}
