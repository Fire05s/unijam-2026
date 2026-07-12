using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class screenTransition : MonoBehaviour
{
    public Image faderImage;
    private void Start()
    {
        StartCoroutine(FadeOut());
    }
    public void FadeAndLoad(string sceneName, float duration)
    {
        StartCoroutine(Fader(sceneName, duration));
    }

    IEnumerator Fader(string sceneName, float duration)
    {
        float t = 0;
        Color c = faderImage.color;
        while(t < duration)
        {
            t += Time.deltaTime;
            c.a = t / duration;
            faderImage.color = c;
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOut()
    {
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
