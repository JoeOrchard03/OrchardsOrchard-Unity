using Unity.VisualScripting;
using UnityEngine;

public class SCR_OpenTutorials : MonoBehaviour, INT_Interactable
{
    public GameObject mainCamera;
    public GameObject tutorialCanvas;
    private bool wasZoomedBeforeOpening = false;
    
    public string currentCamera;
    
    public void Interact(GameObject interactor)
    {
        Debug.Log("Interacting with compendium");
        if (tutorialCanvas.activeInHierarchy)
        {
            CloseTutorials();
            return;
        }
        
        tutorialCanvas.SetActive(true);
        
        wasZoomedBeforeOpening = !mainCamera.activeInHierarchy;

        if (wasZoomedBeforeOpening)
        {
            GetComponent<SCR_CameraZoomOut>().Interact(this.gameObject);
        }
        
        OpenTutorials();
    }

    private void CloseTutorials()
    {
        tutorialCanvas.SetActive(false);
        
        if (wasZoomedBeforeOpening)
        {
            Debug.Log("Camera is zoomed, zooming in");
            gameObject.GetComponent<SCR_CameraZoomIn>().Interact(this.gameObject);
        }
    }

    private void OpenTutorials()
    {
        if (currentCamera != "MainCamera")
        {
            gameObject.GetComponent<SCR_CameraZoomOut>().Interact(this.gameObject);
        }
    }
}
