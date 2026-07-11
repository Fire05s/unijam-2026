using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<DinosaurData> _creatures;
    private List<BodyPartSO> _bodyParts;

    public IReadOnlyList<DinosaurData> Creatures => _creatures;
    public IReadOnlyList<BodyPartSO> BodyParts => _bodyParts;

    public void AddCreature(DinosaurData creature)
    {
        _creatures.Add(creature);
    }

    public void RemoveCreature(DinosaurData creature)
    {
        _creatures.Remove(creature);
    }

    public void AddBodyPart(BodyPartSO part)
    {
        // Sort by highest attack
        int index = _bodyParts.FindIndex(
            existing => existing.BonusStats.Attack < part.BonusStats.Attack
        );

        if (index == -1)
            _bodyParts.Add(part); // Lowest stat, put at end
        else
            _bodyParts.Insert(index, part);
    }

    public void RemoveBodyPart(BodyPartSO part)
    {
        _bodyParts.Remove(part);
    }
}
