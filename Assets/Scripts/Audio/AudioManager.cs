using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] _musicClips;
    [SerializeField] private AudioClip[] _sfxClips;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSourceActive;
    [SerializeField] private AudioSource _musicSourceFade;

    [Header("Ranges")]
    [SerializeField] private int _dinoScreechStart;
    [SerializeField] private int _dinoScreechEnd;
    [SerializeField] private int _hitStart;
    [SerializeField] private int _hitEnd;
    [SerializeField] private int _walkStart;
    [SerializeField] private int _walkEnd;

    public float GetMusicVolume() => _musicVolume;
    public float GetSFXVolume() => _sfxVolume;

    private const string MUSIC_VOL_KEY = "MusicVolume";
    private const string SFX_VOL_KEY = "SFXVolume";

    private float _musicVolume = 1.0f;
    private float _sfxVolume = 1.0f;
    private Dictionary<string, AudioSource> _loopingSFX = new Dictionary<string, AudioSource>();
    private Dictionary<GameObject, AudioSource> inWorldLoops = new Dictionary<GameObject, AudioSource>();
    private Coroutine _crossfadeCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolumeSettings();
    }

    private void Start()
    {
        _musicSourceActive.loop = true;
        if (_musicSourceFade != null) _musicSourceFade.loop = true;
        PlayMusic(0);
    }

    public void LoadVolumeSettings()
    {
        _musicVolume = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 1f);
        _sfxVolume = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f);

        _musicSourceActive.volume = _musicVolume;
        if (_musicSourceFade != null) _musicSourceFade.volume = 0f;
        _sfxSource.volume = _sfxVolume;

        if (_crossfadeCoroutine == null)
        {
            _musicSourceActive.volume = _musicVolume;
        }

        foreach (var source in _loopingSFX.Values)
        {
            if (source != null) source.volume = _sfxVolume;
        }
        foreach (var source in inWorldLoops.Values)
        {
            if (source != null) source.volume = _sfxVolume;
        }
    }

    public void PlayScreechSFX()
    {
        PlayInstantSFX(Random.Range(_dinoScreechStart, _dinoScreechEnd));
    }

    public void PlayHitSFX()
    {
        PlayInstantSFX(Random.Range(_hitStart, _hitEnd));
    }

    public void PlayWalkSFX()
    {
        PlayInstantSFX(Random.Range(_walkStart, _walkEnd));
    }

    public void PlayInstantSFX(int index)
    {
        if (index < 0 || index >= _sfxClips.Length) return;
        _sfxSource.PlayOneShot(_sfxClips[index], _sfxVolume);
    }

    public void PlayLoopingSFX(int index, string loopKey)
    {
        if (index < 0 || index >= _sfxClips.Length) return;
        if (_loopingSFX.ContainsKey(loopKey)) return;

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = _sfxClips[index];
        newSource.loop = true;
        newSource.volume = _sfxVolume;
        newSource.Play();

        _loopingSFX.Add(loopKey, newSource);
    }

    public void StopLoopingSFX(string loopKey)
    {
        if (_loopingSFX.TryGetValue(loopKey, out AudioSource source))
        {
            if (source != null)
            {
                source.Stop();
                Destroy(source);
            }
            _loopingSFX.Remove(loopKey);
        }
    }

    public void PlayInWorldLoop(int index, GameObject emitter)
    {
        if (index < 0 || index >= _sfxClips.Length || emitter == null) return;
        if (inWorldLoops.ContainsKey(emitter)) return; // Already playing a loop on this object

        // Create a new AudioSource directly on the in-world object
        AudioSource source = emitter.AddComponent<AudioSource>();
        source.clip = _sfxClips[index];
        source.loop = true;
        source.volume = _sfxVolume;

        source.spatialBlend = 1.0f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 1.0f;
        source.maxDistance = 20.0f;

        source.Play();
        inWorldLoops.Add(emitter, source);
    }

    public void StopInWorldLoop(GameObject emitter)
    {
        if (emitter == null) return;

        if (inWorldLoops.TryGetValue(emitter, out AudioSource source))
        {
            if (source != null)
            {
                source.Stop();
                Destroy(source);
            }
            inWorldLoops.Remove(emitter);
        }
    }

    public void PlayMusic(int index, float fadeDuration = 1.5f)
    {
        if (index < 0 || index >= _musicClips.Length) return;
        AudioClip newClip = _musicClips[index];

        if (_musicSourceActive.isPlaying && _musicSourceActive.clip == newClip) return;

        if (_crossfadeCoroutine != null)
        {
            StopCoroutine(_crossfadeCoroutine);
        }

        _crossfadeCoroutine = StartCoroutine(CrossfadeMusic(newClip, fadeDuration));
    }

    public void StopMusic(float fadeDuration = 1.5f)
    {
        if (_crossfadeCoroutine != null)
        {
            StopCoroutine(_crossfadeCoroutine);
        }

        _crossfadeCoroutine = StartCoroutine(FadeOutActiveMusic(fadeDuration));
    }

    private IEnumerator CrossfadeMusic(AudioClip newClip, float duration)
    {
        _musicSourceFade.clip = newClip;
        _musicSourceFade.loop = true;
        _musicSourceFade.volume = 0f;
        _musicSourceFade.Play();

        float startActiveVol = _musicSourceActive.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float percent = timer / duration;
            _musicSourceActive.volume = Mathf.Lerp(startActiveVol, 0f, percent);
            _musicSourceFade.volume = Mathf.Lerp(0f, _musicVolume, percent);
            yield return null;
        }

        // stop old track
        _musicSourceActive.Stop();
        _musicSourceActive.volume = 0f;

        // swap active track
        AudioSource temp = _musicSourceActive;
        _musicSourceActive = _musicSourceFade;
        _musicSourceFade = temp;

        // bring back new track
        _musicSourceActive.volume = _musicVolume;
        _crossfadeCoroutine = null;
    }

    private IEnumerator FadeOutActiveMusic(float duration)
    {
        float startActiveVol = _musicSourceActive.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float percent = timer / duration;
            _musicSourceActive.volume = Mathf.Lerp(startActiveVol, 0f, percent);
            yield return null;
        }

        _musicSourceActive.Stop();
        _musicSourceActive.volume = 0f;
        _crossfadeCoroutine = null;
    }
}