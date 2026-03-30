namespace Systems.SimpleStats.Data
{
    public enum ModifierOrder
    {
        /// <summary>
        ///     Flat add modifiers (added to base value)
        /// </summary>
        FlatAdd = -int.MaxValue / 2,

        /// <summary>
        ///     Percentage add modifiers (adds a percentage of the current value after flat adds)
        /// </summary>
        PercentageAdd = -int.MaxValue / 4,

        /// <summary>
        ///     Multiplier for value, applies geometrically 1.1 * 1.1 * 1.1...
        /// </summary>
        Multiply = 0,

        /// <summary>
        ///     Percentage final add modifiers (adds a percentage of the current value after multiplication)
        /// </summary>
        PercentageFinalAdd = int.MaxValue / 4,

        /// <summary>
        ///     Final add modifiers (added to final value, after multiplication)
        /// </summary>
        FinalAdd = int.MaxValue / 2
    }
}