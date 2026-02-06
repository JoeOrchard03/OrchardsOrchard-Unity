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

    [Header("References")]
    [SerializeField] private GameObject SaplingMenu;
    private SCR_PlayerManager playerPlayerManagerScriptRef;
    public Transform saplingInventory;
    
    [Header("Audio")]
    private AudioSource plotAudioSource;
    public AudioClip plotInteract;
    public AudioClip treeDestroyAudio;
    
    [Header("Plot information")]
    public bool plotOccupied = false;
    public int plotNumber;
    private SCR_TreeGrowthCycle plantedTree;
    
    [Header("Save system variables")]
    public bool HasTreePlanted => plantedTree != null;
    public FruitType currentFruitType => plantedTree != null ? plantedTree.fruitType : FruitType.Null;
    public int currentGrowthStage => plantedTree != null ? plantedTree.currentStage : 0;
        
    
    public GameObject SaplingSpawnLocation;
    
    private void Start()
    {
        plotAudioSource = GetComponent<AudioSource>();
        playerPlayerManagerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerManager>();
    }
    
    public void Interact(GameObject interactor)
    {
        if (plotOccupied) { return;}
        plotAudioSource.PlayOneShot(plotInteract, 0.6f);
        playerPlayerManagerScriptRef.selectedPlot = this.gameObject;
        playerPlayerManagerScriptRef.menuOpen = true;
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
        if(Enum.TryParse(SaplingName, out FruitType fruitType))
        {
            Plant(GetTreePrefab(fruitType), fruitType);
        }
        else
        {
            Debug.Log(SaplingName + " is not a valid sapling name");
        }
    }

    private void Plant(GameObject Sapling, FruitType fruitType)
    {
        GameObject instantiatedSapling = Instantiate(Sapling, SaplingSpawnLocation.transform.position, transform.rotation);
        plantedTree = instantiatedSapling.GetComponent<SCR_TreeGrowthCycle>();
        plantedTree.motherPlot = this.gameObject;
        
        playerPlayerManagerScriptRef.selectedPlot = null;
        SaplingMenu.SetActive(false);
        playerPlayerManagerScriptRef.SetCursorHighlight(false);
        playerPlayerManagerScriptRef.menuOpen = false;
        playerPlayerManagerScriptRef.currentSaplingCount--;
        plotOccupied = true;
        GetComponent<AudioSource>().Play();
        GetComponent<SCR_Highlightable>().stopHighlight = true;

        TreeData newTreeData = new TreeData
        {
            dataFruitType = fruitType,
            dataPlotNumber = plotNumber,
        };

        SCR_SaveData data = SCR_ReworkedSaveSystem.LoadGame();
        data.trees.Add(newTreeData);
        SCR_ReworkedSaveSystem.SaveGame(data);
    }

    public void LoadPlantedTree(FruitType fruitType, int growthStage)
    {
        GameObject prefab = GetTreePrefab(fruitType);
        if (prefab == null)
        {
            Debug.LogWarning("No tree prefab found for fruit type: " + fruitType);
            return;
        }
        
        GameObject instantiatedPrefab = Instantiate(prefab, SaplingSpawnLocation.transform.position, transform.rotation);
        plantedTree = instantiatedPrefab.GetComponent<SCR_TreeGrowthCycle>();
        plantedTree.motherPlot = this.gameObject;
        plantedTree.currentStage = growthStage;
        
        plotOccupied = true;
        GetComponent<SCR_Highlightable>().stopHighlight = true;
    }
    
    public void PlayTreeDestroyAudio()
    {
        if (plantedTree != null)
        {
            plantedTree = null;
            plotOccupied = false;
        }
        SCR_ReworkedSaveSystem.RemoveTreeFromSave(plotNumber);
        plotAudioSource.PlayOneShot(treeDestroyAudio);
    }

    public GameObject GetTreePrefab(FruitType fruitType)
    {
        switch (fruitType)
        {
            case FruitType.Apple: return AppleTreePrefab;
            case FruitType.Cherry: return CherryTreePrefab;
            case FruitType.Orange: return OrangeTreePrefab;
            case FruitType.Lyche: return LycheTreePrefab;
            case FruitType.Peach: return PeachTreePrefab;
            case FruitType.Lemon: return LemonTreePrefab;
            case FruitType.Lime: return LimeTreePrefab;
            case FruitType.Papaya: return PapayaTreePrefab;
            case FruitType.Plum: return PlumTreePrefab;
            case FruitType.Olive: return OliveTreePrefab;
            case FruitType.Cocoa: return CocoaTreePrefab;
            case FruitType.Date: return DateTreePrefab;
            case FruitType.Avocado: return AvocadoTreePrefab;
            case FruitType.CrabApple: return CrabAppleTreePrefab;
            case FruitType.Kumquat: return KumquatTreePrefab;
            case FruitType.Banana: return BananaTreePrefab;
            case FruitType.Pear: return PearTreePrefab;
            case FruitType.Coconut: return CoconutTreePrefab;
            case FruitType.Pomelo: return PomeloTreePrefab;
            case FruitType.Grapefruit: return GrapefruitTreePrefab;
            case FruitType.Mullbery: return MullberryTreePrefab;
            default:
                Debug.LogWarning($"No prefab found for {fruitType}");
                return null;
        }
    }
}
