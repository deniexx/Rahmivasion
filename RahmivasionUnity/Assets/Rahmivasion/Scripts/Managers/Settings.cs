using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    public void Awake()
    {
        volumeSlider.value = AudioListener.volume;
    }

    public void OnVolumeChanged()
    {
        AudioListener.volume = volumeSlider.value;
    }
}
