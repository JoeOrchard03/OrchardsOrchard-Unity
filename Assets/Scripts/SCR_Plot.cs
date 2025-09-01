using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Plot : MonoBehaviour, INT_Interactable
{
    public GameObject player;
    public bool highlighted = false;
    public GameObject highlightEffect;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    private void OnMouseOver()
    {
        highlighted = true;
        highlightEffect.SetActive(true);    
        player.GetComponent<SCR_Interact>().hoveredInteractable = this.gameObject;
    }

    private void OnMouseExit()
    {
        highlighted = false;
        highlightEffect.SetActive(false);
        player.GetComponent<SCR_Interact>().hoveredInteractable = null;
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log("Open sapling menu");
    }
}
