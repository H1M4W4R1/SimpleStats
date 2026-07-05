using NUnit.Framework;
using Systems.SimpleStats.Abstract.Modifiers;

namespace Systems.SimpleStats.Tests
{
    public sealed class ModifierImplementationTests : SimpleStatsTestBase
    {
        [TestCase(ModifierKind.FlatAdd, 10f, 3f)]
        [TestCase(ModifierKind.PercentageAdd, 10f, 0.25f)]
        [TestCase(ModifierKind.Multiply, 10f, 1.5f)]
        [TestCase(ModifierKind.PercentageFinalAdd, 10f, 0.25f)]
        [TestCase(ModifierKind.FinalAdd, 10f, 3f)]
        public void StandardModifiers_ReportExpectedOrderAndApplyExpectedValue(
            ModifierKind kind,
            float currentValue,
            float modifierValue)
        {
            IStatModifier modifier = CreateStandardModifier(kind, modifierValue);
            float result = currentValue;

            modifier.Apply(ref result);

            Assert.AreEqual(ExpectedOrder(kind), modifier.Order);
            Assert.AreEqual(ExpectedAppliedValue(kind, currentValue, modifierValue), result, 0.0001f);
        }

        [TestCase(ModifierKind.FlatAdd)]
        [TestCase(ModifierKind.PercentageAdd)]
        [TestCase(ModifierKind.Multiply)]
        [TestCase(ModifierKind.PercentageFinalAdd)]
        [TestCase(ModifierKind.FinalAdd)]
        public void TimedModifiers_TrackDurationAndExpireAfterTimeUpdate(ModifierKind kind)
        {
            IStatModifier modifier = CreateTimedModifier(kind, 2f, 5f);
            ITimedModifier timedModifier = (ITimedModifier)modifier;

            Assert.AreEqual(5f, timedModifier.TotalDuration);
            Assert.AreEqual(5f, timedModifier.TimeRemaining);
            Assert.IsFalse(timedModifier.IsExpired);

            timedModifier.UpdateTime(2f);

            Assert.AreEqual(3f, timedModifier.TimeRemaining);
            Assert.IsFalse(timedModifier.IsExpired);

            timedModifier.UpdateTime(3f);

            Assert.AreEqual(0f, timedModifier.TimeRemaining);
            Assert.IsTrue(timedModifier.IsExpired);
        }

        [TestCase(ModifierKind.FlatAdd, 10f, 3f)]
        [TestCase(ModifierKind.PercentageAdd, 10f, 0.25f)]
        [TestCase(ModifierKind.Multiply, 10f, 1.5f)]
        [TestCase(ModifierKind.PercentageFinalAdd, 10f, 0.25f)]
        [TestCase(ModifierKind.FinalAdd, 10f, 3f)]
        public void ConditionalModifiers_ApplyOnlyWhenConditionAllows(
            ModifierKind kind,
            float currentValue,
            float modifierValue)
        {
            IStatModifier activeModifier = CreateConditionalModifier(kind, modifierValue, true);
            IStatModifier inactiveModifier = CreateConditionalModifier(kind, modifierValue, false);
            IConditionalModifier activeConditional = (IConditionalModifier)activeModifier;
            IConditionalModifier inactiveConditional = (IConditionalModifier)inactiveModifier;
            float activeResult = currentValue;
            float inactiveResult = currentValue;

            if (activeConditional.ShouldApply(default))
                activeModifier.Apply(ref activeResult);

            if (inactiveConditional.ShouldApply(default))
                inactiveModifier.Apply(ref inactiveResult);

            Assert.AreEqual(ExpectedOrder(kind), activeModifier.Order);
            Assert.AreEqual(ExpectedAppliedValue(kind, currentValue, modifierValue), activeResult, 0.0001f);
            Assert.AreEqual(currentValue, inactiveResult, 0.0001f);
        }

        [TestCase(ModifierKind.FlatAdd, 10f, 3f)]
        [TestCase(ModifierKind.PercentageAdd, 10f, 0.25f)]
        [TestCase(ModifierKind.Multiply, 10f, 1.5f)]
        [TestCase(ModifierKind.PercentageFinalAdd, 10f, 0.25f)]
        [TestCase(ModifierKind.FinalAdd, 10f, 3f)]
        public void TimedConditionalModifiers_CombineDurationConditionAndValue(
            ModifierKind kind,
            float currentValue,
            float modifierValue)
        {
            IStatModifier modifier = CreateTimedConditionalModifier(kind, modifierValue, 4f, true);
            ITimedModifier timedModifier = (ITimedModifier)modifier;
            IConditionalModifier conditionalModifier = (IConditionalModifier)modifier;
            float result = currentValue;

            timedModifier.UpdateTime(1.5f);
            if (conditionalModifier.ShouldApply(default))
                modifier.Apply(ref result);

            Assert.AreEqual(4f, timedModifier.TotalDuration);
            Assert.AreEqual(2.5f, timedModifier.TimeRemaining);
            Assert.IsFalse(timedModifier.IsExpired);
            Assert.AreEqual(ExpectedAppliedValue(kind, currentValue, modifierValue), result, 0.0001f);
        }
    }
}
