using System;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Implementations
{
    /// <summary>
    ///     Adds value to base value.
    ///     Unity cannot serialize open generic types. To use in the Inspector, create a concrete
    ///     closed subtype (e.g., <c>class HealthFlatAdd : FlatAddModifier&lt;HealthStat&gt; {}</c>).
    /// </summary>
    [Serializable]
    public sealed class FlatAddModifier<TStatisticType> : IStatModifier<TStatisticType>
        where TStatisticType : StatisticBase
    {
        public FlatAddModifier(float baseValue)
        {
            BaseValue = baseValue;
        }

        [field: SerializeField] public float BaseValue { get; private set; }
        
        public float GetValue() => BaseValue;
        
        public int Order => (int) ModifierOrder.FlatAdd;
        
        public void Apply(ref float currentFloat) => currentFloat += GetValue();
    }
}