using UnityEngine;

public class enemySightlines : MonoBehaviour
{
    [Header("Sightline")]
    public Transform sightLineOrigin;
    public float sightLineDistance;
    [Header("Scene Transition")]
    public GameObject transitionObject;
    public string battleScene;
    public float transitionDuration;


    private bool hasLineOfSight;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    LayerMask layerMask;
    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Default");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward), out hit, sightLineDistance, layerMask))
        {
            if(hit.collider.CompareTag("Player"))
            {
                Debug.DrawRay(sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Hit player");
                transitionObject.GetComponent<screenTransition>().FadeAndLoad(battleScene, transitionDuration);
            }
            else
            {
                Debug.DrawRay(sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                Debug.Log("Did not hit player, hit object with tag " + hit.transform.gameObject.tag.ToString());
                Debug.Log(hit.collider.CompareTag("Player"));
            }
            
        }
        else
        {
            Debug.DrawRay(sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward) * sightLineDistance, Color.white);
            Debug.Log("Hit nothing");
        }
    }
}
