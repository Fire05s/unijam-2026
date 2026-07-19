using UnityEngine;

public class LoseScreen : MonoBehaviour
{
    [SerializeField] private ScreenTransition _transition;
    [SerializeField] private GameObject _mainMenuGO;
    [SerializeField] private GameObject _settingsGO;
    [Header("Audio")]
    [SerializeField] private int _audioListIndex = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Tutorial()
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        _transition.FadeAndLoad("CombinerTutorial", duration: 2f);
    }

    public void ContinueGame()
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        _transition.FadeAndLoad("MainLevel", duration: 2f);
    }

    public void MainMenu()
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        _transition.FadeAndLoad("MainMenu", duration: 2f);
    }

    public void Settings()
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        _mainMenuGO.SetActive(false);
        _settingsGO.SetActive(true);
    }

    public void Quit()
    {
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
        Application.Quit();
    }
}
