using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

internal static class ContextDispatcher
{
    /// <summary>
    /// Dispatches to the appropriate Sort overload based on the runtime type of context.
    /// </summary>
    public static void DispatchSort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context, SortAction<T, TComparer> sortAction)
        where TComparer : IComparer<T>
    {
        switch (context)
        {
            case NullContext ctx:
                sortAction.Invoke<NullContext>(span, comparer, ctx);
                break;
            case StatisticsContext ctx:
                sortAction.Invoke<StatisticsContext>(span, comparer, ctx);
                break;
            case VisualizationContext ctx:
                sortAction.Invoke<VisualizationContext>(span, comparer, ctx);
                break;
            case CompositeContext ctx:
                sortAction.Invoke<CompositeContext>(span, comparer, ctx);
                break;
            default:
                throw new ArgumentException($"Unsupported context type: {context.GetType().Name}. Add support in {nameof(ContextDispatcher)}.{nameof(DispatchSort)}.", nameof(context));
        }
    }

    /// <summary>
    /// Delegate for sort action with generic TContext parameter.
    /// </summary>
    public interface SortAction<T, TComparer> where TComparer : IComparer<T>
    {
        void Invoke<TContext>(Span<T> span, TComparer comparer, TContext context) where TContext : ISortContext;
    }
}
