using System;
using System.Collections.Generic;
using Combat;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Changes elements within the combat scene depending on the Combat Manager 
/// </summary>
public class CombatSceneManager : MonoBehaviour
{
    [Header("References")]
    public static CombatSceneManager Instance {get; private set;}
    [SerializeField] public Transform playerPositionHolder;
    [SerializeField] public Transform enemyPositionHolder;
    [SerializeField] private CombatCreature dinoPrefab;
    private List<GameObject> playerDinoPositions = new List<GameObject>();
    private List<GameObject> enemyDinoPositions = new List<GameObject>();
    private Dictionary<int, CombatCreature> _creaturesObjects = new();

    [SerializeField] private int currentSelectedTarget = -1;

    [Header("Animations")]
    [SerializeField] private float attackMoveDuration = 0.3f;
    [SerializeField] private float attackPauseDuration = 0.4f;
    [SerializeField] private float attackDistance = 2.0f;

    private CombatManager combatManager;

    public Dictionary<int, CombatCreature> CreatureObjects => _creaturesObjects;
    public int CurrentSelectedTarget => currentSelectedTarget;
    public event Action SceneInitialized;

    private void Awake()
    {
        if (Instance != null && Instance != this){ Destroy(Instance.gameObject); }
        Instance = this;
    }

    private void Start()
    {
        combatManager = CombatManager.Instance;
        combatManager.AttackPerformed += OnDinoAttacked;
        combatManager.DinoDamaged += OnDinoDamaged;
    }

    private void OnDestroy()
    {
        combatManager.AttackPerformed -= OnDinoAttacked;
        combatManager.DinoDamaged -= OnDinoDamaged;
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
            // Processes player dinos
            CombatCreature dino = Instantiate(dinoPrefab, playerPositionHolder);
            dino.SetModel(playerDinosData[index].Model);
            _creaturesObjects.Add(index, dino);
            // Player dino follow cam
            CameraManager.Instance.AddCamera(new CameraManager.CameraEntry { Id = index, Camera = dino.FollowCamera }); 

            Vector3 currentDinoPosition = dino.transform.localPosition;
            currentDinoPosition.x = (index - playerDinoPivotIndex) * distanceScaler;
            dino.transform.localPosition = currentDinoPosition;

            playerDinoPositions.Add(dino.gameObject);
        }
        for (int index=0; index < enemyDinosData.Count; ++index)
        {
            // Processes enemy dinos
            CombatCreature dino = Instantiate(dinoPrefab, enemyPositionHolder);
            dino.SetModel(enemyDinosData[index].Model);
            _creaturesObjects.Add(index + 5, dino);

            Vector3 currentDinoPosition = dino.transform.localPosition;
            currentDinoPosition.x = (index - enemyDinoPivotIndex) * distanceScaler;
            currentDinoPosition.z = (index % 2 == 0) ? currentDinoPosition.z : distanceScaler;
            dino.transform.localPosition = currentDinoPosition;

            enemyDinoPositions.Add(dino.gameObject);
        }
        SceneInitialized?.Invoke();

        foreach (var dino in _creaturesObjects.Values)
        {
            // Intro anim
            dino.SlotModel.SetIntro();
            AudioManager.Instance?.PlayHitSFX();
        }
    }

    public void UpdateSceneAfterDeath(int dinosaurID)
    {
        if (combatManager.RemainingPlayerDinosaurs.Contains(dinosaurID))
        {
            //GameObject dinoToDestroy = playerDinoPositions[dinosaurID];
            playerDinoPositions[dinosaurID] = null;
            //Destroy(dinoToDestroy);
        }
        else if (combatManager.RemainingEnemyDinosaurs.Contains(dinosaurID))
        {
            //GameObject dinoToDestroy = enemyDinoPositions[dinosaurID - 5];
            enemyDinoPositions[dinosaurID - 5] = null;
            //Destroy(dinoToDestroy);
        }
        _creaturesObjects[dinosaurID].SlotModel.SetDead();
        AudioManager.Instance?.PlayHitSFX();
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
        Debug.Log($"Selected {currentSelectedTarget}");
        combatManager.targetedDinosaur = currentSelectedTarget + 5; // Standarize the indices on the list to match dino ID

        currentSelectedTarget = -1;
    }
    
    private void UpdateCinemachineCamera()
    {
        if (combatManager.state == TurnStep.PlayerSelect)
        {
            // Player Dino ids correspond to its given index on the field
            if (_creaturesObjects.ContainsKey(currentSelectedTarget + 5))
            {
                CameraManager.Instance.SwitchCamera(combatManager.currentActingNum);
                CameraManager.Instance.SetLookAt(_creaturesObjects[currentSelectedTarget + 5].LookTarget);
            }
        }
        else
        {
            CameraManager.Instance.SwitchCamera(-1);
        }
    }

    private void OnDinoAttacked(int targetId, int attackerId)
    {
        if (!_creaturesObjects.TryGetValue(attackerId, out CombatCreature attacker))
            return;

        if (!_creaturesObjects.TryGetValue(targetId, out CombatCreature target))
            return;

        Transform attackerTransform = attacker.transform;
        Transform targetTransform = target.transform;

        Vector3 startPos = attackerTransform.position;
        Vector3 direction = (targetTransform.position - attackerTransform.position).normalized;
        Vector3 attackPos = targetTransform.position - direction * attackDistance;

        attackerTransform.DOKill();

        DG.Tweening.Sequence seq = DOTween.Sequence();

        seq.Append(attackerTransform.DOMove(attackPos, attackMoveDuration)
            .SetEase(Ease.OutQuad));

        attacker.SlotModel.SetAttack();
        seq.AppendInterval(attackPauseDuration);

        seq.Append(attackerTransform.DOMove(startPos, attackMoveDuration)
            .SetEase(Ease.InQuad));
    }

    private void OnDinoDamaged(int targetId)
    {
        _creaturesObjects[targetId].SlotModel.SetHurt();
        AudioManager.Instance?.PlayScreechSFX();
    }
}
