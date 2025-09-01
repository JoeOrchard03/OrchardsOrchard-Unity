using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Interact : MonoBehaviour
{
    public GameObject hoveredInteractable;
    
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (hoveredInteractable == null) { return;} 
            Debug.Log("Pressing on: " + hoveredInteractable.name);
            hoveredInteractable.GetComponent<INT_Interactable>().Interact(this.gameObject);
        }
    }
}
