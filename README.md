# SimpleStats

A flexible, performance-optimized stat and modifier system for Unity. SimpleStats provides a composable framework for managing character attributes (health, damage, armor, etc.) with support for flat bonuses, percentages, multiplicative scaling, timed effects, and conditional modifiers.

## Features

- **Modular modifier system**: Chain multiple modifier types with well-defined execution order
- **Timed modifiers**: Automatic expiry and cleanup of duration-based buffs/debuffs
- **Conditional modifiers**: Dynamic enable/disable based on game state
- **Event hooks**: Respond to modifier additions, removals, and expirations
- **Zero-allocation design**: Uses ref structs for validation contexts to minimize GC pressure
- **Addressable integration**: Auto-load statistics from addressable assets with `StatsDatabase`

## Requirements

### Dependencies
- **Unity.Addressables** – For dynamic asset loading via `StatsDatabase`
- **Unity.Burst** – Enabled for performance optimization
- **Unity.Collections** – Required by addressables infrastructure
- **Unity.Mathematics** – Referenced by the assembly
- **Unity.ResourceManager** – Underlying addressables system
- **SimpleCore** – Base framework for database and operation result patterns

### Minimum Unity Version
- Unity 2022 LTS or later (based on assembly dependencies)

## Quick Start

### 1. Define a Statistic

Create a concrete statistic by inheriting from `StatisticBase`. The `[AutoCreate]` attribute is already declared on the base class, so all subclasses are automatically registered with `StatsDatabase`:

```csharp
using Systems.SimpleStats.Data.Statistics;

public class HealthStat : StatisticBase
{
    public override float GetFinalClampedValue(float value)
    {
        // Clamp health to valid range
        return Mathf.Clamp(value, 0, 9999);
    }
}
```

### 2. Create a Modifier Target

Implement `IWithStatModifiers` to receive modifier events:

```csharp
using Systems.SimpleStats.Abstract;
using Systems.SimpleStats.Data.Collections;
using Systems.SimpleStats.Data;
using Systems.SimpleCore.Operations;

public class CharacterStats : MonoBehaviour, IWithStatModifiers
{
    private StatModifierCollection _modifiers;

    private void Start()
    {
        _modifiers = new StatModifierCollection(this);
    }

    public IReadOnlyList<IStatModifier> GetAllModifiers()
        => _modifiers.Modifiers;

    // Optional: override for custom validation
    public OperationResult CanApplyModifier(in ModifierContext context)
    {
        // Return error to block certain modifiers
        return ModifierOperations.Permitted();
    }

    // Optional: events for UI/logging
    public void OnModifierAdded(in ModifierContext context, in OperationResult result)
        => Debug.Log($"Buff applied: {context.modifier}");

    public void OnModifierExpired(in ModifierContext context, in OperationResult result)
        => Debug.Log($"Buff expired: {context.modifier}");
}
```

### 3. Apply Modifiers

Modifiers can be instantiated and added to a collection:

```csharp
// Create a flat health bonus (+10)
var flatBonus = new FlatAddModifier<HealthStat>(10);
_modifiers.TryAddModifier(flatBonus);

// Create a timed flat buff (+20 for 5 seconds)
var timedBuff = new TimedFlatAddModifier<HealthStat>(20, 5);
_modifiers.TryAddModifier(timedBuff);

// Calculate final stat value
var healthStat = StatsDatabase.GetAny<HealthStat>();
float finalHealth = healthStat.GetFinalValue(_modifiers);
```

### 4. Update Timed Modifiers

Timed modifiers require manual time updates:

```csharp
private void Update()
{
    // Update each timed modifier
    foreach (var modifier in _modifiers.Modifiers)
    {
        if (modifier is ITimedModifier timed)
            timed.UpdateTime(Time.deltaTime);
    }

    // Remove expired modifiers and fire events
    _modifiers.RecomputeAllModifiers();
}

// Or preferably use Tick System from SimpleCore
```

## Modifier Types

### Execution Order

Modifiers execute in a defined order to ensure consistent results:

1. **FlatAdd** (`ModifierOrder.FlatAdd`) – Added to base value
2. **PercentageAdd** (`ModifierOrder.PercentageAdd`) – Adds a percentage of the value after flat adds (0.1 = +10%)
3. **Multiply** (`ModifierOrder.Multiply`) – Multiplicative scaling (1.5 = ×1.5)
4. **PercentageFinalAdd** (`ModifierOrder.PercentageFinalAdd`) – Adds a percentage after multiplication (0.1 = +10%)
5. **FinalAdd** (`ModifierOrder.FinalAdd`) – Added to the final calculated value

