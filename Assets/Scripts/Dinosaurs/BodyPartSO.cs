using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBodyPart", menuName = "Dinosaur/BodyPart")]
public class BodyPartSO : ScriptableObject
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public Rarity Rarity { get; private set; }
    [field: SerializeField]
    public BodyPartType PartType { get; private set; }
    [field: SerializeField]
    public int ModelID {  get; private set; }
    [field: SerializeField]
    public List<StatParameters> Stats { get; private set; }
    [field: SerializeField]
    public WildCardSO Wild { get; private set; }
}

[Serializable]
public struct StatParameters
{
    public StatType Type;
    public float MinValue;
    public float MaxValue;
    [Range(0f, 1f)]
    public float AppearanceChance;
}

public enum BodyPartType
{
    Head, Arms, Legs
}

public enum Rarity
{
    Normal, Fossil
}
