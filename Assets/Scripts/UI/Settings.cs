using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject _originatorGO;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sensXSlider;
    [SerializeField] private Slider sensYSlider;
    [Header("Audio")]
    [SerializeField] private int _audioListIndex = 0;

    private float defaultValue = 1.0f;

    private void OnEnable()
    {
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", defaultValue);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", defaultValue);
        sensXSlider.value = PlayerPrefs.GetFloat("XSens", defaultValue);
        sensYSlider.value = PlayerPrefs.GetFloat("YSens", defaultValue * 3);
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("XSens", sensXSlider.value);
        PlayerPrefs.SetFloat("YSens", sensYSlider.value * 3);
        _originatorGO.SetActive(true);
        gameObject.SetActive(false);
        AudioManager.Instance?.LoadVolumeSettings();
        AudioManager.Instance.PlayInstantSFX(_audioListIndex);
    }
}
