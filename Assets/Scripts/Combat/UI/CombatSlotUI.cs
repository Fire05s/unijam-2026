using UnityEngine;
using UnityEngine.UI;

public class CombatSlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RawImage _icon;
    [SerializeField] private HealthUI _healthBar;

    public HealthUI HealthBar => _healthBar;

    public void SetIcon(RenderTexture texture)
    {
        _icon.texture = texture;
    }
}
