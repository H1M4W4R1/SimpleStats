using System;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Implementations
{
    /// <summary>
    ///     Adds value to base value
    /// </summary>
    [Serializable]
    public sealed class FlatAddModifier<TStatisticType> : IStatModifier<TStatisticType>
        where TStatisticType : StatisticBase
    {
        public FlatAddModifier(float value)
        {
            Value = value;
        }

        [field: SerializeField] public float Value { get; private set; }
        
        public int Order => (int) ModifierOrder.FlatAdd;
        
        public void Apply(ref float currentFloat) => currentFloat += Value;
    }
}