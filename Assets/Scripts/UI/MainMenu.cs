using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private ScreenTransition _transition;
    [SerializeField] private GameObject _mainMenuGO;
    [SerializeField] private GameObject _settingsGO;

    public void Tutorial()
    {
        _transition.FadeAndLoad("CombinerTutorial", duration: 2f);
    }

    public void MainGame()
    {
        _transition.FadeAndLoad("MainLevel", duration: 2f);
    }

    public void Settings()
    {
        _mainMenuGO.SetActive(false);
        _settingsGO.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}