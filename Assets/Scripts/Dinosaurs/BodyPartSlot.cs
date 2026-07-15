using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartSlot : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private BodyPartType _type;

    public BodyPartType Type => _type;

    public void SetPart(int index)
    {
        if (index >= transform.childCount)
        {
            Debug.LogWarning($"Part id {index} doesn't exist, using default model");
            return;
        }
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(i == index);
    }

    public void DisableAll()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }
}
