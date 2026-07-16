using DG.Tweening;
using UnityEngine;

public class PopupElement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _duration = 1f;
    [SerializeField] private Ease _eastType = Ease.OutBounce;

    private Tween _scaleTween;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        _scaleTween?.Kill();

        transform.localScale = Vector3.zero;
        _scaleTween = transform
            .DOScale(Vector3.one, _duration)
            .SetEase(_eastType);
    }
}
