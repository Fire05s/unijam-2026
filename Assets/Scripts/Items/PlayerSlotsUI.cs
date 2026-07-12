using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Children of the Game Object should only contain the Slot prefab (Slot Prefab have not been created yet)
/// </summary>
public class PlayerSlotsUI : MonoBehaviour
{
    private GameObject[] slots;

    void Start()
    {
        slots = new GameObject[transform.childCount];
        for (int slotIndex = 0; slotIndex < slots.Length; ++slotIndex)
        {
            slots[slotIndex] = transform.GetChild(slotIndex).gameObject;
        }
    }

    void Update()
    {
        
    }
}
