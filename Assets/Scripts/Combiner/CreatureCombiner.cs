using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCombiner : MonoBehaviour
{
    [Header("Base Player Dino Stats")]
    [SerializeField] private BaseStatsSO _baseStats;
    [Header("Slots")]
    [SerializeField] private Dictionary<BodyPartType, DinosaurPart> _partSlots = new();
    [SerializeField] private int _requiredPartCount = 3;
    [Header("Reference")]
    [SerializeField] private UIScreenManager _screenManager;
    [SerializeField] private ModelGenerator _modelGenerator;
    [SerializeField] private ModelDatabaseSO _modelDatabase;
    [Header("Audio")]
    [SerializeField] private int _audioListIndex = 0;

    [Header("Debug")]
    [SerializeField] private List<BodyPartSO> _testParts;

    public static CreatureCombiner Instance { get; private set; }
    public event Action DisplayUpdate;
    public event Action PartSelect;

    private DinosaurData _displayDinosaur;
    private DinosaurPart _selectedPart;
    private int _selectedPartySlot;
    private List<DinosaurPart> _history = new();

    public DinosaurData DisplayDinosaur => _displayDinosaur;
    public DinosaurPart SelectedPart => _selectedPart;
    public Dictionary<BodyPartType, DinosaurPart> PartSlots => _partSlots;
    public ModelDatabaseSO Database => _modelDatabase;

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

    public void Initialize(int selectedSlot, DinosaurData selectedData)
    {
        Debug.Log($"Editing slot: {selectedSlot}");
        _selectedPartySlot = selectedSlot;
        _displayDinosaur = selectedData;
        if (selectedData != null)
        {
            foreach (var part in selectedData.GetBodyParts().Values)
            {
                PlayerInventory.Instance.AddBodyPart(part);
                _history.Add(part);
            }
            _partSlots = new Dictionary<BodyPartType, DinosaurPart>(selectedData.GetBodyParts());
        }
        GenerateDinosaur();
    }
    public void EquipPart(DinosaurPart part)
    {
        if (part == null || _partSlots.ContainsValue(part)) return; // invalid part or already equipped
        AudioManager.Instance?.PlayInstantSFX(_audioListIndex);
        _partSlots[part.Type] = part;

        GenerateDinosaur();
    }

    public void UnequipPart(BodyPartType type)
    {
        AudioManager.Instance?.PlayInstantSFX(_audioListIndex);
        _partSlots.Remove(type);
        GenerateDinosaur();
    }

    public void SelectPart(DinosaurPart part)
    {
        Debug.Log("select part");
        if (_selectedPart == part)
        {
            AudioManager.Instance?.PlayInstantSFX(_audioListIndex);
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
            PartSelect?.Invoke();
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

    public void FinalizeDinosaur()
    {
        AudioManager.Instance?.PlayInstantSFX(_audioListIndex);
        if (!IsValidDinosaur())
        {
            Debug.Log($"Not enough parts selected, must select {_requiredPartCount}");
            return;
        }

        ApplyModel();

        // Add creature to inventory
        if (!PlayerInventory.Instance.SetCreature(_displayDinosaur, _selectedPartySlot))
        {
            // Fallback in case set creature fails
            PlayerInventory.Instance.AddCreature(_displayDinosaur);
        }
        foreach (var part in _partSlots.Values)
        {
            PlayerInventory.Instance.RemoveBodyPart(part);
        }
        _partSlots.Clear();
        _history.Clear();
        UnselectPart();
        _screenManager.SwitchScreen(0); // Party management screen
    }

    private void ApplyModel()
    {
        int headId = _partSlots[BodyPartType.Head].Reference.ModelID;
        int armsId = _partSlots[BodyPartType.Arms].Reference.ModelID;
        int legsId = _partSlots[BodyPartType.Legs].Reference.ModelID;
        CreatureModel model = _modelDatabase.GetModel(headId, armsId, legsId).GetComponent<CreatureModel>();
        if (model != null) _displayDinosaur.SetModel(model);
        else Debug.LogWarning($"Can't find creature model with ID {headId}-{armsId}-{legsId}");
    }

    public void ReturnToParty()
    {
        AudioManager.Instance?.PlayInstantSFX(_audioListIndex);
        foreach (var part in _history)
        {
            PlayerInventory.Instance.RemoveBodyPart(part);
            EquipPart(part);
        }
        _partSlots.Clear();
        _history.Clear();
        UnselectPart();
        _screenManager.SwitchScreen(0);
    }

    public bool IsValidDinosaur()
    {
        return _partSlots.Count >= _requiredPartCount;
    }

    private void GenerateDinosaur()
    {
        if (_displayDinosaur == null) _displayDinosaur = new DinosaurData(_baseStats);

        _displayDinosaur.ClearBodyParts();

        foreach (var part in _partSlots.Values)
        {
            if (part == null) { continue; }
            _displayDinosaur.ApplyBodyPart(part);
        }

        _modelGenerator.SetDinosaur(_displayDinosaur);
        DisplayUpdate?.Invoke();
    }
}