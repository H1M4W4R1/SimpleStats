using System.Collections.Generic;
using NUnit.Framework;
using Systems.SimpleCore.Operations;
using Systems.SimpleCore.Utility.Enums;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data.Collections;
using Systems.SimpleStats.Implementations;
using Systems.SimpleStats.Implementations.TimedModifiers;
using Systems.SimpleStats.Operations;

namespace Systems.SimpleStats.Tests
{
    public sealed class StatModifierCollectionTests : SimpleStatsTestBase
    {
        [Test]
        public void TryAddModifier_NullModifierReturnsErrorWithoutOwnerCallback()
        {
            TestModifierOwner owner = new TestModifierOwner();
            StatModifierCollection collection = new StatModifierCollection(owner);

            OperationResult result = collection.TryAddModifier(null);

            AssertSimilar(ModifierOperations.ModifierIsNull(), result);
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(0, owner.CanApplyCount);
            Assert.AreEqual(0, owner.AddFailedCount);
        }

        [Test]
        public void TryAddModifier_ExternalSuccessAddsModifierAndNotifiesOwner()
        {
            TestModifierOwner owner = new TestModifierOwner();
            StatModifierCollection collection = new StatModifierCollection(owner);
            IStatModifier modifier = new FlatAddModifier<TestStatistic>(5f);

            OperationResult result = collection.TryAddModifier(modifier);

            AssertSimilar(ModifierOperations.ModifierAdded(), result);
            Assert.AreEqual(1, collection.Count);
            Assert.AreSame(modifier, collection.Modifiers[0]);
            Assert.AreEqual(1, owner.CanApplyCount);
            Assert.AreEqual(1, owner.AddedCount);
            Assert.AreSame(modifier, owner.LastModifier);
            Assert.AreSame(owner, owner.LastOwner);
            Assert.AreEqual(ActionSource.External, owner.LastActionSource);
            Assert.AreEqual(ModifierOperations.SUCCESS_MODIFIER_ADDED, owner.LastResultCode);
        }

        [Test]
        public void TryAddModifier_InternalSuccessValidatesButSuppressesAddedCallback()
        {
            TestModifierOwner owner = new TestModifierOwner();
            StatModifierCollection collection = new StatModifierCollection(owner);
            IStatModifier modifier = new FlatAddModifier<TestStatistic>(5f);

            OperationResult result = collection.TryAddModifier(modifier, ActionSource.Internal);

            AssertSimilar(ModifierOperations.ModifierAdded(), result);
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(1, owner.CanApplyCount);
            Assert.AreEqual(0, owner.AddedCount);
            Assert.AreSame(modifier, owner.LastModifier);
            Assert.AreEqual(ActionSource.Internal, owner.LastActionSource);
        }

        [Test]
        public void TryAddModifier_OwnerRejectionBlocksExternalAddAndNotifiesFailure()
        {
            TestModifierOwner owner = new TestModifierOwner { RejectAdds = true };
            StatModifierCollection collection = new StatModifierCollection(owner);
            IStatModifier modifier = new FlatAddModifier<TestStatistic>(5f);

            OperationResult result = collection.TryAddModifier(modifier);

            AssertSimilar(ModifierOperations.MaxModifiersExceeded(), result);
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(1, owner.CanApplyCount);
            Assert.AreEqual(1, owner.AddFailedCount);
            Assert.AreSame(modifier, owner.LastModifier);
            Assert.AreEqual(ModifierOperations.ERROR_MAX_MODIFIERS_EXCEEDED, owner.LastResultCode);
        }

        [Test]
        public void TryAddModifier_OwnerRejectionBlocksInternalAddWithoutFailureCallback()
        {
            TestModifierOwner owner = new TestModifierOwner { RejectAdds = true };
            StatModifierCollection collection = new StatModifierCollection(owner);
            IStatModifier modifier = new FlatAddModifier<TestStatistic>(5f);

            OperationResult result = collection.TryAddModifier(modifier, ActionSource.Internal);

            AssertSimilar(ModifierOperations.MaxModifiersExceeded(), result);
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(1, owner.CanApplyCount);
            Assert.AreEqual(0, owner.AddFailedCount);
        }

