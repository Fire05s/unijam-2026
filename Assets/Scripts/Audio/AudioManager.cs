using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public AudioClip Music;
    [SerializeField] public AudioClip[] SFX;

    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioSource music;
    
    void PlaySFX(int index)
    {
        sfx.clip = SFX[index];
        sfx.Play();
    }
    void PlayMusic()
    {
        music.clip = Music;
        music.Play();
    }
}
