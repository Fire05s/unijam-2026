using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public event Action<CinemachineCamera> CameraChanged;

    [Serializable]
    public class CameraEntry
    {
        public int Id;
        public CinemachineCamera Camera;
    }

    [Header("Registered Cameras")]
    [SerializeField] private List<CameraEntry> _cameras = new();
    [SerializeField] private Camera worldCamera;

    [Header("Settings")]
    [SerializeField] private int _activePriority = 100;
    [SerializeField] private int _inactivePriority = 0;

    private Dictionary<int, CinemachineCamera> _cameraLookup;

    public CinemachineCamera CurrentCamera { get; private set; }
    public CinemachineCamera PreviousCamera { get; private set; }
    public Camera WorldCamera => worldCamera;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _cameraLookup = new Dictionary<int, CinemachineCamera>();

        foreach (var entry in _cameras)
        {
            if (entry.Camera == null)
                continue;

            _cameraLookup[entry.Id] = entry.Camera;

            if (entry.Camera.Priority > _inactivePriority)
                CurrentCamera = entry.Camera;
        }
    }

    public void AddCamera(CameraEntry entry)
    {
        if (entry.Camera == null) return;

        _cameraLookup[entry.Id] = entry.Camera;

        if (entry.Camera.Priority > _inactivePriority)
            CurrentCamera = entry.Camera;
    }

    public void SwitchCamera(int id)
    {
        if (_cameraLookup.TryGetValue(id, out var cam))
            SwitchCamera(cam);
    }

    public void SwitchCamera(CinemachineCamera target)
    {
        if (target == null || target == CurrentCamera)
            return;

        PreviousCamera = CurrentCamera;

        foreach (var pair in _cameraLookup)
            pair.Value.Priority = _inactivePriority;

        target.Priority = _activePriority;

        CurrentCamera = target;

        CameraChanged?.Invoke(CurrentCamera);
    }

    public void SetLookAt(Transform target)
    {
        CurrentCamera.LookAt = target;
    }

    public void SwitchToPrevious()
    {
        if (PreviousCamera != null)
            SwitchCamera(PreviousCamera);
    }
}
