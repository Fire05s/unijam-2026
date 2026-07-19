using UnityEngine;

public class ExcavationPoint : MonoBehaviour
{
    [Header("ID")]
    public int ExcavationID;

    void Start()
    {
        if (MapData.Instance.ExcavationUsedBefore(ExcavationID))
        {
            Destroy(gameObject);
        }
    }
}