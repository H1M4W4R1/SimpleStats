using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data.Collections;
using Systems.SimpleStats.Data.Statistics;

namespace Systems.SimpleStats.Abstract
{
    /// <summary>
    ///     Represents object that can have modifiers
    /// </summary>
    public interface IWithStatModifiers
    {
        /// <summary>
        ///     Get all modifiers registered for this object
        /// </summary>
        /// <returns>Read-only list of modifiers</returns>
        /// <remarks>
        ///     It is heavily recommended to cache modifiers within object to avoid performance issues
        /// </remarks>
        public IReadOnlyList<IStatModifier> GetAllModifiers();

        /// <summary>
        ///     Get modifiers for statistic
        /// </summary>
        /// <typeparam name="TStatisticType">Statistic type</typeparam>
        /// <returns>Read-only list of modifiers</returns>
        public IEnumerable<IStatModifier> GetModifiersFor<TStatisticType>()
            where TStatisticType : StatisticBase;
        
        /// <summary>
        ///     Get modifiers for statistic and add them to collection
        /// </summary>
        /// <param name="statModifierCollection">Collection to add modifiers to</param>
        /// <typeparam name="TStatisticType">Type of statistic</typeparam>
        public void TransferModifiersTo<TStatisticType>([NotNull] StatModifierCollection statModifierCollection);
    }
}