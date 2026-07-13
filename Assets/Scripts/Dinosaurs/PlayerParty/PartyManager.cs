using System;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIScreenManager _screenManager;

    public static PartyManager Instance { get; private set; }
    public event Action UpdateDisplay;

    private DinosaurData _selectedDinosaur;
    private int _selectedSlot;
    private int _maxDinosaurs = 5;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SelectSlot(int slotNum)
    {
        if (slotNum < 0 || slotNum >= PlayerInventory.Instance.Creatures.Count)
        {
            Debug.Log($"Selected invalid slot num: {slotNum}");
        }
        _selectedSlot = slotNum;
        _selectedDinosaur = PlayerInventory.Instance.Creatures[slotNum];

        UpdateDisplay?.Invoke();
    }

    public void UnselectSlot()
    {
        _selectedSlot = 0;
        _selectedDinosaur = null;

        UpdateDisplay?.Invoke();
    }

    public void InitializeCombiner()
    {
        if (_selectedDinosaur == null && PlayerInventory.Instance.Creatures.Count >= _maxDinosaurs)
        {
            Debug.Log("Max dinosaurs reached in party reached, please select an existing member");
            return;
        }

        if (_selectedDinosaur == null)
        {
            _selectedSlot = PlayerInventory.Instance.Creatures.Count;
        }

        CreatureCombiner.Instance.Initialize(_selectedSlot, _selectedDinosaur);
        UnselectSlot();
        _screenManager.SwitchScreen(1); // Combiner screen
    }
}
