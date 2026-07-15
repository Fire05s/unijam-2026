using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreenUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<PartySlotUI> _partySlots;
    [SerializeField] private List<RenderTexture> _partyCameraTextures;
    [SerializeField] private List<ModelGenerator> _partyModels;
    [SerializeField] private TextMeshProUGUI _combinerScreenText;
    [SerializeField] private Button _cancelSelectButton;

    private void OnEnable()
    {
        SubscribeEvents();
        OnDisplayUpdate();
        DisplayParty();
    }

    private void OnDisable()
    {
        PartyManager.Instance.UpdateDisplay -= OnDisplayUpdate;
    }

    private void Start()
    {
        SubscribeEvents();
        OnDisplayUpdate();
        DisplayParty();
    }

    private void SubscribeEvents()
    {
        if (PartyManager.Instance == null) return;
        PartyManager.Instance.UpdateDisplay += OnDisplayUpdate;
    }

    private void DisplayParty()
    {
        if (PlayerInventory.Instance == null) return;

        for (int i = 0;  i < _partySlots.Count; i++)
        {
            PartySlotUI slot = _partySlots[i];
            if (i < PlayerInventory.Instance.Creatures.Count)
            {
                slot.gameObject.SetActive(true);
                slot.SetSlot(i, PlayerInventory.Instance.Creatures[i], _partyCameraTextures[i]);
                _partyModels[i].SetDinosaur(PlayerInventory.Instance.Creatures[i]);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    private void OnDisplayUpdate()
    {
        if (PartyManager.Instance == null) return;
        UpdateSelection();
        UpdateButtons();
    }

    private void UpdateSelection()
    {
        foreach (PartySlotUI slot in _partySlots)
        {
            slot.ToggleSelection(slot.SlotNum == PartyManager.Instance.SelectedSlot);
        }
    }

    private void UpdateButtons()
    {
        if (PartyManager.Instance.SelectedSlot >= 0)
        {
            _combinerScreenText.text = "Synthesize";
            _cancelSelectButton.gameObject.SetActive(true);
        }
        else
        {
            _combinerScreenText.text = "New Dinosaur";
            _cancelSelectButton.gameObject.SetActive(false);
        }
    }
}
