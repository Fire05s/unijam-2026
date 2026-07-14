using UnityEngine;

public class BodyPartSlot : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private BodyPartType _type;

    public BodyPartType Type => _type;

    public void SetPart(int index)
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(i == index);
    }

    public void DisableAll()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }
}
