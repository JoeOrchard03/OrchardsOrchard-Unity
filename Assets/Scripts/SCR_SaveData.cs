using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SCR_SaveData
{
    public float money;
    public float masterVolume;
    public float musicVolume;

    public bool isDay = true;

    public float shopTimer;
    
    public List<TreeData> trees = new List<TreeData>();
    public List<SaplingData> saplings = new List<SaplingData>();
    public List<DecoData> decos = new List<DecoData>();
    public InventoryFruits playerInventory =  new InventoryFruits();
    public List<SaplingShopSlotData> saplingShopSlots = new List<SaplingShopSlotData>();
    public List<DecoShopSlotData> decoShopSlots = new List<DecoShopSlotData>();
    public List<CompendiumEntryData> compendiumEntries = new List<CompendiumEntryData>();
}

[System.Serializable]
public class SaplingShopSlotData
{
    public FruitType fruitType;
    public bool isSold;
}

[System.Serializable]
public class DecoShopSlotData
{
    public DecoType decoType;
    public bool isSold;
}


[System.Serializable]
public class TreeData
{
    public int dataPlotNumber;
    public FruitType dataFruitType;
    public int dataGrowthStage;
    public List<FruitData> fruits = new List<FruitData>();
}

[System.Serializable]
public class SaplingData
{
    public FruitType dataFruitType;
}

public class DecoData
{
    public DecoType dataDecoType;
}

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

[System.Serializable]
public class InventoryFruits
{
    public List<FruitData> fruits = new List<FruitData>();
}

[System.Serializable]
public class InventoryDecos
{
    public List<DecoData> decos = new List<DecoData>();
}

[System.Serializable]
public class CompendiumEntryData
{
    public FruitType fruitType;
    public bool standardCollected;
    public bool goldCollected;
    public bool iridescentCollected;
}
