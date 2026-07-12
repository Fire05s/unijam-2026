using UnityEngine;
using System.Collections.Generic;
public class CombatDinoObjects : MonoBehaviour
{
    [SerializeField] List<GameObject> Items = new List<GameObject>();

    public void Disable(int index)
    {
        //Debug.Log($"Attempted to disable {index}");
        Items[index].SetActive(false);
    }
    public Vector2 GetPosition(int index)
    {
        return new Vector2(Items[index-1].transform.position.x, Items[index-1].transform.position.y);
    }
}
