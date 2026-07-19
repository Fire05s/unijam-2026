using Combat;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatUIManager : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private List<CombatSlotUI> _playerSlots;
    [SerializeField] private TurnSlotUI _turnSlotPrefab;
    [SerializeField] private GameObject _turnSlotPanel;
    [Header("Turn Indicator")]
    [SerializeField] private GameObject _turnPanel;
    [SerializeField] private TextMeshProUGUI _turnText;
    [Header("Enemy Health")]
    [SerializeField] private Transform _healthBarParent;
    [SerializeField] private GameObject _healthBarPrefab;
    [SerializeField] private float _yOffset;
    [Header("Crosshair")]
    [SerializeField] private CombatUIFollower _crosshairFollow;
    [Header("Text")]
    [SerializeField] private GameObject _controlsPanel;

    private Dictionary<int, HealthUI> _healthBars = new();
    private List<KeyValuePair<int, TurnData>> _existingQueue = new();
    private List<TurnSlotUI> _turnSlots = new();

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
        UpdateTurnSlots();
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
            _controlsPanel.SetActive(false);
        }
        else
        {
            _crosshairFollow.gameObject.SetActive(true);
            _controlsPanel.SetActive(true);
            _crosshairFollow.SetTarget(CombatSceneManager.Instance.CreatureObjects[target + 5].LookTarget);
        }
    }

    private void UpdateTurnSlots()
    {
        List<KeyValuePair<int, TurnData>> turnQueue = CombatManager.Instance.TurnQueue.PeekX(5); // peeks 5 turns ahead
        List<(int index, KeyValuePair<int, TurnData> value)> removed = new();
        List<(int index, KeyValuePair<int, TurnData> value)> added = new();

        // Find old slots
        for (int i = _existingQueue.Count - 1; i >= 0; i--)
        {
            if (!turnQueue.Contains(_existingQueue[i]))
            {
                removed.Add((i, _existingQueue[i]));
            }
        }

        // Find new slots
        for (int i = 0; i < turnQueue.Count; i++)
        {
            if (!_existingQueue.Contains(turnQueue[i]))
            {
                added.Add((i, turnQueue[i]));
            }
        }

        // Remove old slots
        foreach (var r in removed)
        {
            _existingQueue.RemoveAt(r.index);
            _turnSlots[r.index].Hide();
            _turnSlots.RemoveAt(r.index);
        }

        // Add new slots
        foreach (var a in added)
        {
            _existingQueue.Insert(a.index, a.value);
            TurnSlotUI newSlot = Instantiate(_turnSlotPrefab, _turnSlotPanel.transform);
            newSlot.SetSlot(a.value.Key, a.value.Value);
            newSlot.transform.SetSiblingIndex(a.index + 1);
            _turnSlots.Insert(a.index, newSlot);
        }
    }
}
