using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Plot : MonoBehaviour, INT_Interactable
{
    [Header("Saplings")] [SerializeField] 
    public GameObject AppleTreePrefab;
    public GameObject CherryTreePrefab;
    public GameObject LemonTreePrefab;
    public GameObject LimeTreePrefab;
    public GameObject LycheTreePrefab;
    public GameObject OrangeTreePrefab;
    public GameObject PeachTreePrefab;
    public GameObject PapayaTreePrefab;
    public GameObject PlumTreePrefab;
    public GameObject OliveTreePrefab;
    public GameObject CocoaTreePrefab;
    public GameObject MullberryTreePrefab;
    public GameObject DateTreePrefab;
    public GameObject AvocadoTreePrefab;
    public GameObject CrabAppleTreePrefab;
    public GameObject KumquatTreePrefab;
    public GameObject BananaTreePrefab;
    public GameObject PearTreePrefab;
    public GameObject CoconutTreePrefab;
    public GameObject PomeloTreePrefab;
    public GameObject GrapefruitTreePrefab;

    [SerializeField] private GameObject SaplingMenu;
    public GameObject SaplingSpawnLocation;
    private SCR_Interact playerInteractScriptRef;
    public bool plotOccupied = false;

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
            case "Apple":
                Plant(AppleTreePrefab);
                break;
            case "Cherry":
                Plant(CherryTreePrefab);
                break;
            case "Orange":
                Plant(OrangeTreePrefab);
                break;
            case "Lyche":
                Plant(LycheTreePrefab);
                break;
            case "Peach":
                Plant(PeachTreePrefab);
                break;
            case "Lemon":
                Plant(LemonTreePrefab);
                break;
            case "Lime":
                Plant(LimeTreePrefab);
                break;
            case "Papaya":
                Plant(PapayaTreePrefab);
                break;
            case "Plum":
                Plant(PlumTreePrefab);
                break;
            case "Olive":
                Plant(OliveTreePrefab);
                break;
            case "Cocoa":
                Plant(CocoaTreePrefab);
                break;
            case "Date":
                Plant(DateTreePrefab);
                break;
            case "Avocado":
                Plant(AvocadoTreePrefab);
                break;
            case "CrabApple":
                Plant(CrabAppleTreePrefab);
                break;
            case "Kumquat":
                Plant(KumquatTreePrefab);
                break;
            case "Banana":
                Plant(BananaTreePrefab);
                break;
            case "Pear":
                Plant(PearTreePrefab);
                break;
            case "Coconut":
                Plant(CoconutTreePrefab);
                break;
            case "Pomelo":
                Plant(PomeloTreePrefab);
                break;
            case "Grapefruit":
                Plant(GrapefruitTreePrefab);
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
        playerInteractScriptRef.SetCursorHighlight(false);
        playerInteractScriptRef.menuOpen = false;
        plotOccupied = true;
        GetComponent<SCR_Highlightable>().stopHighlight = true;
    }
}
