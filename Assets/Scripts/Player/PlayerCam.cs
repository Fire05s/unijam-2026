using Unity.Cinemachine;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera _camera;
    public bool CamUnlocked = true;
    [Header("Sensitivity")]
    public float XSens;
    public float YSens;

    private Transform _orientation;

    private float _xRotation;
    private float _yRotation;

    private Transform _target = null;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _orientation = GameObject.Find("CamOrientation").transform;
        _camera.Follow = GameObject.Find("CamPos").transform;
        XSens = PlayerPrefs.GetFloat("XSens", 1f);
        YSens = PlayerPrefs.GetFloat("YSens", 3f);
    }

    private void Update()
    {
        if (CamUnlocked)
        {
            Debug.Log($"x sens {XSens}, y sens {YSens}");
            float mouseX = Input.GetAxisRaw("Mouse X") * XSens * PlayerPrefs.GetFloat("SensX", defaultValue: 1.0f);
            float mouseY = Input.GetAxisRaw("Mouse Y") * YSens * PlayerPrefs.GetFloat("SensY", defaultValue: 3.0f);

            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            _camera.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            _orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        }
        else if (_target)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_target.position - transform.position), 2 * Time.deltaTime);
        }
    }

    public void UpdateSens()
    {
        XSens = PlayerPrefs.GetFloat("XSens");
        YSens = PlayerPrefs.GetFloat("YSens");
    }

    public void GiveTransformTarget(Transform target)
    {
        _target = target;
    }
}
