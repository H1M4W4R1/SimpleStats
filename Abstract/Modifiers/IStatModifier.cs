using Sirenix.Utilities;
using Systems.SimpleStats.Data.Statistics;

namespace Systems.SimpleStats.Abstract.Modifiers
{
    /// <summary>
    ///     Stat modifier of provided statistic type
    /// </summary>
    /// <typeparam name="TStatisticType">Type of statistic</typeparam>
    public interface IStatModifier<TStatisticType> : IStatModifier
        where TStatisticType : StatisticBase
    {
        bool IStatModifier.IsFor<TSelectStatisticType>()
            => typeof(TSelectStatisticType).ImplementsOrInherits(typeof(TStatisticType));
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
        public bool IsFor<TStatisticType>() where TStatisticType : StatisticBase;
    }
}