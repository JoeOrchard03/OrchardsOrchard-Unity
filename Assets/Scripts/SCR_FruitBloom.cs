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
    public SCR_FruitDatabase fruitDatabase;
    
    [Header("Growth variables")]
    public List<Sprite> spriteGrowthStages;
    public List<float> growthTimes;
    public float uncommonFruitMultiplier;
    public float rareFruitMultiplier;
    

    [Header("Special Variant variables")] 
    public Sprite goldSprite;
    public Sprite iridescentSprite;
    public GameObject goldParticlesPrefab;
    public GameObject iridescentParticlesPrefab;
    private float goldChance;
    private float iridescentChance;
    [HideInInspector] public bool isGold = false;
    [HideInInspector] public bool isIridescent = false;
    
    private AudioSource rareFruitAudioSource;
    public AudioClip goldAppear;
    public AudioClip iridescentAppear;
    
    [Header("Misc variables")]
    [SerializeField] public bool readyToHarvest = false;
    public bool harvested = false;
    public int currentStage = 0;
    private GameObject activeParticles;
    [HideInInspector] public bool isTargeted = false;

    [Header("Save variables")] 
    public int fruitIndex = -1;

    private void Awake()
    {
        goldChance = 0.325f;
        iridescentChance = 0.105f;

        if (goldParticlesPrefab == null)
        {
            goldParticlesPrefab = Resources.Load<GameObject>("Particles/GoldParticles");
        }

        if (iridescentParticlesPrefab == null)
        {
            iridescentParticlesPrefab = Resources.Load<GameObject>("Particles/IridescentParticles");
        }
        
        rareFruitAudioSource = GameObject.Find("RareFruitAudioSource").GetComponent<AudioSource>();
        playerInteractScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_Interact>();
        drone = GameObject.FindGameObjectWithTag("Drone");
        gameObject.GetComponent<SCR_Highlightable>().canHighlight = false;
    }
    
    public void StartGrowthCycle(bool reset = true)
    {
        if (reset)
        {
            ResetFruit();
        }
        if (activeParticles != null)
        {
            Destroy(activeParticles);
            activeParticles = null;
        }
        harvested = false;
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
        if (motherTree == null)
            motherTree = transform.parent?.gameObject;

        if (motherTree == null) return;

        SCR_TreeGrowthCycle tree = motherTree.GetComponent<SCR_TreeGrowthCycle>();
        
        if (tree != null && tree.alternateLeavesSprite != null)
        {
            motherTree.GetComponent<SpriteRenderer>().sprite = tree.alternateLeavesSprite;
        }
    }

    private void SetLeavesToNormal()
    {
        if (motherTree == null)
            motherTree = transform.parent?.gameObject;

        if (motherTree == null) return;

        SCR_TreeGrowthCycle tree = motherTree.GetComponent<SCR_TreeGrowthCycle>();
        
        if (tree != null && tree.alternateLeavesSprite != null)
        {
            motherTree.GetComponent<SpriteRenderer>().sprite = tree.normalLeavesSprite;
        }
    }
    
    IEnumerator GrowFruit()
    {
        spriteRenderer.sprite = spriteGrowthStages[currentStage];
        
        Rarity fruitRarity = fruitDatabase.GetFruit(fruitType).rarity;
        
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
            UpdateSavedFruitStage();
            spriteRenderer.sprite = spriteGrowthStages[currentStage];
        }

        float roll = Random.value;
        if (roll < iridescentChance)
        {
            isIridescent = true;
            rareFruitAudioSource.PlayOneShot(iridescentAppear);
            spriteRenderer.sprite = iridescentSprite;
            activeParticles = Instantiate(iridescentParticlesPrefab, transform);
            activeParticles.transform.localPosition = Vector3.zero;
        }
        else if (roll < iridescentChance + goldChance)
        {
            isGold = true;
            rareFruitAudioSource.PlayOneShot(goldAppear);
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

        if (fruitIndex != -1)
        {
            SCR_SaveData saveData = SCR_SaveSystem.LoadGame();
            int plotNumber = transform.parent.GetComponent<SCR_TreeGrowthCycle>().motherPlot.GetComponent<SCR_Plot>().plotNumber;
            TreeData tree = saveData.trees.Find(t => t.dataPlotNumber == plotNumber);

            if (tree != null && fruitIndex < tree.fruits.Count)
            {
                tree.fruits[fruitIndex].beenHarvested = true;
                SCR_SaveSystem.SaveGame(saveData);
            }
        }
        
        drone.GetComponent<SCR_Drone>().SetTarget(gameObject.GetComponent<SCR_FruitBloom>());
    }

    private void UpdateSavedFruitStage()
    {
        if (fruitIndex == -1) return;
        
        SCR_SaveData saveData = SCR_SaveSystem.LoadGame();

        int plotNumber = transform.parent.GetComponent<SCR_TreeGrowthCycle>().motherPlot.GetComponent<SCR_Plot>().plotNumber;
        TreeData tree = saveData.trees.Find(t => t.dataPlotNumber == plotNumber);

        if (tree != null)
        {
            while (fruitIndex >= tree.fruits.Count)
            {
                tree.fruits.Add(new FruitData());
            }

            tree.fruits[fruitIndex].growthStage = currentStage;
            tree.fruits[fruitIndex].isGold = isGold;
            tree.fruits[fruitIndex].isIridescent = isIridescent;
            tree.fruits[fruitIndex].fruitPos = transform.localPosition;
            SCR_SaveSystem.SaveGame(saveData);
        }
    }
}
