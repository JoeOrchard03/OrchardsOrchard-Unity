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

    [Header("Audio variables")] 
    public AudioSource highlightAudio;
    public AudioClip highlightSound;

    private bool playedHover = false;

    public GameObject saplingContainer;
    
    public int currentTreeCount;
    public int currentSaplingCount;
    
    void Start()
    {
        List<GameObject> trees = new List<GameObject>();
        foreach (GameObject tree in GameObject.FindGameObjectsWithTag("Tree"))
        {
            trees.Add(tree);
        }
        currentTreeCount = trees.Count;
        
        List<GameObject> saplings = new List<GameObject>();
        foreach (Transform child in saplingContainer.transform)
        {
            saplings.Add(child.gameObject);
        }
        currentSaplingCount =  saplings.Count;
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

    public void SetCursorHighlight(bool cursorHighlight)
    {
        if (cursorHighlight)
        {
            //PlayHighlightNoise();
            playedHover = true;
            Cursor.SetCursor(cursorHighlightTexture, cursorHotspot, CursorMode.Auto);
        }
        else
        {
            playedHover = false;
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
        }
    }

    public void SetShovelHighlight(bool shovelHighlight)
    {
        if (shovelHighlight)
        {
            //PlayHighlightNoise();
            playedHover = true;
            Cursor.SetCursor(shovelIconHighlightTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            playedHover = false;
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

    private void PlayHighlightNoise()
    {
        if (!highlightAudio.isPlaying && !playedHover)
        {
            highlightAudio.PlayOneShot(highlightSound);
        }
    }
}
