using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SCR_BuyableDeco : MonoBehaviour
{
    public DecoType decoType;
    public SCR_DecoDatabase decoDatabase;

    public Image decoImage;
    public TextMeshProUGUI priceText;

    private float decoPrice;

    public string decoName;

    public BoxCollider2D decoBoxCollider;
    
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
            decoBoxCollider.enabled = true;
            moneyIcon.SetActive(true);
            decoPrice = deco.decoPrice;
            decoName = deco.DecoName;
            Debug.Log("what is being loaded is: " + decoDatabase.GetDeco(decoType).decoSprite);
            decoImage.sprite = decoDatabase.GetDeco(decoType).decoSprite;
            priceText.text = deco.decoPrice.ToString();
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
        decoBoxCollider.enabled = false;
        BuyTextObj.SetActive(false);
        decoImage.sprite = null;
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
        shopMenuScriptRef.saplingTabTotalText.text = shopMenuScriptRef.moneyTotal.ToString();
        shopMenuScriptRef.decorTabTotalText.text = shopMenuScriptRef.moneyTotal.ToString();
        shopMenuScriptRef.upgradeTabTotalText.text = shopMenuScriptRef.moneyTotal.ToString();
        
        GameObject deco = Instantiate(inventoryDecoPrefab, decoInventory);
        deco.GetComponent<SCR_DecoMenuBox>().decoType = decoType;
        
        DisableSlot();
        
        Debug.Log("Children in decoInventory: " + decoInventory.childCount);

        SCR_ShopInventory shopInventoryScriptRef = GameObject.FindFirstObjectByType<SCR_ShopInventory>().GetComponent<SCR_ShopInventory>();
        
        SCR_ReworkedSaveSystem.SaveDecoShopInventory(
            shopInventoryScriptRef.decoShopSlots,
            shopInventoryScriptRef.decoShopRefreshTime);
        
        SCR_SaveData data = SCR_ReworkedSaveSystem.LoadGame();
        data.money = shopMenuScriptRef.moneyTotal;
        data.decos = SCR_ReworkedSaveSystem.GetInventoryDecoData(decoInventory);
        SCR_ReworkedSaveSystem.SaveGame(data);
    }
}
