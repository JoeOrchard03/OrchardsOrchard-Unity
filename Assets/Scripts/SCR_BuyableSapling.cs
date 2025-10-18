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

    public GameObject BuyTextObj;
    public GameObject outOfStockObj;
    public GameObject buttonObj;
    public GameObject moneyIcon;
    public AudioSource buttonAudioSource;
    public SCR_PlayerManager playerManagerScriptRef;

    private void Awake()
    {
        playerManagerScriptRef = GameObject.Find("PlayerOBJ").GetComponent<SCR_PlayerManager>();
        shopMenuScriptRef = GameObject.FindGameObjectWithTag("ShopMenu").GetComponent<SCR_ShopMenu>();
    }
    
    public void ApplyFruitInfo()
    {
        var fruit = fruitDatabase.GetFruit(fruitType);
        if (fruit != null)
        {
            Debug.Log("Resetting sapling: " + gameObject.name.ToString());
            outOfStockObj.SetActive(false);
            buttonObj.SetActive(true);
            BuyTextObj.SetActive(true);
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
        BuyTextObj.SetActive(false);
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

        buttonAudioSource.Play();
        shopMenuScriptRef.moneyTotal -= saplingPrice;
        shopMenuScriptRef.moneyTotalText.text = shopMenuScriptRef.moneyTotal.ToString();
        
        GameObject sapling = Instantiate(inventorySaplingPrefab, saplingInventory);
        sapling.GetComponent<SCR_MenuBox>().fruitType = fruitType;
        
        DisableSlot();
        
        Debug.Log("Adding " + fruitType.ToString() + " sapling to inventory");

        SCR_ShopInventory shopInventoryScriptRef = GameObject.FindFirstObjectByType<SCR_ShopInventory>().GetComponent<SCR_ShopInventory>();
        
        SCR_SaveSystem.SaveShopInventory(
            shopInventoryScriptRef.shopSlots,
            shopInventoryScriptRef.shopRefreshTime);
        
        SCR_SaveData data = SCR_SaveSystem.LoadGame();
        data.money = shopMenuScriptRef.moneyTotal;
        data.saplings = SCR_SaveSystem.GetSaplingData(saplingInventory);
        SCR_SaveSystem.SaveGame(data);
        
        playerManagerScriptRef.UpdateCounts();
    }
}
