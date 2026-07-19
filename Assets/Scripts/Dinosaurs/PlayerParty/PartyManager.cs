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
    [Header("Audio")]
    [SerializeField] private int _audioListIndex = 0;

    public static PartyManager Instance { get; private set; }
    public event Action UpdateDisplay;

    private DinosaurData _selectedDinosaur;
    private int _selectedSlot = -1; // no slots selected

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
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        _selectedSlot = slotNum;
        _selectedDinosaur = PlayerInventory.Instance.Creatures[slotNum];

        UpdateDisplay?.Invoke();
    }

    public void UnselectSlot()
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        _selectedSlot = -1;
        _selectedDinosaur = null;

        UpdateDisplay?.Invoke();
    }

    public void ExitToMain()
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        _screenTransition.FadeAndLoad(_mainLevelName, _transitionDuration);
    }

    public void InitializeCombiner()
    {
        if (_selectedDinosaur == null && PlayerInventory.Instance.Creatures.Count >= PlayerInventory.Instance.MaxPartySize)
        {
            Debug.Log("Max dinosaurs in party reached, please select an existing member");
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
