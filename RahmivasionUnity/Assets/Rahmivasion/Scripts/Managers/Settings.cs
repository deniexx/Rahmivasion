using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider sfxVolume;

    public void Awake()
    {
        volumeSlider.value = AudioManager.GetInstance().GetMasterVolume();
        musicVolume.value = AudioManager.GetInstance().GetMusicVolume();
        sfxVolume.value = AudioManager.GetInstance().GetSFXVolume();
    }

    public void OnVolumeChanged()
    {
        AudioManager.GetInstance().SetMasterVolume(volumeSlider.value);
    }

    public void OnMusicVolumeChanged()
    {
        AudioManager.GetInstance().ChangeMusicVolume(musicVolume.value);
    }

    public void OnSFXVolumeChanged()
    {
        AudioManager.GetInstance().ChangeSFXVolume(sfxVolume.value);
    }
}
