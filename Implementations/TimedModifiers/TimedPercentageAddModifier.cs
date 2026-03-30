using System;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Implementations.TimedModifiers
{
    /// <summary>
    ///     Percentage add modifier with a limited duration.
    ///     Automatically removed from the collection when expired.
    ///     Unity cannot serialize open generic types — create a closed subtype for Inspector use.
    /// </summary>
    [Serializable]
    public class TimedPercentageAddModifier<TStatisticType> : IStatModifier<TStatisticType>, ITimedModifier
        where TStatisticType : StatisticBase
    {
        [field: SerializeField] public float BaseValue { get; private set; }
        [field: SerializeField] public float TotalDuration { get; private set; }

        public float GetValue() => BaseValue;
        
        public TimedPercentageAddModifier(float baseValue, float duration)
        {
            BaseValue = baseValue;
            TotalDuration = duration;
            TimeRemaining = duration;
        }

        public int Order => (int) ModifierOrder.PercentageAdd;

        public void Apply(ref float currentFloat) => currentFloat += currentFloat * GetValue();

        public float TimeRemaining { get; set; }
    }
}
