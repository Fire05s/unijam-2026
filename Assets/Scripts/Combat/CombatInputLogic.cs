using Unity.Cinemachine;
using UnityEngine;

public class CombatInputLogic : MonoBehaviour
{

    private void Start()
    {
        OnEnable();
    }

    private void OnEnable()
    {
       InputManager.Instance.ConfirmPressed += OnConfirmPressed;
       InputManager.Instance.SelectLeftPressed += OnSelectLeftPressed;
       InputManager.Instance.SelectRightPressed += OnSelectRightPressed;
    }

    private void OnDisable()
    {
       InputManager.Instance.ConfirmPressed -= OnConfirmPressed;
       InputManager.Instance.SelectLeftPressed -= OnSelectLeftPressed;
       InputManager.Instance.SelectRightPressed -= OnSelectRightPressed;
    }

    private void OnConfirmPressed()
    {
        CombatSceneManager.Instance.ConfirmSelectedTarget();
    }

    private void OnSelectLeftPressed()
    {
        CombatSceneManager.Instance.SelectTargetLeft();
    }

    private void OnSelectRightPressed()
    {
        Debug.Log("INPUT IS BEING READ");
        CombatSceneManager.Instance.SelectTargetRight();
    }
}
