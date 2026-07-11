using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private GameplayInputs _actions;

    public Vector2 Move => _actions.Player.Move.ReadValue<Vector2>();

    public event Action JumpPressed;
    public event Action AttackPressed;
    public event Action DashPressed;
    public event Action InteractPressed;

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
    }

    private void OnDisable()
    {
        _actions.Player.Interact.performed -= OnInteract;

        _actions.Disable();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        InteractPressed?.Invoke();
    }
}
