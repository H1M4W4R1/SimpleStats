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
        ///     Collects modifiers valid for the given statistic into the output list.
        ///     Writes directly into the target list to avoid GC allocations from yield return.
        /// </summary>
        /// <param name="statistic">Statistic to filter by</param>
        /// <param name="output">List to receive matching modifiers</param>
        public void GetAllModifiersFor(StatisticBase statistic, [NotNull] List<IStatModifier> output)
        {
            IReadOnlyList<IStatModifier> statModifiers = GetAllModifiers();

            for (int index = 0; index < statModifiers.Count; index++)
            {
                IStatModifier modifier = statModifiers[index];
                if (modifier.IsValidFor(statistic))
                    output.Add(modifier);
            }
        }

        /// <summary>
        ///     Collects modifiers valid for the given statistic type into the output list.
        ///     Writes directly into the target list to avoid GC allocations from yield return.
        /// </summary>
        /// <typeparam name="TStatisticType">Statistic type</typeparam>
        /// <param name="output">List to receive matching modifiers</param>
        public void GetAllModifiersFor<TStatisticType>([NotNull] List<IStatModifier> output)
            where TStatisticType : StatisticBase
        {
            IReadOnlyList<IStatModifier> statModifiers = GetAllModifiers();

            for (int index = 0; index < statModifiers.Count; index++)
            {
                IStatModifier modifier = statModifiers[index];
                if (modifier.IsValidFor<TStatisticType>())
                    output.Add(modifier);
            }
        }

        /// <summary>
        ///     Get modifiers for statistic and add them to collection
        /// </summary>
        /// <param name="statModifierCollection">Collection to add modifiers to</param>
        /// <typeparam name="TStatisticType">Type of statistic</typeparam>
        public void TransferModifiersTo<TStatisticType>([NotNull] StatModifierCollection statModifierCollection)
            where TStatisticType : StatisticBase
        {
            IReadOnlyList<IStatModifier> statModifiers = GetAllModifiers();

            for (int index = 0; index < statModifiers.Count; index++)
            {
                IStatModifier modifier = statModifiers[index];
                if (modifier.IsValidFor<TStatisticType>())
                    statModifierCollection.Add(modifier);
            }
        }
    }
}