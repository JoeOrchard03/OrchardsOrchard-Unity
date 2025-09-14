using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Interact : MonoBehaviour
{
    [Header("Interactable variables")]
    public GameObject hoveredInteractable;
    public bool composting = false;
    
    [Header("GameObjects")]
    public GameObject selectedPlot;
    public GameObject openShopObj;
    public List<GameObject> matureFruits;

    [Header("Inventory variables")]
    public GameObject inventoryBoxPrefab;
    public Transform fruitSellInventory;
    
    [Header("UI variables")]
    public bool shopMenuOpen = false;
    public bool menuOpen = false;
    
    [Header("Cursor variables")]
    public Texture2D cursorTexture;
    
    private Dictionary<FruitType, int> fruits = new Dictionary<FruitType, int>();
    
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Vector2 cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuOpen)
            {
                GameObject.FindGameObjectWithTag("Menu").SetActive(false);
                selectedPlot = null;
                menuOpen = false;
            }

            if (shopMenuOpen)
            {
                GameObject.FindGameObjectWithTag("ShopMenu").SetActive(false);
                openShopObj.GetComponent<SCR_OpenShop>().shopOpen = false;
                shopMenuOpen = false;
            }
        }

        if (shopMenuOpen) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (hoveredInteractable == null) { return;} 
            if (hoveredInteractable.GetComponent<INT_Interactable>() == null) { Debug.Log("Item does not have interactable script"); return;}
            hoveredInteractable.GetComponent<INT_Interactable>().Interact(this.gameObject);
        }
        
    }

    public void AddFruits(Dictionary<FruitType, int> newFruits)
    {
        foreach (var fruit in newFruits)
        {
            for (int i = 0; i < fruit.Value; i++)
            {
                GameObject box = Instantiate(inventoryBoxPrefab, fruitSellInventory);
                
                SCR_InventoryFruit fruitUI = box.GetComponentInChildren<SCR_InventoryFruit>();
                if (fruitUI != null)
                {
                    fruitUI.fruitType = fruit.Key;
                    fruitUI.returnParent = box.transform;
                }
            }
            
            Debug.Log(fruit.Key + " : "  + fruit.Value + " added to inventory");
        }
    }
}
