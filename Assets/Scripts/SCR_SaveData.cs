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
public class FruitData
{
    public int growthStage;
    public bool beenHarvested;
    public bool isGold;
    public bool isIridescent;
    public int batchID = 0;
}
