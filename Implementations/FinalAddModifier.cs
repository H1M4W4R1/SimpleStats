using System;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Implementations
{
    /// <summary>
    ///     Adds value to final value.
    ///     Unity cannot serialize open generic types. To use in the Inspector, create a concrete
    ///     closed subtype (e.g., <c>class HealthFinalAdd : FinalAddModifier&lt;HealthStat&gt; {}</c>).
    /// </summary>
    [Serializable]
    public sealed class FinalAddModifier<TStatisticType> : IStatModifier<TStatisticType>
        where TStatisticType : StatisticBase
    {
        public FinalAddModifier(float baseValue)
        {
            BaseValue = baseValue;
        }

        [field: SerializeField] public float BaseValue { get; private set; }
        
        public float GetValue() => BaseValue;
        
        public int Order => (int) ModifierOrder.FinalAdd;
        
        public void Apply(ref float currentFloat) => currentFloat += GetValue();
    }
}