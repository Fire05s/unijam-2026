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
    private List<BattleEntity> Dinosaurs =  new List<BattleEntity>();
    private int numPlayerDinosaurs;
    private int numEnemyDinosaurs;

    //Turn Combat Data
    [SerializeField] private int turnNumber;
    [SerializeField] private List<int> NonEmptyTurns = new List<int>();
    [SerializeField] private PriorityQueue<int, int> MoveOrderQueue = new PriorityQueue<int, int>();

    //Miscellaneous Values
    [SerializeField] public int currentActingNum;
    public bool canSelectDinosaur;
    [SerializeField] public int selectedDinosaur;
    private enum TurnPhase {Calculations, SelectTarget, Attack, None}
    [SerializeField] private TurnPhase state;
    [SerializeField] private bool awaitNext;

    void Awake()
    {
        if (!Instance && Instance!=this) { Instance = this; }
        else { Destroy(gameObject); }

        Setup();
    }
    void Start()
    {
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 8));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 8));
        Dinosaurs.Add(new BattleEntity(EntitySide.Enemy, sp: 8));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 8));
        Dinosaurs.Add(new BattleEntity(EntitySide.Enemy, sp: 7));
        Dinosaurs.Add(new BattleEntity(EntitySide.Enemy, sp: 4));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 4));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 4));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 2));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 1));

        BuildInitialQueue();

        TurnAdvanced?.Invoke(turnNumber);
        
        // Debug.Log("Queue:");
        // for(int i=0; i<10; i++)
        // {
        //     int dino = PopFromQueue(i);
        //     Debug.Log($"Side: {Dinosaurs[dino].side} | Speed: {Dinosaurs[dino].GetSpeed()}");
        // }

        state = TurnPhase.None;
        awaitNext = true;
    }
    void Setup()
    {
        turnNumber = 0;
        canSelectDinosaur = false;
        selectedDinosaur = -1;
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
    }
    void Update()
    {
        //Main driver of Turn Logic
        if (!awaitNext && state==TurnPhase.SelectTarget && canSelectDinosaur && selectedDinosaur!=-1)
        {
            canSelectDinosaur = false;
            awaitNext = true;
        }
        if (!awaitNext) {return;}

        if (state==TurnPhase.None)
        {
            Debug.Log("Entered Calculations Phase.");
            //If slot is empty or the dinosaur in the slot is dead, skip
            state = TurnPhase.Calculations;
            if (!NonEmptyTurns.Contains(turnNumber))
            {
                Debug.Log("not in nonemptyturns");
                Debug.Log(NonEmptyTurns);
                EmptyTurn();
                return;
            } 
            
            currentActingNum = PopFromQueue(turnNumber);
            if (!Dinosaurs[currentActingNum].IsAlive())
            {
                Debug.Log("dinosaur already dead");
                EmptyTurn();
                return;
            } 
        }
        else if (state==TurnPhase.Calculations)
        {
            Debug.Log("Select Target Phase");
            state = TurnPhase.SelectTarget;
            awaitNext = false;
            if (Dinosaurs[currentActingNum].side == EntitySide.Enemy)
            {
                selectedDinosaur = UnityEngine.Random.Range(0, numPlayerDinosaurs);
            } else
            {
                canSelectDinosaur = true;
            }
        }
        else if (state==TurnPhase.SelectTarget)
        {
            Debug.Log("Attack Phase");
            state = TurnPhase.Attack;
            StartCoroutine(Delay(3f));
            (float,bool) result = Dinosaurs[currentActingNum].CalculateDamage();
            Dinosaurs[selectedDinosaur].DealDamage(result.Item1);
            HandleWildCard(Dinosaurs[selectedDinosaur].GetWildCard());
            AddToQueue(Dinosaurs[currentActingNum].NextTurn(turnNumber), currentActingNum);
            selectedDinosaur = -1;
        }
        else if (state==TurnPhase.Attack)
        {
            Debug.Log("End of Turn Phase");
            state = TurnPhase.None;
            turnNumber++;
            currentActingNum = -1;
            TurnAdvanced?.Invoke(turnNumber);
        }
    }
    void EmptyTurn()
    {
        Debug.Log("Empty Turn");
        StartCoroutine(Delay(2f));
        ApplyDoT();
        turnNumber++;
        state = TurnPhase.None;
        TurnAdvanced?.Invoke(turnNumber);
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
    IEnumerator Delay(float delayLen) {awaitNext=false; yield return new WaitForSeconds(delayLen); awaitNext=true;}
    void HandleWildCard(WildCard card)
    {
        //todo
    }
    
}
