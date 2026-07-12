using System;
using System.Collections.Generic;

public class FieldTiller
{
    CombatDinoObjects[] playerSlots = new CombatDinoObjects[5];

    CombatDinoObjects[] enemySlots = new CombatDinoObjects[10];

    int playerNum;
    int enemyNum;
    FieldTiller(int initialPlayerNum, int initialEnemyNum)
    {
        playerNum = initialPlayerNum;
        enemyNum = initialEnemyNum;
    }
}