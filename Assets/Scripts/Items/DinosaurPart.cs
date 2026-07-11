using System.Collections.Generic;
public class DinosaurPart
{
    DinosaurPart(short r, BodyPartType t, BodyPartSO re) { Rarity = r; Type = t; Reference = re;}
    public short Rarity;
    public BodyPartType Type;
    public List<Stat> Stats = new List<Stat>();
    public WildCard Wildcard;
    public BodyPartSO Reference;
}