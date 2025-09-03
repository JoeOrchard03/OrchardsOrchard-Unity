using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Plot : MonoBehaviour, INT_Interactable
{
    [Header("Saplings")] [SerializeField] 
    private GameObject AppleTreePrefab;

    [SerializeField] private GameObject SaplingMenu;
    public GameObject SaplingSpawnLocation;
    private SCR_Interact playerInteractScriptRef;
    private bool plotOccupied = false;

    private void Start()
    {
        playerInteractScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_Interact>();
    }
    
    public void Interact(GameObject interactor)
    {
        if (plotOccupied) { return;}
        playerInteractScriptRef.selectedPlot = this.gameObject;
        playerInteractScriptRef.menuOpen = true;
        OpenSaplingMenu();
    }

    private void OpenSaplingMenu()
    {
        SaplingMenu.transform.position = new Vector2(
            this.gameObject.transform.position.x,
            this.gameObject.transform.position.y + 4);
        SaplingMenu.SetActive(true);
    }

    public void SaplingToPlant(string SaplingName)
    {
        switch (SaplingName)
        {
            case "AppleTree":
                Plant(AppleTreePrefab);
                break;
            default:
                Debug.Log(SaplingName + " is not a valid sapling name");
                break;
        }
    }

    private void Plant(GameObject Sapling)
    {
        GameObject instantiatedSapling = Instantiate(Sapling, SaplingSpawnLocation.transform.position, transform.rotation);
        instantiatedSapling.GetComponent<SCR_TreeGrowthCycle>().motherPlot = this.gameObject;;
        playerInteractScriptRef.selectedPlot = null;
        SaplingMenu.SetActive(false);
        plotOccupied = true;
        GetComponent<SCR_Highlightable>().stopHighlight = true;
    }
}
