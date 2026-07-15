using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _headerField;
    [SerializeField] private TextMeshProUGUI _contentField;
    [SerializeField] private LayoutElement _layoutElement;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [Header("Settings")]
    [SerializeField] private int _characterWrapLimit = 150;

    private Tween _fadeTween;

    private void Awake()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            _headerField.gameObject.SetActive(false);
        }
        else
        {
            _headerField.gameObject.SetActive(true);
            _headerField.text = header;
        }
        _contentField.text = content;

        int headerLength = _headerField.text.Length;
        int contentLength = _contentField.text.Length;

        // Enable layout element restriction when length above wrap limit
        _layoutElement.enabled = (headerLength > _characterWrapLimit || contentLength > _characterWrapLimit) ? true : false;
    }
    public void FadeIn(float duration = 0.15f)
    {
        _fadeTween?.Kill();

        gameObject.SetActive(true);

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        _fadeTween = _canvasGroup
            .DOFade(1f, duration)
            .SetEase(Ease.OutQuad);
    }

    public void FadeOut(float duration = 0.15f)
    {
        _fadeTween?.Kill();

        _fadeTween = _canvasGroup
            .DOFade(0f, duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    private void Update()
    {
        var position = Input.mousePosition;
        var normalizedPosition = new Vector2(position.x / Screen.width, position.y / Screen.height);
        var pivot = CalculatePivot(normalizedPosition);
        _rectTransform.pivot = pivot;
        transform.position = position;
    }

    private Vector2 CalculatePivot(Vector2 normalizedPosition)
    {
        var pivotTopLeft = new Vector2(-0.05f, 1.05f);
        var pivotTopRight = new Vector2(1.05f, 1.05f);
        var pivotBottomLeft = new Vector2(-0.05f, -0.05f);
        var pivotBottomRight = new Vector2(1.05f, -0.05f);

        if (normalizedPosition.x < 0.5f && normalizedPosition.y >= 0.5f)
        {
            return pivotTopLeft;
        }
        else if (normalizedPosition.x > 0.5f && normalizedPosition.y >= 0.5f)
        {
            return pivotTopRight;
        }
        else if (normalizedPosition.x <= 0.5f && normalizedPosition.y < 0.5f)
        {
            return pivotBottomLeft;
        }
        else
        {
            return pivotBottomRight;
        }
    }
}
