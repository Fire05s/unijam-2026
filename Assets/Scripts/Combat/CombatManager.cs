using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace Combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance {get; private set;}
        public delegate void IntDelegate(int value);
        public event IntDelegate TurnAdvanced;
        public event Action<int> DinoDamaged;
        public event Action<int> DinoHealed;
        public event Action<int> DinoDodged;
        public event Action<int> DinoDied;
        public event Action<List<int>> DOTApplied;
        public event Action<int, int> AttackPerformed;

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
        public TurnQueue TurnQueue => MoveOrderQueue;
        

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
                    curDinosaur.GetCurrentHealth(), curDinosaur.GetAdjustedStat(StatType.Speed),
                    curDinosaur.GetAdjustedStat(StatType.Attack), curDinosaur.GetAdjustedStat(StatType.CritChance),
                    curDinosaur.GetWildCardAbilities());
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
                    curDinosaur.GetAdjustedStat(StatType.Health), curDinosaur.GetAdjustedStat(StatType.Speed),
                    curDinosaur.GetAdjustedStat(StatType.Attack), curDinosaur.GetAdjustedStat(StatType.CritChance),
                    curDinosaur.GetWildCardAbilities());
            }

            Debug.Log($"total dinosaurs {Dinosaurs.Count}");
            Debug.Log($"player dinosaurs list {RemainingPlayerDinosaurs.Count}");
            Debug.Log($"enemy dinosaurs list {RemainingEnemyDinosaurs.Count}");
            
            CombatSceneManager.Instance.SetupCombatScene(PlayerInventory.Instance.Creatures, enemyDinosData);
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
        /// <summary>
        /// Tell the Combat Manager to intiate combat. Run this after all combat entities are setup and the intial queue is created.
        /// </summary>
        public void TriggerCombatStart()
        {
            TurnAdvanced?.Invoke(currentTurnNumber);
            state = TurnStep.TurnStart;
        }
        /// <summary>
        /// Main update loop. Checks for transition steps and shifts the manager to the next phase.
        /// </summary>
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
        /// <summary>
        /// Calls DoT on every dinosaur on field. Stores a list of affected dinosaurs.
        /// </summary>
        /// <returns></returns>
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
            DOTApplied?.Invoke(TakenDamage);
            yield return new WaitForSeconds(DoTDelay);
            state = TurnStep.AwaitEmptyCheck;
        }
        /// <summary>
        /// Checks if the current turn is empty. If the turn does not appear in the list, skip to end phase.
        /// Also checks if the current dinosaur has already died.
        /// </summary>
        void HandleEmptyTurnChecks()
        {
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
        /// <summary>
        /// Stalls the game during an empty turn. Sends the turn system to the end phase.
        /// </summary>
        /// <returns></returns>
        IEnumerator EmptyTurn()
        {
            yield return new WaitForSeconds(EmptyTurnDelay);
            state = TurnStep.AwaitEnd;
        }
        /// <summary>
        /// Selects target for attack. If an enemy is moving, picks a random target. If a player is moving, returns control to Unity.
        /// </summary>
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
                CombatSceneManager.Instance.StartTargetSelection();
            }
        }
        /// <summary>
        /// Double checks the targetedDinosaur ID is within expected values. Does not verify if the ID is absolutely valid. This is only a sanity checker.
        /// </summary>
        /// <exception cref="Exception"></exception>
        void VerifyTarget()
        {
            if (targetedDinosaur <= 4 || targetedDinosaur >= 15) {throw new Exception("Error in Target Selection: id number out of bounds");}
            else {state = TurnStep.AwaitPlayerAttack;}
        }
        /// <summary>
        /// Pulls attack data from the acting dinosaur and applies it to the target. Waits the specified delay. Will readd the dinosaur to the queue if flags are met.
        /// </summary>
        /// <returns></returns>
        IEnumerator HandleAttack()
        {
            (float,bool) result = Dinosaurs[currentActingNum].CalculateAttack();
            thisMoveAttack = result.Item1;
            thisMoveCrit = result.Item2;
            ProcessDamage(targetedDinosaur, thisMoveAttack, true);
            Debug.Log($"{currentActingNum} has {Dinosaurs[currentActingNum]._health} out of {Dinosaurs[currentActingNum]._maxHealth} health");
            Debug.Log($"{currentActingNum} dealt {thisMoveAttack} damage to {targetedDinosaur}.");
            AttackPerformed?.Invoke(targetedDinosaur, currentActingNum);
            yield return new WaitForSeconds(AttackDelay);
            Debug.Log($"{currentActingNum} with speed of {Dinosaurs[currentActingNum]._speed} will move on or after turn {Dinosaurs[currentActingNum].CalculateNextTurn(currentTurnNumber)}");
            
            if (currentMoveData.addToQueue) {AddToQueue(Dinosaurs[currentActingNum].CalculateNextTurn(currentTurnNumber), currentActingNum);}
            state = TurnStep.AwaitWildCard;
        }
        /// <summary>
        /// Handles logic for wildcards. Relies on flags set by earlier steps as well as TurnData.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        IEnumerator HandleWildCard()
        {
            bool repeat = false;
            for (int i=0; i<Dinosaurs[currentActingNum]._wildcards.Count; i++)
            {
                switch(Dinosaurs[currentActingNum]._wildcards[i])
                {
                    case WildCard.Multihit:
                        int left = targetedDinosaur - 1;
                        if (RemainingPlayerDinosaurs.Contains(left) || RemainingEnemyDinosaurs.Contains(left)) { ProcessDamage(left, thisMoveAttack, true); }
                        int right = targetedDinosaur + 1;
                        if (RemainingPlayerDinosaurs.Contains(right) || RemainingEnemyDinosaurs.Contains(right)) { ProcessDamage(right, thisMoveAttack, true); }
                        break;
                    case WildCard.Bleed:
                        Dinosaurs[targetedDinosaur].ApplyDoT(DoT.Bleed);
                        break;
                    case WildCard.Doublehit:
                        if (!currentMoveData.canAttackAgain) {break;}
                        currentMoveData.addToQueue = false;
                        currentMoveData.canAttackAgain = false;
                        repeat = true;
                        break;
                    case WildCard.Ravenousbite:
                        ProcessHeal(currentActingNum, thisMoveAttack * 0.25f);
                        break;
                    case WildCard.Luckystreak:
                        if (thisMoveCrit && Dinosaurs[targetedDinosaur].IsAlive()) 
                        { 
                            (float,bool) result = Dinosaurs[currentActingNum].CalculateAttack();
                            thisMoveAttack = result.Item1;
                            thisMoveCrit = result.Item2;
                            ProcessDamage(targetedDinosaur, thisMoveAttack, true);
                            AttackPerformed?.Invoke(targetedDinosaur, currentActingNum);
                            Debug.Log("lucky streak attack again");
                            yield return new WaitForSeconds(AttackDelay);
                        }
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
                            ProcessHeal(currentActingNum, Dinosaurs[currentActingNum]._maxHealth * 0.25f);
                        }
                        break;
                    case WildCard.Packtreats:
                        float lowestHP = float.MaxValue;
                        int lowestHPID = -1;
                        if (Dinosaurs[currentActingNum]._side==EntitySide.Player)
                        {
                            foreach (int id in RemainingPlayerDinosaurs)
                            {
                                if (Dinosaurs[id]._health < lowestHP)
                                {
                                    lowestHPID = id;
                                }
                            }
                        }
                        else
                        {
                            foreach (int id in RemainingEnemyDinosaurs)
                            {
                                if (Dinosaurs[id]._health < lowestHP)
                                {
                                    lowestHPID = id;
                                }
                            }
                        }
                        if (lowestHPID==-1) {throw new Exception("Error in WildCards: Packtreats did not find any dinosaurs");}
                        ProcessHeal(lowestHPID, thisMoveAttack * .2f);
                        break;
                }
            }
            yield return new WaitForSeconds(WildCardDelay);
            targetedDinosaur = -1;
            if (repeat) {
                ProcessDeath();
                state = TurnStep.AwaitSelect;
            }
            else {
                state = TurnStep.AwaitEnd;
            }
        }
        /// <summary>
        /// Checks for win conditions and resets the flags for next turn.
        /// </summary>
        /// <returns></returns>
        IEnumerator EndTurn()
        {
            state = TurnStep.TurnEnd;
            ProcessDeath();
            if (state != TurnStep.CombatVictory && state != TurnStep.CombatLose) {
                yield return new WaitForSeconds(InterTurnDelay);
                state = TurnStep.TurnStart;
                currentTurnNumber++;
                currentActingNum = -1;
                TurnAdvanced?.Invoke(currentTurnNumber);
            }
        }
        /// <summary>
        /// Sanity check for any dead dinosaurs
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void ProcessDeath()
        {
            foreach (int id in Dinosaurs.Keys)
            {
                if (!Dinosaurs[id].IsAlive() &&
                    (RemainingPlayerDinosaurs.Contains(id) || RemainingEnemyDinosaurs.Contains(id)))
                {
                    CombatSceneManager.Instance.UpdateSceneAfterDeath(id);
                    Debug.Log($"{id} ran out of HP and died.");
                    
                    if (RemainingPlayerDinosaurs.Contains(id))
                    {
                        RemainingPlayerDinosaurs.Remove(id);
                    }
                    else if (RemainingEnemyDinosaurs.Contains(id))
                    {
                        RemainingEnemyDinosaurs.Remove(id);
                    }

                    DinoDied?.Invoke(id);
                }
            }

            if (RemainingEnemyDinosaurs.Count<=0)
            {
                state = TurnStep.CombatVictory;
                int dinoId = 0;
                foreach (DinosaurData dino in PlayerInventory.Instance.Creatures)
                {
                    if (RemainingPlayerDinosaurs.Contains(dinoId))
                    {
                        dino.SetCurrentHealth(Dinosaurs[dinoId]._health);
                    }
                    else
                    {
                        dino.SetCurrentHealth(0f);
                    }
                    dinoId++;
                }
                StopAllCoroutines();
                BattleDataLoader.Instance.TriggerVictory();
            }
            else if (RemainingPlayerDinosaurs.Count<=0)
            {
                state = TurnStep.CombatLose;
                foreach (DinosaurData playerDino in PlayerInventory.Instance.Creatures)
                {
                    playerDino.HealDino(playerDino.GetAdjustedStat(StatType.Health));
                }
                StopAllCoroutines();
                BattleDataLoader.Instance.TriggerDefeat();
            }
        }
        /// <summary>
        /// Adds a dinosaur to the queue.
        /// </summary>
        /// <param name="turn">Minimum next turn</param>
        /// <param name="dinoID">ID</param>
        /// <exception cref="Exception"></exception>
        private void AddToQueue(int turn, int dinoID)
        {
            int actualSlot = MoveOrderQueue.Enqueue(turn, dinoID, Dinosaurs[dinoID]._wildcards);
            if (NonEmptyTurns.Contains(actualSlot)) {throw new Exception("Error in Move Queue: duplicate turn hash found");}
            NonEmptyTurns.Add(actualSlot);
        }
        /// <summary>
        /// Pops the next item from the queue
        /// </summary>
        /// <param name="turn">Current turn number</param>
        /// <returns>ID of next dinosaur</returns>
        /// <exception cref="Exception"></exception>
        private int RemoveFromQueue(int turn)
        {
            (int, TurnData) deq = MoveOrderQueue.Dequeue();
            if (deq.Item1 != currentTurnNumber) {throw new Exception("Error in Move Queue: dequeued turn out of alignment");}
            if (deq.Item1 != turn) {throw new Exception("Error in Move Queue: dequeued turn out of alignment");}
            NonEmptyTurns.Remove(currentTurnNumber);
            currentMoveData = deq.Item2;
            return currentMoveData.dinoID;
        }

        /// <summary>
        /// Wraps the apply damage function with an event call
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="damage"></param>
        private void ProcessDamage(int targetId, float damage, bool canDodge=false)
        {
            if (canDodge) {
                foreach (WildCard card in Dinosaurs[targetId]._wildcards)
                {
                    if (card == WildCard.Dodge && UnityEngine.Random.Range(0, 10) < 3)
                    {
                        DinoDodged?.Invoke(targetId);
                        return;
                    }
                }
            }
            Dinosaurs[targetId].ApplyDamage(damage);
            DinoDamaged?.Invoke(targetId);
        }

        /// <summary>
        /// Wraps the heal function with an event 
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="heal"></param>
        private void ProcessHeal(int targetId, float heal)
        {
            Dinosaurs[targetId].Heal(heal);
            DinoHealed?.Invoke(targetId);
        }
    }
}