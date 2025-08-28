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
        /// <param name="statistic">Statistic to get modifiers for</param>
        /// <returns>Read-only list of modifiers</returns>
        [ItemNotNull] public IEnumerable<IStatModifier> GetAllModifiersFor(StatisticBase statistic)
        {
            // Get all modifiers
            IReadOnlyList<IStatModifier> statModifiers = GetAllModifiers();
            
            // Loop through modifiers and yield return only those that are of type TStatisticType
            for (int index = 0; index < statModifiers.Count; index++)
            {
                IStatModifier modifier = statModifiers[index];
                if (!modifier.IsValidFor(statistic)) continue;
                yield return modifier;
            }
        }
        
        /// <summary>
        ///     Get modifiers for statistic
        /// </summary>
        /// <typeparam name="TStatisticType">Statistic type</typeparam>
        /// <returns>Read-only list of modifiers</returns>
        [ItemNotNull] public IEnumerable<IStatModifier> GetAllModifiersFor<TStatisticType>()
            where TStatisticType : StatisticBase
        {
            // Get all modifiers
            IReadOnlyList<IStatModifier> statModifiers = GetAllModifiers();
            
            // Loop through modifiers and yield return only those that are of type TStatisticType
            for (int index = 0; index < statModifiers.Count; index++)
            {
                IStatModifier modifier = statModifiers[index];
                if (!modifier.IsValidFor<TStatisticType>()) continue;
                yield return modifier;
            }
        }
        
        /// <summary>
        ///     Get modifiers for statistic and add them to collection
        /// </summary>
        /// <param name="statModifierCollection">Collection to add modifiers to</param>
        /// <typeparam name="TStatisticType">Type of statistic</typeparam>
        public void TransferModifiersTo<TStatisticType>([NotNull] StatModifierCollection statModifierCollection)
            where TStatisticType : StatisticBase => statModifierCollection.AddRange(GetAllModifiersFor<TStatisticType>());
    }
}