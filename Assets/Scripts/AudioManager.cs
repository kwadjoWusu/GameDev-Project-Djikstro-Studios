using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private AudioSource audioSource;
    
    void Awake()
    {
        // Singleton pattern - only one AudioManager will exist
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlayMusic(AudioClip music)
    {
        if (audioSource.clip == music && audioSource.isPlaying)
            return;
            
        audioSource.clip = music;
        audioSource.Play();
    }
    
    public void StopMusic()
    {
        StartCoroutine(FadeOutMusic());
    }
    
    public void ChangeMusic(AudioClip newMusic)
    {
        StartCoroutine(FadeAndChange(newMusic));
    }
    
    private IEnumerator FadeOutMusic()
    {
        float startVolume = audioSource.volume;
        
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / 1.5f;
            yield return null;
        }
        
        audioSource.Stop();
        audioSource.volume = startVolume;
    }
    
    private IEnumerator FadeAndChange(AudioClip newMusic)
    {
        float startVolume = audioSource.volume;
        
        // Fade out
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / 1.5f;
            yield return null;
        }
        
        audioSource.Stop();
        audioSource.clip = newMusic;
        audioSource.Play();
        
        // Fade in
        while (audioSource.volume < startVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / 1.5f;
            yield return null;
        }
    }
}