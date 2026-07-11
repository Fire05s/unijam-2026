struct DinosaurPart
{
    BodyPartType type;
    List<Stat> stats;
    WildCard wildcard;
}

struct Stat
{
    Stat(string a, float v)
    {
        type = stringtostat(a); value = v;
    }
    StatType type;
    float value;

    public static string stattostring(StatType s)
    {
        switch(s)
        {
            case StatType.Attack: return "Attack";
            case StatType.Speed: return "Speed";
            case StatType.Health: return "Health";
            case StatType.CritDamage: return "Critical Hit Damage";
            case StatType.CritRate: return "Critical Hit Rate";
        }
    }
    public static StatType stringtostat(string s)
    {
        switch(s)
        {
            case "Attack": return StatType.Attack;
            case "Speed": return StatType.Speed;
            case "Health": return StatType.Health;
            case "Critical Hit Damage": return StatType.CritDamage;
            case "Critical Hit Rate": return StatType.CritRate;
        }
    }
}
enum StatType
{
    Attack,Speed,Health,CritRate,CritDamage
}
enum WildCard
{
    Multihit, Bleed, Doublehit, Ravenousbite, Luckystreak, Bloodlust, Dodge, Scavenger, Packtreats, Packmentality
}