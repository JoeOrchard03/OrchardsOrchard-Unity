using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SCR_SaveData
{
    //General
    public float money;
    public float masterVolume;
    public float musicVolume;
    public bool isDay = true;

    //Timers
    public float saplingShopTimer;
    public float decoShopTimer;

    //Scene state
    public List<TreeData> trees;
    
    //Inventories
    public List<SaplingData> saplings;
    public List<DecoData> decos;
    public InventoryFruits playerInventory =  new InventoryFruits();
    
    //Shop state
    public List<SaplingShopSlotData> saplingShopSlots = new List<SaplingShopSlotData>();
    public List<DecoShopSlotData> decoShopSlots = new List<DecoShopSlotData>();
    
    //Compendium
    public List<CompendiumEntryData> compendiumEntries = new List<CompendiumEntryData>();
    
    //Placed deco data
    public List<PlacedDecoData> placedDecoData = new List<PlacedDecoData>();
}

//Tree in scene's data
[System.Serializable]
public class TreeData
{
    public int dataPlotNumber;
    public FruitType dataFruitType;
    public int dataGrowthStage;
    public List<FruitData> fruits = new List<FruitData>();
}

//Sapling in player's inventory data
[System.Serializable]
public class SaplingData
{
    public FruitType dataFruitType;
}

//Decoration in player's inventory data
[System.Serializable]
public class DecoData
{
    public DecoType dataDecoType;
}

//Placed decoration data
[System.Serializable]
public class PlacedDecoData
{
    public DecoType decoType;
    public bool flipped;
    public Vector2 decoPosition;
}

//Fruit data used for trees and inventory
[System.Serializable]
public class FruitData
{
    public int growthStage;
    public bool beenHarvested;
    public bool isGold;
    public bool isIridescent;
    public FruitType fruitType;
    public int batchID = 0;
}

//Player's fruit inventory
[System.Serializable]
public class InventoryFruits
{
    public List<FruitData> fruits = new List<FruitData>();
}
//Data for compendium entries
[System.Serializable]
public class CompendiumEntryData
{
    public FruitType fruitType;
    public bool standardCollected;
    public bool goldCollected;
    public bool iridescentCollected;
}

//Save data for each sapling shop slot
[System.Serializable]
public class SaplingShopSlotData
{
    public FruitType fruitType;
    public bool isSold;
}

//Save data for each deco shop slot
[System.Serializable]
public class DecoShopSlotData
{
    public DecoType decoType;
    public bool isSold;
}






