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

    public GameObject mainCamera;
    public GameObject leftZoomedCamera;
    public GameObject rightZoomedCamera;
    
    private bool wasZoomedBeforeOpening = false;

    private void Start()
    {
        compendiumAudioSource = gameObject.GetComponent<AudioSource>();
    }
        
    public void Interact(GameObject interactor)
    {
        Debug.Log("Interacting with compendium");
        if (compendiumCanvas.activeInHierarchy)
        {
            CloseCompendium();
            return;
        }
        
        compendiumAudioSource.PlayOneShot(compendiumOpen);
        compendiumCanvas.SetActive(true);
        
        wasZoomedBeforeOpening = !mainCamera.activeInHierarchy;

        if (wasZoomedBeforeOpening)
        {
            GetComponent<SCR_CameraZoomOut>().Interact(this.gameObject);
        }
        
        compendium.GetComponent<SCR_Compendium>().OpenCompendium();
    }

    public void CloseCompendium()
    {
        compendiumAudioSource.PlayOneShot(compendiumClose, 0.25f);
        compendiumCanvas.SetActive(false);
        
        if (wasZoomedBeforeOpening)
        {
            Debug.Log("Camera is zoomed, zooming in");
            gameObject.GetComponent<SCR_CameraZoomIn>().Interact(this.gameObject);
        }
    }
    
}
