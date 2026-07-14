using Combat;
using UnityEngine;

public class EnemySightlines : MonoBehaviour
{
    //This script should handle the enemy's sight. Once it sees the player transition to the combat scene.
    [Header("Sightline")]
    [SerializeField] private Transform _sightLineOrigin;
    [SerializeField] private float _sightLineDistance;
    [Header("ID")]
    [SerializeField] private int _enemyID;
    [Header("Scene Transition")]
    [SerializeField] private ScreenTransition _transitionObject;
    [SerializeField] private string _battleScene;
    [SerializeField] private float _transitionDuration;
    [Header("Associated Battle")]
    [SerializeField] private BattleData _battleData;

    private MapData _mapManager;
    private LayerMask _layerMask;
    void Awake()
    {
        _layerMask = LayerMask.GetMask("Wall", "Default");
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

    void Start()
    {
        _mapManager = GameObject.Find("MapDataManager").GetComponent<MapData>();
        _sightLineOrigin = transform;
        if(_mapManager.EnemyEncounteredBefore(_enemyID))
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        //Use a raycast to see what it's hitting. If it hits the player then change the scene to whatever the name of the combat scene is. If not, debug what it is currently hitting.
        //Kind of simplistic right now so I might change it later on depending on if this is what we need or not.
        RaycastHit hit;
        if (Physics.Raycast(_sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward), out hit, _sightLineDistance, _layerMask))
        {
            if(hit.transform.gameObject.CompareTag("Player"))
            {
                Debug.DrawRay(_sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                //I added in a fading transition from a tutorial which is what this goes to. Should be easy to replace if something else is needed for the transition or scene change.
                Debug.Log("Hit player, triggering battle");
                _mapManager.MarkEnemyEncountered(_enemyID);
                _mapManager.SavePlayerPosition(hit.transform.position);

                if (BattleDataLoader.Instance == null)
                {
                    Debug.LogError("BattleDataLoader does not exist.");
                    return;
                }
                BattleDataLoader.Instance.StartBattle(_battleData);

                _transitionObject.FadeAndLoad(_battleScene, _transitionDuration);
            }
        }
    }
}
