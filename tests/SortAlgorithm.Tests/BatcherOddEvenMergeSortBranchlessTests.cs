using SortAlgorithm.Algorithms;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BatcherOddEvenMergeSortBranchlessTests : BranchlessNetworkTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BatcherOddEvenMergeSortBranchless.Sort(span, context);

    protected override void SortWith<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        => BatcherOddEvenMergeSortBranchless.Sort(span, comparer, context);

    protected override void ConditionalExchangeSort<T, TContext>(Span<T> span, TContext context)
        => BatcherOddEvenMergeSort.Sort(span, context);
}
