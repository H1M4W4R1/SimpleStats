using System.Collections.Generic;
using Systems.SimpleCore.Examples;
using Systems.SimpleCore.Operations;
using Systems.SimpleStats.Abstract;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Collections;
using Systems.SimpleStats.Data.Statistics;
using Systems.SimpleStats.Implementations;
using Systems.SimpleStats.Implementations.TimedModifiers;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private bool _createRuntimeUI = true;
        [SerializeField] private bool _runExampleOnStart;

        private StatModifierCollection _modifiers;
        private ExampleHealthStatistic _healthStatistic;
        private ExampleRuntimePanel _panel;
        private string _lastResult = "none";
        private string _statusMessage = "Ready. Add modifiers to inspect final health.";

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
            if (_createRuntimeUI)
            {
                CreateRuntimeUI();
            }

            if (_runExampleOnStart)
            {
                RunExample();
            }
            else
            {
                RefreshStatus(_statusMessage);
            }
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
            RefreshStatus(_statusMessage);
        }

        [ContextMenu("Run Stats Example")]
        public void RunExample()
        {
            AddFlatModifier();
            AddPercentageModifier();
            AddTimedModifier();
            LogCurrentHealth("Initial");
            RefreshStatus("Added flat, percentage, and timed modifiers.");
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

        private void AddFlatModifier()
        {
            _lastResult = ExampleRuntimePanel.FormatResult(_modifiers.TryAddModifier(new FlatAddModifier<ExampleHealthStatistic>(_flatBonus)));
            RefreshStatus("Added flat health modifier.");
        }

        private void AddPercentageModifier()
        {
            _lastResult = ExampleRuntimePanel.FormatResult(_modifiers.TryAddModifier(new PercentageAddModifier<ExampleHealthStatistic>(_percentageBonus)));
            RefreshStatus("Added percentage health modifier.");
        }

        private void AddTimedModifier()
        {
            _lastResult = ExampleRuntimePanel.FormatResult(_modifiers.TryAddModifier(new TimedFlatAddModifier<ExampleHealthStatistic>(_timedBonus, _timedDuration)));
            RefreshStatus("Added timed health modifier.");
        }

        private void RecomputeModifiers()
        {
            _lastResult = ExampleRuntimePanel.FormatResult(_modifiers.RecomputeAllModifiers());
            RefreshStatus("Recomputed modifiers.");
        }

        private void ResetModifiers()
        {
            _modifiers.Clear();
            RefreshStatus("Cleared all modifiers.");
        }

        private void CreateRuntimeUI()
        {
            _panel = ExampleRuntimePanel.Create(
                "SimpleStats Example",
                "Navigate flat, percentage, and timed modifiers while watching the final stat value.");

            _panel.AddSection("Modifiers");
            Button flatButton = _panel.AddButton("Add Flat Bonus");
            flatButton.onClick.AddListener(AddFlatModifier);

            Button percentageButton = _panel.AddButton("Add Percentage Bonus");
            percentageButton.onClick.AddListener(AddPercentageModifier);

            Button timedButton = _panel.AddButton("Add Timed Bonus");
            timedButton.onClick.AddListener(AddTimedModifier);

            Button recomputeButton = _panel.AddButton("Recompute Modifiers");
            recomputeButton.onClick.AddListener(RecomputeModifiers);

            Button resetButton = _panel.AddButton("Reset Modifiers");
            resetButton.onClick.AddListener(ResetModifiers);

            Button runAllButton = _panel.AddButton("Run Full Example");
            runAllButton.onClick.AddListener(RunExample);
        }

        private void RefreshStatus(string message)
        {
            _statusMessage = message;
            if (ReferenceEquals(_panel, null))
            {
                return;
            }

            float finalHealth = _healthStatistic.GetFinalValue(_modifiers);
            _panel.SetStatus(
                message +
                "\nBase health: " + _baseHealth + " | Final health: " + finalHealth +
                "\nActive modifiers: " + _modifiers.Count +
                "\nLast result: " + _lastResult);
        }
    }
}
