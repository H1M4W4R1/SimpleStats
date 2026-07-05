using System.Collections.Generic;
using NUnit.Framework;
using Systems.SimpleCore.Operations;
using Systems.SimpleCore.Utility.Enums;
using Systems.SimpleStats.Abstract;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;
using Systems.SimpleStats.Implementations;
using Systems.SimpleStats.Implementations.ConditionalModifiers;
using Systems.SimpleStats.Implementations.TimedConditionalModifiers;
using Systems.SimpleStats.Implementations.TimedModifiers;
using Systems.SimpleStats.Operations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.SimpleStats.Tests
{
    public abstract class SimpleStatsTestBase
    {
        private readonly List<Object> _createdObjects = new List<Object>();

        public enum ModifierKind
        {
            FlatAdd,
            PercentageAdd,
            Multiply,
            PercentageFinalAdd,
            FinalAdd
        }

        [TearDown]
        public void TearDown()
        {
            for (int index = _createdObjects.Count - 1; index >= 0; index--)
            {
                Object createdObject = _createdObjects[index];
                if (createdObject) Object.DestroyImmediate(createdObject);
            }

            _createdObjects.Clear();
        }

        protected TestStatistic CreateStatistic(float baseValue = 1f)
        {
            TestStatistic statistic = Track(ScriptableObject.CreateInstance<TestStatistic>());
            statistic.SetBaseValue(baseValue);
            return statistic;
        }

        protected ClampedTestStatistic CreateClampedStatistic(float baseValue, float minValue, float maxValue)
        {
            ClampedTestStatistic statistic = Track(ScriptableObject.CreateInstance<ClampedTestStatistic>());
            statistic.SetBaseValue(baseValue);
            statistic.SetClamp(minValue, maxValue);
            return statistic;
        }

        protected TUnityObject Track<TUnityObject>(TUnityObject unityObject)
            where TUnityObject : Object
        {
            _createdObjects.Add(unityObject);
            return unityObject;
        }

        protected static IStatModifier CreateStandardModifier(ModifierKind kind, float value)
        {
            switch (kind)
            {
                case ModifierKind.FlatAdd:
                    return new FlatAddModifier<TestStatistic>(value);
                case ModifierKind.PercentageAdd:
                    return new PercentageAddModifier<TestStatistic>(value);
                case ModifierKind.Multiply:
                    return new MultiplyModifier<TestStatistic>(value);
                case ModifierKind.PercentageFinalAdd:
                    return new PercentageFinalAddModifier<TestStatistic>(value);
                case ModifierKind.FinalAdd:
                    return new FinalAddModifier<TestStatistic>(value);
                default:
                    Assert.Fail("Unhandled modifier kind " + kind);
                    return null;
            }
        }

        protected static IStatModifier CreateTimedModifier(ModifierKind kind, float value, float duration)
        {
            switch (kind)
            {
                case ModifierKind.FlatAdd:
                    return new TimedFlatAddModifier<TestStatistic>(value, duration);
                case ModifierKind.PercentageAdd:
                    return new TimedPercentageAddModifier<TestStatistic>(value, duration);
                case ModifierKind.Multiply:
                    return new TimedMultiplyModifier<TestStatistic>(value, duration);
                case ModifierKind.PercentageFinalAdd:
                    return new TimedPercentageFinalAddModifier<TestStatistic>(value, duration);
                case ModifierKind.FinalAdd:
                    return new TimedFinalAddModifier<TestStatistic>(value, duration);
                default:
                    Assert.Fail("Unhandled modifier kind " + kind);
                    return null;
            }
        }

        protected static IStatModifier CreateConditionalModifier(
            ModifierKind kind,
            float value,
            bool shouldApply)
        {
            switch (kind)
            {
                case ModifierKind.FlatAdd:
                    return new ToggleConditionalFlatAddModifier(value, shouldApply);
                case ModifierKind.PercentageAdd:
                    return new ToggleConditionalPercentageAddModifier(value, shouldApply);
                case ModifierKind.Multiply:
                    return new ToggleConditionalMultiplyModifier(value, shouldApply);
                case ModifierKind.PercentageFinalAdd:
                    return new ToggleConditionalPercentageFinalAddModifier(value, shouldApply);
                case ModifierKind.FinalAdd:
                    return new ToggleConditionalFinalAddModifier(value, shouldApply);
                default:
                    Assert.Fail("Unhandled modifier kind " + kind);
                    return null;
            }
        }

        protected static IStatModifier CreateTimedConditionalModifier(
            ModifierKind kind,
            float value,
            float duration,
            bool shouldApply)
        {
            switch (kind)
            {
                case ModifierKind.FlatAdd:
                    return new ToggleTimedConditionalFlatAddModifier(value, duration, shouldApply);
                case ModifierKind.PercentageAdd:
                    return new ToggleTimedConditionalPercentageAddModifier(value, duration, shouldApply);
                case ModifierKind.Multiply:
                    return new ToggleTimedConditionalMultiplyModifier(value, duration, shouldApply);
                case ModifierKind.PercentageFinalAdd:
                    return new ToggleTimedConditionalPercentageFinalAddModifier(value, duration, shouldApply);
                case ModifierKind.FinalAdd:
                    return new ToggleTimedConditionalFinalAddModifier(value, duration, shouldApply);
                default:
                    Assert.Fail("Unhandled modifier kind " + kind);
                    return null;
            }
        }

        protected static int ExpectedOrder(ModifierKind kind)
        {
            switch (kind)
            {
                case ModifierKind.FlatAdd:
                    return (int)ModifierOrder.FlatAdd;
                case ModifierKind.PercentageAdd:
                    return (int)ModifierOrder.PercentageAdd;
                case ModifierKind.Multiply:
                    return (int)ModifierOrder.Multiply;
                case ModifierKind.PercentageFinalAdd:
                    return (int)ModifierOrder.PercentageFinalAdd;
                case ModifierKind.FinalAdd:
                    return (int)ModifierOrder.FinalAdd;
                default:
                    Assert.Fail("Unhandled modifier kind " + kind);
                    return 0;
            }
        }

        protected static float ExpectedAppliedValue(ModifierKind kind, float currentValue, float modifierValue)
        {
            switch (kind)
            {
                case ModifierKind.FlatAdd:
                case ModifierKind.FinalAdd:
                    return currentValue + modifierValue;
                case ModifierKind.PercentageAdd:
                case ModifierKind.PercentageFinalAdd:
                    return currentValue + currentValue * modifierValue;
                case ModifierKind.Multiply:
                    return currentValue * modifierValue;
                default:
                    Assert.Fail("Unhandled modifier kind " + kind);
                    return currentValue;
            }
        }

        protected static void AssertSimilar(OperationResult expected, OperationResult actual)
        {
            Assert.IsTrue(
                OperationResult.AreSimilar(expected, actual),
                "Expected similar result to " + expected.resultCode + " but received " + actual.resultCode);
        }
    }

    public class TestStatistic : StatisticBase
    {
        public void SetBaseValue(float value)
        {
            BaseValue = value;
        }
    }

    public sealed class DerivedTestStatistic : TestStatistic
    {
    }

    public sealed class OtherTestStatistic : StatisticBase
    {
    }

    public sealed class ClampedTestStatistic : TestStatistic
    {
        private float _minValue;
        private float _maxValue;

        public void SetClamp(float minValue, float maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public override float GetFinalClampedValue(float value)
        {
            return Mathf.Clamp(value, _minValue, _maxValue);
        }
    }

    public sealed class TestModifierOwner : IWithStatModifiers
    {
        private readonly List<IStatModifier> _modifiers = new List<IStatModifier>();

        public bool RejectAdds { get; set; }
        public int CanApplyCount { get; private set; }
        public int AddedCount { get; private set; }
        public int AddFailedCount { get; private set; }
        public int RemovedCount { get; private set; }
        public int RemoveFailedCount { get; private set; }
        public int ExpiredCount { get; private set; }
        public int RecomputeCompleteCount { get; private set; }
        public IStatModifier LastModifier { get; private set; }
        public IWithStatModifiers LastOwner { get; private set; }
        public ActionSource LastActionSource { get; private set; }
        public ushort LastSystemCode { get; private set; }
        public ushort LastResultCode { get; private set; }

        public IReadOnlyList<IStatModifier> GetAllModifiers()
        {
            return _modifiers;
        }

        public void AddDirect(IStatModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public OperationResult CanApplyModifier(in ModifierContext context)
        {
            CanApplyCount++;
            CaptureContext(in context);
            if (RejectAdds) return ModifierOperations.MaxModifiersExceeded();
            return ModifierOperations.Permitted();
        }

        public void OnModifierAdded(in ModifierContext context, in OperationResult result)
        {
            AddedCount++;
            Capture(in context, in result);
        }

        public void OnModifierAddFailed(in ModifierContext context, in OperationResult result)
        {
            AddFailedCount++;
            Capture(in context, in result);
        }

        public void OnModifierRemoved(in ModifierContext context, in OperationResult result)
        {
            RemovedCount++;
            Capture(in context, in result);
        }

        public void OnModifierRemoveFailed(in ModifierContext context, in OperationResult result)
        {
            RemoveFailedCount++;
            Capture(in context, in result);
        }

        public void OnModifierExpired(in ModifierContext context, in OperationResult result)
        {
            ExpiredCount++;
            Capture(in context, in result);
        }

        public void OnRecomputeComplete(in OperationResult result)
        {
            RecomputeCompleteCount++;
            LastSystemCode = result.systemCode;
            LastResultCode = result.resultCode;
        }

        private void Capture(in ModifierContext context, in OperationResult result)
        {
            CaptureContext(in context);
            LastSystemCode = result.systemCode;
            LastResultCode = result.resultCode;
        }

        private void CaptureContext(in ModifierContext context)
        {
            LastModifier = context.modifier;
            LastOwner = context.owner;
            LastActionSource = context.actionSource;
        }
    }

    public sealed class ToggleConditionalFlatAddModifier : ConditionalFlatAddModifier<TestStatistic>
    {
        private readonly ToggleConditionalRecorder _recorder;

        public ToggleConditionalFlatAddModifier(float baseValue, bool shouldApply) : base(baseValue)
        {
            _recorder = new ToggleConditionalRecorder(shouldApply);
        }

        public int ShouldApplyCount => _recorder.ShouldApplyCount;
        public IStatModifier LastModifier => _recorder.LastModifier;
        public IWithStatModifiers LastOwner => _recorder.LastOwner;
        public ActionSource LastActionSource => _recorder.LastActionSource;
        public bool ShouldApplyResult { get => _recorder.ShouldApplyResult; set => _recorder.ShouldApplyResult = value; }
        public override bool ShouldApply(in ModifierContext context) => _recorder.ShouldApply(in context);
    }

    public sealed class ToggleConditionalPercentageAddModifier : ConditionalPercentageAddModifier<TestStatistic>
    {
        private readonly ToggleConditionalRecorder _recorder;

        public ToggleConditionalPercentageAddModifier(float baseValue, bool shouldApply) : base(baseValue)
        {
            _recorder = new ToggleConditionalRecorder(shouldApply);
        }

        public int ShouldApplyCount => _recorder.ShouldApplyCount;
        public IStatModifier LastModifier => _recorder.LastModifier;
        public IWithStatModifiers LastOwner => _recorder.LastOwner;
        public ActionSource LastActionSource => _recorder.LastActionSource;
        public bool ShouldApplyResult { get => _recorder.ShouldApplyResult; set => _recorder.ShouldApplyResult = value; }
        public override bool ShouldApply(in ModifierContext context) => _recorder.ShouldApply(in context);
    }

    public sealed class ToggleConditionalMultiplyModifier : ConditionalMultiplyModifier<TestStatistic>
    {
        private readonly ToggleConditionalRecorder _recorder;

        public ToggleConditionalMultiplyModifier(float baseValue, bool shouldApply) : base(baseValue)
        {
            _recorder = new ToggleConditionalRecorder(shouldApply);
        }

        public int ShouldApplyCount => _recorder.ShouldApplyCount;
        public IStatModifier LastModifier => _recorder.LastModifier;
        public IWithStatModifiers LastOwner => _recorder.LastOwner;
        public ActionSource LastActionSource => _recorder.LastActionSource;
        public bool ShouldApplyResult { get => _recorder.ShouldApplyResult; set => _recorder.ShouldApplyResult = value; }
        public override bool ShouldApply(in ModifierContext context) => _recorder.ShouldApply(in context);
    }

    public sealed class ToggleConditionalPercentageFinalAddModifier : ConditionalPercentageFinalAddModifier<TestStatistic>
    {
        private readonly ToggleConditionalRecorder _recorder;

        public ToggleConditionalPercentageFinalAddModifier(float baseValue, bool shouldApply) : base(baseValue)
        {
            _recorder = new ToggleConditionalRecorder(shouldApply);
        }

        public int ShouldApplyCount => _recorder.ShouldApplyCount;
        public IStatModifier LastModifier => _recorder.LastModifier;
        public IWithStatModifiers LastOwner => _recorder.LastOwner;
        public ActionSource LastActionSource => _recorder.LastActionSource;
        public bool ShouldApplyResult { get => _recorder.ShouldApplyResult; set => _recorder.ShouldApplyResult = value; }
        public override bool ShouldApply(in ModifierContext context) => _recorder.ShouldApply(in context);
    }

    public sealed class ToggleConditionalFinalAddModifier : ConditionalFinalAddModifier<TestStatistic>
    {
        private readonly ToggleConditionalRecorder _recorder;

        public ToggleConditionalFinalAddModifier(float baseValue, bool shouldApply) : base(baseValue)
        {
            _recorder = new ToggleConditionalRecorder(shouldApply);
        }

        public int ShouldApplyCount => _recorder.ShouldApplyCount;
        public IStatModifier LastModifier => _recorder.LastModifier;
        public IWithStatModifiers LastOwner => _recorder.LastOwner;
        public ActionSource LastActionSource => _recorder.LastActionSource;
        public bool ShouldApplyResult { get => _recorder.ShouldApplyResult; set => _recorder.ShouldApplyResult = value; }
        public override bool ShouldApply(in ModifierContext context) => _recorder.ShouldApply(in context);
    }

    public sealed class ToggleTimedConditionalFlatAddModifier : TimedConditionalFlatAddModifier<TestStatistic>
    {
        private readonly ToggleConditionalRecorder _recorder;

        public ToggleTimedConditionalFlatAddModifier(float baseValue, float duration, bool shouldApply)
            : base(baseValue, duration)
        {
            _recorder = new ToggleConditionalRecorder(shouldApply);
        }

        public int ShouldApplyCount => _recorder.ShouldApplyCount;
        public IStatModifier LastModifier => _recorder.LastModifier;
        public IWithStatModifiers LastOwner => _recorder.LastOwner;
        public ActionSource LastActionSource => _recorder.LastActionSource;
        public bool ShouldApplyResult { get => _recorder.ShouldApplyResult; set => _recorder.ShouldApplyResult = value; }
        public override bool ShouldApply(in ModifierContext context) => _recorder.ShouldApply(in context);
    }

    public sealed class ToggleTimedConditionalPercentageAddModifier : TimedConditionalPercentageAddModifier<TestStatistic>
    {
        private readonly ToggleConditionalRecorder _recorder;

        public ToggleTimedConditionalPercentageAddModifier(float baseValue, float duration, bool shouldApply)
            : base(baseValue, duration)
        {
            _recorder = new ToggleConditionalRecorder(shouldApply);
        }

        public int ShouldApplyCount => _recorder.ShouldApplyCount;
        public IStatModifier LastModifier => _recorder.LastModifier;
        public IWithStatModifiers LastOwner => _recorder.LastOwner;
        public ActionSource LastActionSource => _recorder.LastActionSource;
        public bool ShouldApplyResult { get => _recorder.ShouldApplyResult; set => _recorder.ShouldApplyResult = value; }
        public override bool ShouldApply(in ModifierContext context) => _recorder.ShouldApply(in context);
    }

    public sealed class ToggleTimedConditionalMultiplyModifier : TimedConditionalMultiplyModifier<TestStatistic>
    {
        private readonly ToggleConditionalRecorder _recorder;

        public ToggleTimedConditionalMultiplyModifier(float baseValue, float duration, bool shouldApply)
            : base(baseValue, duration)
        {
            _recorder = new ToggleConditionalRecorder(shouldApply);
        }

        public int ShouldApplyCount => _recorder.ShouldApplyCount;
        public IStatModifier LastModifier => _recorder.LastModifier;
        public IWithStatModifiers LastOwner => _recorder.LastOwner;
        public ActionSource LastActionSource => _recorder.LastActionSource;
        public bool ShouldApplyResult { get => _recorder.ShouldApplyResult; set => _recorder.ShouldApplyResult = value; }
        public override bool ShouldApply(in ModifierContext context) => _recorder.ShouldApply(in context);
    }

    public sealed class ToggleTimedConditionalPercentageFinalAddModifier
        : TimedConditionalPercentageFinalAddModifier<TestStatistic>
    {
        private readonly ToggleConditionalRecorder _recorder;

        public ToggleTimedConditionalPercentageFinalAddModifier(float baseValue, float duration, bool shouldApply)
            : base(baseValue, duration)
        {
            _recorder = new ToggleConditionalRecorder(shouldApply);
        }

        public int ShouldApplyCount => _recorder.ShouldApplyCount;
        public IStatModifier LastModifier => _recorder.LastModifier;
        public IWithStatModifiers LastOwner => _recorder.LastOwner;
        public ActionSource LastActionSource => _recorder.LastActionSource;
        public bool ShouldApplyResult { get => _recorder.ShouldApplyResult; set => _recorder.ShouldApplyResult = value; }
        public override bool ShouldApply(in ModifierContext context) => _recorder.ShouldApply(in context);
    }

    public sealed class ToggleTimedConditionalFinalAddModifier : TimedConditionalFinalAddModifier<TestStatistic>
    {
        private readonly ToggleConditionalRecorder _recorder;

        public ToggleTimedConditionalFinalAddModifier(float baseValue, float duration, bool shouldApply)
            : base(baseValue, duration)
        {
            _recorder = new ToggleConditionalRecorder(shouldApply);
        }

        public int ShouldApplyCount => _recorder.ShouldApplyCount;
        public IStatModifier LastModifier => _recorder.LastModifier;
        public IWithStatModifiers LastOwner => _recorder.LastOwner;
        public ActionSource LastActionSource => _recorder.LastActionSource;
        public bool ShouldApplyResult { get => _recorder.ShouldApplyResult; set => _recorder.ShouldApplyResult = value; }
        public override bool ShouldApply(in ModifierContext context) => _recorder.ShouldApply(in context);
    }

    public sealed class SourceFlatAddModifier : IStatModifier<TestStatistic>, IModifierSource<string>
    {
        private readonly string _source;
        private readonly float _value;

        public SourceFlatAddModifier(string source, float value)
        {
            _source = source;
            _value = value;
        }

        public int Order => (int)ModifierOrder.FlatAdd;

        public void Apply(ref float currentFloat)
        {
            currentFloat += _value;
        }

        public string GetSource()
        {
            return _source;
        }
    }

    public sealed class OtherStatModifier : IStatModifier<OtherTestStatistic>
    {
        public int Order => (int)ModifierOrder.FlatAdd;

        public void Apply(ref float currentFloat)
        {
            currentFloat += 100f;
        }
    }

    internal sealed class ToggleConditionalRecorder
    {
        public ToggleConditionalRecorder(bool shouldApply)
        {
            ShouldApplyResult = shouldApply;
        }

        public bool ShouldApplyResult { get; set; }
        public int ShouldApplyCount { get; private set; }
        public IStatModifier LastModifier { get; private set; }
        public IWithStatModifiers LastOwner { get; private set; }
        public ActionSource LastActionSource { get; private set; }

        public bool ShouldApply(in ModifierContext context)
        {
            ShouldApplyCount++;
            LastModifier = context.modifier;
            LastOwner = context.owner;
            LastActionSource = context.actionSource;
            return ShouldApplyResult;
        }
    }
}
