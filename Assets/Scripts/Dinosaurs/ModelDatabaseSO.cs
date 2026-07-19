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
}

[System.Serializable]
public class CreatureModelEntry
{
    public int headID;
    public int bodyID;
    public int legsID;

    public GameObject prefab;
}
