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

    [Header("UI")]
    public TextMeshProUGUI shopTimerText;
    public TextMeshProUGUI moneyTotalText;
    public TextMeshProUGUI SellTotalText;
    
    [Header("Prefabs")]
    public GameObject shopSaplingPrefab;
    
    [Header("Shop & Inventory slots")]
    public List<SCR_BuyableSapling> shopSlots =  new List<SCR_BuyableSapling>();
    public List<SCR_InventorySlot> inventorySlots = new List<SCR_InventorySlot>();
    
    [Header("Canvases")]
    public GameObject saplingCanvas;
    public GameObject sellCanvas;

    [Header("Misc references")]
    private float sellTotal;
    public float moneyTotal;
    private GameObject player;
    [HideInInspector] public float totalFruitValue;

    private void OnEnable()
    {
        if (shopInventory != null)
        {
            SCR_ShopInventory.OnShopTimerUpdated += UpdateTimerText;
            SCR_ShopInventory.OnShopRefreshed += UpdateShopUI;
        }
        
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
                sellTotal += fruitDatabase.GetValue(slot.fruitInBox.fruitType);
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
        
        moneyTotal += sellTotal;
        sellTotal = 0;
        SellTotalText.text = "0";
        moneyTotalText.text = moneyTotal.ToString();
        Debug.Log("Selling items");
    }
}
