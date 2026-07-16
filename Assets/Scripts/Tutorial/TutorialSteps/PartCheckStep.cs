using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartCheckStep : MonoBehaviour, ITutorialStep
{
    [Header("References")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private List<GameObject> _disabledObjects;
    [SerializeField] private List<GameObject> _activeObjects;
    [Header("Settings")]
    [SerializeField] private int _requiredSelectedCount;
    public IEnumerator Execute(TutorialManager manager)
    {
        OnEnter();

        while (CreatureCombiner.Instance.PartSlots.Count != _requiredSelectedCount)
        {
            yield return null;
        }

        OnExit();
    }

    public void Skip()
    {
        // Unskippable
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
