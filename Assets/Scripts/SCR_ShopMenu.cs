using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SCR_ShopMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SCR_FruitDatabase fruitDatabase;
    public SCR_ShopInventory shopInventory;

    [Header("Rarity multipliers")] 
    public int goldMultiplier;
    public int iridescentMultiplier;
    
    [Header("UI")]
    public TextMeshProUGUI saplingTabTimerText;
    public TextMeshProUGUI saplingTabTotalText;
    public TextMeshProUGUI decorTabTimerText;
    public TextMeshProUGUI decorTabTotalText;
    public TextMeshProUGUI upgradeTabTotalText;
    public TextMeshProUGUI SellTotalText;
    public GameObject shopRefreshNotif;
    
    [Header("Prefabs")]
    public GameObject shopSaplingPrefab;
    
    [Header("Shop & Inventory slots")]
    public List<SCR_BuyableSapling> shopSlots =  new List<SCR_BuyableSapling>();
    public List<SCR_InventorySlot> sellSlots = new List<SCR_InventorySlot>();
    
    [Header("Canvases")]
    public GameObject saplingCanvas;
    public GameObject sellCanvas;
    public GameObject decoCanvas;
    public GameObject upgradeCanvas;
    public SpriteRenderer menuSpriteRenderer;
    public Sprite SellMenuSprite;
    public Sprite SaplingMenuSprite;

    [Header("Sell UI variables")] 
    public GameObject contentHolder;
    public GameObject movedItemHolder;
    
    [Header("Misc references")]
    private float sellTotal;
    public float moneyTotal;
    private GameObject player;
    [HideInInspector] public float totalFruitValue;
    private bool movedFruit = false;
    private AudioSource shopMenuAudioSource;
    public AudioClip sellAudio;

    private void OnEnable()
    {
        var saveData = SCR_ReworkedSaveSystem.LoadGame();
        moneyTotal = saveData.money;
        
        if (shopInventory != null)
        {
            SCR_ShopInventory.OnSaplingShopTimerUpdated += UpdateSaplingTimerText;
            SCR_ShopInventory.OnDecoShopTimerUpdated += UpdateDecoTimerText;
            SCR_ShopInventory.OnShopRefreshed += UpdateShopUI;
        }
        
        shopMenuAudioSource = GetComponent<AudioSource>();
        saplingTabTotalText.text = moneyTotal.ToString();
        decorTabTotalText.text = moneyTotal.ToString();
        upgradeTabTotalText.text = moneyTotal.ToString();
        saplingCanvas.SetActive(true);
        decoCanvas.SetActive(false);
        upgradeCanvas.SetActive(false);
        //menuSpriteRenderer.sprite = SaplingMenuSprite;
        sellCanvas.SetActive(false);
        UpdateShopUI();
    }

    private void OnDisable()
    {
        if (shopInventory != null)
        {
            SCR_ShopInventory.OnSaplingShopTimerUpdated -= UpdateSaplingTimerText;
            SCR_ShopInventory.OnDecoShopTimerUpdated -= UpdateDecoTimerText;
            SCR_ShopInventory.OnShopRefreshed -= UpdateShopUI;
        }
    }

    private void UpdateSaplingTimerText(float saplingTimeRemaining)
    {
        saplingTabTimerText.text = FormatTimer(saplingTimeRemaining);
    }
    
    private void UpdateDecoTimerText(float decoTimeRemaining)
    { 
        decorTabTimerText.text = FormatTimer(decoTimeRemaining);
    }
    
    private string FormatTimer(float timer)
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        return string.Format("{0}:{1:00}", minutes, seconds);
    }
    
    public void UpdateShopUI()
    {
        if (shopInventory == null || shopInventory.saplingShopSlots == null) return;

        for (int i = 0; i < shopSlots.Count; i++)
        {
            if (i >= shopInventory.saplingShopSlots.Count) break;

            var uiSlot = shopSlots[i];
            var stockSlot = shopInventory.saplingShopSlots[i];

            if (uiSlot != null && stockSlot != null)
            {
                uiSlot.fruitType = stockSlot.fruitType;
                uiSlot.fruitDatabase = stockSlot.fruitDatabase;
            }
        }

        shopRefreshNotif.SetActive(false);
    }
    
    public void OpenSaplingsTab()
    {
        if (saplingCanvas.activeSelf == true)
        {
            return;
        }
        
        saplingCanvas.SetActive(true);
        //menuSpriteRenderer.sprite = SaplingMenuSprite;
        sellCanvas.SetActive(false);
        decoCanvas.SetActive(false);
        upgradeCanvas.SetActive(false);
    }

    public void OpenDecoTab()
    {
        if (decoCanvas.activeSelf == true)
        {
            return;
        }

        decoCanvas.SetActive(true);
        //menuSpriteRenderer.sprite = SaplingMenuSprite;
        sellCanvas.SetActive(false);
        saplingCanvas.SetActive(false);
        upgradeCanvas.SetActive(false);
    }
    
    public void OpenSellTab()
    {
        if (sellCanvas.activeSelf== true)
        {
            return;
        }

        sellCanvas.SetActive(true);
        //menuSpriteRenderer.sprite = SellMenuSprite;
        saplingCanvas.SetActive(false);
        decoCanvas.SetActive(false);
        upgradeCanvas.SetActive(false);
    }

    public void OpenUpgradeTab()
    {
        if (upgradeCanvas.activeSelf== true)
        {
            return;
        }

        upgradeCanvas.SetActive(true);
        //menuSpriteRenderer.sprite = SellMenuSprite;
        sellCanvas.SetActive(false);
        saplingCanvas.SetActive(false);
        decoCanvas.SetActive(false);
    }
    
    public void UpdateTotal()
    {
        sellTotal = 0;

        foreach (SCR_InventorySlot slot in sellSlots)
        {
            if (slot.fruitInBox != null)
            {
                float fruitValue = fruitDatabase.GetValue(slot.fruitInBox.fruitType);
                
                if (slot.fruitInBox.isGold)
                {
                    fruitValue *= goldMultiplier;
                }
                else if (slot.fruitInBox.isIridescent)
                {
                    fruitValue *= iridescentMultiplier;
                }
                
                sellTotal += fruitValue;
            }
        }
        
        SellTotalText.text = sellTotal.ToString();
        Debug.Log("Updating total");
    }
    
    public void SellAll()
    {
        SCR_PlayerManager playerManager = GameObject.FindFirstObjectByType<SCR_PlayerManager>();
        if (playerManager == null)
        {
            Debug.Log("SCR_ShopMenu can not find the player manager");
            return;
        }
        
        foreach (SCR_InventorySlot slot in sellSlots)
        {
            if (slot.fruitInBox != null)
            {
                FruitType type = slot.fruitInBox.fruitType;
                bool isGold = slot.fruitInBox.isGold;
                bool isIridescent = slot.fruitInBox.isIridescent;

                FruitData fruitToRemove = playerManager.inventoryFruits.fruits.Find(f => 
                    f.fruitType == type &&
                    f.isGold == isGold &&
                    f.isIridescent == isIridescent);

                if (fruitToRemove != null)
                {
                    playerManager.inventoryFruits.fruits.Remove(fruitToRemove);
                }
            }
            
            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
            
            slot.fruitInBox = null;
        }

        SCR_ReworkedSaveSystem.SaveFruitInventory(playerManager.inventoryFruits);
        
        if (sellTotal >= 1)
        {
            shopMenuAudioSource.PlayOneShot(sellAudio, 0.75f);
        }
        
        moneyTotal += sellTotal;
        sellTotal = 0;
        SellTotalText.text = "0";
        saplingTabTotalText.text = moneyTotal.ToString();
        decorTabTotalText.text = moneyTotal.ToString();
        upgradeTabTotalText.text = moneyTotal.ToString();
        Debug.Log("Selling items");
        var saveData =  SCR_ReworkedSaveSystem.LoadGame();
        saveData.money = moneyTotal;
        SCR_ReworkedSaveSystem.SaveGame(saveData);
    }

    public void QuickMove()
    {
        List<SCR_InventorySlot> freeSlots = new List<SCR_InventorySlot>();
        SCR_InventoryFruit[] contentHolderFruits = contentHolder.GetComponentsInChildren<SCR_InventoryFruit>();
        
        foreach (SCR_InventorySlot slot in sellSlots)
        {
            if (slot.fruitInBox == null)
            {
                Debug.Log("Adding free slot");
                freeSlots.Add(slot);
            }
            else
            {
                Debug.Log(slot.gameObject.name + " fruit in box is not null");
            }
        }
        
        foreach (var fruit in contentHolderFruits)
        {
            if (freeSlots.Count >= 1)
            {
                Debug.Log("Moving fruit");
                movedFruit = true;
                SCR_InventorySlot targetSlot = freeSlots[0];
                freeSlots.RemoveAt(0);
                Transform originalParent = fruit.returnParent;
                
                targetSlot.fruitInBox = fruit;
                fruit.returnParent = targetSlot.transform;

                fruit.transform.SetParent(targetSlot.transform, true);
                fruit.transform.localPosition = Vector3.zero;

                if (originalParent != null)
                {
                    Destroy(originalParent.gameObject);
                }
            }
            else
            {
                Debug.Log("No free slot");
            }
        }

        if (movedFruit)
        {
            shopMenuAudioSource.Play();
            movedFruit = false;
        }
        
        UpdateTotal();
    }
}
