using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleStats.Abstract.Modifiers;

namespace Systems.SimpleStats.Data.Collections
{
    /// <summary>
    ///     Collection of stat modifiers
    /// </summary>
    public sealed class StatModifierCollection
    {
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
                _modifiers.Sort((a, b) => a.Order.CompareTo(b.Order));
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
        public void Add([NotNull] IStatModifier modifier)
        {
            _modifiers.Add(modifier);
            _isSorted = false;
        }

        /// <summary>
        ///     Remove modifier from collection
        /// </summary>
        public void Remove([NotNull] IStatModifier modifier)
        {
            _modifiers.Remove(modifier);
        }

        /// <summary>
        ///     Count of modifiers
        /// </summary>
        public int Count => _modifiers.Count;

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

        // Copy constructor
        public StatModifierCollection([NotNull] IEnumerable<IStatModifier> modifiers)
        {
            AddRange(modifiers);
        }
    }
}