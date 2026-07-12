using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class Combiner : MonoBehaviour
{
    [SerializeField] private Button HeadSlotSelector;
    [SerializeField] private Button ArmSlotSelector;
    [SerializeField] private Button LegSlotSelector;

    private short _selectedSlot;
    void Start()
    {
        HeadSlotSelector.clicked += SelectSlot;
    }

    void SelectSlot()
    {
        switch(_selectedSlot)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }

    void Display()
    {
        
    }
}