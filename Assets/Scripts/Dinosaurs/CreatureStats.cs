using UnityEngine;

public class CreatureStats
{
    public float Health;
    public float Attack;
    public float Speed;
    public float CritChance;

    // Allows for + and += operations
    public static CreatureStats operator +(CreatureStats a, CreatureStats b)
    {
        return new CreatureStats
        {
            Health = a.Health + b.Health,
            Attack = a.Attack + b.Attack,
            Speed = a.Speed + b.Speed,
            CritChance = a.CritChance + b.CritChance
        };
    }

    // Allows for - and -= operations
    public static CreatureStats operator -(CreatureStats a, CreatureStats b)
    {
        return new CreatureStats
        {
            Health = a.Health - b.Health,
            Attack = a.Attack - b.Attack,
            Speed = a.Speed - b.Speed,
            CritChance = a.CritChance - b.CritChance
        };
    }
}

public enum StatType
{
    Health, Attack, Speed, CritChance
}

