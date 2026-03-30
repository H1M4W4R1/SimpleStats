using System;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Implementations
{
    /// <summary>
    ///     Adds a percentage of the current value. Executes after flat adds and before multiply.
    ///     A value of 0.1 adds 10% of the current value.
    ///     Unity cannot serialize open generic types. To use in the Inspector, create a concrete
    ///     closed subtype (e.g., <c>class HealthPercentageAdd : PercentageAddModifier&lt;HealthStat&gt; {}</c>).
    /// </summary>
    [Serializable]
    public sealed class PercentageAddModifier<TStatisticType> : IStatModifier<TStatisticType>
        where TStatisticType : StatisticBase
    {
        public PercentageAddModifier(float baseValue)
        {
            BaseValue = baseValue;
        }

        [field: SerializeField] public float BaseValue { get; private set; }

        public float GetValue() => BaseValue;
        
        public int Order => (int) ModifierOrder.PercentageAdd;

        public void Apply(ref float currentFloat) => currentFloat += currentFloat * GetValue();
    }
}
