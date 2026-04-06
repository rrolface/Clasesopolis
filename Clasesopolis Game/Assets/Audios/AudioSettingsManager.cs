using UnityEngine;
using UnityEngine.Audio; // ¡No olvides esto!
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Referencias")]
    public AudioMixer mainMixer;

    [Header("Sliders")]
    public Slider sliderMusica;
    public Slider sliderVoces;
    public Slider sliderSFX;

    void Start()
    {
        // Cargar valores guardados o poner 0.75 por defecto
        sliderMusica.value = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        sliderVoces.value = PlayerPrefs.GetFloat("VoiceVol", 0.75f);
        sliderSFX.value = PlayerPrefs.GetFloat("SFXVol", 0.75f);

        // Aplicar los volúmenes al iniciar
        SetMusicVolume(sliderMusica.value);
        SetVoiceVolume(sliderVoces.value);
        SetSFXVolume(sliderSFX.value);
    }

    public void SetMusicVolume(float value)
    {
        // Convertimos el valor 0-1 a decibelios (-80 a 20)
        mainMixer.SetFloat("musicVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVol", value);
    }

    public void SetVoiceVolume(float value)
    {
        mainMixer.SetFloat("voiceVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("VoiceVol", value);
    }

    public void SetSFXVolume(float value)
    {
        mainMixer.SetFloat("sfxVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVol", value);
    }
}