using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SCR_AudioManager : MonoBehaviour
{
    public static SCR_AudioManager instance;
    
    public AudioMixer masterAudioMixer;
    public AudioMixer musicAudioMixer;
    public string masterVolParameter = "masterVolume";
    public string musicVolParameter = "musicVolume";

    [Header("Defaults")] 
    public float masterDefaultVol = 0.2f;
    public float musicDefaultVol = 0.2f;
    
    private float currentMasterVolume;
    private float currentMusicVolume;

    private AudioSource sliderAudioSource;
    public AudioClip tick;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        sliderAudioSource = GetComponent<AudioSource>();
        instance = this;
        DontDestroyOnLoad(gameObject);

        var saveData = SCR_SaveSystem.LoadGame();
        currentMasterVolume = (saveData.masterVolume > 0f) ? saveData.masterVolume : masterDefaultVol;
        currentMusicVolume = (saveData.musicVolume > 0f) ? saveData.musicVolume : musicDefaultVol;
        
        currentMasterVolume = Mathf.Clamp01(currentMasterVolume);
        float masterdB = Mathf.Lerp(-80f, 0f, Mathf.Clamp01(currentMasterVolume));
        masterAudioMixer.SetFloat(masterVolParameter, masterdB);
        
        float musicdB = Mathf.Lerp(-80f, 0f, Mathf.Clamp01(currentMusicVolume));
        musicAudioMixer.SetFloat(musicVolParameter, musicdB);
        
        SaveVolumes();
    }
    
    public void SetMasterVolume(float value)
    {
        sliderAudioSource.Play();
        currentMasterVolume = Mathf.Clamp01(value);
        float dB = Mathf.Lerp(-80f, 0f, Mathf.Clamp01(value));
        masterAudioMixer.SetFloat(masterVolParameter, dB);
        SaveVolumes();
    }

    public void SetMusicVolume(float value)
    {
        sliderAudioSource.Play();
        currentMusicVolume = value;
        float dB = Mathf.Lerp(-80f, 0f, Mathf.Clamp01(value));
        musicAudioMixer.SetFloat(musicVolParameter, dB);
        SaveVolumes();
    }
    
    public float GetCurrentMasterVolume() => currentMasterVolume;
    public float GetCurrentMusicVolume() => currentMusicVolume;

    private void SaveVolumes()
    {
        var saveData = SCR_SaveSystem.LoadGame();
        saveData.masterVolume = currentMasterVolume;
        saveData.musicVolume = currentMusicVolume;
        SCR_SaveSystem.SaveGame(saveData);
    }
}
