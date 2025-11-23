using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCR_OpenEditMode : MonoBehaviour
{
    public bool editMode = false;
    public Transform placedDecoHolder;
    public SCR_OpenDecoMenu openDecoMenuScriptRef;
    public SCR_PlayerManager playerManagerScriptRef;

    private void Start()
    {
        editMode = false;
    }
    
    public void enableEditMode()
    {
        editMode = true;
        playerManagerScriptRef.editModeEnabled = true;
        Debug.Log("Enabling edit mode");
        foreach (var decoOBJ in placedDecoHolder.GetComponentsInChildren<Transform>())
        {
            if (decoOBJ.GetComponent<BoxCollider2D>() == null) continue;
            if (decoOBJ.GetComponent<SCR_Highlightable>() == null) continue;

            Debug.Log("Enabling colliders for: " + decoOBJ);
            decoOBJ.GetComponent<BoxCollider2D>().enabled = true;
            decoOBJ.GetComponent<SCR_Highlightable>().enabled = true;
            openDecoMenuScriptRef.CloseMenu();
        }
    }

    public void disableEditMode()
    {
        editMode = false;
        playerManagerScriptRef.editModeEnabled = false;
        Debug.Log("Disabling edit mode");
        foreach (var decoOBJ in placedDecoHolder.GetComponentsInChildren<Transform>())
        {
            if (decoOBJ.GetComponent<BoxCollider2D>() == null) continue;
            if (decoOBJ.GetComponent<SCR_Highlightable>() == null) continue;
                
            Debug.Log("Disabling colliders for: " + decoOBJ);
            decoOBJ.GetComponent<BoxCollider2D>().enabled = false;
            decoOBJ.GetComponent<SCR_Highlightable>().enabled = false;
        }
    }
}
