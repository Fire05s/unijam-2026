using System.Collections.Generic;
using UnityEngine;

public class ModelGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<BodyPartSlot> _slots;

    private Dictionary<BodyPartType, BodyPartSlot> _slotLookup;

    private void Awake()
    {
        _slotLookup = new();

        foreach (var slot in _slots)
            _slotLookup[slot.Type] = slot;
    }

    public void SetDinosaur(DinosaurData data)
    {
        foreach (KeyValuePair<BodyPartType, DinosaurPart> entry in data.GetBodyParts())
        {
            // TODO: some type of matching dinosaur part data to part id
            // SetPart(entry.Key, entry.Value.Reference.ID); // where id is the part's assigned model id
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
