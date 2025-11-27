using System.Collections.Generic;
using UnityEngine;

public class SCR_CameraZoomOut : MonoBehaviour, INT_Interactable
{
    public GameObject mainCamera;
    public GameObject leftZoomedCamera;
    public GameObject rightZoomedCamera;
    
    public List<Canvas> canvases = new List<Canvas>();
    
    public void Interact(GameObject interactor)
    {
        mainCamera.SetActive(true);
        leftZoomedCamera.SetActive(false);
        rightZoomedCamera.SetActive(false);

        foreach (Canvas canvas in canvases)
        {
            canvas.worldCamera = mainCamera.GetComponent<Camera>();
        }
    }
}