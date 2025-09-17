using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SCR_FruitBloom : MonoBehaviour, INT_Interactable
{
    [Header("Object references")]
    private SCR_Interact playerInteractScriptRef;
    private GameObject drone;
    public SpriteRenderer spriteRenderer;
    private GameObject motherTree;
    
    [Header("Fruit info")]
    public FruitType fruitType;
    
    [Header("Growth variables")]
    public List<Sprite> spriteGrowthStages;
    public List<float> growthTimes;

    [Header("Special Variant variables")] 
    public Sprite goldSprite;
    public Sprite iridescentSprite;
    public GameObject goldParticlesPrefab;
    public GameObject iridescentParticlesPrefab;
    [Range(0f, 1f)] public float goldChance = 0.5f;
    [Range(0f, 1f)] public float iridescentChance = 0.25f;
    [HideInInspector] public bool isGold = false;
    [HideInInspector] public bool isIridescent = false;
    
    [Header("Misc variables")]
    [SerializeField] private bool readyToHarvest = false;
    private bool harvested = false;
    private int currentStage = 0;
    private GameObject activeParticles;

    private void Awake()
    {
        goldChance = 0.07f;
        iridescentChance = 0.01f;

        if (goldParticlesPrefab == null)
        {
            goldParticlesPrefab = Resources.Load<GameObject>("Particles/GoldParticles");
        }

        if (iridescentParticlesPrefab == null)
        {
            iridescentParticlesPrefab = Resources.Load<GameObject>("Particles/IridescentParticles");
        }
        
        playerInteractScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_Interact>();
        drone = GameObject.FindGameObjectWithTag("Drone");
        gameObject.GetComponent<SCR_Highlightable>().canHighlight = false;
    }
    
    public void StartGrowthCycle()
    {
        ResetFruit();
        StartCoroutine(GrowFruit());
    }
    
    public void ResetFruit()
    {
        currentStage = 0;
        readyToHarvest = false;
        harvested = false;
        isGold = false;
        isIridescent = false;

        if (activeParticles != null)
        {
            Destroy(activeParticles);
            activeParticles = null;
        }
        
        spriteRenderer.sprite = spriteGrowthStages[currentStage];
        gameObject.GetComponent<SCR_Highlightable>().canHighlight = false;
    }

    private void CheckForBloomLeaves()
    {
        motherTree = transform.parent.gameObject;
        if (motherTree.GetComponent<SCR_TreeGrowthCycle>().alternateLeavesSprite != null)
        {
            motherTree.GetComponent<SpriteRenderer>().sprite = motherTree.GetComponent<SCR_TreeGrowthCycle>().alternateLeavesSprite;
        }
    }

    private void SetLeavesToNormal()
    {
        if (motherTree.GetComponent<SCR_TreeGrowthCycle>().alternateLeavesSprite != null)
        {
            motherTree.GetComponent<SpriteRenderer>().sprite = motherTree.GetComponent<SCR_TreeGrowthCycle>().normalLeavesSprite;
        }
    }
    
    IEnumerator GrowFruit()
    {
        spriteRenderer.sprite = spriteGrowthStages[currentStage];
        
        while (currentStage < spriteGrowthStages.Count - 1)
        {
            if (currentStage == 0)
            {
                CheckForBloomLeaves();
            }

            if (currentStage == 1)
            {
                SetLeavesToNormal();
            }
            
            yield return new WaitForSeconds(growthTimes[currentStage]);
            currentStage++;
            spriteRenderer.sprite = spriteGrowthStages[currentStage];
        }

        float roll = Random.value;
        if (roll < iridescentChance)
        {
            isIridescent = true;
            spriteRenderer.sprite = iridescentSprite;
            activeParticles = Instantiate(iridescentParticlesPrefab, transform);
            activeParticles.transform.localPosition = Vector3.zero;
        }
        else if (roll < iridescentChance + goldChance)
        {
            isGold = true;
            spriteRenderer.sprite = goldSprite;
            activeParticles = Instantiate(goldParticlesPrefab, transform);
            activeParticles.transform.localPosition = Vector3.zero;
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
