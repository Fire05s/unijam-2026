using Combat;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatUIManager : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private List<CombatSlotUI> _playerSlots;
    [Header("Turn Indicator")]
    [SerializeField] private GameObject _turnPanel;
    [SerializeField] private TextMeshProUGUI _turnText;
    [Header("Enemy Health")]
    [SerializeField] private Transform _healthBarParent;
    [SerializeField] private GameObject _healthBarPrefab;
    [SerializeField] private float _yOffset;
    [Header("Crosshair")]
    [SerializeField] private CombatUIFollower _crosshairFollow;

    private Dictionary<int, HealthUI> _healthBars = new();

    private void Start()
    {
        CombatSceneManager.Instance.SceneInitialized += OnSceneInitialized;
        CombatManager.Instance.TurnAdvanced += OnNewTurn;
        CombatManager.Instance.DinoDamaged += UpdateHealthBar;
        CombatManager.Instance.DinoHealed += UpdateHealthBar;
        CombatManager.Instance.DOTApplied += UpdateHealthBars;
        CombatManager.Instance.DinoDied += OnDeath;
        OnSceneInitialized();
    }

    private void OnDestroy()
    {
        CombatSceneManager.Instance.SceneInitialized -= OnSceneInitialized;
        CombatManager.Instance.TurnAdvanced -= OnNewTurn;
        CombatManager.Instance.DinoDamaged -= UpdateHealthBar;
        CombatManager.Instance.DinoHealed -= UpdateHealthBar;
    }

    private void Update()
    {
        UpdateCrosshair();
    }

    private void OnSceneInitialized()
    {
        UpdatePlayerSlots();
        InitializeEnemyHealth();
        UpdateAllHealthBars();
    }

    private void OnNewTurn(int turnNum)
    {
        _turnText.text = $"Turn: {turnNum}";
    }

    private void OnDeath(int id)
    {
        ClearHealthBar(id);
    }

    private void UpdatePlayerSlots()
    {
        for (int i = 0; i < _playerSlots.Count; i++)
        {
            CombatSlotUI slot = _playerSlots[i];
            if (CombatSceneManager.Instance.CreatureObjects.TryGetValue(i, out CombatCreature creature))
            {
                slot.gameObject.SetActive(true);
                slot.SetIcon(creature.CameraTexture);
                _healthBars.Add(i, slot.HealthBar);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    public void InitializeEnemyHealth()
    {
        foreach (int id in CombatSceneManager.Instance.CreatureObjects.Keys)
        {
            if (id >= 5)
            {
                GameObject healthContainer = Instantiate(_healthBarPrefab, _healthBarParent);
                HealthUI enemyHealth = healthContainer.GetComponentInChildren<HealthUI>();
                CombatUIFollower follower = healthContainer.gameObject.AddComponent<CombatUIFollower>();
                follower.Initialize(CombatSceneManager.Instance.CreatureObjects[id].LookTarget, _yOffset);
                _healthBars.Add(id, enemyHealth);
            }
        }
    }

    private void UpdateAllHealthBars()
    {
        if (CombatManager.Instance == null) return;
        foreach (var id in CombatManager.Instance.Dinosaurs.Keys)
        {
            UpdateHealthBar(id);
        }
    }

    private void UpdateHealthBar(int creatureId)
    {
        if (!_healthBars.ContainsKey(creatureId))
        {
            Debug.LogWarning($"Health bars UI don't contain {creatureId}");
            return;
        }
        CombatEntity data = CombatManager.Instance.Dinosaurs[creatureId];
        float healthPercent = data._health / data._maxHealth;
        _healthBars[creatureId].SetHealth(healthPercent);
    }
    private void UpdateHealthBars(List<int> creatureIds)
    {
        foreach (var creatureId in creatureIds)
        {
            if (!_healthBars.ContainsKey(creatureId))
            {
                Debug.LogWarning($"Health bars UI don't contain {creatureId}");
                return;
            }
            CombatEntity data = CombatManager.Instance.Dinosaurs[creatureId];
            float healthPercent = data._health / data._maxHealth;
            _healthBars[creatureId].SetHealth(healthPercent);
        }
    }

    private void ClearHealthBar(int id)
    {
        if (_healthBars.ContainsKey(id))
        {
            Destroy(_healthBars[id].gameObject);
            _healthBars.Remove(id);
        }
    }

    private void UpdateCrosshair()
    {
        if (CombatSceneManager.Instance == null) return;
        int target = CombatSceneManager.Instance.CurrentSelectedTarget;
        if (target == -1 || !CombatSceneManager.Instance.CreatureObjects.ContainsKey(target + 5))
        {
            _crosshairFollow.gameObject.SetActive(false);
        }
        else
        {
            _crosshairFollow.gameObject.SetActive(true);
            _crosshairFollow.SetTarget(CombatSceneManager.Instance.CreatureObjects[target + 5].LookTarget);
        }
    }
}
