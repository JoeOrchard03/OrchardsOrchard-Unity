using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SCR_BuyableSapling : MonoBehaviour
{
    public FruitType fruitType;
    public SCR_FruitDatabase fruitDatabase;

    public SpriteRenderer saplingSprite;
    public TextMeshProUGUI priceText;

    private float saplingPrice;
    
    private SCR_ShopMenu shopMenuScriptRef;

    public GameObject inventorySaplingPrefab;
    public Transform saplingInventory;

    public GameObject outOfStockObj;
    public GameObject buttonObj;
    public GameObject moneyIcon;

    private void Awake()
    {
        shopMenuScriptRef = GameObject.FindGameObjectWithTag("ShopMenu").GetComponent<SCR_ShopMenu>();
        ApplyFruitInfo();
    }
    
    public void ApplyFruitInfo()
    {
        var fruit = fruitDatabase.GetFruit(fruitType);
        if (fruit != null)
        {
            outOfStockObj.SetActive(false);
            buttonObj.SetActive(true);
            moneyIcon.SetActive(true);
            saplingPrice = fruit.saplingPrice;
            saplingSprite.sprite = fruit.saplingSprite;
            priceText.text = fruit.saplingPrice.ToString();
        }
        else
        {
            Debug.LogError("SAPLING NOT FOUND");
        }
    }

    public void DisableSlot()
    {
        outOfStockObj.SetActive(true);
        buttonObj.SetActive(false);
        moneyIcon.SetActive(false);
        saplingSprite.sprite = null;
        priceText.text = "";
    }
    
    public void BuyFruit()
    {
        if (shopMenuScriptRef.moneyTotal < saplingPrice)
        {
            Debug.Log("Cannot afford sapling");
            return;
        }
        
        shopMenuScriptRef.moneyTotal -= saplingPrice;
        shopMenuScriptRef.moneyTotalText.text = shopMenuScriptRef.moneyTotal.ToString();
        
        GameObject sapling = Instantiate(inventorySaplingPrefab, saplingInventory);
        sapling.GetComponent<SCR_MenuBox>().fruitType = fruitType;
        
        Debug.Log("Adding " + fruitType.ToString() + " sapling to inventory");
        
        DisableSlot();
    }
}
