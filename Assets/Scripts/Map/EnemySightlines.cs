using Combat;
using UnityEngine;
using System.Collections;

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
    private LayerMask _layerMask;
    private bool _triggered;
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
        _sightLineOrigin = transform;
        if (MapData.Instance && MapData.Instance.EnemyEncounteredBefore(_enemyID))
        {
            if (BattleDataLoader.Instance && BattleDataLoader.Instance.GetBattleID() == _enemyID && BattleDataLoader.Instance.WasBattleWon() == false)
            {
                MapData.Instance.RemoveEnemyEncountered(_enemyID);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        //Use a raycast to see what it's hitting. If it hits the player then change the scene to whatever the name of the combat scene is. If not, debug what it is currently hitting.
        //Kind of simplistic right now so I might change it later on depending on if this is what we need or not.
        RaycastHit hit;
        if (Physics.Raycast(_sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward), out hit, _sightLineDistance, _layerMask))
        {
            if (hit.transform.gameObject.CompareTag("Player") && !_triggered)
            {
                Transform playerCam = GameObject.Find("CinemachineCamera").transform;
                playerCam.parent.Find("CinemachineCamera").GetComponent<PlayerCam>().CamUnlocked = false;
                GameObject.Find("Player").GetComponent<PlayerController>().ChangeSpeed(0);
                _triggered = true;
                Debug.DrawRay(_sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                //I added in a fading transition from a tutorial which is what this goes to. Should be easy to replace if something else is needed for the transition or scene change.
                //Debug.Log("Hit player, triggering battle");
                MapData.Instance?.MarkEnemyEncountered(_enemyID);
                MapData.Instance?.SavePlayerPosition(hit.transform.position, hit.transform.rotation);
                if (BattleDataLoader.Instance == null)
                {
                    Debug.LogError("BattleDataLoader does not exist.");
                    return;
                }
                //Debug.Log(playerCam);
                StartCoroutine(BattleCoroutine(playerCam));
            }
        }
    }

    IEnumerator BattleCoroutine(Transform cam)
    {
        cam.GetComponent<PlayerCam>().GiveTransformTarget(transform);
        yield return new WaitForSeconds(2);
        BattleDataLoader.Instance.SetBattleID(_enemyID);
        BattleDataLoader.Instance.StartBattle(_battleData);
    }
}
