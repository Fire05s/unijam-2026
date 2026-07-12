using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("InteractSightline")]
    public Transform sightLineOrigin;
    public float sightLineDistance;

    private LayerMask layerMask;
    private GameObject previousObject;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Interactibles");
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(sightLineOrigin.transform.position, sightLineOrigin.transform.TransformDirection(Vector3.forward), out hit, sightLineDistance, layerMask))
        {
            if (previousObject && hit.transform.gameObject != previousObject)
            {
                previousObject.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            }
            if (hit.collider.CompareTag("Interactible"))
            {
                Debug.DrawRay(sightLineOrigin.transform.position, sightLineOrigin.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Facing interactible");
                hit.transform.gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            }
            else
            {
                Debug.DrawRay(sightLineOrigin.transform.position, sightLineOrigin.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                Debug.Log("Not facing interactible, facing " + hit.transform.gameObject.tag.ToString());
            }
            previousObject = hit.transform.gameObject;
        }
        else
        {
            if (previousObject)
            {
                previousObject.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                previousObject = null;
            }
            Debug.DrawRay(sightLineOrigin.transform.position, sightLineOrigin.transform.TransformDirection(Vector3.forward) * sightLineDistance, Color.blue);
            Debug.Log("Hit nothing");
        }
    }
}
