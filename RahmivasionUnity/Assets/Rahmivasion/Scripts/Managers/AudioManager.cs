using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource mainMenuMusic;
    [SerializeField] private AudioSource gameMusic;
    [SerializeField] private AudioSource soundEffect;

    private float masterVolume = 1.0f;
    private float musicVolume = 1.0f;
    private float sfxVolume = 1.0f;

    private static AudioManager instance;

    public static AudioManager GetInstance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            mainMenuMusic.Play();
        }
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public float GetMusicVolume()
    {
        return gameMusic.volume;
    }

    public float GetSFXVolume()
    {
        return soundEffect.volume;
    }

    public void SetMasterVolume(float newVolume)
    {
        masterVolume = newVolume;
        gameMusic.volume = musicVolume * masterVolume;
        mainMenuMusic.volume = sfxVolume * masterVolume;
    }

    public void ChangeMusicVolume(float newVolume)
    {
        musicVolume = newVolume;
        gameMusic.volume = musicVolume * masterVolume;
        mainMenuMusic.volume = musicVolume * masterVolume;
    }

    public void ChangeSFXVolume(float newVolume)
    {
        sfxVolume = newVolume;
        soundEffect.volume = sfxVolume * masterVolume;
    }

    public void PlaySoundEffect(AudioClip soundToPlay)
    {
        soundEffect.clip = soundToPlay;
        soundEffect.Play();
    }

    public void PlayGameMusic()
    {
        StopAllMusic();
        gameMusic.Play();
    }

    public void PlayMainMenuMusic()
    {
        StopAllMusic();
        mainMenuMusic.Play();  
    }

    private void StopAllMusic()
    {
        gameMusic.Stop();
        mainMenuMusic.Stop();
    }
}
