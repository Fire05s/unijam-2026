using Combat;
using UnityEngine;

public class EnemySightlines : MonoBehaviour
{
    [Header("Sightline")]
    [SerializeField] private Transform _sightLineOrigin;
    [SerializeField] private float _sightLineDistance;
    [Header("Scene Transition")]
    [SerializeField] private GameObject _transitionObject;
    [SerializeField] private string _battleScene;
    [SerializeField] private float _transitionDuration;
    [Header("Associated Battle")]
    [SerializeField] private BattleData _battleData;

    private LayerMask layerMask;
    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Default");
    }

    private void OnDrawGizmos()
    {
        if (_sightLineOrigin == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        Gizmos.DrawRay(_sightLineOrigin.position, direction * _sightLineDistance);
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(_sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward), out hit, _sightLineDistance, layerMask))
        {
            if(hit.transform.gameObject.CompareTag("Player"))
            {
                Debug.Log("Hit player, triggering battle");
                // CombatManager.Instance.SetupCombat(_battleData);
                _transitionObject.GetComponent<ScreenTransition>().FadeAndLoad(_battleScene, _transitionDuration);
            }
        }
    }
}
