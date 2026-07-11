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
    [field: SerializeField]
    public WildCard Wild { get; private set; }
}

[Serializable]
public struct StatParameters
{
    public StatType Type;
    public float MinValue;
    public float MaxValue;
    public float AppearanceChance;
}

public enum BodyPartType
{
    Head, Arms, Legs
}
