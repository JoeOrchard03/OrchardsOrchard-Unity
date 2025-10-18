using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_SaveSystem : MonoBehaviour
{
    private const string saveKey = "GameSave";
    public List<SCR_Plot> plots;
    
    public GameObject inventorySaplingPrefab;
    public Transform saplingInventory;

    private void Awake()
    {
        plots = new List<SCR_Plot>(FindObjectsByType<SCR_Plot>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
    }

    private void Start()
    {
        List<TreeData> savedTrees = LoadTrees();

        foreach (TreeData data in savedTrees)
        {
            SCR_Plot plot = plots.Find(p => p.plotNumber == data.dataPlotNumber);
            if (plot != null)
            {
                plot.LoadPlantedTree(data.dataFruitType, data.dataGrowthStage);
            }
        }

        SCR_SaveData saveData = LoadGame();
        if (saveData.compendiumEntries != null)
        {
            LoadCompendiumData(saveData.compendiumEntries);
        }

        if (saveData.saplings == null || saveData.saplings.Count == 0)
        {
            Debug.Log("No saplings found in save, saving starting saplings...");
            saveData.saplings = GetSaplingData(saplingInventory);
            SaveGame(saveData);
        }
        else
        {
            LoadSaplingData(saveData.saplings);
        }

        EnsureStartingSapling(saveData);
        UpdateTreeAndSaplingCounts();
    }

    private void UpdateTreeAndSaplingCounts()
    {
        FindFirstObjectByType<SCR_PlayerManager>().UpdateCounts();
    }
    
    public static void SaveGame(SCR_SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
        //Debug.Log("Game saved, stored at: " + json);
    }

    public static SCR_SaveData LoadGame()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string json = PlayerPrefs.GetString(saveKey);
            SCR_SaveData data = JsonUtility.FromJson<SCR_SaveData>(json);
            //Debug.Log("Game loaded from: " + json);
            return data;
        }

        //Debug.Log("No save data found, creating new save file...");
        return new SCR_SaveData();
    }

    public static void SaveTrees(List<TreeData> trees)
    {
        SCR_SaveData data = LoadGame();
        data.trees = trees;
        SaveGame(data);
    }

    public static List<TreeData> LoadTrees()
    {
        SCR_SaveData data = LoadGame();
        return data.trees ?? new List<TreeData>();
    }

    public static List<SaplingData> LoadSaplings()
    {
        SCR_SaveData data = LoadGame();
        return data.saplings ?? new List<SaplingData>();
    }

    //Save fruit inventory
    public static void SaveFruitInventory(InventoryFruits fruitInventory)
    {
        SCR_SaveData data = LoadGame();
        data.playerInventory = fruitInventory;
        SaveGame(data);
    }

    //Load fruit inventory
    public static InventoryFruits LoadFruitInventory()
    {
        SCR_SaveData data = LoadGame();
        //return player inventory if none exists, make a new one
        return data.playerInventory ?? new InventoryFruits();
    }

    public static void RemoveTreeFromSave(int plotNumber)
    {
        SCR_SaveData data = LoadGame();
        data.trees.RemoveAll(tree => tree.dataPlotNumber == plotNumber);
        SaveGame(data);
    }

    public void ClearSaveData()
    {
        PlayerPrefs.DeleteKey(saveKey);
        PlayerPrefs.Save();
        Debug.Log("Clearing save data...");
    }

    public static List<CompendiumEntryData> GetCompendiumData()
    {
        List<CompendiumEntryData> data = new List<CompendiumEntryData>();

        foreach (var entry in FindObjectsByType<SCR_FruitEntry>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            CompendiumEntryData newEntry = new CompendiumEntryData
            {
                fruitType = entry.fruitType,
                standardCollected = entry.standardCollected,
                goldCollected = entry.goldCollected,
                iridescentCollected = entry.iridescentCollected
            };

            data.Add(newEntry);
        }

        return data;
    }

    private void LoadCompendiumData(List<CompendiumEntryData> data)
    {
        foreach (var entry in data)
        {
            if (SCR_Compendium.instance == null) continue;

            SCR_Compendium.instance.MarkFruit(
                entry.fruitType,
                entry.goldCollected,
                entry.iridescentCollected,
                entry.standardCollected
            );
        }

        foreach (var fruitEntry in FindObjectsByType<SCR_FruitEntry>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            fruitEntry.RefreshEntries();
        }
    }

    public static List<SaplingData> GetSaplingData(Transform saplingInventory)
    {
        Debug.Log("Getting sapling data...");
        List<SaplingData> data = new List<SaplingData>();

        foreach (var sapling in saplingInventory.GetComponentsInChildren<SCR_MenuBox>())
        {
            SaplingData newSapling = new SaplingData
            {
                dataFruitType = sapling.fruitType
            };
            
            data.Add(newSapling);
            Debug.Log("Got sapling data of type: " +  sapling.fruitType);
        }

        Debug.Log("Successfully returned sapling data");
        return data;
    }

    private void LoadSaplingData(List<SaplingData> data)
    {
        foreach (var entry in data)
        {
            GameObject sapling = Instantiate(inventorySaplingPrefab, saplingInventory);
            sapling.GetComponent<SCR_MenuBox>().fruitType = entry.dataFruitType;
        
            Debug.Log("Adding " + entry.dataFruitType + " sapling to inventory");
        }
    }
    
    public void SaveSapling(GameObject saplingOBJ)
    {
        Debug.Log("Trying to save saplings");
        StartCoroutine(SaveSaplings(saplingOBJ));
    }

    private IEnumerator SaveSaplings(GameObject saplingOBJ)
    {
        Debug.Log("Saving Saplings");
        Destroy(saplingOBJ);
        
        yield return new WaitForEndOfFrame();
        
        SCR_SaveData data = LoadGame();
        data.saplings = GetSaplingData(gameObject.transform);
        SaveGame(data);
        UpdateTreeAndSaplingCounts();
    }

    private void EnsureStartingSapling(SCR_SaveData data)
    {
        bool hasNoSaplings = (data.saplings == null || data.saplings.Count == 0);
        bool hasNoTrees = (data.trees == null || data.trees.Count == 0);

        if (hasNoSaplings && hasNoTrees)
        {
            Debug.Log("Player has no saplings or trees, providing apple sapling");

            SaplingData starterSapling = new SaplingData
            {
                dataFruitType = FruitType.Apple
            };
            
            data.saplings = new List<SaplingData>() { starterSapling };
            SaveGame(data);
            
            GameObject sapling = Instantiate(inventorySaplingPrefab, saplingInventory);
            sapling.GetComponent<SCR_MenuBox>().fruitType = starterSapling.dataFruitType;
            
            Debug.Log("Apple sapling provided");
        }
    }

    public static void SaveShopInventory(List<SCR_BuyableSapling> shopSlots, float shopTimer)
    {
        SCR_SaveData data = LoadGame();
        data.shopSlots = new List<ShopSlotData>();

        foreach (var slot in shopSlots)
        {
            if (slot == null) continue;

            ShopSlotData slotData = new ShopSlotData
            {
                fruitType = slot.fruitType,
                isSold = slot.outOfStockObj.activeSelf
            };
            
            data.shopSlots.Add(slotData);
        }
        
        data.shopTimer = shopTimer;
        SaveGame(data);
        Debug.Log("Shop inventory saved.");
    }

    public static void LoadShopInventory(SCR_FruitDatabase fruitDatabase, List<SCR_BuyableSapling> shopSlots, ref float shopTimer)
    {
        SCR_SaveData data = LoadGame();

        if (data.shopSlots == null || data.shopSlots.Count == 0)
        {
            Debug.Log("No saved shop inventory found, generating new stock");
            return;
        }

        shopTimer = data.shopTimer;

        for (int i = 0; i < shopSlots.Count && i < data.shopSlots.Count; i++)
        {
            var slot = shopSlots[i];
            var savedSlot = data.shopSlots[i];
            
            slot.fruitType = savedSlot.fruitType;
            slot.fruitDatabase = fruitDatabase;
            slot.ApplyFruitInfo();

            if (savedSlot.isSold)
            {
                slot.DisableSlot();
            }
        }
        
        Debug.Log("Shop inventory loaded.");
    }
}
