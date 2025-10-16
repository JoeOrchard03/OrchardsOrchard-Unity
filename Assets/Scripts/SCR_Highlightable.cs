using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Highlightable : MonoBehaviour
{
    [SerializeField] public GameObject highlightEffect;
    public SCR_PlayerManager playerPlayerManagerScriptRef;
    public bool stopHighlight = false;
    public bool canHighlight = true;
    public bool bypassHighlight = false;


    private void Start()
    {
        playerPlayerManagerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerManager>();
    }

    private void OnMouseOver()
    {
        if (playerPlayerManagerScriptRef.shopMenuOpen) return;
        
        SCR_FruitBloom fruit = GetComponent<SCR_FruitBloom>();
        if (fruit != null && !fruit.readyToHarvest) 
        {
            playerPlayerManagerScriptRef.SetCursorHighlight(false);
            return;
        }

        if (gameObject.CompareTag("Tree"))
        {
            if (playerPlayerManagerScriptRef.composting)
            {
                playerPlayerManagerScriptRef.hoveredInteractable = this.gameObject;
                playerPlayerManagerScriptRef.SetShovelHighlight(true);
            }
            return;
        }

        if (playerPlayerManagerScriptRef.composting && !gameObject.CompareTag("Composter")) return;

        if (bypassHighlight)
        {
            playerPlayerManagerScriptRef.hoveredInteractable = this.gameObject;
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

        playerPlayerManagerScriptRef.SetCursorHighlight(true);
        playerPlayerManagerScriptRef.hoveredInteractable = this.gameObject;
    }

    private void OnMouseExit()
    {
        if (gameObject.CompareTag("Tree") && playerPlayerManagerScriptRef.composting)
        {
            playerPlayerManagerScriptRef.SetShovelHighlight(false);
            playerPlayerManagerScriptRef.hoveredInteractable = null;
            return;
        }
        
        if (bypassHighlight)
        {
            playerPlayerManagerScriptRef.hoveredInteractable = null;
            return;
        }
        
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }

        if (!playerPlayerManagerScriptRef.composting)
        {
            playerPlayerManagerScriptRef.SetCursorHighlight(false);
        }
        else
        {
            playerPlayerManagerScriptRef.SetShovelHighlight(false);
        }
        
        playerPlayerManagerScriptRef.hoveredInteractable = null;
    }
}