using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Compost : MonoBehaviour, INT_Interactable
{
    private bool composting = false;
    private SCR_PlayerManager playerScriptRef;
    
    public Texture2D shovelTexture;
    public Texture2D cursorTexture;
    public AudioSource composterAudioSource;
    public AudioClip shovelPickUp;
    public AudioClip shovelDrop;

    private void Start()
    {
        playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerManager>();
    }
    
    public void Interact(GameObject interactor)
    {
        if (!composting)
        {
            composterAudioSource.PlayOneShot(shovelPickUp);
            Debug.Log("Enabling composting");
            playerScriptRef.composting = true;
            composting = true;
        }
        else
        {
            composterAudioSource.PlayOneShot(shovelDrop);
            Debug.Log("Disabling composting");
            playerScriptRef.composting = false;
            composting = false;
        }
    }
}
