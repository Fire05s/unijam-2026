using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private ScreenTransition transition;
    [SerializeField] private GameObject SettingsGO;

    public void Play()
    {
        transition.FadeAndLoad("CombinerTutorial", duration: 2f);
    }

    public void Setting()
    {
        SettingsGO.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}