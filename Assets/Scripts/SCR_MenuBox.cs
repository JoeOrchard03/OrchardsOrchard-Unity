using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SCR_MenuBox : MonoBehaviour
{
    public SCR_FruitDatabase fruitDatabase;
    public FruitType fruitType;

    private GameObject player;
    public Image saplingImage;
    private GameObject selectedPlot;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        saplingImage.sprite = fruitDatabase.GetFruit(fruitType).saplingSprite;
    }
    
    public void Plant()
    {
        if (player.GetComponent<SCR_PlayerManager>().selectedPlot == null) { return; }
        selectedPlot = player.GetComponent<SCR_PlayerManager>().selectedPlot;
        selectedPlot.GetComponent<SCR_Plot>().SaplingToPlant(fruitType.ToString());
        player.GetComponent<SCR_PlayerManager>().hoveredInteractable = null;
        Destroy(this.gameObject);
    }
}
