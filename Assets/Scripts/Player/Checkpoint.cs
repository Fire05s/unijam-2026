using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Info")]
    [SerializeField] Vector3 _checkpointPosition;
    
    public Vector3 GetCheckpointPosition()
    {
        return _checkpointPosition;
    }

}
