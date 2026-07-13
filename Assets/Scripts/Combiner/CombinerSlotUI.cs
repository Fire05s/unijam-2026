using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombinerSlotUI : MonoBehaviour, IPointerClickHandler, IDeselectHandler
{
    [Header("Settings")]
    [SerializeField] private BodyPartType _slotType;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _defaultColor;

    private DinosaurPart _heldPart;
    private TextMeshProUGUI _nameText;
    private Image _slotBg;
    private bool _isSelected;

    private void Awake()
    {
        _nameText = GetComponentInChildren<TextMeshProUGUI>();
        _slotBg = GetComponent<Image>();
    }

    public void SetPart(DinosaurPart part)
    {
        _heldPart = part;

        UpdateSlotDisplay();
    }

    public BodyPartType GetSlotType()
    {
        return _slotType;
    }

    private void UpdateSlotDisplay()
    {
        if (_heldPart != null)
        {
            _nameText.text = _heldPart.Reference.Name;
            // TODO: add icons here (if planned)
        }
        else
        {
            _nameText.text = "None";
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (CreatureCombiner.Instance.SelectedPart == _heldPart)
        {
            CreatureCombiner.Instance.UnselectPart();
            ToggleSelect(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CreatureCombiner.Instance.SelectPart(_heldPart);
        ToggleSelect(!_isSelected);
    }

    private void ToggleSelect(bool selected)
    {
        _isSelected = selected;
        if (_isSelected)
        {
            _slotBg.color = _selectedColor;
        }
        else
        {
            _slotBg.color = _defaultColor;
        }
    }
}
