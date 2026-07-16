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
    [SerializeField] public Transform playerPositionHolder;
    [SerializeField] public Transform enemyPositionHolder;
    [SerializeField] private GameObject dinoPlaceHolderPrefab;
    private List<GameObject> playerDinos = new List<GameObject>();
    private List<GameObject> enemyDinos = new List<GameObject>();


    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private int currentSelectedTarget = -1;
    [SerializeField] private int currentSelectedTargetIndex = -1;  

    private CombatManager combatManager;  

    private void Awake()
    {
        if (Instance != null && Instance != this){ Destroy(Instance.gameObject); }
        Instance = this;
    }

    private void Start()
    {
        combatManager = CombatManager.Instance;
    }

    private void Update()
    {
        UpdateCinemachineCamera();
    }

    public void SetupCombatScene(IReadOnlyList<DinosaurData> playerDinosData, IReadOnlyList<DinosaurData> enemyDinosData)
    {
        int playerDinoPivotIndex =  playerDinosData.Count / 2;
        int enemyDinoPivotIndex = enemyDinosData.Count / 2;

        float distanceScaler = 3.0f;

        for (int index=0; index < playerDinosData.Count; ++index)
        {
            GameObject dino = Instantiate(dinoPlaceHolderPrefab, playerPositionHolder);
            dino.GetComponent<ModelGenerator>().SetDinosaur(playerDinosData[index]);

            Vector3 currentDinoPosition = dino.transform.localPosition;
            currentDinoPosition.x = (index - playerDinoPivotIndex) * distanceScaler;
            dino.transform.localPosition = currentDinoPosition;

            playerDinos.Add(dino);
        }
        for (int index=0; index < enemyDinosData.Count; ++index)
        {
            GameObject dino = Instantiate(dinoPlaceHolderPrefab, enemyPositionHolder);
            dino.GetComponent<ModelGenerator>().SetDinosaur(enemyDinosData[index]);

            Vector3 currentDinoPosition = dino.transform.localPosition;
            currentDinoPosition.x = (index - enemyDinoPivotIndex) * distanceScaler;
            currentDinoPosition.z = (index % 2 == 0) ? currentDinoPosition.z : distanceScaler;
            dino.transform.localPosition = currentDinoPosition;

            enemyDinos.Add(dino);
        }
    }

    public void UpdateSceneAfterDeath(int dinosaurID)
    {
        if (combatManager.RemainingPlayerDinosaurs.Contains(dinosaurID))
        {
            Destroy(playerDinos[dinosaurID]);
        }
        else if (combatManager.RemainingEnemyDinosaurs.Contains(dinosaurID))
        {
            Destroy(enemyDinos[dinosaurID - 5]);
        }
    }

    public void StartTargetSelection()
    {
        IReadOnlyList<int> enemyDinoList = combatManager.RemainingEnemyDinosaurs;

        if (enemyDinoList.Count == 0) { currentSelectedTarget = -1; return; }
        currentSelectedTargetIndex = enemyDinoList.Count / 2;
        currentSelectedTarget = enemyDinoList[currentSelectedTargetIndex];
    }
    
    public void SelectTargetLeft()
    {
        IReadOnlyList<int> enemyDinoList = combatManager.RemainingEnemyDinosaurs;

        if (currentSelectedTargetIndex <= 0){ 
            currentSelectedTargetIndex = enemyDinoList.Count - 1; 
        }
        else
        {
            currentSelectedTargetIndex -= 1;
        }

        currentSelectedTarget = enemyDinoList[currentSelectedTargetIndex];
    }

    public void SelectTargetRight()
    {
        IReadOnlyList<int> enemyDinoList = combatManager.RemainingEnemyDinosaurs;

        if (currentSelectedTargetIndex >= enemyDinoList.Count - 1){ 
            currentSelectedTargetIndex = 0; 
        }
        else
        {
            currentSelectedTargetIndex += 1;
        }

        currentSelectedTarget = enemyDinoList[currentSelectedTargetIndex];
    }

    public void ConfirmSelectedTarget()
    {
        if (currentSelectedTarget == -1){ return; }

        combatManager.targetedDinosaur = currentSelectedTarget;

        currentSelectedTarget = -1;
        currentSelectedTargetIndex = -1;
    }
    
    public void UpdateCinemachineCamera()
    {
        if (combatManager.state == TurnStep.PlayerSelect)
        {
            // Player Dino ids correspond to its given index on the field
            cinemachineCamera.Follow = playerDinos[combatManager.currentActingNum].transform;
            cinemachineCamera.LookAt = enemyDinos[currentSelectedTargetIndex].transform;
        }
        else if (combatManager.state == TurnStep.EnemyAttack)
        {
            cinemachineCamera.Follow = playerDinos[combatManager.targetedDinosaur].transform;
            cinemachineCamera.LookAt = enemyDinos[combatManager.currentActingNum - 5].transform;
        }
    }
}
