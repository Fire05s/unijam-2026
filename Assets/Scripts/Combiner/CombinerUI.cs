using TMPro;
using UnityEngine;

public class CombinerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CreatureCombiner _combiner;
    [Header("Stats Panel")]
    [SerializeField] private GameObject _statsPanel;
    [SerializeField] private TextMeshProUGUI _statsText;

    private void OnEnable()
    {
        _combiner.DisplayUpdate += OnDisplayUpdate;
        OnDisplayUpdate();
    }

    private void OnDisplayUpdate()
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        if (_combiner.DisplayDinosaur == null)
        {
            _statsPanel.SetActive(false);
            return;
        }
        _statsPanel.SetActive(true);

        DinosaurData displayData = _combiner.DisplayDinosaur;
        _statsText.text = $"Health: {displayData.GetStat(StatType.Health)}\n" +
                          $"Attack: {displayData.GetStat(StatType.Attack)}\n" +
                          $"Speed: {displayData.GetStat(StatType.Speed)}\n" +
                          $"Crit Chance: {displayData.GetStat(StatType.CritChance)}\n";
    }
}
