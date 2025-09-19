using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Highlightable : MonoBehaviour
{
    [SerializeField] public GameObject highlightEffect;
    public SCR_Interact playerInteractScriptRef;
    public bool stopHighlight = false;
    public bool canHighlight = true;
    public bool bypassHighlight = false;


    private void Start()
    {
        playerInteractScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_Interact>();
    }

    private void OnMouseOver()
    {
        if (playerInteractScriptRef.shopMenuOpen) return;
        
        SCR_FruitBloom fruit = GetComponent<SCR_FruitBloom>();
        if (fruit != null && !fruit.readyToHarvest) 
        {
            playerInteractScriptRef.SetCursorHighlight(false);
            return;
        }

        if (gameObject.CompareTag("Tree"))
        {
            if (playerInteractScriptRef.composting)
            {
                playerInteractScriptRef.hoveredInteractable = this.gameObject;
                playerInteractScriptRef.SetShovelHighlight(true);
            }
            return;
        }

        if (playerInteractScriptRef.composting && !gameObject.CompareTag("Composter")) return;

        if (bypassHighlight)
        {
            playerInteractScriptRef.hoveredInteractable = this.gameObject;
            return;
        }

        if (stopHighlight)
        {
            if (highlightEffect != null) highlightEffect.SetActive(false);
            return;
        }

        if (canHighlight && highlightEffect != null && !highlightEffect.activeSelf)
        {
            highlightEffect.SetActive(true);
        }

        playerInteractScriptRef.SetCursorHighlight(true);
        playerInteractScriptRef.hoveredInteractable = this.gameObject;
    }

    private void OnMouseExit()
    {
        if (gameObject.CompareTag("Tree") && playerInteractScriptRef.composting)
        {
            playerInteractScriptRef.SetShovelHighlight(false);
            playerInteractScriptRef.hoveredInteractable = null;
            return;
        }
        
        if (bypassHighlight)
        {
            playerInteractScriptRef.hoveredInteractable = null;
            return;
        }
        
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }

        if (!playerInteractScriptRef.composting)
        {
            playerInteractScriptRef.SetCursorHighlight(false);
        }
        else
        {
            playerInteractScriptRef.SetShovelHighlight(false);
        }
        
        playerInteractScriptRef.hoveredInteractable = null;
    }
}