using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Compost : MonoBehaviour, INT_Interactable
{
    private bool composting = false;
    private SCR_Interact playerScriptRef;
    
    public Texture2D shovelTexture;
    public Texture2D cursorTexture;

    private void Start()
    {
        playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_Interact>();
    }
    
    public void Interact(GameObject interactor)
    {
        if (!composting)
        {
            Debug.Log("Enabling composting");
            playerScriptRef.composting = true;
            composting = true;
        }
        else
        {
            Debug.Log("Disabling composting");
            playerScriptRef.composting = false;
            composting = false;
        }
    }
}
