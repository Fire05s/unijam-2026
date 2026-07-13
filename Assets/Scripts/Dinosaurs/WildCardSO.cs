using UnityEngine;

[CreateAssetMenu(fileName = "NewWildCard", menuName = "Dinosaur/WildCard")]
public class WildCardSO : ScriptableObject
{
    [field: SerializeField]
    public WildCard WildType { get; private set; }
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField, TextArea(3, 10)]
    public string Description { get; private set; }
}
public enum WildCard
{
    None, Multihit, Bleed, Doublehit, Ravenousbite, Luckystreak, Bloodlust, Dodge, Scavenger, Packtreats, Packmentality
}