        [Test]
        public void TryAddModifier_ExpiredTimedModifierFailsAndNotifiesExternalOwner()
        {
            TestModifierOwner owner = new TestModifierOwner();
            StatModifierCollection collection = new StatModifierCollection(owner);
            TimedFlatAddModifier<TestStatistic> modifier = new TimedFlatAddModifier<TestStatistic>(5f, 1f);
            ITimedModifier timedModifier = modifier;
            timedModifier.UpdateTime(1f);

            OperationResult result = collection.TryAddModifier(modifier);

            AssertSimilar(ModifierOperations.ModifierExpired(), result);
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(0, owner.CanApplyCount);
            Assert.AreEqual(1, owner.AddFailedCount);
            Assert.AreSame(modifier, owner.LastModifier);
            Assert.AreEqual(ModifierOperations.ERROR_MODIFIER_EXPIRED, owner.LastResultCode);
        }

        [Test]
        public void TryRemoveModifier_SuccessAndFailureCallbacksReflectRemovalResult()
        {
            TestModifierOwner owner = new TestModifierOwner();
            StatModifierCollection collection = new StatModifierCollection(owner);
            IStatModifier modifier = new FlatAddModifier<TestStatistic>(5f);
            collection.Add(modifier);

            OperationResult removed = collection.TryRemoveModifier(modifier);
            OperationResult missing = collection.TryRemoveModifier(modifier);

            AssertSimilar(ModifierOperations.ModifierRemoved(), removed);
            AssertSimilar(ModifierOperations.ModifierNotFound(), missing);
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(1, owner.RemovedCount);
            Assert.AreEqual(1, owner.RemoveFailedCount);
            Assert.AreSame(modifier, owner.LastModifier);
            Assert.AreEqual(ModifierOperations.ERROR_MODIFIER_NOT_FOUND, owner.LastResultCode);
        }

        [Test]
        public void TryRemoveModifier_InternalSuccessSuppressesCallbacks()
        {
            TestModifierOwner owner = new TestModifierOwner();
            StatModifierCollection collection = new StatModifierCollection(owner);
            IStatModifier modifier = new FlatAddModifier<TestStatistic>(5f);
            collection.Add(modifier);

            OperationResult result = collection.TryRemoveModifier(modifier, ActionSource.Internal);

            AssertSimilar(ModifierOperations.ModifierRemoved(), result);
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(0, owner.RemovedCount);
            Assert.AreEqual(0, owner.RemoveFailedCount);
        }

        [Test]
        public void LegacyMethods_HandleNullClearAndAddRange()
        {
            StatModifierCollection collection = new StatModifierCollection();
            IStatModifier first = new FinalAddModifier<TestStatistic>(1f);
            IStatModifier second = new FlatAddModifier<TestStatistic>(2f);
            List<IStatModifier> modifiers = new List<IStatModifier> { first, null, second };

            collection.Add(null);
            collection.AddRange(modifiers);

            Assert.AreEqual(2, collection.Count);
            Assert.IsFalse(collection.Remove(null));
            Assert.IsTrue(collection.Remove(first));
            Assert.AreEqual(1, collection.Count);

            collection.Clear();

            Assert.AreEqual(0, collection.Count);
        }

        [Test]
        public void Apply_SortsModifiersByOrderBeforeApplying()
        {
            StatModifierCollection collection = new StatModifierCollection();
            float value = 10f;
            collection.Add(new FinalAddModifier<TestStatistic>(3f));
            collection.Add(new PercentageFinalAddModifier<TestStatistic>(0.25f));
            collection.Add(new MultiplyModifier<TestStatistic>(2f));
            collection.Add(new PercentageAddModifier<TestStatistic>(0.1f));
            collection.Add(new FlatAddModifier<TestStatistic>(5f));

            collection.Apply(ref value);

            Assert.AreEqual(44.25f, value, 0.0001f);
        }

