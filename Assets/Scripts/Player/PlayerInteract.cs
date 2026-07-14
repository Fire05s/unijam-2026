using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteract : MonoBehaviour
{
    //This script is meant to handle the interactables. It should highlight then when they're being looked at and within range.
    [Header("InteractSightline (Player Camera)")]
    [SerializeField] private Transform _sightLineOrigin;
    [SerializeField] private float _sightLineDistance;
    [Header("Fossil Parts")]
    [SerializeField] private List<BodyPartSO> _inventoryList;
    [Header("Scene Transition")]
    [SerializeField] private ScreenTransition _transitionObject;
    [SerializeField] private string _creatureCombinerScene;
    [SerializeField] private float _transitionDuration;

    private List<DinosaurPart> _partsList = new();

    public IReadOnlyList<DinosaurPart> BodyParts => _partsList;

    private PlayerInventory _playerInventory;
    private LayerMask _layerMask;
    private GameObject _previousObject;
    private MapData _mapManager;

    void Awake()
    {
        _layerMask = LayerMask.GetMask("Wall", "Interactables");
        _mapManager = GameObject.Find("MapDataManager").GetComponent<MapData>();
        foreach (var partData in _inventoryList)
        {
            _partsList.Add(new DinosaurPart(partData));
        }
    }

    private void Start()
    {
        transform.position = _mapManager.GetPlayerPosition();
        _playerInventory = GameObject.Find("Inventory").GetComponent<PlayerInventory>();
        List<DinosaurPart> playerPartsInventory = _playerInventory.GetBodyParts();

        //Goes through each part in the given list of parts from the inspector and compares it to what the player already has. If the player has a part, remove that part from the pool.
        for(int i = 0; i < playerPartsInventory.Count; i++)
        {
            for(int j = 0; j < _partsList.Count; j++)
            {
                //Debug.Log("Checking " + playerPartsInventory[i].Reference + " and " + _partsList[j].Reference);
                if(playerPartsInventory[i].Reference == _partsList[j].Reference)
                {
                    //Debug.Log("REMOVING " + _partsList[j]);
                    _partsList.RemoveAt(j);
                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_sightLineOrigin == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        Gizmos.DrawRay(_sightLineOrigin.position, direction * _sightLineDistance);
    }


    private void Update()
    {
        //Uses a raycast to see what the player's looking at. Currently only used for the interactables such as the excavation sites.
        RaycastHit hit;
        if (Physics.Raycast(_sightLineOrigin.transform.position, _sightLineOrigin.transform.TransformDirection(Vector3.forward), out hit, _sightLineDistance, _layerMask))
        {
            //Tracks what is currently being hit by the raycast.
            _previousObject = hit.transform.gameObject;
        }
        else
        {
            if (_previousObject)
            {
                _previousObject = null;
            }
        }
        CheckForInput();
    }
    void CheckForInput()
    {
        //I'm going to be honest I haven't used the new input system much so I'm just using the old one for the moment for the sake of writing this code tonight. I plan on switching it over.
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_previousObject && _previousObject.CompareTag("Interactable"))
            {
                //Adds random part from the list partsList to the player inventory.
                DinosaurPart randomPart = _partsList[Random.Range(0, _partsList.Count)];
                Debug.Log("Adding part " + randomPart.Reference.name);
                _playerInventory.AddBodyPart(randomPart);
                Destroy(_previousObject);
            }
            else if (_previousObject && _previousObject.CompareTag("CreatureCombiner"))
            {
                //Sends you to the creature combiner screen.
                _mapManager.SavePlayerPosition(transform.position);
                _transitionObject.FadeAndLoad(_creatureCombinerScene, _transitionDuration);
            }
        }
    }
}
