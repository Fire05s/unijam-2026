using UnityEngine;

public class enemySightlines : MonoBehaviour
{
    //This script should handle the enemy's sight. Once it sees the player transition to the combat scene.
    [Header("Sightline")]
    public Transform sightLineOrigin;
    public float sightLineDistance;
    [Header("Scene Transition")]
    public GameObject transitionObject;
    public string battleScene;
    public float transitionDuration;


    private bool hasLineOfSight;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    LayerMask layerMask;
    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Default");
    }

    private void Update()
    {
        //Use a raycast to see what it's hitting. If it hits the player then change the scene to whatever the name of the combat scene is. If not, debug what it is currently hitting.
        //Kind of simplistic right now so I might change it later on depending on if this is what we need or not.
        RaycastHit hit;
        if (Physics.Raycast(sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward), out hit, sightLineDistance, layerMask))
        {
            if(hit.collider.CompareTag("Player"))
            {
                Debug.DrawRay(sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Hit player");
                //I added in a fading transition from a tutorial which is what this goes to. Should be easy to replace if something else is needed for the transition or scene change.
                transitionObject.GetComponent<screenTransition>().FadeAndLoad(battleScene, transitionDuration);
            }
            else
            {
                Debug.DrawRay(sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                Debug.Log("Did not hit player, hit object with tag " + hit.transform.gameObject.tag.ToString());
            }
            
        }
        else
        {
            Debug.DrawRay(sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward) * sightLineDistance, Color.white);
            Debug.Log("Hit nothing");
        }
    }
}
