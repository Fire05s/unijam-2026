using UnityEngine;

public class MoveCam : MonoBehaviour
{
    private Transform _camPos;

    private void Start()
    {
        _camPos = GameObject.Find("CamPos").transform;
    }

    private void Update()
    {
        transform.position = _camPos.position;
    }
}
