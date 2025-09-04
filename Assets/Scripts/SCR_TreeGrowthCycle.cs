using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_TreeGrowthCycle : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public List<Sprite> spriteGrowthStages;
    public List<float> growthTimes;
    public float timeToFirstBloom;

    private int currentStage = 0;
    public GameObject motherPlot;
    
    [Header("Bloom objects")]
    public List<GameObject> inactiveFruitBloomObjects;
    public List<GameObject> activeBloomObjects;
    
    public int minNumberOfBloomsToActivate;
    public int maxNumberOfBloomsToActivate;
    
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
    
    IEnumerator GrowTree()
    {
        spriteRenderer.sprite = spriteGrowthStages[currentStage];
        
        while (currentStage < spriteGrowthStages.Count - 1)
        {
            yield return new WaitForSeconds(growthTimes[currentStage]);
            currentStage++;
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
}
