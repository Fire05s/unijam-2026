using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Based on the given creatures, order them, and allow them to take their turns.
/// </summary>
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance {get; private set;}

    private List<Creature> TurnOrder = new List<Creature>();
    void Awake()
    {
        if (!Instance && Instance!=this) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void AddCreature(Creature creature)
    {
        AddingCreatureToTurnOrder(creature);
    }

    // IMPORTANT: THIS FUNCTION IS CURRENTLY USED FOR TESTING, UPDATE ACCORDINGLY !!
    public void GetCurrentCreature()
    {
        Creature creature = TurnOrder[0];
        Debug.Log("Creature Name: " + creature.gameObject.name + " Current Initiative: " + creature.initiative);
        creature.UpdateInitiative();
        NextCreature();
        // return creature;
    }

    public void RemoveCreature()
    {
        // TODO: Implement Logic to Remove Creature
    }

    public void NextCreature()
    {
        Creature creature = TurnOrder[0];
        TurnOrder.RemoveAt(0);
        TurnOrder.Insert(TurnOrder.Count, creature); // TEMPORARY: As I Figure Out How Initiative Should work
        // AddingCreatureToTurnOrder(creature);
    }

    private void AddingCreatureToTurnOrder(Creature creature)
    {
        int index = 0;
        for (; index < TurnOrder.Count; ++index)
        {
            if (TurnOrder[index] == creature){ return; }
            else if (TurnOrder[index].initiative < creature.initiative){ break; }        
        }
        TurnOrder.Insert(index, creature);
    }
}
