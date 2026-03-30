namespace Systems.SimpleStats.Abstract.Modifiers
{
    /// <summary>
    ///     Non-generic modifier source interface for reflection and debugging.
    ///     Implement this on modifiers that need to track where they came from.
    /// </summary>
    public interface IModifierSource
    {
        /// <summary>
        ///     Returns the source object without type knowledge.
        ///     Useful for debugging, logging, and reflection scenarios.
        /// </summary>
        object GetRawSource();
    }

    /// <summary>
    ///     Strongly-typed modifier source tracking.
    ///     Implement this on modifiers to track their origin (e.g., which item, ability, or effect applied them).
    /// </summary>
    /// <typeparam name="TSource">Type of the source (e.g., a weapon, buff, or status effect)</typeparam>
    public interface IModifierSource<out TSource> : IModifierSource
    {
        /// <summary>
        ///     Returns the strongly-typed source of this modifier
        /// </summary>
        TSource GetSource();

        /// <inheritdoc />
        object IModifierSource.GetRawSource() => GetSource();
    }
}
