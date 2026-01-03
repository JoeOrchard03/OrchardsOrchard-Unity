using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SCR_BuyableSapling : MonoBehaviour
{
    public FruitType fruitType;
    public SCR_FruitDatabase fruitDatabase;

    public SpriteRenderer saplingSprite;

    private float saplingPrice;
    
    private SCR_ShopMenu shopMenuScriptRef;

    public GameObject inventorySaplingPrefab;
    public Transform saplingInventory;

    public BoxCollider2D saplingInventoryBoxCollider;
    
    public GameObject BuyTextObj;
    public GameObject outOfStockObj;
    public GameObject buttonObj;
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
            outOfStockObj.SetActive(false);
            buttonObj.SetActive(true);
            BuyTextObj.SetActive(true);
            saplingInventoryBoxCollider.enabled = true;
            saplingPrice = fruit.saplingPrice;
            saplingSprite.sprite = fruit.saplingSprite;
        }
        else
        {
            DisableSlot();
        }
    }

    public void DisableSlot()
    {
        outOfStockObj.SetActive(true);
        buttonObj.SetActive(false);
        saplingInventoryBoxCollider.enabled = false;
        BuyTextObj.SetActive(false);
        saplingSprite.sprite = null;
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
        shopMenuScriptRef.saplingTabTotalText.text = shopMenuScriptRef.moneyTotal.ToString();
        shopMenuScriptRef.decorTabTotalText.text = shopMenuScriptRef.moneyTotal.ToString();
        shopMenuScriptRef.upgradeTabTotalText.text = shopMenuScriptRef.moneyTotal.ToString();
        
        GameObject sapling = Instantiate(inventorySaplingPrefab, saplingInventory);
        sapling.GetComponent<SCR_SaplingMenuBox>().fruitType = fruitType;
        
        DisableSlot();
        
        Debug.Log("Adding " + fruitType.ToString() + " sapling to inventory");

        SCR_ShopInventory shopInventoryScriptRef = GameObject.FindFirstObjectByType<SCR_ShopInventory>().GetComponent<SCR_ShopInventory>();
        
        SCR_ReworkedSaveSystem.SaveSaplingShopInventory(
            shopInventoryScriptRef.saplingShopSlots,
            shopInventoryScriptRef.saplingShopRefreshTime);
        
        SCR_SaveData data = SCR_ReworkedSaveSystem.LoadGame();
        data.money = shopMenuScriptRef.moneyTotal;
        data.saplings = SCR_ReworkedSaveSystem.GetSaplingData(saplingInventory);
        SCR_ReworkedSaveSystem.SaveGame(data);
        
        playerManagerScriptRef.RefreshCountsFromSave();
    }
}
