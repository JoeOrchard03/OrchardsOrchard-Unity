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
    public TextMeshProUGUI shopTimerText;
    public TextMeshProUGUI moneyTotalText;
    public TextMeshProUGUI SellTotalText;
    public GameObject shopRefreshNotif;
    
    [Header("Prefabs")]
    public GameObject shopSaplingPrefab;
    
    [Header("Shop & Inventory slots")]
    public List<SCR_BuyableSapling> shopSlots =  new List<SCR_BuyableSapling>();
    public List<SCR_InventorySlot> inventorySlots = new List<SCR_InventorySlot>();
    
    [Header("Canvases")]
    public GameObject saplingCanvas;
    public GameObject sellCanvas;

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
        var saveData = SCR_SaveSystem.LoadGame();
        moneyTotal = saveData.money;
        
        if (shopInventory != null)
        {
            SCR_ShopInventory.OnShopTimerUpdated += UpdateTimerText;
            SCR_ShopInventory.OnShopRefreshed += UpdateShopUI;
        }
        
        shopMenuAudioSource = GetComponent<AudioSource>();
        moneyTotalText.text = moneyTotal.ToString();
        saplingCanvas.SetActive(true);
        sellCanvas.SetActive(false);
        UpdateShopUI();
    }

    private void OnDisable()
    {
        if (shopInventory != null)
        {
            SCR_ShopInventory.OnShopTimerUpdated -= UpdateTimerText;
            SCR_ShopInventory.OnShopRefreshed -= UpdateShopUI;
        }
    }

    private void UpdateTimerText(float timeRemaining)
    {
        if (shopTimerText != null)
        {
            shopTimerText.text = Mathf.CeilToInt(timeRemaining).ToString();
        }
    }
    
    public void UpdateShopUI()
    {
        if (shopInventory == null || shopInventory.shopSlots == null) return;

        for (int i = 0; i < shopSlots.Count; i++)
        {
            if (i >= shopInventory.shopSlots.Count) break;

            var uiSlot = shopSlots[i];
            var stockSlot = shopInventory.shopSlots[i];

            if (uiSlot != null && stockSlot != null)
            {
                uiSlot.fruitType = stockSlot.fruitType;
                uiSlot.fruitDatabase = stockSlot.fruitDatabase;
                uiSlot.ApplyFruitInfo();
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
        sellCanvas.SetActive(false);
    }

    public void OpenSellTab()
    {
        if (sellCanvas.activeSelf== true)
        {
            return;
        }

        saplingCanvas.SetActive(false);
        sellCanvas.SetActive(true);
    }
    
    public void UpdateTotal()
    {
        sellTotal = 0;

        foreach (SCR_InventorySlot slot in inventorySlots)
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
        foreach (SCR_InventorySlot slot in inventorySlots)
        {
            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
            
            slot.fruitInBox = null;
        }

        if (sellTotal >= 1)
        {
            shopMenuAudioSource.PlayOneShot(sellAudio);
        }
        
        moneyTotal += sellTotal;
        sellTotal = 0;
        SellTotalText.text = "0";
        moneyTotalText.text = moneyTotal.ToString();
        Debug.Log("Selling items");
        var saveData =  SCR_SaveSystem.LoadGame();
        saveData.money = moneyTotal;
        SCR_SaveSystem.SaveGame(saveData);
    }

    public void QuickMove()
    {
        List<SCR_InventorySlot> freeSlots = new List<SCR_InventorySlot>();
        SCR_InventoryFruit[] contentHolderFruits = contentHolder.GetComponentsInChildren<SCR_InventoryFruit>();
        
        foreach (SCR_InventorySlot slot in inventorySlots)
        {
            if (slot.fruitInBox == null)
            {
                freeSlots.Add(slot);
            }
        }
        
        foreach (var fruit in contentHolderFruits)
        {
            if (freeSlots.Count >= 1)
            {
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
        }

        if (movedFruit)
        {
            shopMenuAudioSource.Play();
            movedFruit = false;
        }
        
        UpdateTotal();
    }
}
