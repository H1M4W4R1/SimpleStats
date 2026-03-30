# SimpleStats - Feature Suggestions and Improvements

## FEAT-003: Modifier stacking rules
- **Category:** New Feature
- **Description:** Add configurable stacking behavior for modifiers of the same type. Options could include: stack additively, stack multiplicatively, take highest only, take lowest only, or do not stack (only one allowed).
- **Rationale:** Many games have rules like "the same buff from multiple sources doesn't stack" or "only the strongest version applies." Currently, all modifiers stack implicitly by being added to the collection.

## FEAT-007: Pooling for `StatModifierCollection`
- **Category:** Performance
- **Description:** Implement object pooling for `StatModifierCollection` instances to avoid GC allocations during stat calculations. Provide `StatModifierCollection.Rent()` and `Return()` methods, or integrate with Unity's `ObjectPool<T>`.
- **Rationale:** If stats are recalculated frequently (per frame for many entities), creating and discarding collections generates garbage. Pooling eliminates this overhead.

## FEAT-010: Snapshot/diff support for stat values
- **Category:** New Feature
- **Description:** Add the ability to snapshot a stat's computed value and later compare it to the current value to get a diff. Useful for "damage recap" screens, combat logs, or UI that shows "+5 from buff."
- **Proposed API:** `StatSnapshot Snapshot(StatisticBase stat, StatModifierCollection mods)` with `float GetDelta(StatSnapshot previous)`.

## FEAT-011: Read-only view of `StatModifierCollection`
- **Category:** Architecture
- **Description:** Expose an `IReadOnlyStatModifierCollection` interface that only allows `Apply` and `Count`, without `Add`/`Remove`/`AddRange`. This would let systems pass modifier collections to stat calculation without risking mutation.
- **Rationale:** Follows the principle of least privilege. Prevents accidental modification of shared modifier collections.

## FEAT-014: Modifier priority tiebreaking
- **Category:** Enhancement
- **Description:** When two modifiers share the same `Order` value, the current sort is unstable (depends on `List.Sort` implementation). Add a secondary sort key (e.g., insertion order or an explicit tiebreaker) to guarantee deterministic behavior.
- **Rationale:** Non-deterministic modifier ordering can cause hard-to-reproduce gameplay bugs.

## FEAT-015: Debug/inspector tooling
- **Category:** Usability
- **Description:** Create a custom Unity Editor window or inspector that visualizes stat calculations in real time: base value, each modifier's contribution, and the final value. This would greatly accelerate debugging and tuning.
- **Rationale:** Stat systems are notoriously difficult to debug. Visual tooling turns a frustrating process into a straightforward one.

## FEAT-017: Async-aware database loading
- **Category:** Architecture
- **Description:** `StatsDatabase` inherits synchronous-load behavior that calls `WaitForCompletion()` on addressable handles. This blocks the main thread and can cause hitches. Provide an async initialization path (`await StatsDatabase.LoadAsync()`) that can be called during loading screens.
- **Rationale:** Synchronous addressable loading is discouraged by Unity and causes frame spikes. An async path would integrate better with modern Unity loading patterns.
