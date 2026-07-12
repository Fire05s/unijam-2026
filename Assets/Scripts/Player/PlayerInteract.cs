using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    //This script is meant to handle the interactables. It should highlight then when they're being looked at and within range.
    [Header("InteractSightline")]
    public Transform sightLineOrigin;
    public float sightLineDistance;

    private LayerMask layerMask;
    private GameObject previousObject;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Interactables");
    }

    private void Update()
    {
        //Uses a raycast to see what the player's looking at. Currently only used for the interactables such as the excavation sites.
        RaycastHit hit;
        if (Physics.Raycast(sightLineOrigin.transform.position, sightLineOrigin.transform.TransformDirection(Vector3.forward), out hit, sightLineDistance, layerMask))
        {
            //When an object is no longer being looked at, change its material back to normal by disabling the emission.
            if (previousObject && hit.transform.gameObject != previousObject)
            {
                previousObject.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            }
            //If it's an interactable that's being looked at, highlight it by turning on the emission on its material. Can be swapped for a different form of highlighting here if something else works better.
            if (hit.collider.CompareTag("Interactable"))
            {
                Debug.DrawRay(sightLineOrigin.transform.position, sightLineOrigin.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                //Debug.Log("Facing interactable");
                hit.transform.gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            }
            else
            {
                Debug.DrawRay(sightLineOrigin.transform.position, sightLineOrigin.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                //Debug.Log("Not facing interactable, facing " + hit.transform.gameObject.tag.ToString());
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
            //Debug.Log("Hit nothing");
        }
        CheckForInput();
    }
    void CheckForInput()
    {
        //I'm going to be honest I haven't used the new input system much so I'm just using the old one for the moment for the sake of writing this code tonight. I plan on switching it over.
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");
            if (previousObject && previousObject.CompareTag("Interactable"))
            {
                //Replace later on with what the interactable should do.
                Debug.Log("Interactable interacted with.");
            }
        }
    }
}
