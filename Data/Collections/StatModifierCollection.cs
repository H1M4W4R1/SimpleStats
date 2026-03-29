using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleStats.Abstract.Modifiers;

namespace Systems.SimpleStats.Data.Collections
{
    /// <summary>
    ///     Collection of stat modifiers.
    ///     Note: This collection is not thread-safe. If modifiers are added/removed from one thread
    ///     while Apply iterates on another, the internal list may throw or produce corrupt results.
    ///     Callers must synchronize externally if concurrent access is needed.
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
        private readonly List<IStatModifier> _modifiers = new List<IStatModifier>();

        /// <summary>
        ///     True if modifiers are sorted
        /// </summary>
        private bool _isSorted = true;
        
        /// <summary>
        ///     Apply all modifiers to statistic value
        /// </summary>
        /// <param name="currentFloat">Current statistic value</param>
        public void Apply(ref float currentFloat)
        {
            // Sort if necessary
            if (!_isSorted)
            {
                _modifiers.Sort(OrderComparer);
                _isSorted = true;
            }
            
            // Apply modifiers in order
            for (int index = 0; index < _modifiers.Count; index++)
            {
                IStatModifier modifier = _modifiers[index];
                modifier.Apply(ref currentFloat);
            }
        }

        /// <summary>
        ///     Add modifier to collection
        /// </summary>
        /// <param name="modifier">Modifier to add</param>
        public void Add([CanBeNull] IStatModifier modifier)
        {
            if (ReferenceEquals(modifier, null)) return;
            _modifiers.Add(modifier);
            _isSorted = false;
        }

        /// <summary>
        ///     Remove modifier from collection
        /// </summary>
        /// <returns>True if modifier was found and removed, false otherwise</returns>
        public bool Remove([CanBeNull] IStatModifier modifier)
        {
            if (ReferenceEquals(modifier, null)) return false;
            return _modifiers.Remove(modifier);
        }

        /// <summary>
        ///     Count of modifiers
        /// </summary>
        public int Count => _modifiers.Count;

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
            _modifiers.AddRange(modifiers);
            _isSorted = false;
        }

        public StatModifierCollection()
        {
            // Default constructor
        }

        /// <summary>
        ///     Copy constructor. Note: <see cref="_isSorted"/> is initialized to <c>true</c> by
        ///     the field initializer, but <see cref="AddRange"/> sets it to <c>false</c>.
        ///     If AddRange is ever removed from this constructor, _isSorted must be set explicitly.
        /// </summary>
        public StatModifierCollection([NotNull] IEnumerable<IStatModifier> modifiers)
        {
            AddRange(modifiers);
        }
    }
}