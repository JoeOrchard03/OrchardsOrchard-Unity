using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Highlightable : MonoBehaviour
{
    [SerializeField] public GameObject highlightEffect;
    private SCR_Interact playerInteractScriptRef;
    public bool stopHighlight = false;


    private void Start()
    {
        playerInteractScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_Interact>();
    }

    private void OnMouseOver()
    {
        if (stopHighlight)
        {
            highlightEffect.SetActive(false); 
            return;
        }
        
        if (highlightEffect.activeSelf == true)
        {
            return;
        }

        highlightEffect.SetActive(true);
        playerInteractScriptRef.hoveredInteractable = this.gameObject;
    }

    private void OnMouseExit()
    {
        if (highlightEffect.activeSelf == false)
        {
            return;
        }

        highlightEffect.SetActive(false);
        playerInteractScriptRef.hoveredInteractable = null;
    }
}