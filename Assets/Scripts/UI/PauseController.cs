using Combat;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    [SerializeField] private ScreenTransition _transition;
    [SerializeField] private GameObject _pauseGO;
    [SerializeField] private GameObject _settingsGO;
    [SerializeField] private PlayerCam _playerCam;

    private bool _pauseActive = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _pauseActive = !_pauseActive;
            _pauseGO.SetActive(_pauseActive);
            _playerCam.CamUnlocked = !_pauseActive;
        }

        if (_pauseActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _settingsGO.SetActive(false);
        }

        _playerCam.UpdateSens();
    }

    public void Resume()
    {
        _playerCam.CamUnlocked = true;
        _pauseActive = false;
        _pauseGO.SetActive(false);
    }

    public void Settings()
    {
        _settingsGO.SetActive(true);
        _pauseGO.SetActive(false);
    }

    public void MainMenu()
    {
        Destroy(PlayerInventory.Instance.gameObject);
        Destroy(BattleDataLoader.Instance.gameObject);
        Destroy(MapData.Instance.gameObject);
        _transition.FadeAndLoad("MainMenu", duration: 1f);
    }

    public void Quit()
    {
        Application.Quit();
    }
}