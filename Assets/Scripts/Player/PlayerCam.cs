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
        XSens = PlayerPrefs.GetFloat("XSens", 0.5f);
        YSens = PlayerPrefs.GetFloat("YSens", 0.5f);
    }

    private void Update()
    {
        if (CamUnlocked)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * PlayerPrefs.GetFloat("SensX", 0.5f);
            float mouseY = Input.GetAxisRaw("Mouse Y") * PlayerPrefs.GetFloat("SensY", 0.5f);

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
