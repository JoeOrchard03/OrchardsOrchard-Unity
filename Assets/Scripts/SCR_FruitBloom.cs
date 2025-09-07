using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FruitType {Apple, Cherry}

public class SCR_FruitBloom : MonoBehaviour, INT_Interactable
{
    private SCR_Interact playerInteractScriptRef;
    private GameObject drone;
    
    public SpriteRenderer spriteRenderer;

    private int currentStage = 0;

    public FruitType fruitType;
    
    public List<Sprite> spriteGrowthStages;
    public List<float> growthTimes;
    [SerializeField] private bool readyToHarvest = false;
    private bool harvested = false;

    private void Awake()
    {
        playerInteractScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_Interact>();
        drone = GameObject.FindGameObjectWithTag("Drone");
        gameObject.GetComponent<SCR_Highlightable>().canHighlight = false;
    }
    
    public void StartGrowthCycle()
    {
        StartCoroutine(GrowFruit());
    }
    
    IEnumerator GrowFruit()
    {
        spriteRenderer.sprite = spriteGrowthStages[currentStage];
        
        while (currentStage < spriteGrowthStages.Count - 1)
        {
            yield return new WaitForSeconds(growthTimes[currentStage]);
            currentStage++;
            spriteRenderer.sprite = spriteGrowthStages[currentStage];
        }
        gameObject.GetComponent<SCR_Highlightable>().canHighlight = true;
        readyToHarvest = true;
        playerInteractScriptRef.matureFruits.Add(this.gameObject);
    }

    public void Interact(GameObject interactor)
    {
        if (!readyToHarvest) { Debug.Log("Fruit not ready to harvest"); return;}
        if (harvested) { Debug.Log("Fruit already harvested"); return;}
        harvested = true;
        
        drone.GetComponent<SCR_Drone>().SetTarget(gameObject.GetComponent<SCR_FruitBloom>());
    }
}
