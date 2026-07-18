using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _initialPartyMax = 1;
    // [Header("Debug")]
    // [SerializeField] private List<BodyPartSO> _initialParts;
    public List<BodyPartSO> FossilParts;
    public static PlayerInventory Instance { get; private set; }
    private List<DinosaurData> _creatures = new();
    private List<DinosaurPart> _bodyParts = new();

    public IReadOnlyList<DinosaurData> Creatures => _creatures;
    public IReadOnlyList<DinosaurPart> BodyParts => _bodyParts;
    public int MaxPartySize { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // DEBUG
        // foreach (var partData in _initialParts)
        // {
        //     AddBodyPart(new DinosaurPart(partData));
        // }
    }

    private void Start()
    {
        MaxPartySize = _initialPartyMax;
    }

    public void AddCreature(DinosaurData creature)
    {
        _creatures.Add(creature);
    }

    public bool SetCreature(DinosaurData creature, int indexNum)
    {
        if (indexNum == 0 && _creatures.Count == 0)
        {
            AddCreature(creature); // Handles empty list
        }
        if (indexNum < 0 || indexNum >= _creatures.Count)
        {
            Debug.Log($"Can't set creature at index {indexNum}, current max: {_creatures.Count}");
            return false;
        }
        _creatures[indexNum] = creature;
        return true;
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

    public void ClearBodyParts()
    {
        _bodyParts.Clear();
    }

    public void ClearCreatures()
    {
        _creatures.Clear();
    }

    public void IncrementPartyCount()
    {
        if (MaxPartySize >= 5) return;
        MaxPartySize++;
    }
}
