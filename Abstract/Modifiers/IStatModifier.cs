using JetBrains.Annotations;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Statistics;

namespace Systems.SimpleStats.Abstract.Modifiers
{
    /// <summary>
    ///     Stat modifier of provided statistic type
    /// </summary>
    /// <typeparam name="TStatisticType">Type of statistic</typeparam>
    public interface IStatModifier<out TStatisticType> : IStatModifier
        where TStatisticType : StatisticBase
    {
        /// <summary>
        ///     Gets statistic for modifier
        /// </summary>
        /// <returns>Statistic or null if not found</returns>
        StatisticBase IStatModifier.GetStatistic() => StatsDatabase.GetAny<TStatisticType>();

        bool IStatModifier.IsValidFor(StatisticBase statistic) => statistic is TStatisticType;

        /// <summary>
        ///     Returns true when <typeparamref name="TSelectStatisticType"/> is
        ///     <typeparamref name="TStatisticType"/> or a subclass of it.
        ///     This means a modifier targeting <c>HealthStat</c> will match queries for
        ///     <c>HealthStat</c> and any type deriving from it, but NOT for its base types.
        /// </summary>
        bool IStatModifier.IsValidFor<TSelectStatisticType>()
            => typeof(TStatisticType).IsAssignableFrom(typeof(TSelectStatisticType));
    }
    
    /// <summary>
    ///     Statistic modifier
    /// </summary>
    public interface IStatModifier
    {
        /// <summary>
        ///     Order of execution, lower first
        /// </summary>
        public int Order { get; }
        
        /// <summary>
        ///     Apply modifier to statistic value
        /// </summary>
        /// <param name="currentFloat">Current statistic value</param>
        public void Apply(ref float currentFloat);
        
        /// <summary>
        ///     Checks if modifier is for provided statistic type
        /// </summary>
        /// <typeparam name="TStatisticType">Type of statistic</typeparam>
        /// <returns>True if modifier is for provided statistic type</returns>
        public bool IsValidFor<TStatisticType>() where TStatisticType : StatisticBase;

        /// <summary>
        ///     Checks if modifier is for provided statistic
        /// </summary>
        /// <param name="statistic">Statistic to check</param>
        /// <returns>True if modifier is for provided statistic</returns>
        public bool IsValidFor(StatisticBase statistic);
        
        /// <summary>
        ///     Gets statistic for modifier
        /// </summary>
        /// <returns>Statistic or null if not found</returns>
        [CanBeNull] public StatisticBase GetStatistic();

        /// <summary>
        ///     Gets statistic for modifier
        /// </summary>
        /// <typeparam name="TStatisticType">Cast statistic to this type</typeparam>
        /// <returns>Found statistic or null if not found</returns>
        /// <summary>
        ///     Gets statistic for modifier, cast to the specified type.
        ///     Returns null if the statistic is not found or cannot be cast.
        /// </summary>
        [CanBeNull] public TStatisticType GetStatisticAs<TStatisticType>()
            where TStatisticType : StatisticBase
        {
            StatisticBase statistic = GetStatistic();
            if (ReferenceEquals(statistic, null)) return null;
            return statistic as TStatisticType;
        }
    }
}