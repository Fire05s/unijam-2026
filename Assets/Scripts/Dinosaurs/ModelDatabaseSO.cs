using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/New Model Database")]
public class ModelDatabaseSO : ScriptableObject
{
    [Header("Models")]
    [SerializeField] private List<CreatureModelEntry> _models;

    private Dictionary<(int head, int body, int legs), GameObject> _lookup;

    private void Initialize()
    {
        if (_lookup != null)
            return;

        _lookup = new();

        foreach (var model in _models)
        {
            _lookup[(model.headID, model.bodyID, model.legsID)] = model.prefab;
        }
    }

    public GameObject GetModel(int headID, int bodyID, int legsID)
    {
        Initialize();

        _lookup.TryGetValue((headID, bodyID, legsID), out var prefab);
        return prefab;
    }

    public GameObject GetModelByParts(List<BodyPartSO> parts)
    {
        int headID = 0;
        int bodyID = 0;
        int legsID = 0;
        foreach (var part in parts)
        {
            switch (part.PartType)
            {
                case BodyPartType.Head:
                    headID = part.ModelID;
                    break;
                case BodyPartType.Arms:
                    bodyID = part.ModelID;
                    break;
                case BodyPartType.Legs:
                    legsID = part.ModelID;
                    break;
            }
        }
        Debug.Log($"Getting model {headID}-{bodyID}-{legsID}");
        return GetModel(headID, bodyID, legsID);
    }
}

[System.Serializable]
public class CreatureModelEntry
{
    public int headID;
    public int bodyID;
    public int legsID;

    public GameObject prefab;
}
