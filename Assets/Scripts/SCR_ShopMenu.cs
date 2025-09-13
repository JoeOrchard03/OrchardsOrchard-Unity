using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SCR_ShopMenu : MonoBehaviour
{
    [SerializeField] private SCR_FruitDatabase fruitDatabase;

    public TextMeshProUGUI shopTimerText;
    
    public GameObject shopSaplingPrefab;
    
    public List<SCR_BuyableSapling> shopSlots =  new List<SCR_BuyableSapling>();
    public List<SCR_InventorySlot> inventorySlots = new List<SCR_InventorySlot>();
    
    public GameObject saplingCanvas;
    public GameObject sellCanvas;

    private float sellTotal;
    public float moneyTotal;
    private GameObject player;
    
    public TextMeshProUGUI moneyTotalText;
    public TextMeshProUGUI SellTotalText;
    [HideInInspector] public float totalFruitValue;

    private void Awake()
    {
        moneyTotalText.text = moneyTotal.ToString();
        saplingCanvas.SetActive(true);
        sellCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        SCR_ShopTimer.OnShopRefresh += RefreshShopInventory;
        SCR_ShopTimer.OnShopTimerUpdated += UpdateTimerText;
    }

    private void OnDisable()
    {
        SCR_ShopTimer.OnShopRefresh -= RefreshShopInventory;
        SCR_ShopTimer.OnShopTimerUpdated -= UpdateTimerText;
    }

    private void UpdateTimerText(float timeRemaining)
    {
        if (shopTimerText != null)
            shopTimerText.text = Mathf.CeilToInt(timeRemaining).ToString();
    }

    private void RefreshShopInventory()
    {
        foreach (SCR_BuyableSapling sapling in shopSlots)
        {
            SCR_FruitDatabase.Fruit fruit = GetRandomFruitBySpawnChance();
            sapling.fruitType = fruit.type;
            sapling.ApplyFruitInfo();
        }
    }

    private SCR_FruitDatabase.Fruit GetRandomFruitBySpawnChance()
    {
        var fruits = fruitDatabase.fruits;
        if (fruits == null || fruits.Length == 0)
            return null;
        
        float totalWeight = 0f;
        foreach (var fruit in fruits)
        {
            totalWeight += Mathf.Max(fruit.shopSpawnChance, 0.0001f);
        }

        // Pick a random point within total weight
        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var fruit in fruits)
        {
            cumulative += Mathf.Max(fruit.shopSpawnChance, 0.0001f);
            if (randomValue <= cumulative)
            {
                return fruit;
            }
        }
        
        return fruits[Random.Range(0, fruits.Length)];
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
