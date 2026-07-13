using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the stats and body parts of the dinosaur
/// </summary>
public class DinosaurData
{
    [SerializeField] private BaseStatsSO _baseStats;
    private CreatureStats _stats = new();
    [SerializeField] private Dictionary<BodyPartType, DinosaurPart> _bodyParts = new();
    private List<WildCard> _wildcardAbilities = new();

    public DinosaurData(BaseStatsSO baseStats) {
        _baseStats = baseStats;
        ApplyBaseStats();
    }

    public DinosaurData(BaseStatsSO baseStats, List<BodyPartSO> bodyParts) {
        _baseStats = baseStats;
        ApplyBaseStats();
        foreach (BodyPartSO part in bodyParts)
        {
            ApplyBodyPart(new DinosaurPart(part));
        }
    }

    /// <summary>
    /// Generatees and applies the base stats for this dino
    /// </summary>
    /// <param name="baseStats"> Receives the base stats scriptable object for this dino </param>
    public void ApplyBaseStats() {
        if (_baseStats == null)
        {
            Debug.LogWarning("Can't generate base stats bc baseStats is null");
            return;
        }

        foreach (var stat in _baseStats.Stats)
        {
            float rolledChance = Random.Range(0f,1f);
            if (rolledChance <= stat.AppearanceChance)
            {
                int value = (int)Random.Range(stat.MinValue, stat.MaxValue); // floors the value
                _stats.Add(stat.Type, value);
            }
        }
    }

    /// <summary>
    /// Applies the body part and its stats
    /// </summary>
    /// <param name="bodyPart"></param>
    public void ApplyBodyPart(DinosaurPart dinoPart)
    {
        // remove existing body part
        if (_bodyParts.TryGetValue(dinoPart.Type, out DinosaurPart existing))
        {
            _stats.Subtract(existing.Stats);
            if (existing.Wildcard != null)
                _wildcardAbilities.Remove(existing.Wildcard.WildType);
        }

        _bodyParts[dinoPart.Type] = dinoPart;
        _stats.Add(dinoPart.Stats);
        if (dinoPart.Wildcard != null)
            _wildcardAbilities.Add(dinoPart.Wildcard.WildType);
    }

    /// <summary>
    /// Clears all body parts and their stats
    /// </summary>
    public void ClearBodyParts()
    {
        foreach (var part in _bodyParts.Values)
        {
            _stats.Subtract(part.Stats);
        }
        _bodyParts.Clear();
        _wildcardAbilities.Clear();
    }

    /// <summary>
    /// Retrieves the corresponding stat
    /// </summary>
    /// <param name="type"> Stat to retrieve </param>
    /// <returns> Stat value </returns>
    public float GetStat(StatType type)
    {
        return _stats.Get(type);
    }

    /// <summary>
    /// Retrieves the adjusted corresponding stat (clamped based on the stat)
    /// </summary>
    /// <param name="type"> Stat to retrieve </param>
    /// <returns> Adjusted stat value </returns>
    public float GetAdjustedStat(StatType type)
    {
        float statValue = GetStat(type);
        switch (type)
        {
            case StatType.Attack: return Mathf.Clamp(statValue, 1, statValue);
            case StatType.CritChance: return Mathf.Clamp(statValue, 0, 100);
            case StatType.Health: return Mathf.Clamp(statValue, 1, statValue);
            case StatType.Speed: return Mathf.Clamp(statValue, 1, 10);
            default: Debug.LogError("invalid type given to retrieve"); return 0;
        }
    }

    /// <summary>
    /// Gets all Wildcard abilities this dino has
    /// </summary>
    /// <returns> Returns a list of wildcard ability enum types </returns>
    public List<WildCard> GetWildCardAbilities()
    {
        return _wildcardAbilities;
    }
}