        [Test]
        public void Apply_ConditionalModifierReceivesInternalContextAndSkipsWhenFalse()
        {
            TestModifierOwner owner = new TestModifierOwner();
            StatModifierCollection collection = new StatModifierCollection(owner);
            ToggleConditionalFlatAddModifier active = new ToggleConditionalFlatAddModifier(5f, true);
            ToggleConditionalFinalAddModifier inactive = new ToggleConditionalFinalAddModifier(100f, false);
            float value = 10f;
            collection.Add(active);
            collection.Add(inactive);

            collection.Apply(ref value);

            Assert.AreEqual(15f, value);
            Assert.AreEqual(1, active.ShouldApplyCount);
            Assert.AreEqual(1, inactive.ShouldApplyCount);
            Assert.AreSame(active, active.LastModifier);
            Assert.AreSame(owner, active.LastOwner);
            Assert.AreEqual(ActionSource.Internal, active.LastActionSource);
        }

        [Test]
        public void GetActiveModifiers_SkipsExpiredAndFalseConditionalModifiers()
        {
            TestModifierOwner owner = new TestModifierOwner();
            StatModifierCollection collection = new StatModifierCollection(owner);
            IStatModifier alwaysActive = new FlatAddModifier<TestStatistic>(1f);
            TimedFlatAddModifier<TestStatistic> expired = new TimedFlatAddModifier<TestStatistic>(2f, 1f);
            ToggleConditionalFlatAddModifier inactive = new ToggleConditionalFlatAddModifier(3f, false);
            List<IStatModifier> output = new List<IStatModifier>();
            ITimedModifier timedExpired = expired;
            timedExpired.UpdateTime(1f);
            collection.Add(alwaysActive);
            collection.Add(expired);
            collection.Add(inactive);

            collection.GetActiveModifiers(output);

            Assert.AreEqual(1, output.Count);
            Assert.AreSame(alwaysActive, output[0]);
            Assert.AreEqual(1, inactive.ShouldApplyCount);
            Assert.AreSame(owner, inactive.LastOwner);
        }

        [Test]
        public void RecomputeAllModifiers_RemovesExpiredTimedModifiersAndNotifiesOwner()
        {
            TestModifierOwner owner = new TestModifierOwner();
            StatModifierCollection collection = new StatModifierCollection(owner);
            IStatModifier permanent = new FlatAddModifier<TestStatistic>(1f);
            TimedFlatAddModifier<TestStatistic> expired = new TimedFlatAddModifier<TestStatistic>(2f, 1f);
            ITimedModifier timedExpired = expired;
            timedExpired.UpdateTime(1f);
            collection.Add(permanent);
            collection.Add(expired);

            OperationResult result = collection.RecomputeAllModifiers();

            AssertSimilar(ModifierOperations.RecomputeComplete(), result);
            Assert.AreEqual(1, collection.Count);
            Assert.AreSame(permanent, collection.Modifiers[0]);
            Assert.AreEqual(1, owner.ExpiredCount);
            Assert.AreEqual(1, owner.RecomputeCompleteCount);
            Assert.AreSame(expired, owner.LastModifier);
            Assert.AreEqual(ModifierOperations.SUCCESS_RECOMPUTE_COMPLETE, owner.LastResultCode);
        }

        [Test]
        public void RecomputeAllModifiers_WithoutOwnerStillRemovesExpiredTimedModifiers()
        {
            StatModifierCollection collection = new StatModifierCollection();
            TimedFlatAddModifier<TestStatistic> expired = new TimedFlatAddModifier<TestStatistic>(2f, 1f);
            ITimedModifier timedExpired = expired;
            timedExpired.UpdateTime(1f);
            collection.Add(expired);

            OperationResult result = collection.RecomputeAllModifiers();

            AssertSimilar(ModifierOperations.RecomputeComplete(), result);
            Assert.AreEqual(0, collection.Count);
        }
    }
}
