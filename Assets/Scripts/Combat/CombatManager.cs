using System.Collections.Generic;
using BeardedPlatypus.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Based on the given creatures, order them, and allow them to take their turns.
/// </summary>
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance {get; private set;}

    private List<BattleEntity> Dinosaurs =  new List<BattleEntity>();
    private int turnNumber;

    private HashSet<int> NonEmptyTurns = new HashSet<int>();
    private PriorityQueue<int, int> MoveOrderQueue = new PriorityQueue<int, int>();

    public bool canSelectDinosaur;
    public int selectedDinosaur;

    void Awake()
    {
        if (!Instance && Instance!=this) { Instance = this; }
        else { Destroy(gameObject); }

        Setup();
    }

    void Start()
    {
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 8));
        Dinosaurs.Add(new BattleEntity(EntitySide.Enemy, sp: 8));
        Dinosaurs.Add(new BattleEntity(EntitySide.Enemy, sp: 7));
        Dinosaurs.Add(new BattleEntity(EntitySide.Enemy, sp: 4));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 2));
        Dinosaurs.Add(new BattleEntity(EntitySide.Player, sp: 1));

        BuildInitialQueue();
        
        Debug.Log(MoveOrderQueue);
    }
    void Setup()
    {
        turnNumber = 0;
        canSelectDinosaur = false;
        selectedDinosaur = -1;
        //create list of playerdinosaurs and enemydinosaurs
    }

    void BuildInitialQueue()
    {
        int currentSlotNum = 0;
        for (int i=10; i>0; i--)
        {
            //Build list of Dinosaurs with equivalent speed
            List<int> SameSpeedP = new List<int>();
            foreach (BattleEntity entity in PlayerDinosaurs)
            {
                if (entity.side==EntitySide.Player && entity.GetSpeed()==i) {SameSpeedP.Add(Dinosaurs.IndexOf(entity));}
            }
            List<int> SameSpeedE = new List<int>();
            foreach (BattleEntity entity in EnemyDinosaurs)
            {
                if (entity.side==EntitySide.Enemy && entity.GetSpeed()==i) {SameSpeedE.Add(Dinosaurs.IndexOf(entity));}
            }

            //If there are no dinosaurs with this speed, move to next speed level
            if (SameSpeedP.Count==0 && SameSpeedE.Count==0) {continue;}

            //As long as there are dinos in both lists, add them while alternating
            while(SameSpeedP.Count > 0 && SameSpeedE.Count > 0)
            {
                int randomNum = Random.Range(0, SameSpeedP.Count);
                int dino = SameSpeedP[randomNum];
                SameSpeedP.RemoveAt(randomNum);
                AddToQueue(Dinosaurs[dino].NextTurn(0), dino);

                randomNum = Random.Range(0, SameSpeedE.Count);
                int dino = SameSpeedE[randomNum];
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
    void TurnLogic()
    {
        //If the slot is empty, apply DoT effect
        if (!NonEmptyTurns.Contains(turnNumber))
        {
            ApplyDoT();
            turnNumber++;
            return;
        }

        int dinoNum = PopFromQueue(turnNumber);
        if (!Dinosaurs[dinoNum].IsAlive())
        {
            ApplyDoT();
            turnNumber++;
            return;
        }

        canSelectDinosaurs = true;
        while(selectedDinosaur==-1)
        {
            //some logic here- to be decided
        }
        canSelectDinosaurs = false;

        float dmg = Dinosaurs[dinoNum].CalculateDamage();

        Dinosaurs[selectedDinosaur].DealDamage(dmg);

        AddToQueue(Dinosaurs[dinoNum].NextTurn(turnNumber), dinoNum);

        turnNumber++;
    }
    int PopFromQueue(int turn)
    {
        dinoNum = MoveOrderQueue.Dequeue();
        NonEmptyTurns.Remove(turn);
        return dinoNum;
    }
    void AddToQueue(int turn, int dinoNum)
    {
        while(NonEmptyTurns.Contains(turn))
        {
            turn++;
        }

        NonEmptyQueue.Add(turn);
        MoveOrderQueue.Enqueue(dinoNum, turn);
    }
    void ApplyDoT()
    {
        //toDo
    }
    
    
}
