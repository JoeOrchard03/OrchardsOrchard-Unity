using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;

public class SCR_ReworkedSaveSystem : MonoBehaviour
{
    private const string SaveKey = "GameSave";

    [Header("Inventory")] 
    public GameObject inventorySaplingPrefab;
    public Transform saplingInventory;

    public GameObject inventoryDecoPrefab;
    public Transform decoInventory;
    
    [Header("References")]
    public List<SCR_Plot> plots;
    public SCR_Clock clockScriptRef;
    public SCR_Drone droneScriptRef;
    public Transform placedDecoHolder;
    public SCR_DecoDatabase decoDatabase;

    private void Awake()
    {
        //Populate the tree plots by type
        plots = new List<SCR_Plot>(FindObjectsByType<SCR_Plot>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
    }

    private void Start()
    {
        LoadAll();
    }
    
    #region Bulk Saves and Loads
    
    //Save everything
    public void SaveAll()
    {
        //Create new data to save and get data from each element
        SCR_SaveData data = new SCR_SaveData
        {
            trees = GetTreeData(),
            saplings = GetSaplingData(saplingInventory),
            decos = GetInventoryDecoData(decoInventory),
            compendiumEntries = GetCompendiumData(),
            isDay = clockScriptRef.isDay,
        };
        
        //Save all elements at once
        SaveGame(data);
        Debug.Log("Game saved successfully");
    }
    
    //Load everything at start
    public void LoadAll()
    {
        //Load data
        SCR_SaveData data = LoadGame();
        
        //Trees
        if (data.trees != null)
        {
            //Iterate through each tree in saved data
            foreach (var tree in data.trees)
            {
                //Find the corresponding plot for each tree
                var plot = plots.Find(p => p.plotNumber == tree.dataPlotNumber);
                if (plot != null)
                {
                    //Load the tree with the saved fruit type and growth stage
                    plot.LoadPlantedTree(tree.dataFruitType, tree.dataGrowthStage);
                }
            }
        }

        //Saplings
        ClearInventory(saplingInventory);
        if (data.saplings != null && data.saplings.Count > 0)
        {
            LoadSaplingData(data.saplings);
        }
        else
        {
            EnsureStartingSapling(data);
        }
        
        //Decorations
        ClearInventory(decoInventory);
        if (data.decos != null && data.decos.Count > 0)
        {
            LoadInventoryDecoData(data.decos);
        }

        if (data.placedDecoData != null && data.placedDecoData.Count > 0)
        {
            LoadPlacedDecoData(data.placedDecoData);
        }
        
        //Compendium
        if (data.compendiumEntries != null)
        {
            LoadCompendiumData(data.compendiumEntries);
        }
        
        var upgradeSlots = new List<SCR_BuyableDroneUpgrade>(FindObjectsByType<SCR_BuyableDroneUpgrade>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        
        LoadDroneUpgrades(upgradeSlots, droneScriptRef);
        
        //Clock
        SetDay(data.isDay);
        Debug.Log("Game loaded successfully");
    }
    
    #endregion
    
    #region Core Saves and Loads

    //Saves to play prefs
    public static void SaveGame(SCR_SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    //Loads from player prefs
    public static SCR_SaveData LoadGame()
    {
        //If there is a save key
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            return JsonUtility.FromJson<SCR_SaveData>(json);
        }

        return new SCR_SaveData();
    }
    
    public static void ClearSaveData()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        Debug.Log("Save data cleared");
    }
    
    #endregion
    
    #region Trees

    private List<TreeData> GetTreeData()
    {
        var trees = new List<TreeData>();
        //Iterate through each plot
        foreach (var plot in plots)
        {
            //If plot has a tree saved in it
            if (plot.plotOccupied)
            {
                //Create a new piece of TreeData and add it to trees
                trees.Add(new TreeData
                {
                    //Get data from the plot's data
                    dataPlotNumber = plot.plotNumber,
                    dataFruitType = plot.currentFruitType,
                    dataGrowthStage = plot.currentGrowthStage
                });
            }
        }
        return trees;
    }

    public static void RemoveTreeFromSave(int plotNumber)
    {
        SCR_SaveData data = LoadGame();
        data.trees.RemoveAll(tree => tree.dataPlotNumber == plotNumber);
        SaveGame(data);
    }
    
    #endregion
    
    #region Saplings
    
    public static List<SaplingData> GetSaplingData(Transform parent)
    {
        var data = new List<SaplingData>();
        //Get saplings from all objects in the sapling inventory parent
        foreach (var sapling in parent.GetComponentsInChildren<SCR_SaplingMenuBox>())
        {
            data.Add(new SaplingData {dataFruitType = sapling.fruitType});
        }
        return data;
    }

    private void LoadSaplingData(List<SaplingData> data)
    {
        foreach (var entry in data)
        {
            //Instantiate each sapling in sapling data
            GameObject sapling = Instantiate(inventorySaplingPrefab, saplingInventory);
            //Assign its saved fruit type
            sapling.GetComponent<SCR_SaplingMenuBox>().fruitType = entry.dataFruitType;
        }
    }

    private void EnsureStartingSapling(SCR_SaveData data)
    {
        //If there are no saved saplings or trees
        if ((data.saplings == null || data.saplings.Count == 0) && (data.trees == null || data.trees.Count == 0))
        {
            //Give the player an apple sapling
            SaplingData starter = new SaplingData { dataFruitType = FruitType.Apple };
            data.saplings = new List<SaplingData> { starter };
            //Load and save the new sapling's data
            LoadSaplingData(data.saplings);
            SaveGame(data);
        }
    }
    
    #endregion
    
    #region Decorations

    public static List<DecoData> GetInventoryDecoData(Transform parent)
    {
        var data = new List<DecoData>();
        //Get decos from all children of the deco inventory parent
        foreach (var deco in parent.GetComponentsInChildren<SCR_DecoMenuBox>())
        {
            data.Add(new DecoData{dataDecoType = deco.decoType});
        }

        return data;
    }

    private void LoadInventoryDecoData(List<DecoData> data)
    {
        //For each saved deco
        foreach (var entry in data)
        {
            //Instantiate the deco in the deco inventory
            GameObject deco = Instantiate(inventoryDecoPrefab, decoInventory);
            //Assign its saved type
            deco.GetComponent<SCR_DecoMenuBox>().decoType = entry.dataDecoType;
        }
    }

    public static List <PlacedDecoData> GetPlacedDecoData(Transform parent)
    {
        var data = new List<PlacedDecoData>();
        foreach (Transform deco in parent)
        {
            data.Add(new PlacedDecoData {decoPosition = deco.position, decoType = deco.gameObject.GetComponent<SCR_PlacedDeco>().decoType, flipped = deco.GetComponent<SpriteRenderer>().flipX});
        }

        return data;
    }

    private void LoadPlacedDecoData(List<PlacedDecoData> data)
    {
        foreach (var entry in data)
        {
            GameObject decoObj = decoDatabase.GetDeco(entry.decoType).decoPrefab;
            GameObject instantiatedDecoObj = Instantiate(decoObj, entry.decoPosition, transform.rotation, placedDecoHolder.transform);
            instantiatedDecoObj.GetComponent<SCR_PlacedDeco>().decoType = entry.decoType;
            instantiatedDecoObj.GetComponent<SpriteRenderer>().flipX = entry.flipped;
        }
    }
    
    #endregion
    
    #region Fruit Inventory

    public static void SaveFruitInventory(InventoryFruits fruitInventory)
    {
        SCR_SaveData data = LoadGame();
        data.playerInventory = fruitInventory;
        SaveGame(data);
    }

    public static InventoryFruits LoadFruitInventory()
    {
        SCR_SaveData data = LoadGame();
        return data.playerInventory ?? new InventoryFruits();
    }
    
    #endregion
    
    #region Compendium

    public static List<CompendiumEntryData> GetCompendiumData()
    {
        var data = new List<CompendiumEntryData>();
        //For each object of type SCR_FruitEntry
        foreach (var entry in FindObjectsByType<SCR_FruitEntry>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            //Add new entry using saved data
            data.Add(new CompendiumEntryData
            {
                fruitType = entry.fruitType,
                standardCollected = entry.standardCollected,
                goldCollected = entry.goldCollected,
                iridescentCollected = entry.iridescentCollected,
            });
        }

        return data;
    }

    private void LoadCompendiumData(List<CompendiumEntryData> entries)
    {
        if (SCR_Compendium.instance == null)
        {
            Debug.LogError("SCR_Compendium.instance == null, returning...");
            return;
        }

        //Fill out entries with saved data
        foreach (var entry in entries)
        {
            SCR_Compendium.instance.MarkFruit(
                entry.fruitType,
                entry.standardCollected,
                entry.goldCollected,
                entry.iridescentCollected);
        }

        //Refreshes each entry with saved data
        foreach (var fruitEntry in FindObjectsByType<SCR_FruitEntry>(FindObjectsInactive.Include,
                     FindObjectsSortMode.None))
        {
            fruitEntry.RefreshEntries();
        }
    }

    #endregion
    
    #region Shop : Saplings
    
    public static void SaveSaplingShopInventory(List<SCR_BuyableSapling> shopSlots, float saplingShopTimer)
    {
        //Load data
        SCR_SaveData data = LoadGame();
        //Get the shop slots and timer
        data.saplingShopSlots = new List<SaplingShopSlotData>();
        data.saplingShopTimer = saplingShopTimer;

        foreach (var slot in shopSlots)
        {
            if (slot == null) continue;

            //Create new slot data
            SaplingShopSlotData slotData = new SaplingShopSlotData
            {
                //Assign data from existing slot data
                fruitType = slot.fruitType,
                isSold = slot.outOfStockObj.activeSelf
            };
            
            //Add slot data to data
            data.saplingShopSlots.Add(slotData);
        }
        
        //Save new data
        SaveGame(data);
        Debug.Log("Sapling shop saved");
    }

    public static void LoadSaplingShopInventory(SCR_FruitDatabase fruitDatabase, List<SCR_BuyableSapling> shopSlots, ref float saplingShopTimer)
    {
        SCR_SaveData data = LoadGame();

        if (data.saplingShopSlots == null || data.saplingShopSlots.Count == 0)
        {
            Debug.Log("No saved sapling shop found, skipping...");
            return;
        }
        
        saplingShopTimer = data.saplingShopTimer;

        //For each shop slot
        for (int i = 0; i < shopSlots.Count && i < data.saplingShopSlots.Count; i++)
        {
            //Assign relevant slots so they can be loaded
            var slot = shopSlots[i];
            var savedSlot =  data.saplingShopSlots[i];
            
            //Apply data
            slot.fruitType = savedSlot.fruitType;
            slot.fruitDatabase = fruitDatabase;
            //Load data
            slot.ApplyFruitInfo();

            //Check if that slot has been sold out previously
            if (savedSlot.isSold)
            {
                slot.DisableSlot();
            }
        }
        
        Debug.Log("Sapling shop loaded");
    }
    
    #endregion
    
    #region Shop : Decorations

    public static void SaveDecoShopInventory(List<SCR_BuyableDeco> shopSlots, float decoShopTimer)
    {
        SCR_SaveData data = LoadGame();
        data.decoShopSlots = new List<DecoShopSlotData>();
        data.decoShopTimer = decoShopTimer;

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
        
        SaveGame(data);
        Debug.Log("Deco shop saved");
    }

    public static void LoadDecoShopInventory(SCR_DecoDatabase decoDatabase, List<SCR_BuyableDeco> shopSlots,
        ref float decoShopTimer)
    {
        SCR_SaveData data = LoadGame();

        if (data.decoShopSlots == null || data.decoShopSlots.Count == 0)
        {
            Debug.Log("No saved deco shop found, skipping...");
            return;
        }
        
        decoShopTimer = data.decoShopTimer;
        
        //For each shop slot
        for (int i = 0; i < shopSlots.Count && i < data.decoShopSlots.Count; i++)
        {
            //Assign slots so they can be linked
            var slot = shopSlots[i];
            var savedSlot = data.decoShopSlots[i];
            
            //Apply data
            slot.decoType = savedSlot.decoType;
            slot.decoDatabase = decoDatabase;
            //Load Data
            slot.ApplyDecoInfo();

            //Check if slot was sold out previously
            if (savedSlot.isSold)
            {
                slot.DisableSlot();
            }
        }
        
        Debug.Log("Deco shop loaded");
    }
    
    #endregion
    
    #region Shop : Timer

    public static void SaveSaplingShopTimer(float saplingShopTimer)
    {
        SCR_SaveData data = LoadGame();
        data.saplingShopTimer = saplingShopTimer;
        SaveGame(data);
    }

    public static void SaveDecoShopTimer(float decoShopTimer)
    {
        SCR_SaveData data = LoadGame();
        data.decoShopTimer = decoShopTimer;
        SaveGame(data);
    }

    public static float LoadSaplingShopTimer()
    {
        SCR_SaveData data = LoadGame();
        return data.saplingShopTimer;
    }
    
    public static float LoadDecoShopTimer()
    {
        SCR_SaveData data = LoadGame();
        return data.decoShopTimer;
    }
    
    #endregion
    
    #region Misc Functions

    private void ClearInventory(Transform parent)
    {
        //Clears each child out of the parent inventory
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    //Sets the saved time of day when the player quit
    private void SetDay(bool isDay)
    {
        if (isDay)
        {
            clockScriptRef.SetDay(false);
        }
        else
        {
            clockScriptRef.SetNight(false);
        }
    }
    
    public static void SaveSingleField(Action<SCR_SaveData> modify)
    {
        SCR_SaveData data = LoadGame();
        modify(data);
        SaveGame(data);
    }
    
    #endregion
    
    #region Drone Upgrades

    public static void SaveDroneUpgrades(List<SCR_BuyableDroneUpgrade> upgradeSlots)
    {
        SCR_SaveData data = LoadGame();
        data.droneUpgrades = new List<DroneUpgradeData>();

        foreach (var slot in upgradeSlots)
        {
            if (slot == null)
            {
                continue;
            }
            
            data.droneUpgrades.Add(new DroneUpgradeData
            {
                upgradeType = slot.upgradeType,
                count = slot.upgradeCount
            });
        }
        
        SaveGame(data);
        Debug.Log("Drone upgrades saved");
    }

    public void LoadDroneUpgrades(List<SCR_BuyableDroneUpgrade> upgradeSlots, SCR_Drone drone)
    {
        SCR_SaveData data = LoadGame();

        if (data.droneUpgrades == null || data.droneUpgrades.Count == 0)
        {
            Debug.Log("No saved drone upgrades found, skipping...");
            return;
        }

        foreach (var saved in data.droneUpgrades)
        {
            var slot = upgradeSlots.Find(x => x.upgradeType == saved.upgradeType);
            if (slot == null)
            {
                continue;
            }

            slot.upgradeCount = saved.count;
        }


        foreach (var slot in upgradeSlots)
        {
            slot.RefreshUI();
        }

        drone.ApplyDroneUpgrades(upgradeSlots.ToArray());

        Debug.Log("Drone upgrades loaded");
    }

    #endregion
    
}
