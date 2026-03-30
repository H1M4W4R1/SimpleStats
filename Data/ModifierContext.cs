using Systems.SimpleCore.Utility.Enums;
using Systems.SimpleStats.Abstract;
using Systems.SimpleStats.Abstract.Modifiers;

namespace Systems.SimpleStats.Data
{
    /// <summary>
    ///     Zero-allocation context for modifier operations.
    ///     Passed to validation, events, and conditional checks.
    /// </summary>
    public readonly ref struct ModifierContext
    {
        /// <summary>
        ///     The modifier being added, removed, or evaluated
        /// </summary>
        public readonly IStatModifier modifier;

        /// <summary>
        ///     The stats collection owner
        /// </summary>
        public readonly IWithStatModifiers owner;

        /// <summary>
        ///     Whether this operation was triggered internally or externally
        /// </summary>
        public readonly ActionSource actionSource;

        public ModifierContext(IStatModifier modifier, IWithStatModifiers owner, ActionSource actionSource)
        {
            this.modifier = modifier;
            this.owner = owner;
            this.actionSource = actionSource;
        }
    }
}
