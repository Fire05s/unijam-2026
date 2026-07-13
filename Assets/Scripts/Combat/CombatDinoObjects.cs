using UnityEngine;
using System.Collections.Generic;
public class CombatDinoObjects : MonoBehaviour
{
    [SerializeField] List<GameObject> Items = new List<GameObject>();

    public void Enable(int index)
    {
        Items[index].SetActive(true);
    }
    public void Disable(int index)
    {
        //Debug.Log($"Attempted to disable {index}");
        Items[index].SetActive(false);
    }
    public Vector3 GetPosition(int index)
    {
        //return new Vector2(Items[index-1].transform.position.x, Items[index-1].transform.position.y);
        return Items[index].transform.position;
    }
}
