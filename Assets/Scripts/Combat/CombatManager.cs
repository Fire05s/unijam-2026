using System.Collections.Generic;
using System.Collections;
using BeardedPlatypus.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Based on the given creatures, order them, and allow them to take their turns.
/// </summary>
public class CombatManager : MonoBehaviour
{
    //Public Reference
    public static CombatManager Instance {get; private set;}

    //Necessary Events
    public delegate void IntDelegate(int value);
    public event IntDelegate TurnAdvanced;

    //Dinosaur Storage
    [SerializeField] private List<BattleEntity> Dinosaurs =  new List<BattleEntity>();
    public int numPlayerDinosaurs;
    public int numEnemyDinosaurs;
    [SerializeField] public int remainingPlayerDinosaurs;
    [SerializeField] public int remainingEnemyDinosaurs;
    public List<int> PlayerDinosaurIndicies = new List<int>();
    public List<int> EnemyDinosaurIndicies = new List<int>();

    //Delay floats (for animations and pacing)
    [SerializeField] private float EmptyTurnDelay;
    [SerializeField] private float AttackDelay;
    [SerializeField] private float InterTurnDelay;

    //Turn Combat Data
    [SerializeField] private int currentTurnNumber;
    [SerializeField] private List<int> NonEmptyTurns = new List<int>();
    [SerializeField] private PriorityQueue<int, int> MoveOrderQueue = new PriorityQueue<int, int>();

    //Miscellaneous Values
    [SerializeField] public int currentActingNum;
    [SerializeField] public int targetedDinosaur;
    public enum TurnPhase {None, EmptyCheck, AwaitSelect, SelectPhase, AwaitAttack, Attack, AwaitEnd, EndPhase, Victory, Lose}
    [SerializeField] public TurnPhase state;

