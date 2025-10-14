using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CompendiumOpen : MonoBehaviour, INT_Interactable
{
    public GameObject compendium;
    public GameObject compendiumCanvas;
    private AudioSource compendiumAudioSource;
    public AudioClip compendiumOpen;
    public AudioClip compendiumClose;

    private void Start()
    {
        compendiumAudioSource = gameObject.GetComponent<AudioSource>();
    }
        
    public void Interact(GameObject interactor)
    {
        Debug.Log("Interacting with compendium");
        if (compendiumCanvas.activeInHierarchy)
        {
            compendiumAudioSource.PlayOneShot(compendiumClose, 0.5f);
            compendiumCanvas.SetActive(false);
        }
        else
        {
            compendiumAudioSource.PlayOneShot(compendiumOpen);
            compendiumCanvas.SetActive(true);
            compendium.GetComponent<SCR_Compendium>().OpenCompendium();
        }
    }

    public void CloseCompendium()
    {
        compendiumAudioSource.PlayOneShot(compendiumClose, 0.25f);
        compendiumCanvas.SetActive(false);
    }
}
