using System.Collections.Generic;
using UnityEngine;

public class ModelGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<BodyPartSlot> _slots;

    private Dictionary<BodyPartType, BodyPartSlot> _slotLookup = new();

    private void Awake()
    {
        _slotLookup = new();

        foreach (var slot in _slots)
            _slotLookup[slot.Type] = slot;
    }

    public void SetDinosaur(DinosaurData data)
    {
        DisableAll();
        foreach (KeyValuePair<BodyPartType, DinosaurPart> entry in data.GetBodyParts())
        {
            SetPart(entry.Key, entry.Value.Reference.ModelID); // Id is the part's assigned model id
        }
    }

    public void SetPart(BodyPartType type, int variant)
    {
        if (_slotLookup.TryGetValue(type, out var slot))
            slot.SetPart(variant);
    }

    public void DisableAll()
    {
        foreach (var slot in _slots)
            slot.DisableAll();
    }
}
