using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _abilitiesText;
    private List<WildCardSO> _wildcards;
    private string _content;

    public void SetWildCards(List<WildCardSO> data)
    {
        _wildcards = data;

        _content = "";
        foreach (var so in _wildcards)
        {
            _content += $"{so.Name}: {so.Description}\n";
        }
        _abilitiesText.text = string.Join(", ", _wildcards.Select(w => w.Name));
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.Show(_content);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }
}
