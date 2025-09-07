using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Interact : MonoBehaviour
{
    public GameObject hoveredInteractable;
    public GameObject selectedPlot;

    public List<GameObject> matureFruits;
    
    private Dictionary<FruitType, int> fruits = new Dictionary<FruitType, int>();
    
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

    public void AddFruits(Dictionary<FruitType, int> newFruits)
    {
        foreach (var fruit in newFruits)
        {
            if (!fruits.ContainsKey(fruit.Key))
            {
                fruits[fruit.Key] = fruit.Value;
            }
        }

        foreach (var fruit in fruits)
        {
            Debug.Log(fruit.Key + " : "  + fruit.Value + " added to inventory");
        }
    }
}
