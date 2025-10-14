using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_SaveSystem : MonoBehaviour
{
    private const string saveKey = "GameSave";
    
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
           // Debug.Log("Game loaded from: " + json);
            return data;
        }

        Debug.Log("No save data found, creating new save file...");
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
}
