using Systems.SimpleStats.Data;

namespace Systems.SimpleStats.Abstract.Modifiers
{
    /// <summary>
    ///     Modifier that conditionally applies based on runtime evaluation.
    ///     <see cref="ShouldApply"/> is checked every recalculation, allowing modifiers
    ///     to dynamically enable/disable based on game state.
    /// </summary>
    public interface IConditionalModifier : IStatModifier
    {
        /// <summary>
        ///     Determines whether this modifier should be applied during the current recalculation.
        /// </summary>
        /// <param name="context">Context describing the modifier operation</param>
        /// <returns>True if the modifier should apply, false to skip it</returns>
        bool ShouldApply(in ModifierContext context);
    }
}
