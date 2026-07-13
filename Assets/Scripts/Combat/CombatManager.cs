using UnityEngine;
using System;
using System.Collections.Generic;

namespace Combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance {get; private set;}
        public delegate void IntDelegate(int value);
        public event IntDelegate TurnAdvanced;

        /// <summary>
        /// Total dinosaur list with ID and Combat Entity class
        /// </summary>
        public Dictionary<int, CombatEntity> Dinosaurs = new Dictionary<int, CombatEntity>();
        [Header("Combat Delay Values"), SerializeField] private float EmptyTurnDelay;
        [SerializeField] private float AttackDelay;
        [SerializeField] private float InterTurnDelay;
        [SerializeField] private float DoTDelay;
        [Header("Alive Dinosaur Lists")]
        public List<int> RemainingPlayerDinosaurs = new List<int>();
        public List<int> RemainingEnemyDinosaurs = new List<int>();
        [Header("Runtime Information")]
        [SerializeField] private int currentTurnNumber;
        [SerializeField] private List<int> NonEmptyTurns = new List<int>();
        [SerializeField] private TurnQueue MoveOrderQueue = new TurnQueue();
        [Header("Current Turn Information"), SerializeField] public TurnStep state;
        [SerializeField] public int currentActingNum;
        [SerializeField] public int targetedDinosaur;
        

        void Awake()
        {
            Instance = this;
            if (Instance != this) {Destroy(this);}
        }
        /// <summary>
        /// Initialize the combat session with the player and enemy dinosaurs
        /// </summary>
        /// <param name="players">A list of player dinosaur data</param>
        /// <param name="enemies">A list of enemy dinosaur data</param>
        public void SetupCombat(BattleData currentBattle)//List<DinosaurData> players, List<DinosaurData> enemies)
        {
            currentTurnNumber = 0;
            targetedDinosaur = -1;
            state = TurnStep.TurnStart;

            // player dino id's go from 0-4
            for (int id = 0; id < PlayerInventory.Instance.Creatures.Count; id++)
            {
                // building the IDs for all the player dinos, adding them to the alive player dinos list,
                // and creating their combat entity
                DinosaurData curDinosaur = PlayerInventory.Instance.Creatures[id];
                RemainingPlayerDinosaurs.Add(id);
                Dinosaurs[id] = new CombatEntity(id, EntitySide.Player, curDinosaur.GetAdjustedStat(StatType.Health), 
                    curDinosaur.GetAdjustedStat(StatType.Speed), curDinosaur.GetAdjustedStat(StatType.Attack),
                    curDinosaur.GetAdjustedStat(StatType.CritChance), curDinosaur.GetWildCardAbilities());
            }

            // enemy dino id's go from 5-14
            List<DinosaurData> enemyDinosData = currentBattle.InitializeEnemyDinos();
            for (int id = 5; id < enemyDinosData.Count; id++)
            {
                // building the IDs for all the enemy dinos, adding them to the alive enemy dinos list,
                // and creating their combat entity

                DinosaurData curDinosaur = enemyDinosData[id];
                RemainingEnemyDinosaurs.Add(id);
                Dinosaurs[id] = new CombatEntity(id, EntitySide.Player, curDinosaur.GetAdjustedStat(StatType.Health), 
                    curDinosaur.GetAdjustedStat(StatType.Speed), curDinosaur.GetAdjustedStat(StatType.Attack),
                    curDinosaur.GetAdjustedStat(StatType.CritChance), curDinosaur.GetWildCardAbilities());
            }

            BuildInitialQueue();
        }
        void BuildInitialQueue()
        {
        //     int currentSlotNum = 0;
        //     for (int i=10; i>0; i--)
        //     {
        //         //Build list of Dinosaurs with equivalent speed
        //         List<int> SameSpeedP = new List<int>();
        //         foreach (CombatEntity entity in Dinosaurs.Values)
        //         {
        //             if (entity._side==EntitySide.Player && entity._speed==i) {SameSpeedP.Add(entity._id);}
        //         }
        //         List<int> SameSpeedE = new List<int>();
        //         foreach (CombatEntity entity in Dinosaurs.Values)
        //         {
        //             if (entity._side==EntitySide.Enemy && entity._speed==i) {SameSpeedE.Add(entity._id);}
        //         }

        //         //If there are no dinosaurs with this speed, move to next speed level
        //         if (SameSpeedP.Count==0 && SameSpeedE.Count==0) {continue;}

        //         //As long as there are dinos in both lists, add them while alternating
        //         while(SameSpeedP.Count > 0 && SameSpeedE.Count > 0)
        //         {
        //             int randomNum = UnityEngine.Random.Range(0, SameSpeedP.Count);
        //             int dino = SameSpeedP[randomNum];
        //             SameSpeedP.RemoveAt(randomNum);
        //             AddToQueue(Dinosaurs[dino].NextTurn(0), dino);

        //             randomNum = UnityEngine.Random.Range(0, SameSpeedE.Count);
        //             dino = SameSpeedE[randomNum];
        //             SameSpeedE.RemoveAt(randomNum);
        //             AddToQueue(Dinosaurs[dino].NextTurn(0), dino);
        //         }

        //         //Flush remaining dinosaurs into the queue
        //         foreach(int dino in SameSpeedP)
        //         {
        //             AddToQueue(Dinosaurs[dino].NextTurn(0), dino);
        //         }
        //         foreach(int dino in SameSpeedE)
        //         {
        //             AddToQueue(Dinosaurs[dino].NextTurn(0), dino);
        //         }

        //         //Repeat
        //     }

        //     for(int i=0; i<Dinosaurs.Count; i++)
        //     {
        //         CombatEntity dinosaur = Dinosaurs[i];
        //         if (dinosaur.side == EntitySide.Player) {
        //             numPlayerDinosaurs++;
        //             PlayerDinosaurIndicies.Add(i);
        //         }
        //         else {
        //             numEnemyDinosaurs++;
        //             EnemyDinosaurIndicies.Add(i);
        //         }
        //         remainingPlayerDinosaurs = numPlayerDinosaurs;
        //         remainingEnemyDinosaurs = numEnemyDinosaurs;
        //     }
        }
    }
}