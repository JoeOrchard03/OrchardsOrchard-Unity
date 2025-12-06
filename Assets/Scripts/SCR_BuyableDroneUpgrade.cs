using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SCR_BuyableDroneUpgrade : MonoBehaviour
{
    public enum droneUpgrade
    {
        speedUpgrade,
        armSpeedUpgrade,
        lightUpgrade,
        treeShakerUpgrade
    }
    
    [Header("Upgrade Settings")]
    public int maxUpgradeCount;
    public int upgradeCount;
    
    [Header("UI References")]
    public GameObject buyButtonOBJ;
    public SpriteRenderer upgradeSpriteRenderer;
    public Sprite upgradeSprite;
    public string upgradeName;
    public TextMeshProUGUI upgradeLevelText;
    public TextMeshProUGUI priceText;
    public GameObject BuyTextObj;
    public GameObject moneyIcon;
    public AudioSource buttonAudioSource;
    
    [Header("Upgrade Values")]
    public int upgradePrice;
    public float droneSpeedIncrease;
    public float droneArmSpeedIncrease;

    [Header("References")]
    public SCR_PlayerManager playerManagerScriptRef;
    private SCR_Drone droneScriptRef;
    private SCR_ShopMenu shopMenuScriptRef;
    public BoxCollider2D upgradeBoxCollider;
    
    public droneUpgrade upgradeType;

    private void Awake()
    {
        playerManagerScriptRef = GameObject.Find("PlayerOBJ").GetComponent<SCR_PlayerManager>();
        droneScriptRef = playerManagerScriptRef.droneScriptRef;
        shopMenuScriptRef = GameObject.FindGameObjectWithTag("ShopMenu").GetComponent<SCR_ShopMenu>();
    }
    
    private void Start()
    {
        SCR_SaveData saveData = SCR_ReworkedSaveSystem.LoadGame();

        switch (upgradeType)
        {
            case droneUpgrade.speedUpgrade:
                upgradeCount = saveData.droneSaveData.speedUpgradeCount;
                break;
            case droneUpgrade.armSpeedUpgrade:
                upgradeCount = saveData.droneSaveData.armSpeedUpgradeCount;
                break;
            case droneUpgrade.lightUpgrade:
                upgradeCount = saveData.droneSaveData.lightUpgradeActive ? 1 : 0;
                break;
            case droneUpgrade.treeShakerUpgrade:
                upgradeCount = saveData.droneSaveData.treeShakerActive ? 1 : 0;
                break;
        }

        ApplyUpgradeToDrone(); // apply upgrades after loading
        RefreshUI(); // update UI to reflect saved upgradeCount
    }

    public void RefreshUI()
    {
        upgradeLevelText.text = $"{upgradeCount}/{maxUpgradeCount}";

        if (upgradeCount >= maxUpgradeCount)
        {
            DisableSlot();
        }
        else
        {
            EnableSlot();
        }
    }
    
    public void EnableSlot()
    {
        buyButtonOBJ.SetActive(true);
        BuyTextObj.SetActive(true);
        moneyIcon.SetActive(true);
        upgradeBoxCollider.enabled = true;

        priceText.text = upgradePrice.ToString();
        upgradeSpriteRenderer.sprite = upgradeSprite;
    }

    public void DisableSlot()
    {
        buyButtonOBJ.SetActive(false);
        moneyIcon.SetActive(false);
        upgradeBoxCollider.enabled = false;
        BuyTextObj.SetActive(false);

        priceText.text = "";
    }
    
    public void BuyUpgrade()
    {
        if (shopMenuScriptRef.moneyTotal < upgradePrice)
        {
            Debug.Log("Cannot afford upgrade");
            return;
        }

        buttonAudioSource.Play();
        shopMenuScriptRef.moneyTotal -= upgradePrice;
        
        shopMenuScriptRef.saplingTabTotalText.text = shopMenuScriptRef.moneyTotal.ToString();
        shopMenuScriptRef.decorTabTotalText.text = shopMenuScriptRef.moneyTotal.ToString();
        shopMenuScriptRef.upgradeTabTotalText.text = shopMenuScriptRef.moneyTotal.ToString();

        upgradeCount++;
        ApplyUpgradeToDrone();

        RefreshUI();
        
        SCR_ReworkedSaveSystem.SaveSingleField(data => {data.money = shopMenuScriptRef.moneyTotal;});

        SaveDroneUpgrades(FindObjectsByType<SCR_BuyableDroneUpgrade>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        
        if (upgradeCount >= maxUpgradeCount)
        {
            DisableSlot();
        }
    }

    public void ApplyUpgradeToDrone()
    {
        switch (upgradeType)
        {
            case droneUpgrade.speedUpgrade:
                droneScriptRef.droneDriveSpeed = droneScriptRef.baseDroneDriveSpeed + (upgradeCount * droneSpeedIncrease);
                Debug.Log("Applying drone Speed Upgrade");
                break;
            case droneUpgrade.armSpeedUpgrade:
                droneScriptRef.armExtendSpeed = droneScriptRef.baseArmExtendSpeed + (upgradeCount * droneArmSpeedIncrease);
                Debug.Log("Applying drone Arm Upgrade");
                break;
            case droneUpgrade.lightUpgrade:
                droneScriptRef.lightActive = upgradeCount > 0;
                Debug.Log("Applying drone Light Upgrade");
                break;
            case droneUpgrade.treeShakerUpgrade:
                if (upgradeCount > 0)
                {
                    droneScriptRef.EnableTreeShaker();
                    Debug.Log("Applying drone Tree Shaker Upgrade");
                }
                break;
        }
    }
    
    public void SaveDroneUpgrades(SCR_BuyableDroneUpgrade[] upgrades)
    {
        SCR_SaveData saveData = SCR_ReworkedSaveSystem.LoadGame();
        if (saveData.droneSaveData == null)
        {
            saveData.droneSaveData = new DroneSaveData();
        }

        switch (upgradeType)
        {
            case droneUpgrade.speedUpgrade:
                saveData.droneSaveData.speedUpgradeCount = upgradeCount;
                saveData.droneSaveData.droneSpeedIncrease = droneSpeedIncrease;
                break;
            case droneUpgrade.armSpeedUpgrade:
                saveData.droneSaveData.armSpeedUpgradeCount = upgradeCount;
                saveData.droneSaveData.armSpeedIncrease = droneArmSpeedIncrease;
                break;
            case droneUpgrade.lightUpgrade:
                saveData.droneSaveData.lightUpgradeActive = upgradeCount > 0;
                break;
            case droneUpgrade.treeShakerUpgrade:
                saveData.droneSaveData.treeShakerActive = upgradeCount > 0;
                break;
        }

        SCR_ReworkedSaveSystem.SaveGame(saveData);
    }
}
