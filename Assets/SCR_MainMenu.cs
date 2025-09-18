using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_MainMenu : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject settingsMenu;
    
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
    }
    
    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
