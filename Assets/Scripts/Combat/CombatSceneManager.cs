using System.Collections.Generic;
using Combat;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Changes elements within the combat scene depending on the Combat Manager 
/// </summary>
public class CombatSceneManager : MonoBehaviour
{
    public static CombatSceneManager Instance {get; private set;}
    [SerializeField] public List<Transform> playerPositions;
    [SerializeField] public List<Transform> enemyPositions;
    [SerializeField] private GameObject dinoPlaceHolderPrefab;

    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private int currentSelectedTarget = -1;
    [SerializeField] private int currentSelectedTargetIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this){ Destroy(Instance.gameObject); }
        Instance = this;
    }

    private void Update()
    {
        UpdateCinemachineCamera();
    }

    public void SetupCombatScene(IReadOnlyList<DinosaurData> playerDinosData, IReadOnlyList<DinosaurData> enemyDinosData)
    {
        if (playerDinosData.Count > playerPositions.Count){Debug.LogWarning("NOT ENOUGH POSITIONS TO HOLD PLAYER DINOS");}
        if (enemyDinosData.Count > enemyPositions.Count){Debug.LogWarning("NOT ENOUGH POSITIONS TO HOLD ENEMY DINOS");}

        for (int index=0; index < playerDinosData.Count; ++index)
        {
            GameObject dino = Instantiate(dinoPlaceHolderPrefab, playerPositions[index]);
            dino.GetComponent<ModelGenerator>().SetDinosaur(playerDinosData[index]);
        }
        for (int index=0; index < enemyDinosData.Count; ++index)
        {
            GameObject dino = Instantiate(dinoPlaceHolderPrefab, enemyPositions[index]);
            dino.GetComponent<ModelGenerator>().SetDinosaur(enemyDinosData[index]);
        }
    }

    public void StartTargetSelection()
    {
        if (CombatManager.Instance.RemainingEnemyDinosaurs.Count == 0) { currentSelectedTarget = -1; return; }
        currentSelectedTargetIndex = 0;
        currentSelectedTarget = CombatManager.Instance.RemainingEnemyDinosaurs[currentSelectedTargetIndex];
    }
    
    public void SelectTargetLeft()
    {
        if (currentSelectedTargetIndex == -1){return;}
        // Even index goes left (based on my implementation)
        if (currentSelectedTargetIndex % 2 == 0)
            currentSelectedTargetIndex = (currentSelectedTargetIndex + 2) % CombatManager.Instance.RemainingEnemyDinosaurs.Count;
        else
            currentSelectedTargetIndex = (currentSelectedTargetIndex + 1) % CombatManager.Instance.RemainingEnemyDinosaurs.Count;

        currentSelectedTarget = CombatManager.Instance.RemainingEnemyDinosaurs[currentSelectedTargetIndex];
    }

    public void SelectTargetRight()
    {
        if (currentSelectedTargetIndex == -1){return;}
        // Odd index goes right (based on my implementation)
        if (currentSelectedTargetIndex % 2 == 0)
            currentSelectedTargetIndex = (currentSelectedTargetIndex + 1) % CombatManager.Instance.RemainingEnemyDinosaurs.Count;
        else
            currentSelectedTargetIndex = (currentSelectedTargetIndex + 2) % CombatManager.Instance.RemainingEnemyDinosaurs.Count;

        currentSelectedTarget = CombatManager.Instance.RemainingEnemyDinosaurs[currentSelectedTargetIndex];
    }

    public void ConfirmSelectedTarget()
    {
        if (currentSelectedTarget == -1){ return; }
        CombatManager.Instance.targetedDinosaur = currentSelectedTarget;
        currentSelectedTarget = -1;
        currentSelectedTargetIndex = -1;
    }
    
    public void UpdateCinemachineCamera()
    {
        if (CombatManager.Instance.state == TurnStep.PlayerSelect)
        {
            // Player Dino ids correspond to its given index on the field
            cinemachineCamera.Follow = playerPositions[CombatManager.Instance.currentActingNum];
            cinemachineCamera.LookAt = enemyPositions[currentSelectedTargetIndex];
        }
    }
}
