using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    private TextMeshProUGUI _nameText;
    private DinosaurPart _heldItem;

    private void Awake()
    {
        _nameText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetItem(DinosaurPart item)
    {
        _heldItem = item;

        UpdateSlot();
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


}
