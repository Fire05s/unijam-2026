using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventorySlotUI _slotPrefab;
    [SerializeField] private Transform _contentParent;

    private List<InventorySlotUI> _slots;

    private void OnEnable()
    {
        _slots = GetComponentsInChildren<InventorySlotUI>().ToList();
        Clear();
    }

    private void Start()
    {
        DisplayAll();
    }

    /// <summary>
    /// Clears all the displayed slots
    /// </summary>
    private void Clear()
    {
        foreach (var slot in _slots)
        {
            Destroy(slot.gameObject);
        }

        _slots.Clear();
    }

    /// <summary>
    /// Displays only the specified body part type in inventory
    /// </summary>
    /// <param name="type"> Specified type </param>
    public void DisplayType(string typeName)
    {
        Clear();

        BodyPartType type = StringToBodyType(typeName);
        foreach (var item in PlayerInventory.Instance.BodyParts)
        {
            if (item.Type != type) continue;
            InventorySlotUI newSlot = Instantiate(_slotPrefab, _contentParent);
            newSlot.SetItem(item);
            _slots.Add(newSlot);
        }
    }

    /// <summary>
    /// Displays all items in inventory
    /// </summary>
    public void DisplayAll()
    {
        Clear();

        Debug.Log($"Amount in Inv: {PlayerInventory.Instance.BodyParts.Count}");

        foreach (var item in PlayerInventory.Instance.BodyParts)
        {
            InventorySlotUI newSlot = Instantiate(_slotPrefab, _contentParent);
            newSlot.SetItem(item);
            _slots.Add(newSlot);
        }
    }

    private BodyPartType StringToBodyType(string name)
    {
        if (Enum.TryParse(name, out BodyPartType state))
        {
            return state;
        }
        else
        {
            Debug.LogWarning($"'{name}' is not a valid body type. Defaulted to Head type");
            return BodyPartType.Head;
        }
    }
}
