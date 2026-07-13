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

    public static CreatureCombiner Instance { get; private set; }
    public event Action DisplayUpdate;

    private DinosaurData _displayDinosaur;
    private DinosaurPart _selectedPart;
    private PlayerInventory _playerInventory;
    private int _selectedPartySlot;

    public DinosaurData DisplayDinosaur => _displayDinosaur;
    public DinosaurPart SelectedPart => _selectedPart;
    public Dictionary<BodyPartType, DinosaurPart> PartSlots => _partSlots;

    private void Awake()
    {
        if (Instance !=  null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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
        if (part == null || _partSlots.ContainsValue(part)) return; // invalid part or already equipped
        _partSlots[part.Type] = part;

        GenerateDinosaur();
    }

    public void UnequipPart(BodyPartType type)
    {
        _partSlots.Remove(type);
        GenerateDinosaur();
    }

    public void SelectPart(DinosaurPart part)
    {
        if (_selectedPart == part)
        {
            if (_partSlots.ContainsValue(part))
            {
                // Part is in slot
                UnequipPart(part.Type);
            }
            else
            {
                // Part is in inventory
                EquipPart(_selectedPart);
            }
            UnselectPart();
        }
        else
        {
            _selectedPart = part;
        }
    }

    public void UnselectPart()
    {
        _selectedPart = null;
    }

    private void GenerateDinosaur()
    {
        if (_displayDinosaur == null) _displayDinosaur = new DinosaurData();

        _displayDinosaur.ClearBodyParts();

        foreach (var part in _partSlots.Values)
        {
            _displayDinosaur.ApplyBodyPart(part);
        }

        DisplayUpdate?.Invoke();
    }
}