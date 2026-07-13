namespace Combat {
    public enum EntitySide
    {
        Player, Enemy
    }

    public enum TurnStep
    {
        TurnStart,
        ApplyDoT,
        AwaitEmptyCheck,
        EmptyCheck,
        AwaitSelect,
        PlayerSelect,
        EnemySelect,
        AwaitPlayerAttack,
        PlayerAttack,
        AwaitEnemyAttack,
        EnemyAttack,
        WildCardActivity,
        AwaitEnd,
        TurnEnd,
        CombatVictory,
        CombatLose
    }
}