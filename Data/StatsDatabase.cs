using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Systems.SimpleStats.Data
{
    /// <summary>
    ///     Database of all statistics in game
    /// </summary>
    public static class StatsDatabase
    {
        public const string ADDRESSABLE_LABEL = "SimpleStats.Statistics";
        private static readonly List<StatisticBase> _items = new();

        /// <summary>
        ///     If true this means that all objects have been loaded
        /// </summary>
        private static bool _isLoaded;

        /// <summary>
        ///     If true this means that objects are currently being loaded
        /// </summary>
        private static bool _isLoading;

        /// <summary>
        ///     Total number of objects in database
        /// </summary>
        public static int Count
        {
            get
            {
                EnsureLoaded();
                return _items.Count;
            }
        }

        /// <summary>
        ///     Ensures that all objects are loaded
        /// </summary>
        internal static void EnsureLoaded()
        {
            if (!_isLoaded) Load();
        }

        /// <summary>
        ///     Loads all objects from Resources folder
        /// </summary>
        private static void Load()
        {
            // Prevent multiple loads
            if (_isLoading) return;
            _isLoading = true;

            // Load items
            AsyncOperationHandle<IList<StatisticBase>> request = Addressables.LoadAssetsAsync<StatisticBase>(
                new[] {ADDRESSABLE_LABEL}, OnItemLoaded,
                Addressables.MergeMode.Union);
            request.WaitForCompletion();

            OnItemsLoadComplete(request);
        }

        private static void OnItemsLoadComplete(AsyncOperationHandle<IList<StatisticBase>> _)
        {
            _isLoaded = true;
            _isLoading = false;
        }

        private static void OnItemLoaded<TObject>(TObject obj)
        {
            if (obj is not StatisticBase item) return;
            _items.Add(item);
        }


        /// <summary>
        ///     Gets first object of specified type
        /// </summary>
        /// <typeparam name="TStatisticType">Object type to get </typeparam>
        /// <returns>First object of specified type or null if no object of specified type is found</returns>
        [CanBeNull] public static TStatisticType GetStatistic<TStatisticType>()
            where TStatisticType : StatisticBase
        {
            EnsureLoaded();

            // Loop through all items
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] is TStatisticType item) return item;
            }

            Assert.IsNotNull(null, "Item not found in database");
            return null;
        }

        /// <summary>
        ///     Gets all objects of specified type
        /// </summary>
        /// <typeparam name="TStatisticType">Type of object to get</typeparam>
        /// <returns>Read-only list of objects of specified type</returns>
        [NotNull] public static IReadOnlyList<TStatisticType> GetAll<TStatisticType>()
            where TStatisticType : StatisticBase
        {
            EnsureLoaded();

            List<TStatisticType> items = new();

            // Loop through all items
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] is TStatisticType item) items.Add(item);
            }

            return items;
        }
    }
}