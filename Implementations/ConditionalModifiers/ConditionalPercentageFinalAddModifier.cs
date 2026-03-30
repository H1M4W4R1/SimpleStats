using System;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Implementations.ConditionalModifiers
{
    /// <summary>
    ///     Percentage final add modifier that only applies when <see cref="ShouldApply"/> returns true.
    ///     Override <see cref="ShouldApply"/> in a concrete subclass to define the condition.
    ///     Unity cannot serialize open generic types — create a closed subtype for Inspector use.
    /// </summary>
    [Serializable]
    public abstract class ConditionalPercentageFinalAddModifier<TStatisticType> : IStatModifier<TStatisticType>, IConditionalModifier
        where TStatisticType : StatisticBase
    {
        [field: SerializeField] public float BaseValue { get; private set; }

        public virtual float GetValue() => BaseValue;
        
        public ConditionalPercentageFinalAddModifier(float baseValue)
        {
            BaseValue = baseValue;
        }

        public int Order => (int) ModifierOrder.PercentageFinalAdd;

        public void Apply(ref float currentFloat) => currentFloat += currentFloat * GetValue();

        /// <inheritdoc />
        public abstract bool ShouldApply(in ModifierContext context);
    }
}
