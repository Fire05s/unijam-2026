using UnityEngine;
using System;
using System.Collections;
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
        [SerializeField] private float WildCardDelay;


        [Header("Alive Dinosaur Lists")]
        public List<int> RemainingPlayerDinosaurs = new List<int>();
        public List<int> RemainingEnemyDinosaurs = new List<int>();


        [Header("Runtime Information")]
        [SerializeField] private int currentTurnNumber;
        [SerializeField] private List<int> NonEmptyTurns = new List<int>();
        [SerializeField] private TurnQueue MoveOrderQueue = new TurnQueue();


        [Header("Current Turn Information"), SerializeField] public TurnStep state;
        [SerializeField] public int currentActingNum;
        private TurnData currentMoveData;
        [SerializeField] public int targetedDinosaur;
        private bool thisMoveCrit;
        private float thisMoveAttack;
        

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
        public void SetupCombat(List<DinosaurData> enemyDinosData)
        {
            currentTurnNumber = 0;
            targetedDinosaur = -1;
            state = TurnStep.None;
            thisMoveCrit = false;

            Debug.Log($"player dinos {PlayerInventory.Instance.Creatures.Count}");
            Debug.Log($"enemy dinos {enemyDinosData.Count}");

            if (PlayerInventory.Instance.Creatures.Count > 5) {throw new Exception("Error in CombatManager: CombatSetup - Player Dinosaur Count Has Exceeded Limit");}
            // player dino id's go from 0-4
            for (int id = 0; id < PlayerInventory.Instance.Creatures.Count; id++)
            {
                // building the IDs for all the player dinos, adding them to the alive player dinos list,
                // and creating their combat entity
                Debug.Log("Adding a player dino");
                DinosaurData curDinosaur = PlayerInventory.Instance.Creatures[id];
                RemainingPlayerDinosaurs.Add(id);
                Dinosaurs[id] = new CombatEntity(id, EntitySide.Player, curDinosaur.GetAdjustedStat(StatType.Health), 
                    curDinosaur.GetAdjustedStat(StatType.Speed), curDinosaur.GetAdjustedStat(StatType.Attack),
                    curDinosaur.GetAdjustedStat(StatType.CritChance), curDinosaur.GetWildCardAbilities());
            }

            // enemy dino id's go from 5-14
            for (int id = 5; id < enemyDinosData.Count + 5; id++)
            {
                // building the IDs for all the enemy dinos, adding them to the alive enemy dinos list,
                // and creating their combat entity
                Debug.Log("Adding an enemy dino");
                DinosaurData curDinosaur = enemyDinosData[id - 5];
                RemainingEnemyDinosaurs.Add(id);
                Dinosaurs[id] = new CombatEntity(id, EntitySide.Enemy, curDinosaur.GetAdjustedStat(StatType.Health), 
                    curDinosaur.GetAdjustedStat(StatType.Speed), curDinosaur.GetAdjustedStat(StatType.Attack),
                    curDinosaur.GetAdjustedStat(StatType.CritChance), curDinosaur.GetWildCardAbilities());
            }

            Debug.Log($"total dinosaurs {Dinosaurs.Count}");
            Debug.Log($"player dinosaurs list {RemainingPlayerDinosaurs.Count}");
            Debug.Log($"enemy dinosaurs list {RemainingEnemyDinosaurs.Count}");

            BuildInitialQueue();
        }
        private void BuildInitialQueue()
        {
            for (int i=10; i>0; i--)
            {
                //Build list of Dinosaurs with equivalent speed
                List<int> SameSpeedP = new List<int>();
                List<int> SameSpeedE = new List<int>();
                foreach (CombatEntity entity in Dinosaurs.Values)
                {
                    if (entity._side==EntitySide.Player && entity._speed==i) {SameSpeedP.Add(entity._id);}
                    else if (entity._side==EntitySide.Enemy && entity._speed==i) {SameSpeedE.Add(entity._id);}
                }

                //If there are no dinosaurs with this speed, move to next speed level
                if (SameSpeedP.Count==0 && SameSpeedE.Count==0) {continue;}

                //As long as there are dinos in both lists, add them while alternating
                while(SameSpeedP.Count > 0 && SameSpeedE.Count > 0)
                {
                    int randomNum = UnityEngine.Random.Range(0, SameSpeedP.Count);
                    int dino = SameSpeedP[randomNum];
                    SameSpeedP.RemoveAt(randomNum);
                    AddToQueue(Dinosaurs[dino].CalculateNextTurn(0), dino);

                    randomNum = UnityEngine.Random.Range(0, SameSpeedE.Count);
                    dino = SameSpeedE[randomNum];
                    SameSpeedE.RemoveAt(randomNum);
                    AddToQueue(Dinosaurs[dino].CalculateNextTurn(0), dino);
                }

                //Flush remaining dinosaurs into the queue
                foreach(int dino in SameSpeedP)
                {
                    AddToQueue(Dinosaurs[dino].CalculateNextTurn(0), dino);
                }
                foreach(int dino in SameSpeedE)
                {
                    AddToQueue(Dinosaurs[dino].CalculateNextTurn(0), dino);
                }

                //Repeat
            }
        }
        public void TriggerCombatStart()
        {
            TurnAdvanced?.Invoke(currentTurnNumber);
            state = TurnStep.TurnStart;
        }
        void Update()
        {
            if (state == TurnStep.TurnStart)
            {
                state = TurnStep.ApplyDoT;
                StartCoroutine(HandleDoT());
            }
            else if (state == TurnStep.AwaitEmptyCheck)
            {
                state = TurnStep.EmptyCheck;
                HandleEmptyTurnChecks();
            }
            else if (state == TurnStep.AwaitSelect)
            {
                HandleTargetSelection();
            }
            else if (state == TurnStep.PlayerSelect && targetedDinosaur != -1)
            {
                VerifyTarget();
            }
            else if (state == TurnStep.AwaitPlayerAttack)
            {
                state = TurnStep.PlayerAttack;
                StartCoroutine(HandleAttack());
            }
            else if (state == TurnStep.AwaitEnemyAttack)
            {
                state = TurnStep.EnemyAttack;
                StartCoroutine(HandleAttack());
            }
            else if (state == TurnStep.AwaitWildCard)
            {
                state = TurnStep.WildCardActivity;
                StartCoroutine(HandleWildCard());
            }
            else if (state == TurnStep.AwaitEnd)
            {
                StartCoroutine(EndTurn());
            }
        }
        IEnumerator HandleDoT()
        {
            //Tick DoT for all dinosaurs on field, store list of affected dinosaurs for UX
            List<int> TakenDamage = new();
            foreach(int id in RemainingPlayerDinosaurs)
            {
                if (Dinosaurs[id].TickDoT()) {TakenDamage.Add(id);}
            }
            foreach(int id in RemainingEnemyDinosaurs)
            {
                if (Dinosaurs[id].TickDoT()) {TakenDamage.Add(id);}
            }
            ProcessDeath();
            yield return new WaitForSeconds(DoTDelay);
            state = TurnStep.AwaitEmptyCheck;
        }
        void HandleEmptyTurnChecks()
        {
            //If the turn is not in the set, it is an empty slot
            if (!NonEmptyTurns.Contains(currentTurnNumber))
            {
                StartCoroutine(EmptyTurn());
                return;
            }
            // If the current acting dinosaur is already dead, the slot must be empty
            // The dead dinosaur is not readded to the queue
            currentActingNum = RemoveFromQueue(currentTurnNumber);
            if (!Dinosaurs[currentActingNum].IsAlive())
            {
                Debug.Log("dinosaur already dead");
                StartCoroutine(EmptyTurn());
                return;
            }
            state = TurnStep.AwaitSelect;
        }
        IEnumerator EmptyTurn()
        {
            yield return new WaitForSeconds(EmptyTurnDelay);
            state = TurnStep.AwaitEnd;
        }
        void HandleTargetSelection()
        {
            if (Dinosaurs[currentActingNum]._side == EntitySide.Enemy)
            {
                state = TurnStep.EnemySelect;
                Debug.Log("Random target");
                targetedDinosaur = RemainingPlayerDinosaurs[UnityEngine.Random.Range(0, RemainingPlayerDinosaurs.Count)];
                state = TurnStep.AwaitEnemyAttack;
            } else
            {
                state = TurnStep.PlayerSelect;
            }
        }
        void VerifyTarget()
        {
            if (targetedDinosaur < 4 && targetedDinosaur > 15) {throw new Exception("Error in Target Selection: id number out of bounds");}
            else {state = TurnStep.AwaitPlayerAttack;}
        }
        IEnumerator HandleAttack()
        {
            (float,bool) result = Dinosaurs[currentActingNum].CalculateAttack();
            thisMoveAttack = result.Item1;
            thisMoveCrit = result.Item2;
            Dinosaurs[targetedDinosaur].ApplyDamage(thisMoveAttack);
            Debug.Log($"{currentActingNum} dealt {thisMoveAttack} damage to {targetedDinosaur}.");
            yield return new WaitForSeconds(AttackDelay);
            Debug.Log($"{currentActingNum} with speed of {Dinosaurs[currentActingNum]._speed} will move on or after turn {Dinosaurs[currentActingNum].CalculateNextTurn(currentTurnNumber)}");
            
            if (currentMoveData.addToQueue) {AddToQueue(Dinosaurs[currentActingNum].CalculateNextTurn(currentTurnNumber), currentActingNum);}
            state = TurnStep.AwaitWildCard;
        }
        IEnumerator HandleWildCard()
        {
            bool repeat = false;
            for (int i=0; i<Dinosaurs[currentActingNum]._wildcards.Count; i++)
            {
                switch(Dinosaurs[currentActingNum]._wildcards[i])
                {
                    case WildCard.Multihit:
                        int left = currentActingNum - 1;
                        if (RemainingPlayerDinosaurs.Contains(left) || RemainingEnemyDinosaurs.Contains(left)) {Dinosaurs[left].ApplyDamage(thisMoveAttack);}
                        int right = currentActingNum - 1;
                        if (RemainingPlayerDinosaurs.Contains(right) || RemainingEnemyDinosaurs.Contains(right)) {Dinosaurs[right].ApplyDamage(thisMoveAttack);}
                        break;
                    case WildCard.Bleed:
                        Dinosaurs[targetedDinosaur].ApplyDoT(DoT.Bleed);
                        break;
                    case WildCard.Doublehit:
                        currentMoveData.addToQueue = false;
                        repeat = true;
                        break;
                    case WildCard.Ravenousbite:
                        Dinosaurs[currentActingNum].Heal(thisMoveAttack * 0.25f);
                        break;
                    case WildCard.Luckystreak:
                        if (thisMoveCrit) {Dinosaurs[targetedDinosaur].ApplyDamage(thisMoveAttack);}
                        break;
                    case WildCard.Bloodlust:
                        if (!Dinosaurs[targetedDinosaur].IsAlive())
                        {
                            currentMoveData.addToQueue = false;
                            repeat = true;
                        }
                        break;
                    case WildCard.Scavenger:
                        if (!Dinosaurs[targetedDinosaur].IsAlive())
                        {
                            Dinosaurs[currentActingNum].Heal(Dinosaurs[currentActingNum]._maxHealth * 0.25f);
                        }
                        break;
                    case WildCard.Packtreats:
                        float lowestHP = float.MaxValue;
                        int lowestHPID = -1;
                        foreach (int id in RemainingPlayerDinosaurs)
                        {
                            if (Dinosaurs[id]._health < lowestHP)
                            {
                                lowestHPID = id;
                            }
                        }
                        if (lowestHPID==-1) {throw new Exception("Error in WildCards: Packtreats did not find any dinosaurs");}
                        Dinosaurs[lowestHPID].Heal(thisMoveAttack * .2f);
                        break;
                }
            }
            yield return new WaitForSeconds(WildCardDelay);
            if (repeat) {ProcessDeath(); state = TurnStep.AwaitSelect;}
            else { state = TurnStep.AwaitEnd;}
        }
        IEnumerator EndTurn()
        {
            state = TurnStep.TurnEnd;
            ProcessDeath();
            if (RemainingEnemyDinosaurs.Count<=0)
            {
                state = TurnStep.CombatVictory;
                //handle win
            }
            else if (RemainingPlayerDinosaurs.Count<=0)
            {
                state = TurnStep.CombatLose;
                //handle loss
            }
            else {
                yield return new WaitForSeconds(InterTurnDelay);
                state = TurnStep.TurnStart;
                currentTurnNumber++;
                currentActingNum = -1;
                TurnAdvanced?.Invoke(currentTurnNumber);
            }
        }
        private void ProcessDeath()
        {
            foreach (int id in RemainingPlayerDinosaurs)
            {
                if (!Dinosaurs[id].IsAlive())
                {
                    Debug.Log($"{id} ran out of HP and died.");
                    RemainingPlayerDinosaurs.Remove(id);
                }
            }
            foreach (int id in RemainingEnemyDinosaurs)
            {
                if (!Dinosaurs[id].IsAlive())
                {
                    Debug.Log($"{id} ran out of HP and died.");
                    RemainingEnemyDinosaurs.Remove(id);
                }
            }
        }
        private void AddToQueue(int turn, int dinoID)
        {
            int actualSlot = MoveOrderQueue.Enqueue(turn, dinoID, Dinosaurs[dinoID]._wildcards);
            if (NonEmptyTurns.Contains(actualSlot)) {throw new Exception("Error in Move Queue: duplicate turn hash found");}
            NonEmptyTurns.Add(actualSlot);
        }
        private int RemoveFromQueue(int turn)
        {
            (int, TurnData) deq = MoveOrderQueue.Dequeue();
            if (deq.Item1 != currentTurnNumber) {throw new Exception("Error in Move Queue: dequeued turn out of alignment");}
            if (deq.Item1 != turn) {throw new Exception("Error in Move Queue: dequeued turn out of alignment");}
            NonEmptyTurns.Remove(currentTurnNumber);
            currentMoveData = deq.Item2;
            return currentMoveData.dinoID;
        }
    }
}