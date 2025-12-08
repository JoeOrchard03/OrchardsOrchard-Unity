using System.Collections.Generic;
using UnityEngine;

public class SCR_CameraZoomOut : MonoBehaviour, INT_Interactable
{
    public GameObject mainCamera;
    public GameObject leftZoomedCamera;
    public GameObject rightZoomedCamera;
    
    public SCR_HoverName hoverNameOBJ;

    public GameObject moveCameraRightButton;
    public GameObject moveCameraLeftButton;
    
    public List<Canvas> canvases = new List<Canvas>();
    
    public void Interact(GameObject interactor)
    {
        mainCamera.SetActive(true);
        leftZoomedCamera.SetActive(false);
        rightZoomedCamera.SetActive(false);

        moveCameraRightButton.SetActive(false);
        moveCameraLeftButton.SetActive(false);
        
        foreach (Canvas canvas in canvases)
        {
            canvas.worldCamera = mainCamera.GetComponent<Camera>();
        }
        
        hoverNameOBJ.cam = mainCamera.GetComponent<Camera>();
    }
}