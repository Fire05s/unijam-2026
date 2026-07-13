using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the stats and body parts of the dinosaur
/// </summary>
public class DinosaurData
{
    private CreatureStats _stats = new();
    private Dictionary<BodyPartType, DinosaurPart> _bodyParts = new();

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
        }

        _bodyParts[dinoPart.Type] = dinoPart;
        _stats.Add(dinoPart.Stats);
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
