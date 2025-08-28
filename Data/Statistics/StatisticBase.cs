using JetBrains.Annotations;
using Systems.SimpleStats.Data.Collections;
using UnityEngine;

namespace Systems.SimpleStats.Data.Statistics
{
    /// <summary>
    ///     Base statistic to implement modifiers
    /// </summary>
    public abstract class StatisticBase : ScriptableObject
    {
        /// <summary>
        ///     Base value of statistic, can be modified by modifiers
        /// </summary>
        [field: SerializeField] public float BaseValue { get; private set; } = 1;

        /// <summary>
        ///     Get final value of statistic with modifiers
        /// </summary>
        /// <param name="modifiers">Modifiers to apply</param>
        /// <returns>Final value of statistic</returns>
        public float GetFinalValue([NotNull] StatModifierCollection modifiers)
        {
            float result = BaseValue;
            modifiers.Apply(ref result);
            return result;
        }
    }
}