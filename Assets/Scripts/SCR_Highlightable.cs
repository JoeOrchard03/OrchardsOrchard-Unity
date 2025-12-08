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

        if (playerPlayerManagerScriptRef.pickRangeUpgrade)
        {
            if (fruit != null && fruit.readyToHarvest)
            {
                SCR_TreeGrowthCycle treeScript = fruit.transform.parent.GetComponent<SCR_TreeGrowthCycle>();
                if (treeScript != null)
                {
                    treeScript.SetAllFruitHighlights(true);
                }
            }
            
            if(gameObject.CompareTag("Tree"))
            {
                SCR_TreeGrowthCycle tree = GetComponentInParent<SCR_TreeGrowthCycle>();
                
                if(tree != null)
                {
                    if (tree.HasHarvestableFruit())
                    {
                        playerPlayerManagerScriptRef.SetCursorHighlight(true);
                        tree.SetAllFruitHighlights(true);
                    }
                    else
                    {
                        playerPlayerManagerScriptRef.SetCursorHighlight(false);
                    }
                }
            }
        }
        
        if (gameObject.CompareTag("Tree"))
        {
            if (playerPlayerManagerScriptRef.composting)
            {
                playerPlayerManagerScriptRef.hoveredInteractable = this.gameObject;
                playerPlayerManagerScriptRef.SetShovelHighlight(true);
            }
            else
            {
                playerPlayerManagerScriptRef.hoveredInteractable = this.gameObject;
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
        SCR_FruitBloom fruit = GetComponent<SCR_FruitBloom>();
        
        if (gameObject.CompareTag("Tree") && playerPlayerManagerScriptRef.composting)
        {
            playerPlayerManagerScriptRef.SetShovelHighlight(false);
            playerPlayerManagerScriptRef.hoveredInteractable = null;
            return;
        }
        
        if (playerPlayerManagerScriptRef.pickRangeUpgrade)
        {
            if (fruit != null)
            {
                SCR_TreeGrowthCycle tree = fruit.transform.parent.GetComponent<SCR_TreeGrowthCycle>();
                if (tree != null)
                {
                    tree.SetAllFruitHighlights(false);
                }
            }
            
            if (gameObject.CompareTag("Tree"))
            {
                SCR_TreeGrowthCycle tree = GetComponentInParent<SCR_TreeGrowthCycle>();
                if (tree != null)
                {
                    playerPlayerManagerScriptRef.SetCursorHighlight(false);
                    tree.SetAllFruitHighlights(false);
                }
            }
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