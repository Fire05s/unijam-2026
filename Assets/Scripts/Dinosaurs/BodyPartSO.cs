using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBodyPart", menuName = "Dinosaur/BodyPart")]
public class BodyPartSO : ScriptableObject
{
    [field: SerializeField]
    public BodyPartType PartType { get; private set; }
    [field: SerializeField]
    public GameObject Model {  get; private set; }
    [field: SerializeField]
    public List<StatParameters> Stats { get; private set; }

    // TODO: Add bonus ability references here
}

[Serializable]
public struct StatParameters
{
    StatType type;
    float minValue;
    float maxValue;
    float appearanceChance;
}

public enum BodyPartType
{
    Head, Arms, Legs
}
