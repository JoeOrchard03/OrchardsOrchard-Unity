using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_SaveSystem : MonoBehaviour
{
    private const string saveKey = "GameSave";
    public List<SCR_Plot> plots;
    
    public GameObject inventorySaplingPrefab;
    public Transform saplingInventory;
    
    public GameObject inventoryDecoPrefab;
    public Transform decoInventory;

    public SCR_Clock clockScriptRef;

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
            saveData.saplings = GetSaplingData(saplingInventory);
            SaveGame(saveData);
        }
        else
        {
            LoadSaplingData(saveData.saplings);
        }

        SetDay(saveData.isDay);
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
        Debug.Log("Save data cleared");
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
        List<SaplingData> data = new List<SaplingData>();

        foreach (var sapling in saplingInventory.GetComponentsInChildren<SCR_SaplingMenuBox>())
        {
            SaplingData newSapling = new SaplingData
            {
                dataFruitType = sapling.fruitType
            };
            
            data.Add(newSapling);
        }
        
        return data;
    }

    private void LoadSaplingData(List<SaplingData> data)
    {
        foreach (var entry in data)
        {
            GameObject sapling = Instantiate(inventorySaplingPrefab, saplingInventory);
            sapling.GetComponent<SCR_SaplingMenuBox>().fruitType = entry.dataFruitType;
        }
    }
    
    public void SaveSapling(GameObject saplingOBJ)
    {
        StartCoroutine(SaveSaplings(saplingOBJ));
    }

    private IEnumerator SaveSaplings(GameObject saplingOBJ)
    {
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
            SaplingData starterSapling = new SaplingData
            {
                dataFruitType = FruitType.Apple
            };
            
            data.saplings = new List<SaplingData>() { starterSapling };
            SaveGame(data);
            
            GameObject sapling = Instantiate(inventorySaplingPrefab, saplingInventory);
            sapling.GetComponent<SCR_SaplingMenuBox>().fruitType = starterSapling.dataFruitType;
        }
    }

    public static void SaveSaplingShopInventory(List<SCR_BuyableSapling> shopSlots, float shopTimer)
    {
        SCR_SaveData data = LoadGame();
        data.saplingShopSlots = new List<SaplingShopSlotData>();

        foreach (var slot in shopSlots)
        {
            if (slot == null) continue;

            SaplingShopSlotData slotData = new SaplingShopSlotData
            {
                fruitType = slot.fruitType,
                isSold = slot.outOfStockObj.activeSelf
            };
            
            data.saplingShopSlots.Add(slotData);
        }
        
        data.shopTimer = shopTimer;
        SaveGame(data);
    }
    
    public static void SaveDecoShopInventory(List<SCR_BuyableDeco> shopSlots, float shopTimer)
    {
        SCR_SaveData data = LoadGame();
        data.decoShopSlots = new List<DecoShopSlotData>();

        foreach (var slot in shopSlots)
        {
            if (slot == null) continue;

            DecoShopSlotData slotData = new DecoShopSlotData
            {
                decoType = slot.decoType,
                isSold = slot.outOfStockObj.activeSelf
            };
            
            data.decoShopSlots.Add(slotData);
        }
        
        data.shopTimer = shopTimer;
        SaveGame(data);
    }

    public static void LoadSaplingShopInventory(SCR_FruitDatabase fruitDatabase, List<SCR_BuyableSapling> shopSlots, ref float shopTimer)
    {
        SCR_SaveData data = LoadGame();

        if (data.saplingShopSlots == null || data.saplingShopSlots.Count == 0)
        {
            return;
        }

        shopTimer = data.shopTimer;

        for (int i = 0; i < shopSlots.Count && i < data.saplingShopSlots.Count; i++)
        {
            var slot = shopSlots[i];
            var savedSlot = data.saplingShopSlots[i];
            
            slot.fruitType = savedSlot.fruitType;
            slot.fruitDatabase = fruitDatabase;
            Debug.Log("Calling apply fruit info");
            slot.ApplyFruitInfo();

            if (savedSlot.isSold)
            {
                slot.DisableSlot();
            }
        }
    }
    
    public static void LoadDecoShopInventory(SCR_DecoDatabase decoDatabase, List<SCR_BuyableDeco> shopSlots, ref float shopTimer)
    {
        SCR_SaveData data = LoadGame();

        if (data.decoShopSlots == null || data.decoShopSlots.Count == 0)
        {
            return;
        }

        shopTimer = data.shopTimer;

        for (int i = 0; i < shopSlots.Count && i < data.saplingShopSlots.Count; i++)
        {
            var slot = shopSlots[i];
            var savedSlot = data.decoShopSlots[i];
            
            slot.decoType = savedSlot.decoType;
            slot.decoDatabase = decoDatabase;
            slot.ApplyDecoInfo();

            if (savedSlot.isSold)
            {
                slot.DisableSlot();
            }
        }
    }
    
    public static List<DecoData> GetDecoData(Transform decoInventory)
    {
        List<DecoData> data = new List<DecoData>();

        foreach (var deco in decoInventory.GetComponentsInChildren<SCR_DecoMenuBox>())
        {
            DecoData newDeco = new DecoData
            {
                dataDecoType = deco.decoType
            };
            
            data.Add(newDeco);
        }
        
        return data;
    }

    private void LoadDecoData(List<DecoData> data)
    {
        foreach (var entry in data)
        {
            GameObject deco = Instantiate(inventoryDecoPrefab, decoInventory);
            deco.GetComponent<SCR_DecoMenuBox>().decoType = entry.dataDecoType;
        }
    }
    
    public void SaveDeco(GameObject decoOBJ)
    {
        StartCoroutine(SaveDecos(decoOBJ));
    }

    private IEnumerator SaveDecos(GameObject decoOBJ)
    {
        Destroy(decoOBJ);
        
        yield return new WaitForEndOfFrame();
        
        SCR_SaveData data = LoadGame();
        data.decos = GetDecoData(gameObject.transform);
        SaveGame(data);
    }

    private void SetDay(bool setDay)
    {
        if (setDay)
        {
            clockScriptRef.SetDay(false);
        }
        else
        {
            clockScriptRef.SetNight(false);
        }
    }
}
