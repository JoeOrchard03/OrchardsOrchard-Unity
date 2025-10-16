using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SCR_SaveData
{
    public float money;
    public float masterVolume;
    public float musicVolume;
    public List<TreeData> trees = new List<TreeData>();
    public List<SaplingData> saplings = new List<SaplingData>();
    public InventoryFruits playerInventory =  new InventoryFruits();
    public List<CompendiumEntryData> compendiumEntries = new List<CompendiumEntryData>();
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

[System.Serializable]
public class FruitData
{
    public int growthStage;
    public bool beenHarvested;
    public bool isGold;
    public bool isIridescent;
    public Vector3 fruitPos;
    public FruitType fruitType;
    public int batchID = 0;
}

[System.Serializable]
public class InventoryFruits
{
    public List<FruitData> fruits = new List<FruitData>();
}

[System.Serializable]
public class CompendiumEntryData
{
    public FruitType fruitType;
    public bool standardCollected;
    public bool goldCollected;
    public bool iridescentCollected;
}
