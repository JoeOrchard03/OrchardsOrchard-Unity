using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Compendium : MonoBehaviour
{
    public static SCR_Compendium instance;
    private SCR_FruitEntry[] fruitEntries;

    private int currentPage;

    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject page4;
    public GameObject page5;
    public GameObject page6;
    
    private Dictionary<FruitType, SCR_FruitEntry> entries = new Dictionary<FruitType, SCR_FruitEntry>();

    private void Awake()
    {
        if(instance == null) instance = this;

        fruitEntries = GetComponentsInChildren<SCR_FruitEntry>(true);
        foreach (var fruitEntry in fruitEntries)
        {
            fruitEntry.RunSetUp();
            if (!entries.ContainsKey(fruitEntry.fruitType))
            {
                entries.Add(fruitEntry.fruitType, fruitEntry);
            }
        }
    }

    public void OpenCompendium()
    {
        foreach (var fruitEntry in fruitEntries)
        {
            fruitEntry.RefreshEntries();
        }
        
        currentPage = 1;
        page1.SetActive(true);
        page2.SetActive(false);
        page3.SetActive(false);
        page4.SetActive(false);
        page5.SetActive(false);
        page6.SetActive(false);
    }

    public void NextPage()
    {
        switch (currentPage)
        {
            case 1:
                page2.SetActive(true);
                currentPage = 2;
                page1.SetActive(false);
                break;
            case 2:
                page3.SetActive(true);
                currentPage = 3;
                page2.SetActive(false);
                break;
            case 3:
                page4.SetActive(true);
                currentPage = 4;
                page3.SetActive(false);
                break;
            case 4:
                page5.SetActive(true);
                currentPage = 5;
                page4.SetActive(false);
                break;
            case 5:
                page6.SetActive(true);
                currentPage = 6;
                page5.SetActive(false);
                break;
        }
        Debug.Log("Next Page");
    }

    public void PreviousPage()
    {
        switch (currentPage)
        {
            case 2:
                page1.SetActive(true);
                currentPage = 1;
                page2.SetActive(false);
                break;
            case 3:
                page2.SetActive(true);
                currentPage = 2;
                page3.SetActive(false);
                break;
            case 4:
                page3.SetActive(true);
                currentPage = 3;
                page4.SetActive(false);
                break;
            case 5:
                page4.SetActive(true);
                currentPage = 4;
                page5.SetActive(false);
                break;
            case 6:
                page5.SetActive(true);
                currentPage = 5;
                page6.SetActive(false);
                break;
        }
        Debug.Log("Previous Page");
    }
    
    public void MarkFruit(FruitType fruitType, bool isGold, bool isIridescent)
    {
        if (entries.TryGetValue(fruitType, out SCR_FruitEntry entry))
        {
            Debug.Log("Trying to mark fruit");
            if(isIridescent) entry.MarkIridescentFruit();
            else if(isGold) entry.MarkGoldFruit();
            else entry.MarkStandardFruit();
        }
    }
}
