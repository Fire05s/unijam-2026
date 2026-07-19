using Combat;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private RawImage _icon;
    [SerializeField] private Image _bg;
    [Header("Settings")]
    [SerializeField] private Color _playerColor;
    [SerializeField] private Color _enemyColor;
    [Header("Animation")]
    [SerializeField] private float _duration = 0.5f;

    private Tween _scaleTween;

    private void OnEnable()
    {
        _scaleTween?.Kill();

        transform.localScale = Vector3.zero;
        _scaleTween = transform
            .DOScale(Vector3.one, _duration)
            .SetEase(Ease.OutSine);
    }

    public void SetSlot(int turnNum, TurnData data)
    {
        _turnText.text = $"Turn: {turnNum}";
        if (CombatSceneManager.Instance.CreatureObjects.TryGetValue(data.dinoID, out CombatCreature creature))
        {
            _icon.texture = creature.CameraTexture;
            _bg.color = (data.dinoID >= 5) ? _enemyColor : _playerColor;
        }
        else
        {
            _icon.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        _scaleTween?.Kill();

        transform.localScale = Vector3.one;
        _scaleTween = transform
            .DOScale(Vector3.zero, _duration)
            .SetEase(Ease.OutSine)
            .OnComplete(() => Destroy(gameObject));
    }
}
