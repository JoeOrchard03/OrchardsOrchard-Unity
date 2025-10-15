using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_TreeLoader : MonoBehaviour
{
    // public List<SCR_Plot> plots;
    //
    // private void Awake()
    // {
    //     plots = new List<SCR_Plot>(FindObjectsByType<SCR_Plot>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
    // }
    //
    // private void Start()
    // {
    //     List<TreeData> savedTrees = SCR_SaveSystem.LoadTrees();
    //
    //     foreach (TreeData data in savedTrees)
    //     {
    //         SCR_Plot plot = plots.Find(p => p.plotNumber == data.dataPlotNumber);
    //         if (plot != null)
    //         {
    //             plot.LoadPlantedTree(data.dataFruitType, data.dataGrowthStage);
    //         }
    //     }
    // }
}
