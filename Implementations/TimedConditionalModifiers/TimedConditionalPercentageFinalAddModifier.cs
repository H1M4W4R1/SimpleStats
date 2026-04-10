using System;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Implementations.TimedConditionalModifiers
{
    /// <summary>
    ///     Percentage final add modifier that is both time-limited and conditional.
    ///     Only applies when <see cref="ShouldApply"/> returns true, and auto-removes when expired.
    ///     Override <see cref="ShouldApply"/> in a concrete subclass to define the condition.
    ///     Unity cannot serialize open generic types — create a closed subtype for Inspector use.
    /// </summary>
    [Serializable]
    public abstract class TimedConditionalPercentageFinalAddModifier<TStatisticType>
        : IStatModifier<TStatisticType>, ITimedModifier, IConditionalModifier
        where TStatisticType : StatisticBase
    {
        [field: SerializeField] public float BaseValue { get; private set; }
        [field: SerializeField] public float TotalDuration { get; private set; }

        public virtual float GetValue() => BaseValue;
        
        public TimedConditionalPercentageFinalAddModifier(float baseValue, float duration)
        {
            BaseValue = baseValue;
            TotalDuration = duration;
            TimeRemaining = duration;
        }

        public int Order => (int) ModifierOrder.PercentageFinalAdd;

        public void Apply(ref float currentFloat) => currentFloat += currentFloat * GetValue();

        public float TimeRemaining { get; set; }

        /// <inheritdoc />
        public abstract bool ShouldApply(in ModifierContext context);
    }
}
