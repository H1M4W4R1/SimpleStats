using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleCore.Operations;
using Systems.SimpleCore.Utility.Enums;
using Systems.SimpleStats.Abstract;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Operations;

namespace Systems.SimpleStats.Data.Collections
{
    /// <summary>
    ///     Collection of stat modifiers with validation, callbacks, and expiry support.
    ///     Callbacks and validation are delegated to the <see cref="IWithStatModifiers"/> owner.
    ///     Timed modifier updates are the responsibility of the owning entity.
    ///     Note: This collection is not thread-safe. Callers must synchronize externally
    ///     if concurrent access is needed.
    /// </summary>
    public sealed class StatModifierCollection
    {
        /// <summary>
        ///     Cached comparer to avoid delegate allocation on every sort
        /// </summary>
        private static readonly IComparer<IStatModifier> OrderComparer =
            Comparer<IStatModifier>.Create((a, b) => a.Order.CompareTo(b.Order));

        /// <summary>
        ///     Internal modifier storage
        /// </summary>
        private readonly List<IStatModifier> _modifiers = new();

        /// <summary>
        ///     Owner of this modifier collection, receives callbacks and validation calls
        /// </summary>
        [CanBeNull] private readonly IWithStatModifiers _owner;

        /// <summary>
        ///     True if modifiers are sorted
        /// </summary>
        private bool _isSorted = true;

        public StatModifierCollection()
        {
        }

        /// <summary>
        ///     Creates a collection with an owner for context-aware operations.
        ///     The owner receives all callbacks and validation calls.
        /// </summary>
        public StatModifierCollection([CanBeNull] IWithStatModifiers owner)
        {
            _owner = owner;
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        public StatModifierCollection([NotNull] IEnumerable<IStatModifier> modifiers,
            [CanBeNull] IWithStatModifiers owner = null)
        {
            _owner = owner;
            _modifiers.AddRange(modifiers);
            _isSorted = false;
        }

        /// <summary>
        ///     Count of modifiers in the collection
        /// </summary>
        public int Count => _modifiers.Count;

        /// <summary>
        ///     Read-only access to internal modifiers
        /// </summary>
        public IReadOnlyList<IStatModifier> Modifiers => _modifiers;

        /// <summary>
        ///     Apply all modifiers to statistic value.
        ///     Conditional modifiers that return false from ShouldApply are skipped.
        /// </summary>
        /// <param name="currentFloat">Current statistic value</param>
        public void Apply(ref float currentFloat)
        {
            EnsureSorted();

            for (int index = 0; index < _modifiers.Count; index++)
            {
                IStatModifier modifier = _modifiers[index];

                if (modifier is IConditionalModifier conditional)
                {
                    ModifierContext context = new(modifier, _owner, ActionSource.Internal);
                    if (!conditional.ShouldApply(in context))
                        continue;
                }

                modifier.Apply(ref currentFloat);
            }
        }

        /// <summary>
        ///     Add modifier with full three-phase validation.
        ///     Phase 1: Parameter validation (null, expired).
        ///     Phase 2: Business logic via <see cref="IWithStatModifiers.CanApplyModifier"/>.
        ///     Phase 3: Callback dispatch on success/failure.
        /// </summary>
        public OperationResult TryAddModifier(
            [CanBeNull] IStatModifier modifier,
            ActionSource actionSource = ActionSource.External)
        {
            // Phase 1: Parameter validation
            // Note: no OnModifierAddFailed callback here because ModifierContext requires a non-null modifier.
            if (ReferenceEquals(modifier, null))
                return ModifierOperations.ModifierIsNull();

            if (modifier is ITimedModifier {IsExpired: true})
            {
                OperationResult expired = ModifierOperations.ModifierExpired();
                if (actionSource == ActionSource.External && _owner != null)
                {
                    ModifierContext expiredContext = new(modifier, _owner, actionSource);
                    _owner.OnModifierAddFailed(in expiredContext, in expired);
                }
                return expired;
            }

            ModifierContext context = new(modifier, _owner, actionSource);

            // Phase 2: Business logic validation (delegated to owner)
            if (_owner != null)
            {
                OperationResult canApply = _owner.CanApplyModifier(in context);
                if (!canApply)
                {
                    if (actionSource == ActionSource.External)
                        _owner.OnModifierAddFailed(in context, in canApply);
                    return canApply;
                }
            }

            // Execute
            _modifiers.Add(modifier);
            _isSorted = false;

            OperationResult result = ModifierOperations.ModifierAdded();

            // Phase 3: Callbacks
            if (actionSource == ActionSource.External && _owner != null)
                _owner.OnModifierAdded(in context, in result);

            return result;
        }

        /// <summary>
        ///     Remove modifier with validation and callbacks
        /// </summary>
        public OperationResult TryRemoveModifier(
            [CanBeNull] IStatModifier modifier,
            ActionSource actionSource = ActionSource.External)
        {
            // Phase 1: Parameter validation
            if (ReferenceEquals(modifier, null))
                return ModifierOperations.ModifierIsNull();

            ModifierContext context = new(modifier, _owner, actionSource);

            // Execute
            if (!_modifiers.Remove(modifier))
            {
                OperationResult notFound = ModifierOperations.ModifierNotFound();
                if (actionSource == ActionSource.External && _owner != null)
                    _owner.OnModifierRemoveFailed(in context, in notFound);
                return notFound;
            }

            OperationResult result = ModifierOperations.ModifierRemoved();

            // Phase 3: Callbacks
            if (actionSource == ActionSource.External && _owner != null)
                _owner.OnModifierRemoved(in context, in result);

            return result;
        }

        /// <summary>
        ///     Add modifier without validation (legacy compatibility).
        ///     Prefer <see cref="TryAddModifier"/> for new code.
        /// </summary>
        public void Add([CanBeNull] IStatModifier modifier)
        {
            if (ReferenceEquals(modifier, null)) return;
            _modifiers.Add(modifier);
            _isSorted = false;
        }

        /// <summary>
        ///     Remove modifier without callbacks (legacy compatibility).
        ///     Prefer <see cref="TryRemoveModifier"/> for new code.
        /// </summary>
        public bool Remove([CanBeNull] IStatModifier modifier)
        {
            if (ReferenceEquals(modifier, null)) return false;
            return _modifiers.Remove(modifier);
        }

        /// <summary>
        ///     Clear all modifiers from collection
        /// </summary>
        public void Clear()
        {
            _modifiers.Clear();
            _isSorted = true;
        }

        /// <summary>
        ///     Add range of modifiers to collection
        /// </summary>
        public void AddRange([NotNull] IEnumerable<IStatModifier> modifiers)
        {
            if (modifiers is IReadOnlyList<IStatModifier> readOnlyList)
            {
                for (int index = 0; index < readOnlyList.Count; index++)
                {
                    IStatModifier modifier = readOnlyList[index];
                    if (ReferenceEquals(modifier, null)) continue;
                    _modifiers.Add(modifier);
                }
            }
            else if (modifiers is IList<IStatModifier> list)
            {
                for (int index = 0; index < list.Count; index++)
                {
                    IStatModifier modifier = list[index];
                    if (ReferenceEquals(modifier, null)) continue;
                    _modifiers.Add(modifier);
                }
            }
            else
            {
                using IEnumerator<IStatModifier> enumerator = modifiers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IStatModifier modifier = enumerator.Current;
                    if (ReferenceEquals(modifier, null)) continue;
                    _modifiers.Add(modifier);
                }
            }

            _isSorted = false;
        }

