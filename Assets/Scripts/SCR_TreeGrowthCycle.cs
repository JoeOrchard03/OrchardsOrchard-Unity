using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SCR_TreeGrowthCycle : MonoBehaviour, INT_Interactable
{
    public SpriteRenderer spriteRenderer;
    private SCR_Interact playerScriptRef;

    public Sprite normalLeavesSprite;
    public Sprite alternateLeavesSprite;
    
    public List<Sprite> spriteGrowthStages;
    public List<float> growthTimes;
    public float timeToFirstBloom;
    public float timeBetweenBloomWaves = 5f;

    private int currentStage = 0;
    public GameObject motherPlot;
    
    [Header("Bloom objects")]
    public List<GameObject> inactiveFruitBloomObjects;
    public List<GameObject> activeBloomObjects;
    
    public int minNumberOfBloomsToActivate;
    public int maxNumberOfBloomsToActivate;

    private void Awake()
    {
        playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_Interact>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
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
        if (playerScriptRef.composting)
        {
            Debug.Log("Taking down tree");
            motherPlot.SetActive(true);
            motherPlot.GetComponent<SCR_Highlightable>().stopHighlight = false;
            motherPlot.GetComponent<SCR_Plot>().plotOccupied  = false;
            Destroy(this.gameObject);
        }
    }

    IEnumerator GrowTree()
    {
        spriteRenderer.sprite = spriteGrowthStages[currentStage];
        
        Vector3 originalPos = transform.localPosition;
        transform.localPosition = originalPos + new Vector3(0f, 0.3f, 0f);
        
        while (currentStage < spriteGrowthStages.Count - 1)
        {
            yield return new WaitForSeconds(growthTimes[currentStage]);
            currentStage++;

            if (spriteGrowthStages[currentStage] == spriteGrowthStages[1])
            {
                transform.localPosition = originalPos;
                motherPlot.SetActive(false);
            }
            spriteRenderer.sprite = spriteGrowthStages[currentStage];
        }
        yield return new WaitForSeconds(timeToFirstBloom);
        StartBloomCycle();
    }

    public void StartBloomCycle()
    {
        StartCoroutine(BloomCycleCoroutine());
    }

    private IEnumerator BloomCycleCoroutine()
    {
        while (true)
        {
            if (inactiveFruitBloomObjects.Count > 0)
            {
                if (alternateLeavesSprite != null)
                {
                    spriteRenderer.sprite = alternateLeavesSprite;
                }
                
                int numberToActivate = Random.Range(minNumberOfBloomsToActivate, maxNumberOfBloomsToActivate);

                for (int i = 0; i < numberToActivate; i++)
                {
                    if (inactiveFruitBloomObjects.Count == 0) break;
                    
                    int index = Random.Range(0, inactiveFruitBloomObjects.Count);
                    GameObject fruitBloom = inactiveFruitBloomObjects[index];

                    if (!fruitBloom.activeSelf)
                    {
                        fruitBloom.SetActive(true);
                        fruitBloom.GetComponent<SCR_FruitBloom>().ResetFruit();
                        fruitBloom.GetComponent<SCR_FruitBloom>().StartGrowthCycle();
                    }
                    
                    activeBloomObjects.Add(fruitBloom);
                    inactiveFruitBloomObjects.RemoveAt(index);
                }
            }
            
            yield return new WaitForSeconds(timeBetweenBloomWaves);
            
            if (alternateLeavesSprite != null && spriteRenderer.sprite == alternateLeavesSprite)
            {
                spriteRenderer.sprite = normalLeavesSprite;
            }
        }
    }
}
