using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Combat;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    // This script is meant to handle the interactables. It should highlight then when they're being looked at and within range.
    [Header("InteractSightline (Player Camera)")]
    [SerializeField] private Transform _sightLineOrigin;
    [SerializeField] private float _sightLineDistance;
    [Header("Excavating")]
    [SerializeField] private int _timeToExcavate = 1;
    [SerializeField] private int _audioListIndex = 0;
    [Header("Fossil Parts")]
    [SerializeField] private List<BodyPartSO> _inventoryList;
    [Header("Scene Transition")]
    [SerializeField] private ScreenTransition _transitionObject;
    [SerializeField] private string _creatureCombinerScene;
    [SerializeField] private float _transitionDuration;
    [Header("Interactable Text")]
    [SerializeField] private TextMeshProUGUI _interactableText;

    private LayerMask _layerMask;
    private GameObject _previousObject;
    private MapData _mapManager;
    private BattleDataLoader _battleManager;

    void Awake()
    {
        _layerMask = LayerMask.GetMask("Wall", "Interactables");
    }

    private void Start()
    {
        if(BattleDataLoader.Instance.WasBattleWon())
        {
            transform.position = MapData.Instance.GetPlayerPosition();
            transform.rotation = MapData.Instance.GetPlayerRotation();
        }
        else
        {
            transform.position = MapData.Instance.GetCheckpointPosition();
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
        // Uses a raycast to see what the player's looking at. Currently only used for the interactables such as the excavation sites.
        RaycastHit hit;
        if (Physics.Raycast(_sightLineOrigin.transform.position, _sightLineOrigin.transform.TransformDirection(Vector3.forward), out hit, _sightLineDistance, _layerMask))
        {
            // Tracks what is currently being hit by the raycast.
            _previousObject = hit.transform.gameObject;
            _interactableText.gameObject.SetActive(true);
        }
        else
        {
            if (_previousObject)
            {
                _previousObject = null;
            }
            _interactableText.gameObject.SetActive(false);
        }

        if (_transitionObject == null)
        {
            _transitionObject = FindAnyObjectByType<ScreenTransition>();
        }

        CheckForInput();
    }
    void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_previousObject && _previousObject.CompareTag("Interactable"))
            {
                //Adds random part from the list partsList to the player inventory.
                StartCoroutine(ExcavateEnumerator(_timeToExcavate, _previousObject));
            }
            else if (_previousObject && _previousObject.CompareTag("CreatureCombiner"))
            {
                // Sends you to the creature combiner screen.
                MapData.Instance.SavePlayerPosition(transform.position, transform.rotation);
                _transitionObject.FadeAndLoad(_creatureCombinerScene, _transitionDuration);
            }
        }
    }

    IEnumerator ExcavateEnumerator(int seconds, GameObject excavationObject)
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        yield return new WaitForSeconds(seconds);

        // Adds a new random part from all possible fossil parts.
        BodyPartSO randomBodyPartSO = PlayerInventory.Instance.FossilParts[Random.Range(0, PlayerInventory.Instance.FossilParts.Count)];
        DinosaurPart randomPart = new DinosaurPart(randomBodyPartSO);
        Debug.Log("Adding part " + randomPart.Reference.name);
        PlayerInventory.Instance.AddBodyPart(randomPart);
        PlayerInventory.Instance.FossilParts.Remove(randomBodyPartSO);
        Destroy(excavationObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            MapData.Instance.SavePlayerCheckpoint(other.gameObject.GetComponent<Checkpoint>().GetCheckpointPosition());
        }
    }
}
