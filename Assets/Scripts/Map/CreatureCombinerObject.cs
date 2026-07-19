using UnityEngine;

public class CreatureCombinerObject : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private int _audioListIndex1 = 1;
    [SerializeField] private int _audioListIndex2 = 2;
    [SerializeField] private bool _disableSounds;

    void Start()
    {
        if (!_disableSounds)
        {
            AudioManager.Instance.PlayInWorldLoop(_audioListIndex1, gameObject);
            AudioManager.Instance.PlayInWorldLoop(_audioListIndex2, gameObject);
        }
    }
}