        /// <summary>
        ///     Removes expired timed modifiers and fires expiry callbacks.
        ///     Should be called by the owning entity after updating timed modifiers.
        /// </summary>
        public OperationResult RecomputeAllModifiers()
        {
            // Remove expired timed modifiers (iterate backwards for safe removal)
            for (int i = _modifiers.Count - 1; i >= 0; i--)
            {
                if (_modifiers[i] is not ITimedModifier {IsExpired: true}) continue;

                IStatModifier modifier = _modifiers[i];
                _modifiers.RemoveAt(i);

                if (_owner == null) continue;
                ModifierContext context = new(modifier, _owner, ActionSource.Internal);
                OperationResult expiredResult = ModifierOperations.ModifierRemoved();
                _owner.OnModifierExpired(in context, in expiredResult);
            }

            OperationResult result = ModifierOperations.RecomputeComplete();
            _owner?.OnRecomputeComplete(in result);
            return result;
        }

        /// <summary>
        ///     Collects modifiers that are currently active (not expired, conditions met)
        /// </summary>
        public void GetActiveModifiers([NotNull] List<IStatModifier> output)
        {
            for (int i = 0; i < _modifiers.Count; i++)
            {
                IStatModifier modifier = _modifiers[i];

                if (modifier is ITimedModifier { IsExpired: true })
                    continue;

                if (modifier is IConditionalModifier conditional)
                {
                    ModifierContext context = new(modifier, _owner, ActionSource.Internal);
                    if (!conditional.ShouldApply(in context))
                        continue;
                }

                output.Add(modifier);
            }
        }

        #region Private Helpers

        private void EnsureSorted()
        {
            if (_isSorted) return;
            _modifiers.Sort(OrderComparer);
            _isSorted = true;
        }

        #endregion
    }
}
