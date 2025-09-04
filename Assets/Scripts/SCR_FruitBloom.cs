using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SCR_FruitBloom : MonoBehaviour, INT_Interactable
{
    private SCR_Interact playerInteractScriptRef;
    private GameObject drone;
    
    public SpriteRenderer spriteRenderer;

    private int currentStage = 0;
    
    public List<Sprite> spriteGrowthStages;
    public List<float> growthTimes;
    [SerializeField] private bool readyToHarvest = false;

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
        Debug.Log("Harvesting fruit");
        drone.GetComponent<SCR_Drone>().SetTarget(this.gameObject.transform);
    }
}
