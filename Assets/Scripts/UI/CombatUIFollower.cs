using UnityEngine;

public class CombatUIFollower : MonoBehaviour
{
    private Transform _target;
    private Vector3 _offset;

    private RectTransform _rectTransform;
    private Camera _camera;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (_target == null || _camera == null) return;

        Vector3 screenPos = _camera.WorldToScreenPoint(
            _target.position + _offset
        );

        _rectTransform.position = screenPos;
    }

    public void Initialize(Transform target, float _yOffset)
    {
        _target = target;
        _offset = new Vector3(0, _yOffset, 0);
        _camera = CameraManager.Instance.WorldCamera;
    }
}
