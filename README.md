<div align="center">
  <h1>Simple Stats</h1>
</div>

# About

Simple Stats is a lightweight, composable statistics system:

- ScriptableObject statistics with base values
- Modifier pipeline with deterministic execution order
- Ready‑made modifiers (flat add, multiply, final add)
- Database accessors and a simple interface for providers

*For requirements check .asmdef*

# Usage

## Defining a statistic

Create a ScriptableObject that extends `StatisticBase`. It has a `BaseValue` and computes a final value by applying a set of modifiers.

```csharp
using Systems.SimpleCore.Automation.Attributes;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

public sealed class MoveSpeed : StatisticBase { }
```

Statistics are auto-created into the database via the existing `AutoCreate` attribute on `StatisticBase`.

## Implementing modifiers

Implement `IStatModifier<TStatisticType>` to target a specific statistic type, or `IStatModifier` for generic logic. Use the provided implementations for common cases.

```csharp
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data.Statistics;
using Systems.SimpleStats.Implementations;

// Using built-ins
IStatModifier moveSpeedFlat = new FlatAddModifier<MoveSpeed>(+1.5f);
IStatModifier moveSpeedMult = new MultiplyModifier<MoveSpeed>(1.2f);

// Custom modifier example
public sealed class ClampFinalMoveSpeed : IStatModifier<MoveSpeed>
{
    public int Order => int.MaxValue;            // ensure it runs last (after FinalAdd)
    public void Apply(ref float current) => current = Mathf.Clamp(current, 0f, 20f);
}
```

Execution order is controlled by `Order` (lower runs earlier). Use `ModifierOrder` enum values for built-ins to ensure consistent sequencing.

## Providing modifiers from objects

Objects that can supply stat modifiers implement `IWithStatModifiers` and return their current list of `IStatModifier`s. This pattern is used by packages like SimpleEntities.

```csharp
using System.Collections.Generic;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data.Collections;
using Systems.SimpleStats.Data.Statistics;

public sealed class BootsItem : MonoBehaviour, IWithStatModifiers
{
    private readonly List<IStatModifier> _modifiers = new();

    void Awake()
    {
        _modifiers.Add(new FlatAddModifier<MoveSpeed>(+0.5f));
        _modifiers.Add(new MultiplyModifier<MoveSpeed>(1.1f));
    }

    public IReadOnlyList<IStatModifier> GetAllModifiers() => _modifiers;
}

// Computing a final value using a provider's modifiers
float GetMoveSpeed(IWithStatModifiers provider)
{
    var mods = new StatModifierCollection();
    mods.AddRange(provider.GetAllModifiers());

    var stat = StatsDatabase.GetAbstract<MoveSpeed>();
    return stat.GetFinalValue(mods);
}
```

## Accessing statistics

Use `StatsDatabase` to retrieve a statistic asset instance when computing values or building UI.

```csharp
var healthStat = StatsDatabase.GetAbstract<MaxHealth>();
var mods = new StatModifierCollection(equipment.GetAllModifiers());
float maxHealth = healthStat.GetFinalValue(mods);
```

## Built-in modifiers and order

The package ships with common modifiers:

- Flat add: `FlatAddModifier<TStatistic>` (order: `ModifierOrder.FlatAdd`)
- Multiply: `MultiplyModifier<TStatistic>` (order: `ModifierOrder.Multiply`)
- Final add: `FinalAddModifier<TStatistic>` (order: `ModifierOrder.FinalAdd`)

You can mix multiple modifiers of each type; ordering ensures consistent results.

# Notes

- Keep modifiers small and composable; avoid embedding complex cross-stat logic in a single modifier.
- Providers should cache and refresh their modifier lists when equipment or states change to minimize allocations.
- For clamping/validation, place such modifiers at the end by using a large `Order` value.

