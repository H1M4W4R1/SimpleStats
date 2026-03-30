using System;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Implementations
{
    /// <summary>
    ///     Multiplies value by given multiplier.
    ///     Unity cannot serialize open generic types. To use in the Inspector, create a concrete
    ///     closed subtype (e.g., <c>class HealthMultiply : MultiplyModifier&lt;HealthStat&gt; {}</c>).
    /// </summary>
    [Serializable]
    public sealed class MultiplyModifier<TStatisticType> : IStatModifier<TStatisticType>
        where TStatisticType : StatisticBase
    {
        public MultiplyModifier(float baseValue)
        {
            BaseValue = baseValue;
        }

        [field: SerializeField] public float BaseValue { get; private set; }
        
        public float GetValue() => BaseValue;
        
        public int Order => (int) ModifierOrder.Multiply;
        
        public void Apply(ref float currentFloat) => currentFloat *= GetValue();
    }
}