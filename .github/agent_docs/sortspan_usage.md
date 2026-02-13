# SortSpan Usage Guidelines

When implementing sorting algorithms, **always use SortSpan<T, TComparer>** for all array/span operations.

## Why SortSpan?

- **Accurate statistics** for algorithm analysis via ISortContext
- **Clean abstraction** for tracking operations
- **Minimal performance impact** with conditional compilation
- **Consistent code style** across all sorting implementations
- **Separation of concerns** - algorithm logic vs observation
- **Zero-alloc comparisons** - generic `TComparer : IComparer<T>` enables devirtualization

## Performance: DEBUG vs RELEASE Builds

**SortSpan uses conditional compilation (`#if DEBUG`) to optimize performance:**

### DEBUG Build (Tests)
- ✅ **Full statistics tracking** - all operations recorded
- ✅ **Accurate analysis** - perfect for testing and profiling
- ✅ **Context callbacks** - OnIndexRead, OnIndexWrite, OnCompare, OnSwap
- ⚠️ **Slight overhead** - acceptable for testing

### RELEASE Build (Production)
- ✅ **Zero overhead** - context calls are omitted
- ✅ **Direct Span operations** - `_span[i]` instead of `context.OnIndexRead()`
- ✅ **Maximum performance** - equivalent to raw Span operations
- ✅ **Inlined comparisons** - `_comparer.Compare(_span[i], _span[j])` (devirtualized when TComparer is a struct)

**Example:**

```csharp
// SortSpan.Read() implementation
public T Read(int i)
{
#if DEBUG
    _context.OnIndexRead(i, _bufferId);  // ← Only in DEBUG
#endif
    return _span[i];  // ← Always direct access
}

// SortSpan.Compare() implementation
public int Compare(int i, int j)
{
#if DEBUG
    var a = Read(i);
    var b = Read(j);
    var result = _comparer.Compare(a, b);
    _context.OnCompare(i, j, result, _bufferId, _bufferId);
    return result;
#else
    return _comparer.Compare(_span[i], _span[j]);  // ← Devirtualized comparison in RELEASE
#endif
}
```

**Result:** In RELEASE builds, SortSpan operations compile to simple Span operations with zero abstraction overhead. When `TComparer` is a struct (e.g., `Comparer<T>.Default`), the JIT devirtualizes and inlines the comparison call.

## Required Operations

### 1. Copying Ranges: `s.CopyTo(sourceIndex, destination, destinationIndex, length)`

Copies a range of elements from source to destination, notifies context via `OnRangeCopy(sourceIndex, destinationIndex, length, sourceBufferId, destBufferId, values)`

```csharp
// ✅ Correct - copying from temp to main buffer
temp.CopyTo(0, s, 0, s.Length);

// ✅ Correct - copying from source to destination SortSpan
source.CopyTo(sourceStart, dest, destStart, count);

// ✅ Correct - copying to regular Span (also tracked)
temp.CopyTo(0, regularSpan, 0, length);

// ❌ Incorrect - manual loop bypasses context
for (var i = 0; i < length; i++)
{
    s.Write(i, temp.Read(i));  // Each operation tracked separately, slower
}

// ❌ Incorrect - direct CopyTo bypasses context
temp._span.CopyTo(s._span);  // No tracking!
```

**When to use `CopyTo`:**
- ✅ Copying entire buffers after sorting (e.g., merge sort, bucket sort)
- ✅ Copying ranges between SortSpan instances
- ✅ Better performance than manual loops (SIMD optimization potential)
- ✅ Clearer intent - "copy this range" vs "loop and write each element"

### 2. Reading Elements: `s.Read(i)`

Notifies context via `OnIndexRead(i)`

```csharp
// ✅ Correct - uses Read
var value = s.Read(i);

// ❌ Incorrect - bypasses context
var value = span[i];
```

### 3. Writing Elements: `s.Write(i, value)`

Notifies context via `OnIndexWrite(i)`

```csharp
// ✅ Correct - uses Write
s.Write(i, value);

// ❌ Incorrect - bypasses context
span[i] = value;
```

### 4. Comparing Elements: `s.Compare(i, j)`

Reads both elements, compares via `_comparer.Compare()`, and notifies context via `OnCompare(i, j, result)`

