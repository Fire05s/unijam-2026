using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private GameplayInputs _actions;

    public Vector2 Move => _actions.Player.Move.ReadValue<Vector2>();

    public event Action InteractPressed;

    public event Action ConfirmPressed;

    public event Action SelectLeftPressed;

    public event Action SelectRightPressed;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _actions = new GameplayInputs();
    }

    private void OnEnable()
    {
        _actions.Enable();

        _actions.Player.Interact.performed += OnInteract;

        _actions.Player.Confirm.performed += OnConfirm;
        _actions.Player.SelectLeft.performed += OnSelectLeft;
        _actions.Player.SelectRight.performed += OnSelectRight;
    }

    private void OnDisable()
    {
        _actions.Player.Interact.performed -= OnInteract;

        _actions.Player.Confirm.performed -= OnConfirm;
        _actions.Player.SelectLeft.performed -= OnSelectLeft;
        _actions.Player.SelectRight.performed -= OnSelectRight;

        _actions.Disable();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        InteractPressed?.Invoke();
    }

    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        ConfirmPressed?.Invoke();
    }

    private void OnSelectLeft(InputAction.CallbackContext ctx)
    {
        SelectLeftPressed?.Invoke();
    }
    
    private void OnSelectRight(InputAction.CallbackContext ctx)
    {
        SelectRightPressed?.Invoke();
    }
}
