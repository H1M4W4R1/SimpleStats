namespace Systems.SimpleStats.Data
{
    public enum ModifierOrder
    {
        /// <summary>
        ///     Flat add modifiers (added to base value)
        /// </summary>
        FlatAdd = -int.MaxValue / 2,
  
        /// <summary>
        ///     Multiplier for value, applies geometrically 1.1 * 1.1 * 1.1...
        /// </summary>
        Multiply = 0,
        
        /// <summary>
        ///     Final add modifiers (added to final value, after multiplication)
        /// </summary>
        FinalAdd = int.MaxValue / 2
    }
}