using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenTransition : MonoBehaviour
{
    public Image faderImage;
    private void Start()
    {
        StartCoroutine(FadeOut());
    }
    public void FadeAndLoad(string sceneName, float duration, bool loadScene = true)
    {
        StartCoroutine(FadeIn(sceneName, duration, loadScene));
    }

    public IEnumerator FadeIn(string sceneName, float duration, bool loadScene)
    {
        Debug.Log("fading in");
        float t = 0;
        Color c = faderImage.color;
        while(t < duration)
        {
            t += Time.deltaTime;
            c.a = t / duration;
            faderImage.color = c;
            yield return null;
        }
        if (loadScene)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            StartCoroutine(FadeOut());
        }
    }

    public IEnumerator FadeOut()
    {
        Debug.Log("fading out");
        float t = 0;
        Color c = faderImage.color;
        while (t < 1)
        {
            t += Time.deltaTime;
            c.a = 1f - (t / 1f);
            faderImage.color = c;
            yield return null;
        }
    }
}
