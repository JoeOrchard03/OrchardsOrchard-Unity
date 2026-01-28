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

    [Header("Collider variables")]
    public BoxCollider2D bulkHarvestCollider;
    
    private int currentBatch = 0;
    
    void Start()
    {
        LoadFruits();

        inactiveFruitBloomObjects ??= new List<GameObject>();
        activeBloomObjects ??= new List<GameObject>();
        
        if (playerScriptRef.pickRangeUpgrade)
        {
            bulkHarvestCollider.enabled = true;
        }
        
        if (currentStage == 0)
        {
            spriteRenderer.sprite = spriteGrowthStages[0];
        }

        if (bloomCycleCoroutine != null)
        {
            StopCoroutine(bloomCycleCoroutine);
            bloomCycleCoroutine = null;
        }
        bloomCycleRunning = false;
        
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
        if (bulkHarvestCollider.enabled && playerScriptRef.pickRangeUpgrade && IsTreeFullyGrown())
        {
            Debug.Log("Triggering bulk havest from leaf collider");
            BulkHarvestAllActiveFruit();
            return;
        }
        
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
        inactiveFruitBloomObjects ??= new List<GameObject>();
        activeBloomObjects ??= new List<GameObject>();
        
        if (this == null || gameObject == null)
            return;

        if (!IsTreeFullyGrown())
            return;
        
        if (bloomCycleRunning)
        {
            Debug.LogWarning("Bloom cycle already running, skipping duplicate bloom cycle");
            return;
        }
        
        if (motherPlot == null)
        {
            Debug.LogWarning("StartBloomCycle aborted: motherPlot is null");
            bloomCycleRunning = false;
            return;
        }

        SCR_Plot plot = motherPlot.GetComponent<SCR_Plot>();
        if (plot == null)
        {
            Debug.LogWarning("StartBloomCycle aborted: SCR_Plot missing on motherPlot");
            bloomCycleRunning = false;
            return;
        }
        
        SCR_SaveData saveData = SCR_ReworkedSaveSystem.LoadGame();
        if (saveData == null || saveData.trees == null)
        {
            Debug.LogWarning("StartBloomCycle aborted: saveData or trees list is null");
            bloomCycleRunning = false;
            return;
        }
        
        TreeData tree = saveData.trees.Find(t => t.dataPlotNumber == plot.plotNumber);
        if (tree == null)
        {
            Debug.LogWarning("TreeData not found for plot " + plot.plotNumber);
            bloomCycleRunning = false;
            return;
        }
        
        if (tree.fruits == null)
        {
            tree.fruits = new List<FruitData>();
        }

        bloomCycleRunning = true;

        int prefabFruitCount = GetComponentsInChildren<SCR_FruitBloom>(true).Length;
        if (tree.fruits.Count > prefabFruitCount)
        {
            Debug.LogWarning("Prefab fruits count was " + tree.fruits.Count + ", greater than " + prefabFruitCount + " trimming...");
            tree.fruits.RemoveRange(prefabFruitCount, tree.fruits.Count - prefabFruitCount);
        }

        bool allHarvested = tree.fruits.Count > 0 && tree.fruits.TrueForAll(f => f.beenHarvested);
        if (allHarvested)
        {
            //Debug.Log("All previous fruits have been harvested, resetting fruit list for new bloom cycle...");
            tree.fruits.Clear();

            inactiveFruitBloomObjects.Clear();
            activeBloomObjects.Clear();

            foreach (Transform child in transform)
            {
                if (child.GetComponent<SCR_FruitBloom>() != null)
                    inactiveFruitBloomObjects.Add(child.gameObject);
            }
        }
        
        bool batchActive = tree.fruits.Exists(f => f.batchID == currentBatch && !f.beenHarvested);
        if (batchActive)
        {
            Debug.Log("Batch still active, skipping new bloom");
            bloomCycleRunning = false;
            return;
        }
        
        int numberOfBloomsToActivate =  Random.Range(minNumberOfBloomsToActivate, maxNumberOfBloomsToActivate);
        //Debug.Log("Activating " + numberOfBloomsToActivate + " blooms");
        
        for (int i = 0; i < numberOfBloomsToActivate; i++)
        {
            if (inactiveFruitBloomObjects.Count == 0) break;
            
            int randomIndex = Random.Range(0, inactiveFruitBloomObjects.Count);
            GameObject fruitOBJ = inactiveFruitBloomObjects[randomIndex];
            if (fruitOBJ == null)
            {
                Debug.LogWarning("Fruit object in inactiveFruitBloomObjects is null at index " + randomIndex);
                inactiveFruitBloomObjects.RemoveAt(randomIndex);
                continue;
            }
            
            SCR_FruitBloom fruit = fruitOBJ.GetComponent<SCR_FruitBloom>();
            if (fruit == null)
            {
                Debug.LogWarning("SCR_FruitBloom missing on fruitOBJ: " + fruitOBJ.name);
                continue;
            }
            
            if (tree.fruits.Count >= prefabFruitCount)
            {
                Debug.LogWarning("Max fruit data entries reached, reusing existing slot");
                fruit.fruitIndex = tree.fruits.Count - 1;
            }
            else
            {
                FruitData newFruit = new FruitData { batchID = currentBatch};
                tree.fruits.Add(newFruit);
                fruit.fruitIndex = tree.fruits.Count - 1;
            }
            
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
            SCR_ReworkedSaveSystem.SaveGame(saveData);
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
        
        SCR_SaveData saveData = SCR_ReworkedSaveSystem.LoadGame();
        TreeData tree = saveData.trees.Find(t => t.dataPlotNumber == motherPlot.GetComponent<SCR_Plot>().plotNumber);

        if (tree == null || tree.fruits == null)
        {
            return;
        }


        if (fruitScriptRef.fruitIndex < 0 || fruitScriptRef.fruitIndex >= tree.fruits.Count)
        {
            return;
        }
        
        tree.fruits[fruitScriptRef.fruitIndex].beenHarvested = true;
        SCR_ReworkedSaveSystem.SaveGame(saveData);

        fruitScriptRef.fruitIndex = -1;
        
        if (activeBloomObjects.Contains(fruit))
        {
            activeBloomObjects.Remove(fruit);
            inactiveFruitBloomObjects.Add(fruit);
            fruit.SetActive(false);
        }

        if (activeBloomObjects.Count == 0)
        {
            Debug.Log("active blooms.count is 0");
            if (bloomCycleCoroutine != null)
            {
                StopCoroutine(bloomCycleCoroutine);
            }
            bloomCycleCoroutine = StartCoroutine(RestartBloomCycle());
        }
    }

    private IEnumerator RestartBloomCycle()
    {
        if (!IsTreeFullyGrown())
        {
            Debug.Log("Tree not fully grown");
            yield break;
        }
        yield return new WaitForSeconds(timeToFirstBloom);

        if (this == null || gameObject == null)
        {
            yield break;
        }
        
        StartBloomCycle();
        bloomCycleCoroutine = null;
    }

    private void UpdateSavedGrowthStage()
    {
        SCR_SaveData saveData = SCR_ReworkedSaveSystem.LoadGame();
        
        TreeData tree = saveData.trees.Find(t => t.dataPlotNumber == motherPlot.GetComponent<SCR_Plot>().plotNumber);
        if (tree != null)
        {
            tree.dataGrowthStage = currentStage;
            SCR_ReworkedSaveSystem.SaveGame(saveData);
        }
    }

    private void LoadFruits()
    {
        inactiveFruitBloomObjects ??= new List<GameObject>();
        activeBloomObjects ??= new List<GameObject>();
        
        playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerManager>();
        SCR_SaveData saveData = SCR_ReworkedSaveSystem.LoadGame();
        TreeData tree = saveData.trees.Find(t => t.dataPlotNumber == motherPlot.GetComponent<SCR_Plot>().plotNumber);

        if (tree == null)
        {
            return;
        }

        if (tree.fruits == null)
        {
            tree.fruits = new List<FruitData>();
        }
        
        inactiveFruitBloomObjects.Clear();
        activeBloomObjects.Clear();
        
        int fruitIndex = 0;

        foreach (Transform child in transform)
        {
            SCR_FruitBloom fruit = child.GetComponent<SCR_FruitBloom>();
            if (fruit == null)
                continue;

            if (fruitIndex >= tree.fruits.Count)
                break;

            fruit.fruitIndex = fruitIndex;
            FruitData savedFruit = tree.fruits[fruitIndex];

            fruit.currentStage = savedFruit.growthStage;
            fruit.isGold = savedFruit.isGold;
            fruit.isIridescent = savedFruit.isIridescent;

            if (!savedFruit.beenHarvested)
            {
                child.gameObject.SetActive(true);

                if (fruit.isGold || fruit.isIridescent)
                    fruit.GoldOrIriVisuals(false);
                else
                    fruit.spriteRenderer.sprite = fruit.spriteGrowthStages[fruit.currentStage];

                if (fruit.currentStage < fruit.spriteGrowthStages.Count - 1)
                {
                    fruit.StartGrowthCycle(false);
                }
                else
                {
                    fruit.readyToHarvest = true;
                    fruit.GetComponent<SCR_Highlightable>().canHighlight = true;
                }

                activeBloomObjects.Add(child.gameObject);
            }
            else
            {
                child.gameObject.SetActive(false);
                inactiveFruitBloomObjects.Add(child.gameObject);
            }

            if (savedFruit.batchID >= currentBatch)
                currentBatch = savedFruit.batchID + 1;

            fruitIndex++;
        }
        
        // Any remaining fruit objects that weren't in save data → inactive
        foreach (Transform child in transform)
        {
            if (child.GetComponent<SCR_FruitBloom>() == null)
                continue;

            if (!activeBloomObjects.Contains(child.gameObject) &&
                !inactiveFruitBloomObjects.Contains(child.gameObject))
            {
                child.gameObject.SetActive(false);
                inactiveFruitBloomObjects.Add(child.gameObject);
            }
        }

        if (activeBloomObjects.Count == 0 && IsTreeFullyGrown() && !bloomCycleRunning)
        {
            bloomCycleCoroutine = StartCoroutine(RestartBloomCycle());
        }
    }

    public void EnableBulkCollider()
    {
        bulkHarvestCollider.enabled = true;
    }

    public void SetAllFruitHighlights(bool state)
    {
        foreach(GameObject fruit in activeBloomObjects)
        {
            if (fruit == null)
            {
                continue;
            }

            if (!(fruit.GetComponent<SCR_FruitBloom>().readyToHarvest))
            {
                continue;
            }
            
            SCR_Highlightable highlightable = fruit.gameObject.GetComponent<SCR_Highlightable>();
            if (highlightable != null && fruit.activeInHierarchy)
            {
                highlightable.highlightEffect.SetActive(state);
            }
        }
    }
    
    private void BulkHarvestAllActiveFruit()
    {
        foreach (var fruit in activeBloomObjects)
        {
            if (fruit.GetComponent<SCR_FruitBloom>().readyToHarvest)
            {
                fruit.GetComponent<SCR_Highlightable>().stopHighlight = true;
                fruit.GetComponent<SCR_Highlightable>().highlightEffect.SetActive(false);
            }
        }
        
        foreach (GameObject fruitOBJ in activeBloomObjects)
        {
            if(fruitOBJ == null)
            {
                continue;
            }
            
            SCR_FruitBloom fruitBloomScript = fruitOBJ.GetComponent<SCR_FruitBloom>();
            if (fruitBloomScript != null && fruitBloomScript.readyToHarvest && !fruitBloomScript.harvested)
            {
                fruitBloomScript.Harvest();
            }
        }
    }
    
    private bool IsTreeFullyGrown()
    {
        return currentStage >= spriteGrowthStages.Count - 1;
    }
    
    public bool HasHarvestableFruit()
    {
        foreach (var fruit in activeBloomObjects)
        {
            if (fruit.GetComponent<SCR_FruitBloom>().readyToHarvest)
                return true;
        }
        return false;
    }
}
