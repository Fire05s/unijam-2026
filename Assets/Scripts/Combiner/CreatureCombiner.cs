using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System;

public class CreatureCombiner : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private Dictionary<BodyPartType, DinosaurPart> _partSlots = new();

    [Header("Debug")]
    [SerializeField] private List<BodyPartSO> _testParts;

    public event Action DisplayUpdate;

    private DinosaurData _displayDinosaur;
    private PlayerInventory _playerInventory;
    private int _selectedPartySlot;

    public DinosaurData DisplayDinosaur => _displayDinosaur;
    void Start()
    {
        // DEBUG: directly generate from provided body parts
        foreach (var partData in _testParts)
        {
            EquipPart(new DinosaurPart(partData));
        }
    }
    public void EquipPart(DinosaurPart part)
    {
        _partSlots[part.Type] = part;

        GenerateDinosaur();
    }

    private void GenerateDinosaur()
    {
        if (_displayDinosaur == null) _displayDinosaur = new DinosaurData();

        foreach (var part in _partSlots.Values)
        {
            _displayDinosaur.ApplyBodyPart(part);
        }

        DisplayUpdate?.Invoke();
    }
}