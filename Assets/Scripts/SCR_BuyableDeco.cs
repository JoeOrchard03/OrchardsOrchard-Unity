using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SCR_BuyableDeco : MonoBehaviour
{
    public DecoType decoType;
    public SCR_DecoDatabase decoDatabase;

    public SpriteRenderer decoSprite;
    public TextMeshProUGUI priceText;

    private float decoPrice;
    
    private SCR_ShopMenu shopMenuScriptRef;

    public GameObject inventoryDecoPrefab;
    public Transform decoInventory;

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
    
    public void ApplyDecoInfo()
    {
        var deco = decoDatabase.GetDeco(decoType);
        if (deco != null)
        {
            Debug.Log("Resetting deco: " + gameObject.name.ToString());
            outOfStockObj.SetActive(false);
            buttonObj.SetActive(true);
            BuyTextObj.SetActive(true);
            moneyIcon.SetActive(true);
            decoPrice = deco.decoPrice;
            decoSprite.sprite = deco.decoSprite;
            priceText.text = decoPrice.ToString();
        }
        else
        {
            Debug.LogError("DECO NOT FOUND");
        }
    }

    public void DisableSlot()
    {
        outOfStockObj.SetActive(true);
        buttonObj.SetActive(false);
        moneyIcon.SetActive(false);
        BuyTextObj.SetActive(false);
        decoSprite.sprite = null;
        priceText.text = "";
    }
    
    public void BuyDeco()
    {
        if (shopMenuScriptRef.moneyTotal < decoPrice)
        {
            Debug.Log("Cannot afford deco");
            return;
        }

        buttonAudioSource.Play();
        shopMenuScriptRef.moneyTotal -= decoPrice;
        shopMenuScriptRef.moneyTotalText.text = shopMenuScriptRef.moneyTotal.ToString();
        
        GameObject deco = Instantiate(inventoryDecoPrefab, decoInventory);
        deco.GetComponent<SCR_DecoMenuBox>().decoType = decoType;
        
        DisableSlot();
        
        Debug.Log("Adding " + decoType.ToString() + " deco to inventory");

        SCR_ShopInventory shopInventoryScriptRef = GameObject.FindFirstObjectByType<SCR_ShopInventory>().GetComponent<SCR_ShopInventory>();
        
        SCR_SaveSystem.SaveSaplingShopInventory(
            shopInventoryScriptRef.shopSlots,
            shopInventoryScriptRef.shopRefreshTime);
        
        SCR_SaveData data = SCR_SaveSystem.LoadGame();
        data.money = shopMenuScriptRef.moneyTotal;
        data.decos = SCR_SaveSystem.GetDecoData(decoInventory);
        SCR_SaveSystem.SaveGame(data);
        
        playerManagerScriptRef.UpdateCounts();
    }
}
