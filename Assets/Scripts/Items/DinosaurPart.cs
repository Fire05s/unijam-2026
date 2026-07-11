using System.Collections.Generic;
using UnityEngine;
public class DinosaurPart
{
    public Rarity Rarity;
    public BodyPartType Type;
    public List<Stat> Stats = new List<Stat>();
    public WildCard Wildcard;
    public BodyPartSO Reference;

    public DinosaurPart(BodyPartSO partRef)
    {
        Rarity = partRef.Rarity;
        Type = partRef.PartType;
        Reference = partRef;
        Wildcard = partRef.Wild;

        GenerateStats();
    }

    public void GenerateStats()
    {
        if (Reference == null)
        {
            Debug.LogWarning("Can't generate stats bc part reference is null");
            return;
        }

        foreach (var stat in Reference.Stats)
        {
            float rolledChance = Random.Range(0f,1f);
            if (rolledChance <= stat.AppearanceChance)
            {
                Stat newStat = new Stat
                {
                    Type = stat.Type,
                    Value = Random.Range(stat.MinValue, stat.MaxValue)
                };
                Stats.Add(newStat);
            }
        }
    }
}