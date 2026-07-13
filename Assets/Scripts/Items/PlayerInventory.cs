using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private List<BodyPartSO> _initialParts;
    public static PlayerInventory Instance { get; private set; }
    private List<DinosaurData> _creatures = new();
    private List<DinosaurPart> _bodyParts = new();

    public IReadOnlyList<DinosaurData> Creatures => _creatures;
    public IReadOnlyList<DinosaurPart> BodyParts => _bodyParts;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // DEBUG
        foreach (var partData in _initialParts)
        {
            AddBodyPart(new DinosaurPart(partData));
        }
    }

    public void AddCreature(DinosaurData creature)
    {
        _creatures.Add(creature);
    }

    public void RemoveCreature(DinosaurData creature)
    {
        _creatures.Remove(creature);
    }

    public void AddBodyPart(DinosaurPart part)
    {
        _bodyParts.Add(part);
    }

    public void RemoveBodyPart(DinosaurPart part)
    {
        _bodyParts.Remove(part);
    }

    public List<DinosaurPart> GetBodyParts()
    {
        return _bodyParts;
    }
}
