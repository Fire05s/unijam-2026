using System;
using UnityEngine;

/// <summary>
/// Stores the base logic for the creatures (WIP - acting as a template right now)
/// </summary>
public class Creature : MonoBehaviour
{
    [field: SerializeField] public float speed {get; private set;}
    public float initiative {get; private set;}

    private void Start()
    {
        initiative = speed;
        TurnManager.Instance.AddCreature(this);
    }

    public void UpdateInitiative()
    {
        initiative += speed;
    }
}