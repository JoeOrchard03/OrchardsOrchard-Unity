using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class SCR_MoveCamera : MonoBehaviour, INT_Interactable
{
    public GameObject mainCamera;
    public GameObject leftCamera;
    public GameObject rightCamera;
    
    public GameObject moveCameraRightButton;
    public GameObject moveCameraLeftButton;
    
    public List<Canvas> canvases = new List<Canvas>();
    
    public void Interact(GameObject interactor)
    {
        if (leftCamera.activeInHierarchy)
        {
            rightCamera.SetActive(true);
            moveCameraLeftButton.SetActive(true);
            
            foreach (Canvas canvas in canvases)
            {
                canvas.worldCamera = rightCamera.GetComponent<Camera>();
            }
            
            leftCamera.SetActive(false);
            moveCameraRightButton.SetActive(false);
        }
        else
        {
            leftCamera.SetActive(true);
            moveCameraRightButton.SetActive(true);

            foreach (Canvas canvas in canvases)
            {
                canvas.worldCamera = leftCamera.GetComponent<Camera>();
            }
            
            rightCamera.SetActive(false);
            moveCameraLeftButton.SetActive(false);
        }
    }
}
