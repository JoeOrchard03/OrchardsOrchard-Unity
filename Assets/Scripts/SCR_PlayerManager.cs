using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_PlayerManager : MonoBehaviour
{
    [Header("Interactable variables")]
    public GameObject hoveredInteractable;
    public bool composting = false;
    private bool playedHover = false;
    
    [Header("GameObjects")]
    public GameObject selectedPlot;
    public GameObject openShopObj;
    public List<GameObject> matureFruits;

    [Header("Inventory variables")]
    public GameObject inventoryBoxPrefab;
    public Transform fruitSellInventory;
    public GameObject saplingContainer;
    public int currentTreeCount;
    public int currentSaplingCount;
    
    [Header("References")]
    public SCR_OpenEditMode openEditModeScriptRef;
    public SCR_Drone droneScriptRef;
    public GameObject shopMenu;
    public GameObject clickParticle;
    
    [Header("UI variables")]
    public bool shopMenuOpen = false;
    public bool decoMenuOpen = false;
    public bool menuOpen = false;
    public bool editModeEnabled = false;
    private Dictionary<FruitType, int> fruits = new Dictionary<FruitType, int>();
    
    [Header("Upgrade variables")]
    public bool pickRangeUpgrade = false;
    
    [Header("Mouse variables")]
    public Texture2D cursorTexture;
    public Texture2D cursorHighlightTexture;
    public Texture2D shovelIconTexture;
    public Texture2D shovelIconHighlightTexture;
    public Vector2 cursorHotspot;

    [Header("Audio variables")] 
    public AudioSource highlightAudio;
    public AudioClip highlightSound;

    [Header("Save/Load variables")]
    public InventoryFruits inventoryFruits = new InventoryFruits();
    
    void Start()
    {
        shopMenu.SetActive(false);
        
        SCR_SaveData saveData = SCR_ReworkedSaveSystem.LoadGame();

        currentTreeCount = saveData.trees.Count;
        currentSaplingCount = saveData.saplings.Count;
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);

        inventoryFruits = SCR_ReworkedSaveSystem.LoadFruitInventory();
        LoadInventoryUI();
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

            if (editModeEnabled)
            {
                DisableEditMode();
            }
        }

        if (shopMenuOpen) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ClickEffect());
            if (hoveredInteractable == null) { return;} 
            if (hoveredInteractable.GetComponent<INT_Interactable>() == null) { Debug.Log("Item does not have interactable script"); return;}
            hoveredInteractable.GetComponent<INT_Interactable>().Interact(this.gameObject);
            hoveredInteractable = null;
            SetCursorHighlight(false);
        }
    }

    public void CloseSaplingMenu()
    {
        GameObject.FindGameObjectWithTag("Menu").SetActive(false);
        selectedPlot = null;
        menuOpen = false;
    }

    public void CloseDecoMenu()
    {
        GameObject.FindGameObjectWithTag("DecoMenu").SetActive(false);
        decoMenuOpen = false;
    }
    
    public void CloseShop()
    {
        GameObject.FindGameObjectWithTag("ShopMenu").SetActive(false);
        openShopObj.GetComponent<SCR_OpenShop>().shopOpen = false;
        shopMenuOpen = false;
    }

    public void DisableEditMode()
    {
        if (openEditModeScriptRef)
        {
            openEditModeScriptRef.disableEditMode();
        }
    }

    public void AddFruits(List<SCR_Drone.HarvestedFruit> newFruits)
    {
        foreach (var fruit in newFruits)
        {
            FruitData data = new FruitData
            {
                isGold = fruit.isGold,
                isIridescent = fruit.isIridescent,
                fruitType = fruit.fruitType
            };
            
            inventoryFruits.fruits.Add(data);
            
            GameObject box = Instantiate(inventoryBoxPrefab, fruitSellInventory);
            SCR_InventoryFruit fruitUI = box.GetComponentInChildren<SCR_InventoryFruit>();
            
            if (fruitUI != null)
            {
                fruitUI.fruitType = fruit.fruitType;
                fruitUI.isGold = fruit.isGold;
                fruitUI.isIridescent = fruit.isIridescent;
                fruitUI.returnParent = box.transform;
                fruitUI.ApplyVisuals();
            } 
            
            Debug.Log($"{fruit.fruitType} added (Gold: {fruit.isGold}, Iridescent: {fruit.isIridescent})");
        }
        
        SCR_ReworkedSaveSystem.SaveFruitInventory(inventoryFruits);
    }

    private void LoadInventoryUI()
    {
        foreach (FruitData data in inventoryFruits.fruits)
        {
            GameObject box = Instantiate(inventoryBoxPrefab, fruitSellInventory);
            SCR_InventoryFruit fruitUI = box.GetComponentInChildren<SCR_InventoryFruit>();

            if (fruitUI != null)
            {
                fruitUI.isGold = data.isGold;
                fruitUI.isIridescent = data.isIridescent;
                fruitUI.fruitType = data.fruitType;
                fruitUI.returnParent = box.transform;
                fruitUI.ApplyVisuals();
            }
        }
    }
    
    private void PlayHighlightNoise()
    {
        if (!highlightAudio.isPlaying && !playedHover)
        {
            highlightAudio.PlayOneShot(highlightSound);
        }
    }
    
    public void RefreshCountsFromSave()
    {
        SCR_SaveData data = SCR_ReworkedSaveSystem.LoadGame();
        currentTreeCount = data.trees?.Count ?? 0;
        currentSaplingCount = data.saplings?.Count ?? 0;
    }

    IEnumerator ClickEffect()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        
        GameObject particle = Instantiate(clickParticle, worldPos, Quaternion.identity);
        yield return new WaitForSeconds(1.0f);
        Destroy(particle);
    }
}
