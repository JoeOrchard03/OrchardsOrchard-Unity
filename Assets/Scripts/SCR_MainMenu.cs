using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SCR_MainMenu : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject settingsMenu;

    public AudioMixer audioMixer;

    public float masterDefaultVol = 0.2f;
    public float musicDefaultVol = 0.2f;
    
    public string masterVolParameter = "masterVolume";
    public string musicVolParameter = "musicVolume";
    
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    
    private void Start()
    {
        var saveData = SCR_SaveSystem.LoadGame();
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (saveData.masterVolume > 0f && saveData.musicVolume > 0f)
        {
            masterVolumeSlider.value = saveData.masterVolume;
            SetMasterVolume(saveData.masterVolume);
            
            musicVolumeSlider.value = saveData.musicVolume;
            SetMusicVolume(saveData.musicVolume);
        }
        else
        {
            masterVolumeSlider.value = masterDefaultVol;
            SetMasterVolume(masterDefaultVol);
            musicVolumeSlider.value = musicDefaultVol;
            SetMusicVolume(musicDefaultVol);
        }
        
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void SetMasterVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        audioMixer.SetFloat(masterVolParameter, dB);
    }
    
    public void SetMusicVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        audioMixer.SetFloat(musicVolParameter, dB);
    }
    
    public void Play()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    
    public void OpenSettings()
    {
        Debug.Log("Opening settings");
        settingsMenu.SetActive(true);
    }

    public void ClosingSettings()
    {
        Debug.Log("Closing settings");
        settingsMenu.SetActive(false);
        
        var saveData = SCR_SaveSystem.LoadGame();
        saveData.masterVolume = masterVolumeSlider.value;
        saveData.musicVolume = musicVolumeSlider.value;
        SCR_SaveSystem.SaveGame(saveData);
    }
    
    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
