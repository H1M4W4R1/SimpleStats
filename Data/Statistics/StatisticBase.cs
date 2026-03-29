using JetBrains.Annotations;
using Systems.SimpleCore.Automation.Attributes;
using Systems.SimpleStats.Data.Collections;
using UnityEngine;

namespace Systems.SimpleStats.Data.Statistics
{
    /// <summary>
    ///     Base statistic to implement modifiers
    /// </summary>
    [AutoCreate("Statistics", StatsDatabase.LABEL)]
    public abstract class StatisticBase : ScriptableObject
    {
        /// <summary>
        ///     Base value of statistic, can be modified by modifiers
        /// </summary>
        [field: SerializeField] public float BaseValue { get; protected internal set; } = 1;

        /// <summary>
        ///     This method is used to clamp final stat value into valid range to provide ability to
        ///     limit stat values into desired ranges that won't break game systems.
        /// </summary>
        public virtual float GetFinalClampedValue(float value)
        {
            return value;
        }
        
        /// <summary>
        ///     Get final value of statistic with modifiers
        /// </summary>
        /// <param name="modifiers">Modifiers to apply</param>
        /// <returns>Final value of statistic</returns>
        public float GetFinalValue([NotNull] StatModifierCollection modifiers)
        {
            float result = BaseValue;
            modifiers.Apply(ref result);
            result = GetFinalClampedValue(result);
            return result;
        }
    }
}