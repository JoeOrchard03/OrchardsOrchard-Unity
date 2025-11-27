using System.Collections.Generic;
using UnityEngine;

public class SCR_CameraZoomIn : MonoBehaviour, INT_Interactable
{
    public GameObject mainCamera;
    public GameObject leftZoomedCamera;
    public GameObject moveCameraRightButton;
    
    public List<Canvas> canvases = new List<Canvas>();
    
    public void Interact(GameObject interactor)
    {
        mainCamera.SetActive(false);
        leftZoomedCamera.SetActive(true);

        foreach (Canvas canvas in canvases)
        {
            canvas.worldCamera = leftZoomedCamera.GetComponent<Camera>();
        }

        moveCameraRightButton.SetActive(true);
        
        Debug.Log("SCR_CameraZoomIn interact");
        
    }
}
