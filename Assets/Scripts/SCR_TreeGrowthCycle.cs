using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_TreeGrowthCycle : MonoBehaviour, INT_Interactable
{
    [Header("Fruit variables")]
    public FruitType fruitType;
    public SCR_FruitDatabase fruitDatabase;
    
    [Header("Tree Sprites")]
    public SpriteRenderer spriteRenderer;
    public Sprite normalLeavesSprite;
    public Sprite alternateLeavesSprite;
    public List<Sprite> spriteGrowthStages;
    
    [Header("Growth times")]
    public List<float> growthTimes;
    public float uncommonFruitMultiplier;
    public float rareFruitMultiplier;
    public float timeToFirstBloom;

    [Header("Misc variables")]
    public int currentStage = 0;
    public GameObject motherPlot;
    private SCR_PlayerManager playerScriptRef;
    private Coroutine bloomCycleCoroutine;
    private bool bloomCycleRunning;
    
    [Header("Bloom variables")]
    public List<GameObject> inactiveFruitBloomObjects;
    public List<GameObject> activeBloomObjects;
    public int minNumberOfBloomsToActivate;
    public int maxNumberOfBloomsToActivate;

    private int currentBatch = 0;
    
    void Start()
    {
        LoadFruits();

        if (bloomCycleCoroutine != null)
        {
            StopCoroutine(bloomCycleCoroutine);
            bloomCycleCoroutine = null;
        }
        bloomCycleRunning = false;
        
        playerScriptRef.currentTreeCount++;
        playerScriptRef.currentSaplingCount--;
        
        bool treeFullyGrown = currentStage >= spriteGrowthStages.Count - 1;
        
        if (!treeFullyGrown)
        {
            if (spriteGrowthStages.Count > 1 && growthTimes.Count == spriteGrowthStages.Count - 1)
            {
                StartCoroutine(GrowTree());
            }
            else
            {
                Debug.LogWarning("Growth stages or durations not set correctly");
            }
        }
        else if(IsTreeFullyGrown() && activeBloomObjects.Count == 0)
        {
            bloomCycleCoroutine = StartCoroutine(RestartBloomCycle());
        }
        
        if (motherPlot != null && currentStage >= 1)
        {
            motherPlot.SetActive(false);
        }
    }
    
    public void Interact(GameObject interactor)
    {
        if (playerScriptRef.composting && (playerScriptRef.currentTreeCount > 1 || playerScriptRef.currentSaplingCount >= 1))
        {
            Debug.Log("Taking down tree");
            motherPlot.SetActive(true);
            motherPlot.GetComponent<SCR_Plot>().PlayTreeDestroyAudio();
            motherPlot.GetComponent<SCR_Highlightable>().stopHighlight = false;
            motherPlot.GetComponent<SCR_Plot>().plotOccupied  = false;
            playerScriptRef.currentTreeCount--;
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("Cannot take down tree");
        }
    }

    
    IEnumerator GrowTree()
    {
        spriteRenderer.sprite = spriteGrowthStages[currentStage];
        Vector3 originalPos = transform.localPosition;
        transform.localPosition = originalPos + new Vector3(0f, 0.3f, 0f);

        Rarity fruitRarity = fruitDatabase.GetFruit(fruitType).rarity;
        
        while (currentStage < spriteGrowthStages.Count - 1)
        {
            float waitTime = growthTimes[currentStage];

            if (fruitRarity == Rarity.Uncommon)
            {
                waitTime *= uncommonFruitMultiplier;
            }
            else if (fruitRarity == Rarity.Rare)
            {
                waitTime *= rareFruitMultiplier;
            }
            
            yield return new WaitForSeconds(waitTime);
            currentStage++;
            UpdateSavedGrowthStage();
            
            if (spriteGrowthStages[currentStage] == spriteGrowthStages[1])
            {
                transform.localPosition = originalPos;
                if (motherPlot.activeInHierarchy)
                {
                    motherPlot.SetActive(false);
                }
            }
            
            spriteRenderer.sprite = spriteGrowthStages[currentStage];
        }
        
        yield return new WaitForSeconds(timeToFirstBloom);
        StartBloomCycle();
    }
    
    public void StartBloomCycle()
    {
        if (bloomCycleRunning)
        {
            Debug.LogWarning("Bloom cycle already running, skipping duplicate bloom cycle");
            return;
        }
        
        bloomCycleRunning = true;
        
        SCR_SaveData saveData = SCR_SaveSystem.LoadGame();
        TreeData tree = saveData.trees.Find(t => t.dataPlotNumber == motherPlot.GetComponent<SCR_Plot>().plotNumber);
        
        if (tree == null)
        {
            Debug.LogWarning("TreeData not found for plot " + motherPlot.GetComponent<SCR_Plot>().plotNumber);
            return;
        }

        bool batchActive = tree.fruits.Exists(f => f.batchID == currentBatch && !f.beenHarvested);
        if (batchActive) return;
        
        int numberOfBloomsToActivate =  Random.Range(minNumberOfBloomsToActivate, maxNumberOfBloomsToActivate);
        Debug.Log("Activating " + numberOfBloomsToActivate + " blooms");
        
        for (int i = 0; i < numberOfBloomsToActivate; i++)
        {
            if (inactiveFruitBloomObjects.Count == 0) break;
            
            int randomIndex = Random.Range(0, inactiveFruitBloomObjects.Count);
            GameObject fruitOBJ = inactiveFruitBloomObjects[randomIndex];
            SCR_FruitBloom fruit = fruitOBJ.GetComponent<SCR_FruitBloom>();

            FruitData newFruit = new FruitData { batchID = currentBatch, fruitPos = fruitOBJ.transform.localPosition};
            tree.fruits.Add(newFruit);
            fruit.fruitIndex = tree.fruits.Count - 1;
            
            fruitOBJ.SetActive(true);
            fruit.currentStage = 0;
            fruit.readyToHarvest = false;
            fruit.harvested = false;
            fruit.StartGrowthCycle(false);
            
            activeBloomObjects.Add(fruitOBJ);
            inactiveFruitBloomObjects.RemoveAt(randomIndex);
        }

        if (numberOfBloomsToActivate > 0)
        {
            currentBatch++;
            if (currentBatch > 1000) currentBatch = 0;
            SCR_SaveSystem.SaveGame(saveData);
        }

        bloomCycleRunning = false;
    }

    public void OnFruitHarvested(GameObject fruit)
    {
        SCR_FruitBloom fruitScriptRef = fruit.GetComponent<SCR_FruitBloom>();

        if (fruitScriptRef.fruitIndex == -1)
        {
            Debug.LogWarning("Harvested fruit has no fruit Index assigned!");
            return;
        }
        
        SCR_SaveData saveData = SCR_SaveSystem.LoadGame();
        TreeData tree = saveData.trees.Find(t => t.dataPlotNumber == motherPlot.GetComponent<SCR_Plot>().plotNumber);
        if (tree != null && fruitScriptRef.fruitIndex < tree.fruits.Count)
        {
            tree.fruits[fruitScriptRef.fruitIndex].beenHarvested = true;
            SCR_SaveSystem.SaveGame(saveData);
        }

        fruitScriptRef.fruitIndex = -1;
        
        if (activeBloomObjects.Contains(fruit))
        {
            activeBloomObjects.Remove(fruit);
            inactiveFruitBloomObjects.Add(fruit);
            fruit.SetActive(false);
        }

        if (activeBloomObjects.Count == 0)
        {
            if (bloomCycleCoroutine != null)
            {
                StopCoroutine(bloomCycleCoroutine);
            }
            bloomCycleCoroutine = StartCoroutine(RestartBloomCycle());
        }
    }

    private IEnumerator RestartBloomCycle()
    {
        if (bloomCycleCoroutine != null)
        {
            StopCoroutine(bloomCycleCoroutine);
            bloomCycleCoroutine = null;
        }
        if (!IsTreeFullyGrown()) yield break;
        yield return new WaitForSeconds(timeToFirstBloom);
        StartBloomCycle();
        bloomCycleCoroutine = null;
        bloomCycleRunning = false;
    }

    private void UpdateSavedGrowthStage()
    {
        SCR_SaveData saveData = SCR_SaveSystem.LoadGame();
        
        TreeData tree = saveData.trees.Find(t => t.dataPlotNumber == motherPlot.GetComponent<SCR_Plot>().plotNumber);
        if (tree != null)
        {
            tree.dataGrowthStage = currentStage;
            SCR_SaveSystem.SaveGame(saveData);
        }
        else
        {
            Debug.LogWarning("TreeData not found for plot " + motherPlot.GetComponent<SCR_Plot>().plotNumber);
        }
    }

    private void LoadFruits()
    {
        playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerManager>();
        SCR_SaveData saveData = SCR_SaveSystem.LoadGame();
        TreeData tree = saveData.trees.Find(t => t.dataPlotNumber == motherPlot.GetComponent<SCR_Plot>().plotNumber);

        if (tree == null) return;

        inactiveFruitBloomObjects.Clear();
        activeBloomObjects.Clear();
        
        for (int i = 0; i < tree.fruits.Count; i++)
        {
            if (i >= gameObject.transform.childCount) break;
            
            GameObject fruitOBJ = gameObject.transform.GetChild(i).gameObject;
            SCR_FruitBloom fruit = fruitOBJ.GetComponent<SCR_FruitBloom>();

            fruit.fruitIndex = i;
            FruitData savedFruit = tree.fruits[i];
            fruitOBJ.transform.localPosition = savedFruit.fruitPos;
            fruit.currentStage = savedFruit.growthStage;
            fruit.isGold = savedFruit.isGold;
            fruit.isIridescent = savedFruit.isIridescent;

            if (!savedFruit.beenHarvested)
            {
                fruitOBJ.SetActive(true);

                if (fruit.isGold || fruit.isIridescent)
                {
                    fruit.GoldOrIriVisuals(false);
                }
                else
                {
                    fruit.spriteRenderer.sprite = fruit.spriteGrowthStages[fruit.currentStage];
                }
                
                if (fruit.currentStage < fruit.spriteGrowthStages.Count - 1)
                {
                    fruit.StartGrowthCycle(false);
                }
                else
                {
                    fruit.readyToHarvest = true;
                    fruit.gameObject.GetComponent<SCR_Highlightable>().canHighlight = true;
                    
                    if (fruit.isGold || fruit.isIridescent)
                    {
                        Debug.Log("Fruit detected as either gold or iri, running coroutine");
                        fruit.GoldOrIriVisuals(false);
                    }
                    else
                    {
                        fruit.spriteRenderer.sprite = fruit.spriteGrowthStages[fruit.currentStage];
                    }
                }

                activeBloomObjects.Add(fruitOBJ);
            }
            else
            {
                fruitOBJ.SetActive(false);
                inactiveFruitBloomObjects.Add(fruitOBJ);
            }

            if (savedFruit.batchID >= currentBatch)
            {
                currentBatch = savedFruit.batchID + 1;
            }
        }
        
        for (int i = tree.fruits.Count; i < gameObject.transform.childCount; i++)
        {
            inactiveFruitBloomObjects.Add(gameObject.transform.GetChild(i).gameObject);
        }

        if (activeBloomObjects.Count == 0 && IsTreeFullyGrown() && !bloomCycleRunning)
        {
            bloomCycleCoroutine = StartCoroutine(RestartBloomCycle());
        }
    }

    private bool IsTreeFullyGrown()
    {
        return currentStage >= spriteGrowthStages.Count - 1;
    }
}
