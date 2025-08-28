using System;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Implementations
{
    /// <summary>
    ///     Multiplies value by given multiplier
    /// </summary>
    [Serializable]
    public sealed class MultiplyModifier<TStatisticType> : IStatModifier<TStatisticType>
        where TStatisticType : StatisticBase
    {
        public MultiplyModifier(float value)
        {
            Value = value;
        }

        [field: SerializeField] public float Value { get; private set; }
        
        public int Order => (int) ModifierOrder.Multiply;
        
        public void Apply(ref float currentFloat) => currentFloat *= Value;
    }
}