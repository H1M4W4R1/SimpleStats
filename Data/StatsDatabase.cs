using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleCore.Storage;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Systems.SimpleStats.Data
{
    /// <summary>
    ///     Database of all statistics in game
    /// </summary>
    public sealed class StatsDatabase : AddressableDatabase<StatsDatabase, StatisticBase>
    {
        [NotNull] protected override string AddressableLabel => "SimpleStats.Statistics";
    }
}