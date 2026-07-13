using UnityEngine;

namespace Combat {

public class CombatEntity
{
    public int _id {get; private set;}
    public EntitySide _side {get; private set;}
    float _maxHealth;
    float _health;
    public float _speed {get; private set;}
    private float _attack;
    private float _crit; 
    public WildCard _wild {get; private set;}
    public Vector2 _boardPosition {get; private set;}

    CombatEntity(int id, EntitySide side, float health, float speed, float attack, float critchance, WildCard wildcard)
    {
        _id = id; _side = side; _maxHealth = health; _health = _maxHealth; _speed = speed; _attack = attack; _crit = critchance; _wild = wildcard;
    }
    public void SetPosition(Vector2 newPos) {_boardPosition = newPos;}
    /// <summary>
    /// Calculates the next valid turn this entity can move based on speed
    /// </summary>
    /// <param name="currentTurnNumber"></param>
    /// <returns>Minimum next valid turn slot</returns>
    public int CalculateNextTurn(int currentTurnNumber) {return currentTurnNumber + (11 - (int)_speed);}
    public bool IsAlive() {return _health > 0;}
    /// <summary>
    /// Increase a combat entity's health by a certain amount. Can not overheal.
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount)
    {
        _health += amount;
        if (_health > _maxHealth) {_health = _maxHealth;}
    }
    /// <summary>
    /// Appplies damage to a combat entity.
    /// </summary>
    /// <param name="amount">Damage Amount</param>
    /// <returns>A float representing overflow (extra damage)</returns>
    public float ApplyDamage(float amount)
    {
        _health -= amount;
        return 0 - _health;
    }
    /// <summary>
    /// Generates outgoing attack damage including crit
    /// </summary>
    /// <returns>The finalized attack damage</returns>
    public (float, bool) CalculateAttack()
    {
        float outgoingDmg = _attack;
        bool outgoingCrit = false;
        if (Random.Range(0,100) <= _crit)
        {
            outgoingDmg *= 2;
            outgoingCrit = true;
        }
        return (outgoingDmg, outgoingCrit);
    }
}
}