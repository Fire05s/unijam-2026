using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the stats and body parts of the dinosaur
/// </summary>
public class DinosaurData
{
    private CreatureStats _stats;
    private Dictionary<BodyPartType, DinosaurPart> _bodyParts = new();

    // TODO: create a list of abilities obtained from body parts

    /// <summary>
    /// Applies the body part and its stats
    /// </summary>
    /// <param name="bodyPart"></param>
    public void ApplyBodyPart(BodyPartSO bodyPart)
    {
        // remove existing body part
        if (_bodyParts.TryGetValue(bodyPart.PartType, out DinosaurPart part))
        {
           _stats.Subtract(part.Stats);
        }

        DinosaurPart newPart = new DinosaurPart(bodyPart);
        _bodyParts[bodyPart.PartType] = newPart;
        // _stats += bodyPart.BonusStats;
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
}
