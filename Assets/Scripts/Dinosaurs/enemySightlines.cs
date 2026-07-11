using UnityEngine;

public class enemySightlines : MonoBehaviour
{
    public Transform sightLineOrigin;
    public float sightLineDirection;
    private LineRenderer laserLine;
    private bool hasLineOfSight;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        laserLine = GetComponent<LineRenderer>();
    }
    LayerMask layerMask;
    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Character");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(sightLineOrigin.transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }
}
