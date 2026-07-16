using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CombinerTutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialManager _manager;

    private void Start()
    {
        foreach(var step in GetComponentsInChildren<ITutorialStep>())
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
}
