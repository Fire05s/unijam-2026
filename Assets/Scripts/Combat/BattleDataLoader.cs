using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Combat
{
    public class BattleDataLoader : MonoBehaviour
    {
        public static BattleDataLoader Instance { get; private set; }
        public List<DinosaurData> EnemyDinosData;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartBattle(BattleData battleData)
        {
            EnemyDinosData = battleData.InitializeEnemyDinos();
            StartCoroutine(BattleDelay());
        }

        private IEnumerator BattleDelay()
        {
            yield return SceneManager.LoadSceneAsync("Combat");
            if (CombatManager.Instance == null)
            {
                Debug.LogError("CombatManager does not exist. Scene may not have loaded.");
            }
            CombatManager.Instance.SetupCombat(EnemyDinosData);
            CombatManager.Instance.TriggerCombatStart();
        }
    }
}