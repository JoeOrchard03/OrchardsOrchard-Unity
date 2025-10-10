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
    private int currentStage = 0;
    public GameObject motherPlot;
    private SCR_Interact playerScriptRef;
    
    [Header("Bloom variables")]
    public List<GameObject> inactiveFruitBloomObjects;
    public List<GameObject> activeBloomObjects;
    public int minNumberOfBloomsToActivate;
    public int maxNumberOfBloomsToActivate;
    
    // Start is called before the first frame update
    void Start()
    {
        playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_Interact>();
        playerScriptRef.currentTreeCount++;
        playerScriptRef.currentSaplingCount--;
        
        if (spriteGrowthStages.Count > 1 && growthTimes.Count == spriteGrowthStages.Count - 1)
        {
            StartCoroutine(GrowTree());
        }
        else
        {
            Debug.LogWarning("Growth stages or durations not set correctly.");
        }
        
        var fruitBlooms = new Transform[gameObject.transform.childCount];
        for (int i = 0; i < fruitBlooms.Length; i++)
        {
            inactiveFruitBloomObjects.Add(gameObject.transform.GetChild(i).gameObject);
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
            
            if (spriteGrowthStages[currentStage] == spriteGrowthStages[1])
            {
                transform.localPosition = originalPos;
                motherPlot.SetActive(false);
            }
            
            if(spriteGrowthStages[currentStage] == spriteGrowthStages[1]) {motherPlot.SetActive(false);}
            spriteRenderer.sprite = spriteGrowthStages[currentStage];
        }
        yield return new WaitForSeconds(timeToFirstBloom);
        StartBloomCycle();
    }
    
    public void StartBloomCycle()
    {
        int numberOfBloomsToActivate =  Random.Range(minNumberOfBloomsToActivate, maxNumberOfBloomsToActivate);
        Debug.Log("Activating " + numberOfBloomsToActivate + " blooms");
        for (int i = 0; i < numberOfBloomsToActivate; i++)
        {
            int bloomToActivateIndex = Random.Range(0, inactiveFruitBloomObjects.Count);
            inactiveFruitBloomObjects[bloomToActivateIndex].SetActive(true);
            inactiveFruitBloomObjects[bloomToActivateIndex].GetComponent<SCR_FruitBloom>().StartGrowthCycle();
            activeBloomObjects.Add(inactiveFruitBloomObjects[bloomToActivateIndex]);
            inactiveFruitBloomObjects.RemoveAt(bloomToActivateIndex);
        }
    }

    public void OnFruitHarvested(GameObject fruit)
    {
        if (activeBloomObjects.Contains(fruit))
        {
            activeBloomObjects.Remove(fruit);
            inactiveFruitBloomObjects.Add(fruit);
            fruit.SetActive(false);
        }

        if (activeBloomObjects.Count == 0)
        {
            StartCoroutine(RestartBloomCycle());
        }
    }

    private IEnumerator RestartBloomCycle()
    {
        yield return new WaitForSeconds(timeToFirstBloom);
        StartBloomCycle();
    }
}
