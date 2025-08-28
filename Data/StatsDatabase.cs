using JetBrains.Annotations;
using Systems.SimpleCore.Storage;
using Systems.SimpleStats.Data.Statistics;

namespace Systems.SimpleStats.Data
{
    /// <summary>
    ///     Database of all statistics in game
    /// </summary>
    public sealed class StatsDatabase : AddressableDatabase<StatsDatabase, StatisticBase>
    {
        public const string LABEL = "SimpleStats.Statistics";
        [NotNull] protected override string AddressableLabel => LABEL;
    }
}