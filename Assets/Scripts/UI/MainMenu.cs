using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private ScreenTransition transition;

    public void Play()
    {
        transition.FadeAndLoad("MainLevel", duration: 2f);
    }

    public void Setting()
    {
        
    }

    public void Exit()
    {
        Application.Quit();
    }
}