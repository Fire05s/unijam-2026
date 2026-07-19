using UnityEngine;

public class Grass : MonoBehaviour
{
    void Update() {
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
