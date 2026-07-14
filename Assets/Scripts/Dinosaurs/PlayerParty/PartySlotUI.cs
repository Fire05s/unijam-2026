using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartySlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _slotNumText;
    [SerializeField] private TextMeshProUGUI _statLabels;
    [SerializeField] private TextMeshProUGUI _statValues;
    [SerializeField] private Image _bgImage;
    [SerializeField] private AbilityTooltipTrigger _ability;
    [Header("Settings")]
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _defaultColor;
    public int SlotNum { get; private set; }
    private DinosaurData _heldDinosaur;

    public void SetSlot(int partyNum, DinosaurData slotData)
    {
        SlotNum = partyNum;
        _heldDinosaur = slotData;

        UpdateSlotDisplay();
    }

    private void UpdateSlotDisplay()
    {
        _slotNumText.text = (SlotNum + 1).ToString();
        string labels = "";
        string values = "";
        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            labels += $"{stat.ToString()}:\n";
            values += _heldDinosaur.GetAdjustedStat(stat).ToString() + "\n";
        }
        _statLabels.text = labels;
        _statValues.text = values;
        _ability.SetWildCards(_heldDinosaur.GetWildCardData());
    }

    public void ToggleSelection(bool isSelected)
    {
        _bgImage.color = isSelected ? _selectedColor : _defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PartyManager.Instance.SelectSlot(SlotNum);
    }
}
