using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI _nameText;
    private DinosaurPart _heldItem;

    private void Awake()
    {
        _nameText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnDestroy()
    {
        TooltipSystem.Hide();
    }
    public void SetItem(DinosaurPart item)
    {
        _heldItem = item;

        UpdateSlot();
    }

    public DinosaurPart GetItem()
    {
        return _heldItem;
    }

    private void UpdateSlot()
    {
        if (_heldItem != null)
        {
            _nameText.text = _heldItem.Reference.Name;
            // TODO: update item display (possibly icons)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (CreatureCombiner.Instance?.SelectedPart == _heldItem)
        {
            CreatureCombiner.Instance?.UnselectPart();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CreatureCombiner.Instance?.SelectPart(_heldItem);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.ShowPart(_heldItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }
}
