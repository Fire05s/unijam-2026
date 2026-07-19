using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreenUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<PartySlotUI> _partySlots;
    [SerializeField] private List<RenderTexture> _partyCameraTextures;
    [SerializeField] private List<CreatureModel> _partyModels;
    [SerializeField] private TextMeshProUGUI _combinerScreenText;
    [SerializeField] private Button _combinerButton;
    [SerializeField] private Button _cancelSelectButton;
    [SerializeField] private TextMeshProUGUI _slotsText;

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

                UpdateModel(i);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateModel(int index)
    {
        DinosaurData data = PlayerInventory.Instance.Creatures[index];
        if (data.Model != null)
        {
            CreatureModel newModel = Instantiate(data.Model, _partyModels[index].transform.position, _partyModels[index].transform.rotation);
            Destroy(_partyModels[index].gameObject);
            _partyModels[index] = newModel;
        }
    }

    private void OnDisplayUpdate()
    {
        if (PartyManager.Instance == null) return;
        UpdateSelection();
        UpdateButtons();
        UpdateSlotsText();
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
            _combinerButton.gameObject.SetActive(true);
            _combinerScreenText.text = "Synthesize";
            _cancelSelectButton.gameObject.SetActive(true);
        }
        else if (PlayerInventory.Instance.Creatures.Count < PlayerInventory.Instance.MaxPartySize)
        {
            _combinerButton.gameObject.SetActive(true);
            _combinerScreenText.text = "New Dinosaur";
            _cancelSelectButton.gameObject.SetActive(false);
        }
        else
        {
            _combinerButton.gameObject.SetActive(false);
            _cancelSelectButton.gameObject.SetActive(false);
        }
    }

    private void UpdateSlotsText()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        _slotsText.text = $"{inventory.Creatures.Count}/{inventory.MaxPartySize} Slots";
    }
}
