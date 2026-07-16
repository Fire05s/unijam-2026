using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForActiveStep : MonoBehaviour, ITutorialStep
{
    [Header("References")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private GameObject _targetActive;
    [SerializeField] private List<GameObject> _disabledObjects;
    [SerializeField] private List<GameObject> _activeObjects;
    public IEnumerator Execute(TutorialManager manager)
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

        while (!_targetActive.activeSelf)
        {
            yield return null;
        }

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

    public void Skip()
    {
        // Unskippable
    }
}
