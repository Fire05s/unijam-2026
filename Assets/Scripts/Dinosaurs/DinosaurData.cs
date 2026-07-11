using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the stats and body parts of the dinosaur
/// </summary>
public class DinosaurData
{
    private CreatureStats _stats;
    private Dictionary<BodyPartType, BodyPartSO> _bodyParts = new();

    // TODO: create a list of abilities obtained from body parts

    /// <summary>
    /// Applies the body part and its stats
    /// </summary>
    /// <param name="bodyPart"></param>
    public void ApplyBodyPart(BodyPartSO bodyPart)
    {
        // remove existing body part
        if (_bodyParts.TryGetValue(bodyPart.PartType, out BodyPartSO part))
        {
            _stats -= part.BonusStats;
        }

        _bodyParts[bodyPart.PartType] = bodyPart;
        _stats += bodyPart.BonusStats;
    }

    /// <summary>
    /// Retrieves the corresponding stat
    /// </summary>
    /// <param name="type"> Stat to retrieve </param>
    /// <returns> Stat value </returns>
    public float GetStat(StatType type)
    {
        switch (type)
        {
            case StatType.Attack:
                return _stats.Attack;
            case StatType.Health:
                return _stats.Health;
            case StatType.Speed:
                return _stats.Speed;
            case StatType.CritChance:
                return _stats.CritChance;
            default:
                Debug.Log($"Stat type {type.ToString()} not properly handled, please update the GetStat function");
                return 0; // Return 0 by default for non-handled stats
        }
    }
}
