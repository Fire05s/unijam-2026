using UnityEngine;


public static class CombatAbilities{
    public static void Attack(DinosaurData attackingDino, DinosaurData receivingDino)
    {
        float totalDamage = AttackCalculation(attackingDino, receivingDino);
        foreach(WildCard wildCard in attackingDino.GetWildCardAbilities())
        {
            switch (wildCard)
            {
                case WildCard.Multihit: Multihit(); break;
                case WildCard.Bleed: Bleed(); break;
                case WildCard.Doublehit: DoubleHit(); break;
                case WildCard.Ravenousbite: RavenousBite(attackingDino, receivingDino, totalDamage); break;
                case WildCard.Luckystreak: LuckyStreak(attackingDino, totalDamage); break;
                case WildCard.Bloodlust: Bloodlust(attackingDino, receivingDino); break;
                case WildCard.Dodge: Dodge(); break;
                case WildCard.Scavenger: Scavenger(attackingDino, receivingDino); break;
                case WildCard.Packtreats: PackTreats(); break;
                case WildCard.Packmentality: PackMentality(); break;
            }
        }
    }
    private static void Multihit()
    {
        // TODO: Requires Combat Manager to be Completed
    }

    private static void Bleed()
    {
        // TODO: Requires Combat Manager to be Completed
    }

    private static void DoubleHit()
    {
        // TODO: Requires Combat Manager to be Completed 
    }

    private static void RavenousBite(DinosaurData attackingDino, DinosaurData receivingDino, float totalDamage)
    {
        if (receivingDino.GetStat(StatType.Health) <= 0)
        {
            float heal = System.MathF.Ceiling(totalDamage * 0.25f);
            // TODO: Heal 25% of damage
        }
    }

    private static void LuckyStreak(DinosaurData attackingDino, float totalDamage)
    {
        if (attackingDino.GetStat(StatType.Attack) != totalDamage)
        {
            // attackingDino takes another turn
        }
    }

    private static void Bloodlust(DinosaurData attackingDino, DinosaurData receivingDino)
    {
        if (receivingDino.GetStat(StatType.Health) <= 0)
        {
            // attackingDino takes an extra turn
        }
    }

    private static void Dodge()
    {
        // TODO: Requires Combat Manager to be Completed 
    }

    private static void Scavenger(DinosaurData attackingDino, DinosaurData receivingDino)
    {
        if (receivingDino.GetStat(StatType.Health) <= 0)
        {
            // TODO: Heal 25% of missing health for attackingDino
        }
    }

    private static void PackTreats()
    {
        // TODO: Requires Combat Manager to be Completed 
    }

    private static void PackMentality()
    {
        // TODO: Requires Combat Manager to be Completed 
    }

    private static float AttackCalculation(DinosaurData attackingDino, DinosaurData receivingDino)
    {
        float totalDamage = (Random.value <= attackingDino.GetStat(StatType.CritChance)/100) ? attackingDino.GetStat(StatType.Attack) * 2 : attackingDino.GetStat(StatType.Attack);
        // receiving Dino takes damage
        return totalDamage;
    }
}
