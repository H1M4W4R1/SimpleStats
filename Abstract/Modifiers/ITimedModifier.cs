using JetBrains.Annotations;

namespace Systems.SimpleStats.Abstract.Modifiers
{
    /// <summary>
    ///     Modifier with a limited duration. Time is updated by the owning entity
    ///     and expired modifiers are automatically removed during recalculation.
    /// </summary>
    public interface ITimedModifier : IStatModifier
    {
        /// <summary>
        ///     Remaining time in seconds before this modifier expires
        /// </summary>
        float TimeRemaining { get; set; }

        /// <summary>
        ///     Total duration this modifier was created with
        /// </summary>
        [UsedImplicitly] float TotalDuration { get; }

        /// <summary>
        ///     True when <see cref="TimeRemaining"/> has reached zero or below
        /// </summary>
        bool IsExpired => TimeRemaining <= 0f;

        /// <summary>
        ///     Advances the modifier's internal timer. Called by the owning entity each tick.
        /// </summary>
        /// <param name="deltaTime">Elapsed time in seconds</param>
        void UpdateTime(float deltaTime) => TimeRemaining -= deltaTime;
    }
}
