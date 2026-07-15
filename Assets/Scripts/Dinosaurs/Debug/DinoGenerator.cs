using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DinoGenerator : MonoBehaviour
{
    [Header("Initial Dino Parts")]
    [SerializeField] private List<BodyPartSO> _initialParts;
    [SerializeField] private BaseStatsSO _baseStats;

    private void Start()
    {
        DinosaurData startDino = new DinosaurData(_baseStats, _initialParts);
        PlayerInventory.Instance.AddCreature(startDino);
    }
}