    void Awake()
    {
        if (!Instance && Instance!=this) { Instance = this; }
        else { Destroy(gameObject); }

        Setup();

        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 8, atk:10,cc:25));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 8));
        Dinosaurs.Add(new BattleEntity(EntitySide.Enemy, sp: 8,cc:90));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 8));
        Dinosaurs.Add(new BattleEntity(EntitySide.Enemy, sp: 7,atk:5));
        Dinosaurs.Add(new BattleEntity(EntitySide.Enemy, sp: 4));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 4,atk:1));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 4,atk:5,cc:50));
        Dinosaurs.Add(new BattleEntity(EntitySide.Enemy, sp: 2,cc:20));
        Dinosaurs.Add(new BattleEntity(EntitySide.Enemy, sp: 1));

        BuildInitialQueue();
    }
    void Start()
    {
        TurnAdvanced?.Invoke(currentTurnNumber);
        
        // Debug.Log("Queue:");
        // for(int i=0; i<10; i++)
        // {
        //     int dino = PopFromQueue(i);
        //     Debug.Log($"Side: {Dinosaurs[dino].side} | Speed: {Dinosaurs[dino].GetSpeed()}");
        // }
    }
    void Setup()
    {
        currentTurnNumber = 0;
        targetedDinosaur = -1;
        state = TurnPhase.None;
    }
    void BuildInitialQueue()
    {
        int currentSlotNum = 0;
        for (int i=10; i>0; i--)
        {
            //Build list of Dinosaurs with equivalent speed
            List<int> SameSpeedP = new List<int>();
            foreach (BattleEntity entity in Dinosaurs)
            {
                if (entity.side==EntitySide.Player && entity.GetSpeed()==i) {SameSpeedP.Add(Dinosaurs.IndexOf(entity));}
            }
            List<int> SameSpeedE = new List<int>();
            foreach (BattleEntity entity in Dinosaurs)
            {
                if (entity.side==EntitySide.Enemy && entity.GetSpeed()==i) {SameSpeedE.Add(Dinosaurs.IndexOf(entity));}
            }

            //If there are no dinosaurs with this speed, move to next speed level
            if (SameSpeedP.Count==0 && SameSpeedE.Count==0) {continue;}

            //As long as there are dinos in both lists, add them while alternating
            while(SameSpeedP.Count > 0 && SameSpeedE.Count > 0)
            {
                int randomNum = UnityEngine.Random.Range(0, SameSpeedP.Count);
                int dino = SameSpeedP[randomNum];
                SameSpeedP.RemoveAt(randomNum);
                AddToQueue(Dinosaurs[dino].NextTurn(0), dino);

                randomNum = UnityEngine.Random.Range(0, SameSpeedE.Count);
                dino = SameSpeedE[randomNum];
                SameSpeedE.RemoveAt(randomNum);
                AddToQueue(Dinosaurs[dino].NextTurn(0), dino);
            }

            //Flush remaining dinosaurs into the queue
            foreach(int dino in SameSpeedP)
            {
                AddToQueue(Dinosaurs[dino].NextTurn(0), dino);
            }
            foreach(int dino in SameSpeedE)
            {
                AddToQueue(Dinosaurs[dino].NextTurn(0), dino);
            }

            //Repeat
        }

        for(int i=0; i<Dinosaurs.Count; i++)
        {
            BattleEntity dinosaur = Dinosaurs[i];
            if (dinosaur.side == EntitySide.Player) {
                numPlayerDinosaurs++;
                PlayerDinosaurIndicies.Add(i);
            }
            else {
                numEnemyDinosaurs++;
                EnemyDinosaurIndicies.Add(i);
            }
            remainingPlayerDinosaurs = numPlayerDinosaurs;
            remainingEnemyDinosaurs = numEnemyDinosaurs;
        }
    }
    void Update()
    {
        if (state == TurnPhase.None)
        {
            state = TurnPhase.EmptyCheck;
            HandleEmptyTurnChecks();
        }
        else if (state == TurnPhase.AwaitSelect)
        {
            state = TurnPhase.SelectPhase;
            HandleTargetSelection();
        }
        else if (state == TurnPhase.SelectPhase && targetedDinosaur!=-1)
        {
            state = TurnPhase.AwaitAttack;
        }
        else if (state == TurnPhase.AwaitAttack)
        {
            state = TurnPhase.Attack;
            StartCoroutine(HandleAttack());
        }
        else if (state == TurnPhase.AwaitEnd)
        {
            StartCoroutine(HandleEndTurn());
        }
    }
    
    int PopFromQueue(int turn)
    {
        int dinoNum = MoveOrderQueue.Dequeue();
        NonEmptyTurns.Remove(turn);
        return dinoNum;
    }
    void AddToQueue(int turn, int dinoNum)
    {
        while(NonEmptyTurns.Contains(turn))
        {
            turn++;
        }

        NonEmptyTurns.Add(turn);
        MoveOrderQueue.Enqueue(dinoNum, turn);
    }
    void ApplyDoT()
    {
        //toDo
    }
    void HandleWildCard(WildCard card)
    {
        //todo
    }
    void HandleEmptyTurnChecks()
    {
        // If the current turn number is not in the list, the slot must be empty
        if (!NonEmptyTurns.Contains(currentTurnNumber))
        {
            Debug.Log("not in nonemptyturns");
            StartCoroutine(EmptyTurn());
            return;
        } 
        // If the current acting dinosaur is already dead, the slot must be empty
        // The dead dinosaur is not readded to the queue
        currentActingNum = PopFromQueue(currentTurnNumber);
        if (!Dinosaurs[currentActingNum].IsAlive())
        {
            Debug.Log("dinosaur already dead");
            StartCoroutine(EmptyTurn());
            return;
        }
        state = TurnPhase.AwaitSelect;
    }
    IEnumerator EmptyTurn()
    {
        ApplyDoT();
        yield return new WaitForSeconds(EmptyTurnDelay);
        state = TurnPhase.AwaitEnd;
    }
    void HandleTargetSelection()
    {
        if (Dinosaurs[currentActingNum].side == EntitySide.Enemy)
        {
            Debug.Log("Random target");
            targetedDinosaur = PlayerDinosaurIndicies[UnityEngine.Random.Range(0, numPlayerDinosaurs)];
        }
    }
    IEnumerator HandleAttack()
    {
        (float,bool) result = Dinosaurs[currentActingNum].CalculateDamage();
        Dinosaurs[targetedDinosaur].DealDamage(result.Item1);
        Debug.Log($"{currentActingNum} dealt {result.Item1} damage to {targetedDinosaur}.");
        if (!Dinosaurs[targetedDinosaur].IsAlive())
        {
            Debug.Log($"{targetedDinosaur} ran out of HP and died.");
            if (Dinosaurs[targetedDinosaur].side == EntitySide.Player)
            {
                remainingPlayerDinosaurs--;
                PlayerDinosaurIndicies.Remove(targetedDinosaur);
            }
            else
            {
                remainingEnemyDinosaurs--;
                EnemyDinosaurIndicies.Remove(targetedDinosaur);
            }
        }
        HandleWildCard(Dinosaurs[targetedDinosaur].GetWildCard());
        yield return new WaitForSeconds(AttackDelay);
        Debug.Log($"{currentActingNum} with speed of {Dinosaurs[currentActingNum].GetSpeed()} will move on or after turn {Dinosaurs[currentActingNum].NextTurn(currentTurnNumber)}");
        AddToQueue(Dinosaurs[currentActingNum].NextTurn(currentTurnNumber), currentActingNum);
        targetedDinosaur = -1;
        state = TurnPhase.AwaitEnd;
    }
    IEnumerator HandleEndTurn()
    {
        state = TurnPhase.EndPhase;
        if (remainingEnemyDinosaurs<=0)
        {
            state = TurnPhase.Victory;
            //handle win
        }
        else if (remainingPlayerDinosaurs<=0)
        {
            state = TurnPhase.Lose;
            //handle loss
        }
        else {
            yield return new WaitForSeconds(InterTurnDelay);
            state = TurnPhase.None;
            currentTurnNumber++;
            currentActingNum = -1;
            TurnAdvanced?.Invoke(currentTurnNumber);
        }
    }
}
