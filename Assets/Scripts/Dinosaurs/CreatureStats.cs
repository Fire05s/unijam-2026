using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public int Add(List<Stat> stats, float currentHealth)
    {
        int addedCurrentHealth = 0;
        foreach (Stat stat in stats)
        {
            if (stat.Type == StatType.Health)
            {
                float currentHealthPercent = currentHealth / _statValues[StatType.Health];
                addedCurrentHealth = Mathf.RoundToInt(stat.Value * currentHealthPercent);
                Debug.Log($"current health {currentHealth}, added health percent {currentHealthPercent}, added current health {addedCurrentHealth}");
            }
            _statValues[stat.Type] = Get(stat.Type) + stat.Value;
        }
        return addedCurrentHealth;
    }

    public int Subtract(List<Stat> stats, float currentHealth)
    {
        int addedCurrentHealth = 0;
        foreach (Stat stat in stats)
        {
            if (stat.Type == StatType.Health)
            {
                float currentHealthPercent = currentHealth / _statValues[StatType.Health];
                addedCurrentHealth = -1 * Mathf.RoundToInt(stat.Value * currentHealthPercent);
            }
            _statValues[stat.Type] -= stat.Value;
        }
        return addedCurrentHealth;
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

