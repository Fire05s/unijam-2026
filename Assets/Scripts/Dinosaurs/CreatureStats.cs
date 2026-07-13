using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class CreatureStats
{
    private Dictionary<StatType, float> _statValues = new();
    public float Get(StatType stat)
    {
        return _statValues.GetValueOrDefault(stat);
    }

    public Dictionary<StatType, float> GetStats()
    {
        return _statValues;
    }

    public IEnumerable<StatType> GetKeys()
    {
        return _statValues.Keys.ToList();
    }

    public void Add(StatType stat, int amount)
    {
        _statValues[stat] = Get(stat) + amount;
    }

    public void Add(List<Stat> stats)
    {
        foreach (Stat stat in stats)
        {
            _statValues[stat.Type] = Get(stat.Type) + stat.Value;
        }
    }

    public void Subtract(List<Stat> stats)
    {
        foreach (Stat stat in stats)
        {
            if (Get(stat.Type) == 0) continue;
            _statValues[stat.Type] -= stat.Value;
        }
    }
}

public enum StatType
{
    Health, Attack, Speed, CritChance
}

[Serializable]
public struct Stat
{
    public StatType Type;
    public float Value;
}

