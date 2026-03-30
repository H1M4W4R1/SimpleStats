using Systems.SimpleCore.Operations;

namespace Systems.SimpleStats.Operations
{
    public static class ModifierOperations
    {
        public const ushort SYSTEM_MODIFIER = 0x0010;

        // Success result codes
        public const ushort SUCCESS_MODIFIER_ADDED = 1;
        public const ushort SUCCESS_MODIFIER_REMOVED = 2;
        public const ushort SUCCESS_RECOMPUTE_COMPLETE = 3;

        // Error result codes
        public const ushort ERROR_MODIFIER_IS_NULL = 1;
        public const ushort ERROR_CONDITIONAL_FALSE = 2;
        public const ushort ERROR_INVALID_MODIFIER_TYPE = 3;
        public const ushort ERROR_MAX_MODIFIERS_EXCEEDED = 4;
        public const ushort ERROR_INCOMPATIBLE_WITH_EXISTING = 5;
        public const ushort ERROR_MODIFIER_NOT_FOUND = 6;
        public const ushort ERROR_MODIFIER_EXPIRED = 7;

        public static OperationResult Permitted()
            => OperationResult.Success(SYSTEM_MODIFIER, OperationResult.SUCCESS_PERMITTED);

        public static OperationResult ModifierAdded()
            => OperationResult.Success(SYSTEM_MODIFIER, SUCCESS_MODIFIER_ADDED);

        public static OperationResult ModifierRemoved()
            => OperationResult.Success(SYSTEM_MODIFIER, SUCCESS_MODIFIER_REMOVED);

        public static OperationResult RecomputeComplete()
            => OperationResult.Success(SYSTEM_MODIFIER, SUCCESS_RECOMPUTE_COMPLETE);

        public static OperationResult ModifierIsNull()
            => OperationResult.Error(SYSTEM_MODIFIER, ERROR_MODIFIER_IS_NULL);

        public static OperationResult ConditionalFalse()
            => OperationResult.Error(SYSTEM_MODIFIER, ERROR_CONDITIONAL_FALSE);

        public static OperationResult InvalidModifierType()
            => OperationResult.Error(SYSTEM_MODIFIER, ERROR_INVALID_MODIFIER_TYPE);

        public static OperationResult MaxModifiersExceeded()
            => OperationResult.Error(SYSTEM_MODIFIER, ERROR_MAX_MODIFIERS_EXCEEDED);

        public static OperationResult IncompatibleWithExisting()
            => OperationResult.Error(SYSTEM_MODIFIER, ERROR_INCOMPATIBLE_WITH_EXISTING);

        public static OperationResult ModifierNotFound()
            => OperationResult.Error(SYSTEM_MODIFIER, ERROR_MODIFIER_NOT_FOUND);

        public static OperationResult ModifierExpired()
            => OperationResult.Error(SYSTEM_MODIFIER, ERROR_MODIFIER_EXPIRED);
    }
}
