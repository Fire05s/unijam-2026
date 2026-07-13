using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteract : MonoBehaviour
{
    //This script is meant to handle the interactables. It should highlight then when they're being looked at and within range.
    [Header("InteractSightline")]
    public Transform sightLineOrigin;
    public float sightLineDistance;
    [Header("Fossil Parts")]
    [SerializeField] private List<BodyPartSO> inventoryList;

    public static PlayerInventory Instance { get; private set; }
    private List<DinosaurPart> partsList = new();

    public IReadOnlyList<DinosaurPart> BodyParts => partsList;

    private PlayerInventory playerInventory;
    private LayerMask layerMask;
    private GameObject previousObject;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Interactables");

        foreach (var partData in inventoryList)
        {
            partsList.Add(new DinosaurPart(partData));
        }
    }

    private void Start()
    {
        Debug.Log(inventoryList.Count);
        Debug.Log(partsList.Count);
        playerInventory = GameObject.Find("Inventory").GetComponent<PlayerInventory>();
        Debug.Log(playerInventory);
        List<DinosaurPart> playerPartsInventory = playerInventory.GetBodyParts();
        for(int i = 0; i < playerPartsInventory.Count; i++)
        {
            Debug.Log(playerPartsInventory[i].Reference + " " + partsList.Count);
            for(int j = 0; j < partsList.Count; j++)
            {
                Debug.Log("Checking " + playerPartsInventory[i].Reference + " and " + partsList[j].Reference);
                if(playerPartsInventory[i].Reference == partsList[j].Reference)
                {
                    Debug.Log("REMOVING " + partsList[j] + " But not actually since I still need it");
                    //partsList.RemoveAt(j);
                    break;
                }
            }
        }
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (previousObject && previousObject.CompareTag("Interactable"))
            {
                //Adds random part from the list partsList to the player inventory.
                Debug.Log("Interactable interacted with.");
                DinosaurPart randomPart = partsList[Random.Range(0, partsList.Count)];
                Debug.Log("Adding part " + randomPart.Reference.name);
                playerInventory.AddBodyPart(randomPart);

            }
        }
    }
}
