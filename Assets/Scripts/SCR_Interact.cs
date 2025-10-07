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
    private Dictionary<FruitType, int> fruits = new Dictionary<FruitType, int>();
    
    [Header("Mouse variables")]
    public Texture2D cursorTexture;
    public Texture2D cursorHighlightTexture;
    public Texture2D shovelIconTexture;
    public Texture2D shovelIconHighlightTexture;
    public Vector2 cursorHotspot;
    
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

    public void SetCursorHighlight(bool cursorHighlight)
    {
        if (cursorHighlight)
        {
            Cursor.SetCursor(cursorHighlightTexture, cursorHotspot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
        }
    }

    public void SetShovelHighlight(bool shovelHighlight)
    {
        if (shovelHighlight)
        {
            Cursor.SetCursor(shovelIconHighlightTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(shovelIconTexture, Vector2.zero, CursorMode.Auto);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuOpen)
            {
                CloseSaplingMenu();
            }

            if (shopMenuOpen)
            {
                CloseShop();
            }
        }

        if (shopMenuOpen) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (hoveredInteractable == null) { return;} 
            if (hoveredInteractable.GetComponent<INT_Interactable>() == null) { Debug.Log("Item does not have interactable script"); return;}
            hoveredInteractable.GetComponent<INT_Interactable>().Interact(this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayerPrefs.DeleteKey("GameSave");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Clearing save data...");
        }
        
    }

    public void CloseSaplingMenu()
    {
        GameObject.FindGameObjectWithTag("Menu").SetActive(false);
        selectedPlot = null;
        menuOpen = false;
    }
    
    public void CloseShop()
    {
        GameObject.FindGameObjectWithTag("ShopMenu").SetActive(false);
        openShopObj.GetComponent<SCR_OpenShop>().shopOpen = false;
        shopMenuOpen = false;
    }

    public void AddFruits(List<SCR_Drone.HarvestedFruit> newFruits)
    {
        foreach (var fruit in newFruits)
        {
            GameObject box = Instantiate(inventoryBoxPrefab, fruitSellInventory);
            
            SCR_InventoryFruit fruitUI = box.GetComponentInChildren<SCR_InventoryFruit>();
            if (fruitUI != null)
            {
                fruitUI.fruitType = fruit.fruitType;
                fruitUI.isGold = fruit.isGold;
                fruitUI.isIridescent = fruit.isIridescent;
                fruitUI.returnParent = box.transform;
            } 
            
            Debug.Log($"{fruit.fruitType} added (Gold: {fruit.isGold}, Iridescent: {fruit.isIridescent})");
        }
    }
}
