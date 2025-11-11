using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SCR_SaplingMenuBox : MonoBehaviour
{
    public SCR_FruitDatabase fruitDatabase;
    public FruitType fruitType;

    private GameObject player;
    public Image saplingImage;
    private GameObject selectedPlot;
    private GameObject saplingInventory;
    public SCR_ReworkedSaveSystem saveSystem;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        saplingInventory = transform.parent.gameObject;
        saveSystem = GameObject.Find("SaveManager").GetComponent<SCR_ReworkedSaveSystem>();
        LoadImage();
    }

    public void LoadImage()
    {
        saplingImage.sprite = fruitDatabase.GetFruit(fruitType).saplingSprite;
    }
    
    public void Plant()
    {
        if (player.GetComponent<SCR_PlayerManager>().selectedPlot == null) { return; }
        
        selectedPlot = player.GetComponent<SCR_PlayerManager>().selectedPlot;
        selectedPlot.GetComponent<SCR_Plot>().SaplingToPlant(fruitType.ToString());
        player.GetComponent<SCR_PlayerManager>().hoveredInteractable = null;

        RemoveSaplingAfterPlant();
    }

    private void RemoveSaplingAfterPlant()
    {
        SCR_SaveData data = SCR_ReworkedSaveSystem.LoadGame();
        
        //Finds a sapling that matches the fruit type that was planted
        var saplingToRemove = data.saplings.Find(s => s.dataFruitType == fruitType);
        if (saplingToRemove != null)
        {
            data.saplings.Remove(saplingToRemove);
            Debug.Log($"Removed {fruitType} from saved sapling inventory after planting.");
        }
        else
        {
            Debug.LogWarning($"Tried to remove {fruitType}, but it wasn't found in save data.");
        }
        
        SCR_ReworkedSaveSystem.SaveGame(data);
        Destroy(gameObject);
    }
}
