using TMPro;
using UnityEngine;

public class CombatUIFollower : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _smoothTime = 0.08f;

    [SerializeField] private Transform _target;
    private Vector3 _offset;
    private Vector3 _velocity;

    private RectTransform _rectTransform;
    private Camera _camera;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (_target == null || _camera == null) return;

        Vector3 targetPosition = _camera.WorldToScreenPoint(
            _target.position + _offset
        );

        _rectTransform.position = Vector3.SmoothDamp(
            _rectTransform.position,
            targetPosition,
            ref _velocity,
            _smoothTime
        );
    }

    public void Initialize(Transform target, float _yOffset)
    {
        _target = target;
        _offset = new Vector3(0, _yOffset, 0);
        _camera = CameraManager.Instance.WorldCamera;
        _rectTransform.position = _camera.WorldToScreenPoint(_target.position + _offset);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _camera = CameraManager.Instance.WorldCamera;
    }
}
