using UnityEngine;
using UnityEngine.UI;

public class CombatSlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RawImage _icon;
    [SerializeField] private Slider _healthBar;

    public void SetIcon(RenderTexture texture)
    {
        _icon.texture = texture;
    }

    public void SetHealthValue(float value)
    {
        _healthBar.value = value;
    }
}
