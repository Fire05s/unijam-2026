using Unity.Cinemachine;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera _camera;
    public bool camUnlocked = true;
    [Header("Sensitivity")]
    public float sensX;
    public float sensY;

    private Transform _orientation;

    private float _xRotation;
    private float _yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _orientation = GameObject.Find("CamOrientation").transform;
        _camera.Follow = GameObject.Find("CamPos").transform;
    }

    private void Update()
    {
        if (camUnlocked)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            _orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        }
    }

    public void ChangeSens(float sensXValue, float sensYValue)
    {
        sensX = sensXValue;
        sensY = sensYValue;
    }
}
