using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class CombatHUDDriver : MonoBehaviour
{
    [SerializeField] CombatTurnUIDriver currentTurn;
    [SerializeField] CombatTurnUIDriver nextTurn;
    [SerializeField] CombatDinoObjects playerSide;
    [SerializeField] CombatDinoObjects enemySide;
    int[] playerDinos = new int[5];
    int[] enemyDinos = new int[10];
    Dictionary<int, GameObject> modelLookup = new Dictionary<int, GameObject>();

    void Start()
    {
        CombatManager.Instance.TurnAdvanced += UpdateUI;
        CombatManager.Instance.TurnAdvanced += UpdateField;
    }
    public void UpdateUI(int turn)
    {
        currentTurn.Next(turn);
        nextTurn.Next(turn + 1);
    }
    public void UpdateField(int turn)
    {
        //Update Player Dinosaurs
        List<int> playerIndices = CombatManager.Instance.PlayerDinosaurIndicies;
        ClearPlayerField();
        switch(playerIndices.Count)
        {
            case 1:
                playerDinos[2] = playerIndices[0];
                break;
            case 2:
                playerDinos[1] = playerIndices[0];
                playerDinos[2] = playerIndices[1];
                break;
            case 3:
                playerDinos[1] = playerIndices[0];
                playerDinos[2] = playerIndices[1];
                playerDinos[3] = playerIndices[2];
                break;
            case 4:
                playerDinos[0] = playerIndices[0];
                playerDinos[1] = playerIndices[1];
                playerDinos[2] = playerIndices[2];
                playerDinos[3] = playerIndices[3];
                break;
            case 5:
                for (int i=0; i<5; i++)
                {
                    playerDinos[i] = playerIndices[i];
                }
                break;
        }
        for(int i=0; i<5; i++)
        {
            if (playerDinos[i] != -1)
            {
                playerSide.Enable(i);
                //modelLookup[playerDinos[i]].transform.position = playerSide.GetPosition(i);
            } else
            {
                playerSide.Disable(i);
            }
        }
        //Update Enemy Dinosaurs
        List<int> enemyIndices = CombatManager.Instance.EnemyDinosaurIndicies;
        ClearEnemyField();
        
    }
    public void ClearPlayerField()
    {
        for(int i=0; i<5; i++)
        {
            playerDinos[i] = -1;
        }
    }
    public void ClearEnemyField()
    {
        for(int i=0; i<10; i++)
        {
            enemyDinos[i] = -1;
        }
    }
    
}