using Combat;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatUIManager : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private List<CombatSlotUI> _playerSlots;
    [Header("Turn Indicator")]
    [SerializeField] private GameObject _turnPanel;
    [SerializeField] private TextMeshProUGUI _turnText;

    private void Start()
    {
        CombatSceneManager.Instance.SceneInitialized += OnSceneInitialized;
        CombatManager.Instance.TurnAdvanced += OnNewTurn;
        OnSceneInitialized();
    }

    private void OnDestroy()
    {
        CombatSceneManager.Instance.SceneInitialized -= OnSceneInitialized;
        CombatManager.Instance.TurnAdvanced -= OnNewTurn;
    }

    private void OnSceneInitialized()
    {
        UpdatePlayerSlots();
    }

    private void OnNewTurn(int turnNum)
    {
        if (!_turnPanel.gameObject.activeSelf) _turnText.gameObject.SetActive(true);
        _turnText.text = $"Turn: {turnNum}";
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
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }
}
