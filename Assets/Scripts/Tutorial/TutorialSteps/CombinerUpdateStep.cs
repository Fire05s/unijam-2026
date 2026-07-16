using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerUpdateStep : MonoBehaviour, ITutorialStep
{
    [Header("References")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private List<GameObject> _disabledObjects;
    [SerializeField] private List<GameObject> _activeObjects;

    private bool _isUpdated;

    private void Start()
    {
        CreatureCombiner.Instance.PartSelect += OnPartSelect;
    }
    public IEnumerator Execute(TutorialManager manager)
    {
        _isUpdated = false;
        OnEnter();

        while (!_isUpdated)
        {
            yield return null;
        }

        OnExit();
    }

    private void OnPartSelect()
    {
        _isUpdated = true;
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
