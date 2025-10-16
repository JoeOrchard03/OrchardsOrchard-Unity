using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_OpenShop : MonoBehaviour, INT_Interactable
{
    private GameObject player;
    public GameObject shopMenu;
    public bool shopOpen = false;
    private AudioSource shopAudioSource;

    public void Start()
    {
        shopAudioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    public void Interact(GameObject interactor)
    {
        Debug.Log("Interacting with shop");
        if (!shopOpen)
        {
            shopAudioSource.Play();
            Debug.Log("Opening shop menu");
            player.GetComponent<SCR_PlayerManager>().shopMenuOpen = true;
            shopOpen = true;
            shopMenu.SetActive(true);
        }
        else
        {
            Debug.Log("Closing shop menu");
            player.GetComponent<SCR_PlayerManager>().shopMenuOpen = false;
            shopOpen = false;
            shopMenu.SetActive(false);
        }
    }

    public void CloseMenu()
    {
        Debug.Log("Closing shop menu");
        player.GetComponent<SCR_PlayerManager>().shopMenuOpen = false;
        shopOpen = false;
        shopMenu.SetActive(false);
    }
}
