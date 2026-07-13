using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the stats and body parts of the dinosaur
/// </summary>
public class DinosaurData
{
    private BaseStatsSO _baseStats = new();
    private CreatureStats _stats = new();
    private Dictionary<BodyPartType, DinosaurPart> _bodyParts = new();
    private List<WildCard> _wildcardAbilities= new();

    // TODO: create a list of abilities obtained from body parts

    /// <summary>
    /// Applies the body part and its stats
    /// </summary>
    /// <param name="bodyPart"></param>
    public void ApplyBodyPart(DinosaurPart dinoPart)
    {
        // remove existing body part
        if (_bodyParts.TryGetValue(dinoPart.Type, out DinosaurPart part))
        {
            _stats.Subtract(part.Stats);
            _wildcardAbilities.Remove(dinoPart.Wildcard);
        }

        _bodyParts[dinoPart.Type] = dinoPart;
        _stats.Add(dinoPart.Stats);
        _wildcardAbilities.Add(dinoPart.Wildcard);
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
}
