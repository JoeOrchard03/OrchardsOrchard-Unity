using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Interact : MonoBehaviour
{
    public GameObject hoveredInteractable;
    public GameObject selectedPlot;

    public List<GameObject> matureFruits;
    
    public bool menuOpen = false;
    
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (hoveredInteractable == null) { return;} 
            if (hoveredInteractable.GetComponent<INT_Interactable>() == null) { Debug.Log("Item does not have interactable script"); return;}
            hoveredInteractable.GetComponent<INT_Interactable>().Interact(this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && menuOpen)
        {
            GameObject.FindGameObjectWithTag("Menu").SetActive(false);
            selectedPlot = null;
            menuOpen = false;
        }
    }
}
