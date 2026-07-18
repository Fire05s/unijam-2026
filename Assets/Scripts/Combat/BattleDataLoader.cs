using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Combat
{
    public class BattleDataLoader : MonoBehaviour
    {
        public static BattleDataLoader Instance { get; private set; }

        [Header("Battle Info")]
        public List<DinosaurData> EnemyDinosData;
        public BaseStatsSO PlayerDinoBaseStats;
        public bool IsRewardingNewDino;
        [Range(0f, 1f)]
        public float HealPercent;
        [Header("Scene Transition")]
        [SerializeField] private ScreenTransition _transitionObject;
        [SerializeField] private string _mainLevelScene;
        [SerializeField] private float _transitionDuration;

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
            IsRewardingNewDino = battleData.rewardsNewDino;
            StartCoroutine(BattleDelay());
        }

        private IEnumerator BattleDelay()
        {
            StartCoroutine(_transitionObject.FadeIn(_mainLevelScene, _transitionDuration, false));
            yield return SceneManager.LoadSceneAsync("Combat");
            StartCoroutine(_transitionObject.FadeOut());
            if (CombatManager.Instance == null)
            {
                Debug.LogError("CombatManager does not exist. Scene may not have loaded.");
            }
            CombatManager.Instance.SetupCombat(EnemyDinosData);
            CombatManager.Instance.TriggerCombatStart();
        }

        public void TriggerVictory()
        {
            foreach (DinosaurData dino in PlayerInventory.Instance.Creatures)
            {
                dino.HealDino(Mathf.Ceil(HealPercent * dino.GetAdjustedStat(StatType.Health)));
            }

            if (IsRewardingNewDino)
            {
                PlayerInventory.Instance.IncrementPartyCount();
            }

            float limbDropChance = Random.Range(0f, 1f);
            int droppedLimbs;
            if (limbDropChance < 0.3f)
            {
                droppedLimbs = 0;
            }
            else if (limbDropChance < 0.6f)
            {
                droppedLimbs = 1;
            }
            else if (limbDropChance < 0.8f)
            {
                droppedLimbs = 2;
            }
            else if (limbDropChance < 0.925f)
            {
                droppedLimbs = 3;
            }
            else if (limbDropChance < 0.975f)
            {
                droppedLimbs = 4;
            }
            else
            {
                droppedLimbs = 5;
            }
            droppedLimbs = EnemyDinosData.Count < droppedLimbs ? EnemyDinosData.Count : droppedLimbs;

            float limbChoice;
            for (int i = 1; i <= droppedLimbs; i++)
            {
                DinosaurData randomDino = EnemyDinosData[Random.Range(0, EnemyDinosData.Count - 1)];
                limbChoice = Random.Range(0f, 1f);
                if (limbChoice < 0.2f)
                {
                    PlayerInventory.Instance.AddBodyPart(randomDino.GetBodyParts()[BodyPartType.Head]);
                }
                else if (limbChoice < 0.6f)
                {
                    PlayerInventory.Instance.AddBodyPart(randomDino.GetBodyParts()[BodyPartType.Arms]);
                }
                else
                {
                    PlayerInventory.Instance.AddBodyPart(randomDino.GetBodyParts()[BodyPartType.Legs]);
                }
            }

            // TODO: Call Victory UI Screen here

            _transitionObject.FadeAndLoad(_mainLevelScene, _transitionDuration);
        }

        public void TriggerDefeat()
        {
            // TODO: Call Defeat UI Screen Here
            _transitionObject.FadeAndLoad(_mainLevelScene, _transitionDuration);
        }
    }
}