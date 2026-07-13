using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombinerUI : MonoBehaviour
{
    [Header("Stats Panel")]
    [SerializeField] private GameObject _statsPanel;
    [SerializeField] private TextMeshProUGUI _statsText;
    [Header("Slots")]
    [SerializeField] private List<CombinerSlotUI> _slotObjs;

    private void OnEnable()
    {
        CreatureCombiner.Instance.DisplayUpdate += OnDisplayUpdate;
        OnDisplayUpdate();
    }

    private void OnDisplayUpdate()
    {
        UpdateStats();
        UpdateSlots();
    }

    /// <summary>
    /// Updates the stats panel
    /// </summary>
    private void UpdateStats()
    {
        if (CreatureCombiner.Instance.DisplayDinosaur == null)
        {
            _statsPanel.SetActive(false);
            return;
        }
        _statsPanel.SetActive(true);

        DinosaurData displayData = CreatureCombiner.Instance.DisplayDinosaur;
        _statsText.text = $"Health: {displayData.GetStat(StatType.Health)}\n" +
                          $"Attack: {displayData.GetStat(StatType.Attack)}\n" +
                          $"Speed: {displayData.GetStat(StatType.Speed)}\n" +
                          $"Crit Chance: {displayData.GetStat(StatType.CritChance)}\n";
    }

    /// <summary>
    /// Updates the combiner slot displays
    /// </summary>
    private void UpdateSlots()
    {
        foreach (var slot in  _slotObjs)
        {
            if (CreatureCombiner.Instance.PartSlots.TryGetValue(slot.GetSlotType(), out DinosaurPart part))
            {
                slot.SetPart(part);
            }
            else
            {
                slot.SetPart(null);
            }
        }
    }
}
