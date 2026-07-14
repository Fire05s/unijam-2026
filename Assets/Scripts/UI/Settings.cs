using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensXSlider;
    [SerializeField] private Slider sensYSlider;

    private float defaultValue = 0.5f;

    private void OnEnable()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", defaultValue);
        sensXSlider.value = PlayerPrefs.GetFloat("SensX", defaultValue);
        sensYSlider.value = PlayerPrefs.GetFloat("SensY", defaultValue);
    }

    public void Back()
    {
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("SensX", sensXSlider.value);
        PlayerPrefs.SetFloat("SensY", sensYSlider.value);
    }
}
