using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject _originatorGO;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensXSlider;
    [SerializeField] private Slider sensYSlider;

    private float defaultValue = 1.0f;

    private void OnEnable()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", defaultValue);
        sensXSlider.value = PlayerPrefs.GetFloat("XSens", defaultValue);
        sensYSlider.value = PlayerPrefs.GetFloat("YSens", defaultValue * 3);
    }

    public void Back()
    {
        _originatorGO.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("XSens", sensXSlider.value);
        PlayerPrefs.SetFloat("YSens", sensYSlider.value * 3);
    }
}
