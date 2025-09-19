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
    
    [Header("Audio settings")]
    public AudioMixer audioMixer;
    public string masterVolParameter = "masterVolume";
    public string musicVolParameter = "musicVolume";
    public float masterDefaultVol = 0.2f;
    public float musicDefaultVol = 0.2f;
    
    [Header("Audio sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    
    [Header("Mouse variables")]
    public Texture2D cursorTexture;
    public Texture2D cursorHighlightTexture;
    
    private void Start()
    {
        var saveData = SCR_SaveSystem.LoadGame();
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

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
    
    public void SetCursorHighlight(bool cursorHighlight)
    {
        if (cursorHighlight)
        {
            Cursor.SetCursor(cursorHighlightTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }
}
