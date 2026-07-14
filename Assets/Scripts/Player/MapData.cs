using UnityEngine;
using System.Collections.Generic;

public class MapData : MonoBehaviour
{
    public static MapData Instance { get; private set; }

    [SerializeField] private Vector3 PlayerPosition;
    private List<int> enemiesEncountered;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        enemiesEncountered = new List<int>();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkEnemyEncountered(int id)
    {
        if(!enemiesEncountered.Contains(id))
        {
            enemiesEncountered.Add(id);
        }
    }
    public void SavePlayerPosition(Vector3 pos)
    {
        PlayerPosition = pos;
    }
    public bool EnemyEncounteredBefore(int id)
    {
        if(enemiesEncountered.Contains(id))
        {
            return true;
        }
        return false;
    }
    public Vector3 GetPlayerPosition()
    {
        return PlayerPosition;
    }
}
