using UnityEngine;
using System.Collections.Generic;

public class MapData : MonoBehaviour
{
    public static MapData Instance { get; private set; }

    [SerializeField] private Vector3 _PlayerPosition;
    [SerializeField] private Quaternion _PlayerRotation;
    [SerializeField] private Vector3 _checkpointPosition;
    private List<int> _enemiesEncountered;
    private List<int> _excavationUsed;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _enemiesEncountered = new List<int>();
        _excavationUsed = new List<int>();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkEnemyEncountered(int id)
    {
        if(!_enemiesEncountered.Contains(id))
        {
            _enemiesEncountered.Add(id);
        }
    }
    public void RemoveEnemyEncountered(int id)
    {
        if(_enemiesEncountered.Contains(id))
        {
            _enemiesEncountered.Remove(id);
        }
    }
    public void SavePlayerPosition(Vector3 pos, Quaternion rot)
    public void MarkExcavationUsed(int id)
    {
        if(!_excavationUsed.Contains(id))
        {
            _excavationUsed.Add(id);
        }
    }
    public void SavePlayerPosition(Vector3 pos)
    {
        _PlayerPosition = pos;
        _PlayerRotation = rot;
    }
    public void SavePlayerCheckpoint(Vector3 pos)
    {
        _checkpointPosition = pos;
    }
    public bool EnemyEncounteredBefore(int id)
    {
        if(_enemiesEncountered.Contains(id))
        {
            return true;
        }
        return false;
    }
    public bool ExcavationUsedBefore(int id)
    {
        if(_excavationUsed.Contains(id))
        {
            return true;
        }
        return false;
    }
    public Vector3 GetPlayerPosition()
    {
        return _PlayerPosition;
    }

    public Quaternion GetPlayerRotation()
    {
        return _PlayerRotation;
    }
    
    public Vector3 GetCheckpointPosition()
    {
        return _checkpointPosition;
    }
}
