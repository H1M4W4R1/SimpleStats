using JetBrains.Annotations;
using Systems.SimpleStats.Abstract;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Collections;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine.Assertions;

namespace Systems.SimpleStats.Utility
{
    /// <summary>
    ///     Object statistic API used to simplify statistic access
    /// </summary>
    public static class StatAPI
    {
        /// <summary>
        ///     Calculates final value of statistic based on modifiers
        /// </summary>
        /// <param name="obj">Object to calculate final value for</param>
        /// <typeparam name="TStatisticType">Type of statistic</typeparam>
        /// <returns>Calculated final value of statistic</returns>
        public static float GetStatisticValue<TStatisticType>([NotNull] this IWithStatModifiers obj)
            where TStatisticType : StatisticBase
        {
            // Create modifier collection
            StatModifierCollection modifierCollection = new(obj.GetAllModifiersFor<TStatisticType>());
            
            // Get statistic
            TStatisticType statistic = StatsDatabase.GetStatistic<TStatisticType>();
            Assert.IsNotNull(statistic);
            
            // Perform calculation
            return statistic.GetFinalValue(modifierCollection);
        }
        
    }
}