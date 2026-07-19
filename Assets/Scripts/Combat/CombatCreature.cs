using Unity.Cinemachine;
using UnityEngine;

public class CombatCreature : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CreatureModel _slotModel;
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private Camera _slotCam;
    [SerializeField] private CinemachineCamera _followCam;
    public CreatureModel SlotModel => _slotModel;
    public Transform LookTarget => _lookTarget;
    public RenderTexture CameraTexture { get; private set; }
    public CinemachineCamera FollowCamera => _followCam;

    private void Awake()
    {
        GenerateRenderTexture();
    }

    private void OnDestroy()
    {
        if (CameraTexture != null)
        {
            CameraTexture.Release();
            _slotCam.targetTexture = null;
            Destroy(CameraTexture);
        }
    }

    private void GenerateRenderTexture()
    {
        int width = 512;
        int height = 512;
        int depth = 24;

        CameraTexture = new RenderTexture(width, height, depth);
        CameraTexture.Create();
        _slotCam.targetTexture = CameraTexture;
    }

    public void SetModel(CreatureModel model)
    {
        if (model == null)
        {
            Debug.LogWarning($"Model for doesn't exist, using default");
            return;
        }
        if (_slotModel != null)
        {
            Destroy(_slotModel.gameObject);
        }
        _slotModel = Instantiate(model, transform);
    }
}
