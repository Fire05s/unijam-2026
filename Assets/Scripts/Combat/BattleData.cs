using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Combat
{
    public class BattleData : MonoBehaviour
    {
        public bool rewardsNewDino = false;
        public List<EnemyDino> enemyDinos = new();

        public BattleData(BattleData other)
        {
            enemyDinos = new List<EnemyDino>(other.enemyDinos);
        }

        public List<DinosaurData> InitializeEnemyDinos()
        {
            List<DinosaurData> enemyDinosData = new();
            foreach (EnemyDino enemyDino in enemyDinos)
            {
                enemyDinosData.Add(new DinosaurData(enemyDino.BaseStats, enemyDino.BodyParts));
            }
            return enemyDinosData;
        }
    }

    [Serializable]
    public struct EnemyDino
    {
        public BaseStatsSO BaseStats;
        public List<BodyPartSO> BodyParts;
    }
}