using UnityEngine;

public class WildCardSO : ScriptableObject
{
    [field: SerializeField]
    public WildCard WildType { get; private set; }
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    [TextArea]
    public string Description { get; private set; }
}
