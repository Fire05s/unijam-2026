using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private readonly Queue<ITutorialStep> _steps = new();
    private ITutorialStep _currentStep;

    public void AddStep(ITutorialStep step)
    {
        _steps.Enqueue(step);
    }

    public void BeginTutorial()
    {
        Debug.Log($"Starting tutorial with {_steps.Count} steps");
        StartCoroutine(RunTutorial());
    }

    public void SkipCurrentStep()
    {
        if (_currentStep == null) return;
        _currentStep.Skip();
    }

    private IEnumerator RunTutorial()
    {
        while (_steps.Count > 0)
        {
            _currentStep = _steps.Dequeue();
            yield return _currentStep.Execute(this);
        }

        Debug.Log("Tutorial finished");
    }
}
