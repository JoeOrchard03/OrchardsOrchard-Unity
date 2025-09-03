using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_TreeGrowthCycle : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public List<Sprite> spriteGrowthStages;
    public List<float> growthTimes;

    private int currentStage = 0;
    public GameObject motherPlot;
    
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
    }
}
