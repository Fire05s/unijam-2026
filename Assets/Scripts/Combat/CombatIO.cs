using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CombatIO : MonoBehaviour
{
    [SerializeField] private InputAction _left;
    [SerializeField] private InputAction _right;
    [SerializeField] private InputAction _select;
    
    [SerializeField] Vector3 NormalCameraPosition;
    [SerializeField] Vector3 NormalCameraRotation;
    [SerializeField] Vector3 SelectCameraPosition;
    [SerializeField] Vector3 SelectCameraRotation;

    

    bool inSelectPhase;
    void Awake()
    {
        inSelectPhase = false;
    }
    void Start()
    {
        transform.position = NormalCameraPosition;
        transform.eulerAngles = NormalCameraRotation;

        _left.Enable();
        _right.Enable();
        _select.Enable();

        _left.performed += OnLeftInput;
        _right.performed += OnRightInput;
        _select.performed += OnSelect;

        

        
    }

    void Update()
    {
        if (CombatManager.Instance.state == CombatManager.TurnPhase.SelectPhase && !inSelectPhase)
        {
            inSelectPhase = true;
            transform.position = SelectCameraPosition;
            transform.eulerAngles = SelectCameraRotation;

            // currentlySelected = 0;
            // while(!enemydinos[currentlySelected].isAlive)
            // {
            //     currentlySelected++;
            // }
            // selectionIndicator.gameObject.SetActive(true);
        }
        else if (CombatManager.Instance.state != CombatManager.TurnPhase.SelectPhase && inSelectPhase)
        {
            inSelectPhase = false;
            transform.position = NormalCameraPosition;
            transform.eulerAngles = NormalCameraRotation;
        }
    }
    void OnLeftInput(InputAction.CallbackContext context)
    {
        
    }
    void OnRightInput(InputAction.CallbackContext context)
    {
        
    }
    void OnSelect(InputAction.CallbackContext context)
    {
        
    }
}