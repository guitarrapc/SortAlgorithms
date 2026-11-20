using SortAlgorithm.Algorithms;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class PairwiseSortingNetworkBranchlessTests : BranchlessNetworkTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => PairwiseSortingNetworkBranchless.Sort(span, context);

    protected override void SortWith<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        => PairwiseSortingNetworkBranchless.Sort(span, comparer, context);

    protected override void ConditionalExchangeSort<T, TContext>(Span<T> span, TContext context)
        => PairwiseSortingNetwork.Sort(span, context);
}
