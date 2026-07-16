using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueStep : MonoBehaviour, ITutorialStep
{
    [Header("References")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private List<GameObject> _disabledObjects;
    [SerializeField] private List<GameObject> _activeObjects;
    [Header("Settings")]
    [SerializeField] private float _duration;
    [SerializeField] private bool _isSkippable;
    private bool _skipRequested;
    public IEnumerator Execute(TutorialManager manager)
    {
        _skipRequested = false;
        OnEnter();

        float timer = 0f;

        while (timer < _duration && !_skipRequested)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        OnExit();
    }

    public void Skip()
    {
        if (_isSkippable)
        {
            _skipRequested |= true;
        }
    }

    private void OnEnter()
    {
        _dialoguePanel.SetActive(true);
        foreach (GameObject obj in _disabledObjects)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in _activeObjects)
        {
            obj.SetActive(true);
        }
    }

    private void OnExit()
    {
        foreach (GameObject obj in _disabledObjects)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in _activeObjects)
        {
            obj.SetActive(false);
        }

        _dialoguePanel.SetActive(false);
    }
}
