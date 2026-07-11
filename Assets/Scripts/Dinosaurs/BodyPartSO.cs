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
    public StatType type;
    public float minValue;
    public float maxValue;
    public float appearanceChance;
}

public enum BodyPartType
{
    Head, Arms, Legs
}
