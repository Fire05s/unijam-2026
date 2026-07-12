using UnityEngine;
using TMPro;

public class CombatHUDDriver : MonoBehaviour
{
    [SerializeField] CombatTurnUIDriver currentTurn;
    [SerializeField] CombatTurnUIDriver nextTurn;
    [SerializeField] CombatDinoObjects playerSide;
    [SerializeField] CombatDinoObjects enemySide;

    void Start()
    {
        CombatManager.Instance.TurnAdvanced += UpdateUI;
    }
    public void UpdateUI(int turn)
    {
        currentTurn.Next(turn);
        nextTurn.Next(turn + 1);
    }
    void Update()
    {
        if (!CombatManager.Instance.canSelectDinosaur) {return;}



    }
}