using System.Collections.Generic;
using Systems.SimpleCore.Operations;
using Systems.SimpleStats.Abstract;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Collections;
using Systems.SimpleStats.Data.Statistics;
using Systems.SimpleStats.Implementations;
using Systems.SimpleStats.Implementations.TimedModifiers;
using UnityEngine;

namespace Systems.SimpleStats.Examples.Scripts
{
    [DisallowMultipleComponent]
    public sealed class ExampleStatOwner : MonoBehaviour, IWithStatModifiers
    {
        [SerializeField] private float _baseHealth = 100f;
        [SerializeField] private float _maxHealth = 250f;
        [SerializeField] private float _flatBonus = 25f;
        [SerializeField] private float _percentageBonus = 0.1f;
        [SerializeField] private float _timedBonus = 50f;
        [SerializeField] private float _timedDuration = 3f;

        private StatModifierCollection _modifiers;
        private ExampleHealthStatistic _healthStatistic;

        public IReadOnlyList<IStatModifier> GetAllModifiers()
        {
            return _modifiers.Modifiers;
        }

        private void Awake()
        {
            _modifiers = new StatModifierCollection(this);
            _healthStatistic = ScriptableObject.CreateInstance<ExampleHealthStatistic>();
            _healthStatistic.Configure(_baseHealth, _maxHealth);
        }

        private void Start()
        {
            _modifiers.TryAddModifier(new FlatAddModifier<ExampleHealthStatistic>(_flatBonus));
            _modifiers.TryAddModifier(new PercentageAddModifier<ExampleHealthStatistic>(_percentageBonus));
            _modifiers.TryAddModifier(new TimedFlatAddModifier<ExampleHealthStatistic>(_timedBonus, _timedDuration));
            LogCurrentHealth("Initial");
        }

        private void Update()
        {
            IReadOnlyList<IStatModifier> modifiers = _modifiers.Modifiers;
            for (int modifierIndex = 0; modifierIndex < modifiers.Count; modifierIndex++)
            {
                IStatModifier modifier = modifiers[modifierIndex];
                if (modifier is ITimedModifier timedModifier)
                    timedModifier.UpdateTime(Time.deltaTime);
            }

            _modifiers.RecomputeAllModifiers();
        }

        public void OnModifierAdded(in ModifierContext context, in OperationResult result)
        {
            Debug.Log("[SimpleStats] Modifier added: " + context.modifier.GetType().Name);
        }

        public void OnModifierExpired(in ModifierContext context, in OperationResult result)
        {
            LogCurrentHealth("Expired " + context.modifier.GetType().Name);
        }

        private void LogCurrentHealth(string label)
        {
            float finalHealth = _healthStatistic.GetFinalValue(_modifiers);
            Debug.Log("[SimpleStats] " + label + " health: " + finalHealth);
        }
    }
}