```csharp
// ✅ Correct - comparing two indices
if (s.Compare(i, j) < 0) { ... }

// ❌ Incorrect - bypasses context
if (comparer.Compare(span[i], span[j]) < 0) { ... }
```

For comparing with a value (not an index):

```csharp
var value = s.Read(someIndex);

// ✅ Correct - comparing index with value
if (s.Compare(i, value) < 0) { ... }

// ✅ Correct - comparing value with index
if (s.Compare(value, i) < 0) { ... }

// ❌ Incorrect - direct comparer.Compare bypasses context
if (s.Comparer.Compare(s.Read(i), value) < 0) { ... }
```

**Important:** Never use `comparer.Compare()` or `.CompareTo()` directly. All comparisons must go through `SortSpan` methods. Access `s.Comparer` only when absolutely necessary (e.g., passing to a helper that cannot take a SortSpan).

### 5. Swapping Elements: `s.Swap(i, j)`

Reads both elements, notifies context via `OnSwap(i, j)`, then writes

```csharp
// ✅ Correct
s.Swap(i, j);

// ❌ Incorrect - bypasses context
(span[i], span[j]) = (span[j], span[i]);
```

## Implementation Guidelines

### Always Use SortSpan Operations

Even though RELEASE builds optimize away the overhead, **always write code using SortSpan methods**:

```csharp
// ✅ Correct - works efficiently in both DEBUG and RELEASE
private static void InsertIterative<T, TComparer>(Span<Node> arena, ..., SortSpan<T, TComparer> s)
    where TComparer : IComparer<T>
{
    // Cache value (reduces Read() calls in DEBUG, direct access in RELEASE)
    var insertValue = s.Read(itemIndex);
    var currentValue = s.Read(current.ItemIndex);

    // Direct comparison (tracked in DEBUG, devirtualized+inlined in RELEASE)
    var cmp = s.Compare(insertValue, currentValue);
}

// ❌ Incorrect - bypasses statistics in DEBUG
private static void InsertIterative<T>(Span<T> span, IComparer<T> comparer, ...)
{
    var insertValue = span[itemIndex];  // No tracking!
    var cmp = comparer.Compare(insertValue, span[j]);  // No tracking!
}
```

### Value Caching for Performance

When comparing the same value multiple times, cache it:

```csharp
// ✅ Good - read once, compare many times
var insertValue = s.Read(itemIndex);  // 1 read
while (...)
{
    var currentValue = s.Read(current.ItemIndex);  // 1 read per iteration
    if (s.Compare(insertValue, currentValue) < 0) { ... }  // Cached comparison
}

// ❌ Less efficient - reads twice per comparison in DEBUG
while (...)
{
    if (s.Compare(itemIndex, current.ItemIndex) < 0) { ... }  // 2 reads per comparison
}
```

## Buffer Management

When using internal temporary buffers, always track them with a unique `bufferId`:

```csharp
public static class MySort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary merge buffer
    private const int BUFFER_AUX = 2;        // Auxiliary buffer

    public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context)
        where TComparer : IComparer<T>
    {
        var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);

        // For temporary buffers - reuse the same comparer
        Span<T> tempBuffer = stackalloc T[span.Length];
        var temp = new SortSpan<T, TComparer>(tempBuffer, context, comparer, BUFFER_TEMP);
    }
}
```

**Rules:**

1. ✅ **Always use SortSpan for internal buffers** - even if they're temporary arrays
2. ✅ **Assign unique bufferIds** - starting from 0 for main array
3. ✅ **Document buffer purpose** - use clear constant names
4. ❌ **Never bypass SortSpan** - direct array access loses statistics

## Usage Examples

```csharp
// Production - no statistics
MySort.Sort<int>(array);

// With statistics
var stats = new StatisticsContext();
MySort.Sort(array.AsSpan(), stats);
Console.WriteLine($"Compares: {stats.CompareCount}, Swaps: {stats.SwapCount}");

// With visualization
var viz = new VisualizationContext(
    onSwap: (i, j) => RenderSwap(i, j),
    onCompare: (i, j, result) => HighlightCompare(i, j)
);
MySort.Sort(array.AsSpan(), viz);
```
