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
    private List<GameObject> playerDinoPositions = new List<GameObject>();
    private List<GameObject> enemyDinoPositions = new List<GameObject>();


    [SerializeField] private CinemachineCamera followCamera;
    [SerializeField] private CinemachineCamera overviewCamera;

    [SerializeField] private int currentSelectedTarget = -1; 

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

            playerDinoPositions.Add(dino);
        }
        for (int index=0; index < enemyDinosData.Count; ++index)
        {
            GameObject dino = Instantiate(dinoPlaceHolderPrefab, enemyPositionHolder);
            dino.GetComponent<ModelGenerator>().SetDinosaur(enemyDinosData[index]);

            Vector3 currentDinoPosition = dino.transform.localPosition;
            currentDinoPosition.x = (index - enemyDinoPivotIndex) * distanceScaler;
            currentDinoPosition.z = (index % 2 == 0) ? currentDinoPosition.z : distanceScaler;
            dino.transform.localPosition = currentDinoPosition;

            enemyDinoPositions.Add(dino);
        }
    }

    public void UpdateSceneAfterDeath(int dinosaurID)
    {
        if (combatManager.RemainingPlayerDinosaurs.Contains(dinosaurID))
        {
            GameObject dinoToDestroy = playerDinoPositions[dinosaurID];
            playerDinoPositions[dinosaurID] = null;
            Destroy(dinoToDestroy);
        }
        else if (combatManager.RemainingEnemyDinosaurs.Contains(dinosaurID))
        {
            GameObject dinoToDestroy = enemyDinoPositions[dinosaurID - 5];
            enemyDinoPositions[dinosaurID - 5] = null;
            Destroy(dinoToDestroy);
        }
    }

    public void StartTargetSelection()
    {
        IReadOnlyList<int> enemyDinoList = combatManager.RemainingEnemyDinosaurs;

        if (enemyDinoList.Count == 0) { currentSelectedTarget = -1; return; }
        int midIndex = enemyDinoList.Count / 2;
        currentSelectedTarget = enemyDinoList[midIndex] - 5; // Standarize the dino ID to match the indices on the list
    }
    
    public void SelectTargetLeft()
    {
        if (currentSelectedTarget <= 0){ 
            currentSelectedTarget = enemyDinoPositions.Count - 1; 
        }
        else
        {
            currentSelectedTarget -= 1;
        }

        if (enemyDinoPositions[currentSelectedTarget] == null)
        {
            SelectTargetLeft();
        }
    }

    public void SelectTargetRight()
    {
        if (currentSelectedTarget >= enemyDinoPositions.Count - 1){ 
            currentSelectedTarget = 0; 
        }
        else
        {
            currentSelectedTarget += 1;
        }

        if (enemyDinoPositions[currentSelectedTarget] == null)
        {
            SelectTargetRight();
        }
    }

    public void ConfirmSelectedTarget()
    {
        if (currentSelectedTarget == -1){ return; }

        combatManager.targetedDinosaur = currentSelectedTarget + 5; // Standarize the indices on the list to match dino ID

        currentSelectedTarget = -1;
    }
    
    private void UpdateCinemachineCamera()
    {
        if (combatManager.state == TurnStep.PlayerSelect)
        {
            // Player Dino ids correspond to its given index on the field
            followCamera.Follow = playerDinoPositions[combatManager.currentActingNum]?.transform;
            followCamera.LookAt = enemyDinoPositions[currentSelectedTarget]?.transform;

            followCamera.Prioritize();
        }
        else
        {
            overviewCamera.Prioritize();
        }
    }
}
