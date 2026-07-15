using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIScreenManager _screenManager;
    [Header("Scene Transition")]
    [SerializeField] private string _mainLevelName = "MainLevel";
    [SerializeField] private float _transitionDuration = 0.5f;
    [SerializeField] private ScreenTransition _screenTransition;

    public static PartyManager Instance { get; private set; }
    public event Action UpdateDisplay;

    private DinosaurData _selectedDinosaur;
    private int _selectedSlot = -1; // no slots selected
    private int _maxDinosaurs = 5;

    public DinosaurData SelectedDinosaur => _selectedDinosaur;
    public int SelectedSlot => _selectedSlot;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SelectSlot(int slotNum)
    {
        if (slotNum < 0 || slotNum >= PlayerInventory.Instance.Creatures.Count)
        {
            Debug.Log($"Selected invalid slot num: {slotNum}");
        }
        Debug.Log($"Selected slot: {slotNum}");
        _selectedSlot = slotNum;
        _selectedDinosaur = PlayerInventory.Instance.Creatures[slotNum];

        UpdateDisplay?.Invoke();
    }

    public void UnselectSlot()
    {
        _selectedSlot = -1;
        _selectedDinosaur = null;

        UpdateDisplay?.Invoke();
    }

    public void ExitToMain()
    {
        _screenTransition.FadeAndLoad(_mainLevelName, _transitionDuration);
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
