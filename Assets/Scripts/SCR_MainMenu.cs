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
    
    [Header("Audio sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    
    [Header("Mouse variables")]
    public Texture2D cursorTexture;
    public Texture2D cursorHighlightTexture;
    
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        
        masterVolumeSlider.value = SCR_AudioManager.instance.GetCurrentMasterVolume();
        musicVolumeSlider.value = SCR_AudioManager.instance.GetCurrentMusicVolume();
        
        SCR_AudioManager.instance.SetMasterVolume(masterVolumeSlider.value);
        SCR_AudioManager.instance.SetMusicVolume(musicVolumeSlider.value);
        
        masterVolumeSlider.onValueChanged.AddListener(SCR_AudioManager.instance.SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SCR_AudioManager.instance.SetMusicVolume);
    }
    
    public void Play()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    
    public void OpenSettings()
    {
        Debug.Log("Opening settings");
        settingsMenu.SetActive(true);
        
        masterVolumeSlider.value = SCR_AudioManager.instance.GetCurrentMasterVolume();
        musicVolumeSlider.value = SCR_AudioManager.instance.GetCurrentMusicVolume();
    }

    public void ClosingSettings()
    {
        Debug.Log("Closing settings");
        settingsMenu.SetActive(false);
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
