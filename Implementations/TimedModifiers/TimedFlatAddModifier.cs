using System;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Implementations.TimedModifiers
{
    /// <summary>
    ///     Flat add modifier with a limited duration.
    ///     Automatically removed from the collection when expired.
    ///     Unity cannot serialize open generic types — create a closed subtype for Inspector use.
    /// </summary>
    [Serializable]
    public class TimedFlatAddModifier<TStatisticType> : IStatModifier<TStatisticType>, ITimedModifier
        where TStatisticType : StatisticBase
    {
        [field: SerializeField] public float BaseValue { get; private set; }
        [field: SerializeField] public float TotalDuration { get; private set; }

        public float GetValue() => BaseValue;
        
        public TimedFlatAddModifier(float baseValue, float duration)
        {
            BaseValue = baseValue;
            TotalDuration = duration;
            TimeRemaining = duration;
        }

        public int Order => (int) ModifierOrder.FlatAdd;

        public void Apply(ref float currentFloat) => currentFloat += GetValue();

        public float TimeRemaining { get; set; }
    }
}
