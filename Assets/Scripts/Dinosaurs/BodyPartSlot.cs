using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BodyPartSlot : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private BodyPartType _type;
    [SerializeField] private List<Transform> _parts;

    public BodyPartType Type => _type;

    private void Start()
    {
        if (_parts == null)
            _parts = GetComponentsInChildren<Transform>().ToList();
    }

    public void SetPart(int index)
    {
        if (index >= _parts.Count)
        {
            Debug.LogWarning($"Part id {index} doesn't exist, using default model");
            return;
        }
        for (int i = 0; i < _parts.Count; i++)
            _parts[i].gameObject.SetActive(i == index);
    }

    public void DisableAll()
    {
        foreach (Transform child in _parts)
            child.gameObject.SetActive(false);
    }
}
