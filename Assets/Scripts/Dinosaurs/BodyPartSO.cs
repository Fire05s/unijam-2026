using UnityEngine;

[CreateAssetMenu(fileName = "NewBodyPart", menuName = "Dinosaur/BodyPart")]
public class BodyPartSO : ScriptableObject
{
    [field: SerializeField]
    public BodyPartType PartType { get; private set; }
    [field: SerializeField]
    public GameObject Model {  get; private set; }
    [field: SerializeField]
    public CreatureStats BonusStats { get; private set; }

    // TODO: Add bonus ability references here
}

public enum BodyPartType
{
    Head, Arms, Legs
}
