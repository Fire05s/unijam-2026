using System.Collections.Generic;
enum WildCard
{
    Multihit, Bleed, Doublehit, Ravenousbite, Luckystreak, Bloodlust, Dodge, Scavenger, Packtreats, Packmentality
}
class DinosaurPart
{
    DinosaurPart(short r, BodyPartType t, BodyPartSO re) {rarity = r; type = t; reference = re;}
    short rarity;
    BodyPartType type;
    public List<Stat> stats = new List<Stat>();
    WildCard wildcard;
    BodyPartSO reference;
}

struct Stat
{
    StatType type;
    float value;
}