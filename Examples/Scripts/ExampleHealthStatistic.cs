using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleStats.Examples.Scripts
{
    public sealed class ExampleHealthStatistic : StatisticBase
    {
        private float _maxValue;

        public void Configure(float baseValue, float maxValue)
        {
            BaseValue = baseValue;
            _maxValue = maxValue;
        }

        public override float GetFinalClampedValue(float value)
        {
            return Mathf.Clamp(value, 0f, _maxValue);
        }
    }
}