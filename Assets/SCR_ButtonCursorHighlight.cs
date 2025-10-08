using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SCR_ButtonCursorHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private SCR_Interact playerInteractScriptRef;
    private SCR_MainMenu mainMenuScriptRef;
    private AudioSource playerAudioSource;
    public AudioClip buttonPressClip;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerObj.TryGetComponent(out playerInteractScriptRef);
            playerObj.TryGetComponent(out playerAudioSource);
        }
        
        if (playerInteractScriptRef == null)
        {
            GameObject menuObj = GameObject.FindWithTag("MainMenu");
            if (menuObj != null)
            {
                menuObj.TryGetComponent(out mainMenuScriptRef);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playerInteractScriptRef != null)
        {
            playerInteractScriptRef.SetCursorHighlight(true);
        }
        else if (mainMenuScriptRef != null)
        {
            mainMenuScriptRef.SetCursorHighlight(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (playerInteractScriptRef != null)
        {
            playerInteractScriptRef.SetCursorHighlight(false);
        }
        else if (mainMenuScriptRef != null)
        {
            mainMenuScriptRef.SetCursorHighlight(false);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (playerInteractScriptRef != null)
        {
            playerAudioSource.PlayOneShot(buttonPressClip, 1.0f);
            playerInteractScriptRef.SetCursorHighlight(false);
        }
        else if (mainMenuScriptRef != null)
        {
            mainMenuScriptRef.SetCursorHighlight(false);
        }
    }
}
