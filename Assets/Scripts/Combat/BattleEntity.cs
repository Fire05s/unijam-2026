using UnityEngine;
public enum EntitySide {Player,Enemy}
public class BattleEntity
{
    /// <summary>
    /// Create a new Battle Entity
    /// </summary>
    public BattleEntity(EntitySide s, float hp=10, float sp=1, float atk=2, float cc=0, WildCard w=WildCard.None)
    {
        side = s;
        maxHealth = hp;
        health = maxHealth;
        speed = sp;
        attack = atk;
        crit = cc;
        wild = w;
    }
    public EntitySide side;
    private float maxHealth;
    private float health;
    private float speed;
    private float attack;
    private float crit;
    private WildCard wild;

    public int GetSpeed() {return (int)speed;}
    public bool IsAlive() {return health>0;}
    public WildCard GetWildCard() {return wild;}
    public (float, bool) CalculateDamage()
    {
        float outgoingDmg = attack;
        bool outgoingCrit = false;
        if (Random.Range(0,100) <= crit)
        {
            outgoingDmg *= 2;
            outgoingCrit = true;
        }

        return (outgoingDmg, outgoingCrit);
    }
    public void Heal(float amount)
    {
        health += amount;
        if (health > maxHealth) {health = maxHealth;}
    }
    public void DealDamage(float amount)
    {
        health -= amount;
        if (health < 0) {health=0;}
    }
    public int NextTurn(int currentTurn)
    {
        return currentTurn += 11 - (int)speed;
    }

}