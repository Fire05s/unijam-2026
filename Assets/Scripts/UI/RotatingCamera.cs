using UnityEngine;

public class RotatingCamera : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}