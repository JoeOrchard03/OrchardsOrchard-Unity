using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CompendiumOpen : MonoBehaviour, INT_Interactable
{
    public GameObject compendium;
    public GameObject compendiumCanvas;
    
    public void Interact(GameObject interactor)
    {
        Debug.Log("Interacting with compendium");
        if (compendiumCanvas.activeInHierarchy)
        {
            compendiumCanvas.SetActive(false);
        }
        else
        {
            compendiumCanvas.SetActive(true);
            compendium.GetComponent<SCR_Compendium>().OpenCompendium();
        }
    }

    public void CloseCompendium()
    {
        compendiumCanvas.SetActive(false);
    }
}
