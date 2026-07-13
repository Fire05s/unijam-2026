using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBaseStats", menuName = "Dinosaur/BaseStats")]
public class BaseStatsSO : ScriptableObject
{
    [field: SerializeField]
    public List<StatParameters> Stats { get; private set; }
}
