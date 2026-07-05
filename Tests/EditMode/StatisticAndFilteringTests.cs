using System.Collections.Generic;
using NUnit.Framework;
using Systems.SimpleStats.Abstract;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data.Collections;
using Systems.SimpleStats.Implementations;

namespace Systems.SimpleStats.Tests
{
    public sealed class StatisticAndFilteringTests : SimpleStatsTestBase
    {
        [Test]
        public void Statistic_GetFinalValue_AppliesModifiersThenClampsResult()
        {
            ClampedTestStatistic statistic = CreateClampedStatistic(90f, 0f, 100f);
            StatModifierCollection modifiers = new StatModifierCollection();
            modifiers.Add(new FlatAddModifier<TestStatistic>(20f));

            float finalValue = statistic.GetFinalValue(modifiers);

            Assert.AreEqual(100f, finalValue);
        }

        [Test]
        public void IWithStatModifiers_GetAllModifiersForType_FiltersCompatibleModifiers()
        {
            TestModifierOwner owner = new TestModifierOwner();
            IStatModifier matching = new FlatAddModifier<TestStatistic>(1f);
            IStatModifier other = new OtherStatModifier();
            List<IStatModifier> output = new List<IStatModifier>();
            IWithStatModifiers withModifiers = owner;
            owner.AddDirect(matching);
            owner.AddDirect(other);

            withModifiers.GetAllModifiersFor<TestStatistic>(output);

            Assert.AreEqual(1, output.Count);
            Assert.AreSame(matching, output[0]);
        }

        [Test]
        public void IWithStatModifiers_GetAllModifiersForStatistic_FiltersByStatisticInstance()
        {
            TestModifierOwner owner = new TestModifierOwner();
            TestStatistic statistic = CreateStatistic();
            OtherTestStatistic otherStatistic = Track(UnityEngine.ScriptableObject.CreateInstance<OtherTestStatistic>());
            IStatModifier matching = new FlatAddModifier<TestStatistic>(1f);
            IStatModifier other = new OtherStatModifier();
            List<IStatModifier> output = new List<IStatModifier>();
            IWithStatModifiers withModifiers = owner;
            owner.AddDirect(matching);
            owner.AddDirect(other);

            withModifiers.GetAllModifiersFor(statistic, output);

            Assert.AreEqual(1, output.Count);
            Assert.AreSame(matching, output[0]);

            output.Clear();
            withModifiers.GetAllModifiersFor(otherStatistic, output);

            Assert.AreEqual(1, output.Count);
            Assert.AreSame(other, output[0]);
        }

        [Test]
        public void IWithStatModifiers_TransferModifiersTo_CopiesCompatibleModifiersToCollection()
        {
            TestModifierOwner owner = new TestModifierOwner();
            IStatModifier matching = new FlatAddModifier<TestStatistic>(1f);
            IStatModifier other = new OtherStatModifier();
            StatModifierCollection target = new StatModifierCollection();
            IWithStatModifiers withModifiers = owner;
            owner.AddDirect(matching);
            owner.AddDirect(other);

            withModifiers.TransferModifiersTo<TestStatistic>(target);

            Assert.AreEqual(1, target.Count);
            Assert.AreSame(matching, target.Modifiers[0]);
        }

        [Test]
        public void IStatModifier_IsValidFor_AllowsBaseModifierForDerivedStatisticQueries()
        {
            IStatModifier modifier = new FlatAddModifier<TestStatistic>(1f);
            DerivedTestStatistic derivedStatistic = Track(UnityEngine.ScriptableObject.CreateInstance<DerivedTestStatistic>());
            OtherTestStatistic otherStatistic = Track(UnityEngine.ScriptableObject.CreateInstance<OtherTestStatistic>());

            Assert.IsTrue(modifier.IsValidFor<TestStatistic>());
            Assert.IsTrue(modifier.IsValidFor<DerivedTestStatistic>());
            Assert.IsTrue(modifier.IsValidFor(derivedStatistic));
            Assert.IsFalse(modifier.IsValidFor<OtherTestStatistic>());
            Assert.IsFalse(modifier.IsValidFor(otherStatistic));
        }

        [Test]
        public void ModifierSource_GetRawSourceReturnsTypedSource()
        {
            SourceFlatAddModifier modifier = new SourceFlatAddModifier("Sword", 5f);
            IModifierSource rawSource = modifier;
            float value = 10f;

            modifier.Apply(ref value);

            Assert.AreEqual("Sword", modifier.GetSource());
            Assert.AreEqual("Sword", rawSource.GetRawSource());
            Assert.AreEqual(15f, value);
        }
    }
}
