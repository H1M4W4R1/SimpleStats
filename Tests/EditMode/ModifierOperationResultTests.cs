using NUnit.Framework;
using Systems.SimpleCore.Operations;
using Systems.SimpleStats.Operations;

namespace Systems.SimpleStats.Tests
{
    public sealed class ModifierOperationResultTests : SimpleStatsTestBase
    {
        [Test]
        public void SuccessFactories_CreateExpectedModifierSystemResults()
        {
            OperationResult permitted = ModifierOperations.Permitted();
            OperationResult added = ModifierOperations.ModifierAdded();
            OperationResult removed = ModifierOperations.ModifierRemoved();
            OperationResult recomputed = ModifierOperations.RecomputeComplete();

            Assert.IsTrue(permitted);
            Assert.IsTrue(added);
            Assert.IsTrue(removed);
            Assert.IsTrue(recomputed);
            Assert.AreEqual(ModifierOperations.SYSTEM_MODIFIER, permitted.systemCode);
            Assert.AreEqual(OperationResult.SUCCESS_PERMITTED, permitted.resultCode);
            Assert.AreEqual(ModifierOperations.SUCCESS_MODIFIER_ADDED, added.resultCode);
            Assert.AreEqual(ModifierOperations.SUCCESS_MODIFIER_REMOVED, removed.resultCode);
            Assert.AreEqual(ModifierOperations.SUCCESS_RECOMPUTE_COMPLETE, recomputed.resultCode);
        }

        [Test]
        public void ErrorFactories_CreateExpectedModifierSystemResults()
        {
            ushort[] expectedCodes =
            {
                ModifierOperations.ERROR_MODIFIER_IS_NULL,
                ModifierOperations.ERROR_CONDITIONAL_FALSE,
                ModifierOperations.ERROR_INVALID_MODIFIER_TYPE,
                ModifierOperations.ERROR_MAX_MODIFIERS_EXCEEDED,
                ModifierOperations.ERROR_INCOMPATIBLE_WITH_EXISTING,
                ModifierOperations.ERROR_MODIFIER_NOT_FOUND,
                ModifierOperations.ERROR_MODIFIER_EXPIRED
            };

            for (int index = 0; index < expectedCodes.Length; index++)
            {
                OperationResult result = CreateErrorResult(index);
                Assert.IsFalse(result);
                Assert.IsTrue(OperationResult.IsError(result));
                Assert.IsTrue(OperationResult.IsFromSystem(result, ModifierOperations.SYSTEM_MODIFIER));
                Assert.AreEqual(expectedCodes[index], result.resultCode);
            }
        }

        private static OperationResult CreateErrorResult(int index)
        {
            switch (index)
            {
                case 0:
                    return ModifierOperations.ModifierIsNull();
                case 1:
                    return ModifierOperations.ConditionalFalse();
                case 2:
                    return ModifierOperations.InvalidModifierType();
                case 3:
                    return ModifierOperations.MaxModifiersExceeded();
                case 4:
                    return ModifierOperations.IncompatibleWithExisting();
                case 5:
                    return ModifierOperations.ModifierNotFound();
                case 6:
                    return ModifierOperations.ModifierExpired();
                default:
                    Assert.Fail("Unhandled error result index " + index);
                    return ModifierOperations.ModifierIsNull();
            }
        }
    }
}
