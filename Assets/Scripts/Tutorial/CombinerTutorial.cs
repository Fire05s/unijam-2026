using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CombinerTutorial : MonoBehaviour
{
    [Header("Initial Parts")]
    [SerializeField] private List<BodyPartSO> _initialParts;
    [SerializeField] private BaseStatsSO _baseStats;
    [Header("References")]
    [SerializeField] private TutorialManager _manager;

    private void Start()
    {
        // Initial inventory setup
        foreach (var partData in _initialParts)
        {
            PlayerInventory.Instance.AddBodyPart(new DinosaurPart(partData));
        }

        // Tutorial setup
        foreach (var step in GetComponentsInChildren<ITutorialStep>())
        {
            _manager.AddStep(step);
        }

        _manager.BeginTutorial();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _manager.SkipCurrentStep();
        }
    }

    public void SkipAll()
    {
        PlayerInventory.Instance.ClearBodyParts();
        PlayerInventory.Instance.ClearCreatures();
        PlayerInventory.Instance.AddCreature(new DinosaurData(_baseStats, _initialParts));
        PartyManager.Instance.ExitToMain();
    }
}
