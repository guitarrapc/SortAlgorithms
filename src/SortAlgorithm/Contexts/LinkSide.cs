namespace SortAlgorithm.Contexts;

/// <summary>
/// Identifies which child slot of a parent node a link write targets.
/// Used by <see cref="ISortContext.OnLink"/>.
/// </summary>
public enum LinkSide
{
    /// <summary>
    /// No child slot: the child became the root of the tree.
    /// The parent index reported alongside this value is -1.
    /// </summary>
    None = 0,

    /// <summary>The parent's left child slot.</summary>
    Left = 1,

    /// <summary>The parent's right child slot.</summary>
    Right = 2,
}
