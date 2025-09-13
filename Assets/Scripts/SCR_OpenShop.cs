using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_OpenShop : MonoBehaviour, INT_Interactable
{
    private GameObject player;
    public GameObject shopMenu;
    public bool shopOpen = false;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    public void Interact(GameObject interactor)
    {
        Debug.Log("Interacting with shop");
        if (!shopOpen)
        {
            Debug.Log("Opening shop menu");
            player.GetComponent<SCR_Interact>().shopMenuOpen = true;
            shopOpen = true;
            shopMenu.SetActive(true);
        }
        else
        {
            Debug.Log("Closing shop menu");
            player.GetComponent<SCR_Interact>().shopMenuOpen = false;
            shopOpen = false;
            shopMenu.SetActive(false);
        }
    }

    public void CloseMenu()
    {
        Debug.Log("Closing shop menu");
        player.GetComponent<SCR_Interact>().shopMenuOpen = false;
        shopOpen = false;
        shopMenu.SetActive(false);
    }
}
