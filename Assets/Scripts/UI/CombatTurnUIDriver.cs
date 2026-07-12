using UnityEngine;
using TMPro;

public class CombatTurnUIDriver : MonoBehaviour
{
    [SerializeField] TMP_Text turnNum;

    public void Next(int turn)
    {
        turnNum.text = $"{turn}";
    }
}
