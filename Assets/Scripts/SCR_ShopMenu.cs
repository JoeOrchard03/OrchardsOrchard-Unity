using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SCR_ShopMenu : MonoBehaviour
{
    [SerializeField] private SCR_FruitDatabase fruitDatabase;
    
    public List<SCR_InventorySlot> inventorySlots = new List<SCR_InventorySlot>();
    public GameObject saplingCanvas;
    public GameObject sellCanvas;

    public TextMeshProUGUI MoneyTotalText;
    [HideInInspector] public float totalFruitValue;

    private void Awake()
    {
        saplingCanvas.SetActive(true);
        sellCanvas.SetActive(false);
    }

    public void OpenSaplingsTab()
    {
        if (saplingCanvas.active == true)
        {
            return;
        }
        
        saplingCanvas.SetActive(true);
        sellCanvas.SetActive(false);
    }

    public void OpenSellTab()
    {
        if (sellCanvas.active == true)
        {
            return;
        }

        saplingCanvas.SetActive(false);
        sellCanvas.SetActive(true);
    }
    
    public void UpdateTotal()
    {
        totalFruitValue = 0;

        foreach (SCR_InventorySlot slot in inventorySlots)
        {
            if (slot.fruitInBox != null)
            {
                totalFruitValue += fruitDatabase.GetValue(slot.fruitInBox.fruitType);
            }
        }
        
        MoneyTotalText.text = totalFruitValue.ToString();
        Debug.Log("Updating total");
    }
    
    public void SellAll()
    {
        Debug.Log("Selling items");
    }
}
