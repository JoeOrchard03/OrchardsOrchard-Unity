using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_OpenShop : MonoBehaviour, INT_Interactable
{
    public GameObject shopMenu;
    private bool shopOpen = false;
    
    public void Interact(GameObject interactor)
    {
        Debug.Log("Interacting with shop");
        if (!shopOpen)
        {
            Debug.Log("Opening shop menu");
            shopOpen = true;
            shopMenu.SetActive(true);
        }
        else
        {
            Debug.Log("Closing shop menu");
            shopOpen = false;
            shopMenu.SetActive(false);
        }
    }
}
