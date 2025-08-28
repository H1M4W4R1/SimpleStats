using System;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Implementations
{
    /// <summary>
    ///     Adds value to final value
    /// </summary>
    [Serializable]
    public sealed class FinalAddModifier<TStatisticType> : IStatModifier<TStatisticType>
        where TStatisticType : StatisticBase
    {
        public FinalAddModifier(float value)
        {
            Value = value;
        }

        [field: SerializeField] public float Value { get; private set; }
        
        public int Order => (int) ModifierOrder.FinalAdd;
        
        public void Apply(ref float currentFloat) => currentFloat += Value;
    }
}