### Built-in Implementations

**Standard Modifiers:**
- `FlatAddModifier<T>` – Adds a fixed amount
- `PercentageAddModifier<T>` – Adds a percentage of the current value (0.1 = +10%)
- `MultiplyModifier<T>` – Multiplies value (1.5 = ×1.5)
- `PercentageFinalAddModifier<T>` – Adds percentage after multiplication
- `FinalAddModifier<T>` – Adds to final value

**Timed Variants:** `Timed[Type]Modifier<T>` – Any modifier with duration tracking

**Conditional Variants:** `Conditional[Type]Modifier<T>` – Abstract base classes; override `ShouldApply()` to define the condition

**Timed+Conditional Variants:** `TimedConditional[Type]Modifier<T>` – Combines timing and conditional logic; also abstract

Example: `TimedConditionalFlatAddModifier<HealthStat>` requires a concrete subclass overriding `ShouldApply()`.

## Validation and Events

### Validation

The `IWithStatModifiers` interface defines validation hooks:

```csharp
public OperationResult CanApplyModifier(in ModifierContext context)
{
    // Block modifiers based on game logic
    if (context.actionSource == ActionSource.External && IsFrozen)
        return ModifierOperations.MaxModifiersExceeded();
    
    return ModifierOperations.Permitted();
}
```

### Events

Subscribe to modifier lifecycle events:

- `OnModifierAdded(context, result)` – Modifier successfully added
- `OnModifierAddFailed(context, result)` – Addition rejected
- `OnModifierRemoved(context, result)` – Modifier removed
- `OnModifierRemoveFailed(context, result)` – Removal failed
- `OnModifierExpired(context, result)` – Timed modifier expired
- `OnRecomputeComplete(result)` – Expiry pass finished

## Advanced Usage

### Custom Modifiers

Implement `IStatModifier<T>` for fully custom logic:

```csharp
public class DamageResistanceModifier : IStatModifier<DamageStat>
{
    private float _resistance;

    public DamageResistanceModifier(float resistance) => _resistance = resistance;

    public int Order => (int)ModifierOrder.FinalAdd;

    public void Apply(ref float currentFloat)
    {
        currentFloat *= (1f - Mathf.Clamp01(_resistance));
    }
}
```

### Conditional Modifiers

Conditional modifier variants are abstract base classes. Create a concrete subclass and override `ShouldApply()`:

```csharp
public class BerserkDamageModifier : ConditionalMultiplyModifier<DamageStat>
{
    private readonly Character _character;

    public BerserkDamageModifier(Character character) : base(1.5f)
    {
        _character = character;
    }

    // Only applies when health is below 25%
    public override bool ShouldApply(in ModifierContext context)
        => _character.CurrentHealth < _character.MaxHealth * 0.25f;
}
```

### Modifier Source Tracking

Implement `IModifierSource<T>` on a modifier to track its origin (useful for debugging or removing all modifiers from a specific source):

```csharp
public class WeaponFlatAddModifier : FlatAddModifier<DamageStat>, IModifierSource<WeaponItem>
{
    private readonly WeaponItem _weapon;

    public WeaponFlatAddModifier(WeaponItem weapon, float bonus) : base(bonus)
    {
        _weapon = weapon;
    }

    public WeaponItem GetSource() => _weapon;
}
```

> Using structs or identifiers as sources is preferred as it improves system serializability.

### Filtering Modifiers

Get modifiers for a specific statistic:

```csharp
// Get all modifiers targeting a stat type
var healthModifiers = new List<IStatModifier>();
character.GetAllModifiersFor<HealthStat>(healthModifiers);

// Get only currently active modifiers (non-expired, conditions met)
var active = new List<IStatModifier>();
_modifiers.GetActiveModifiers(active);

// Transfer modifiers to another collection
var targetCollection = new StatModifierCollection();
character.TransferModifiersTo<DamageStat>(targetCollection);
```

## Performance Considerations

- **Caching**: Implement `GetAllModifiers()` with caching to avoid repeated allocations
- **Lazy sorting**: Collections sort only when needed (on `Apply()`)
- **Zero-allocation contexts**: `ModifierContext` is a readonly ref struct
- **Pooling**: Consider object pooling `StatModifierCollection` for frequently-created instances
- **Batch updates**: Update timed modifiers once per frame, not per-modifier

## License

See [LICENSE.md](LICENSE.md) in this directory.
