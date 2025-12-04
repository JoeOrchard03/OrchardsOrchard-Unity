using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SCR_ShopInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SCR_FruitDatabase fruitDatabase;
    [SerializeField] private SCR_DecoDatabase decoDatabase;

    [Header("Shop settings")]
    public float saplingShopRefreshTime = 30f;
    public float decoShopRefreshTime = 30f;
    private float saplingShopTimer;
    private float decoShopTimer;

    [Header("Shop slots")]
    public List<SCR_BuyableSapling> saplingShopSlots = new List<SCR_BuyableSapling>();
    public List<SCR_BuyableSapling> commonSaplingShopSlots = new List<SCR_BuyableSapling>();
    public List<SCR_BuyableSapling> uncommonSaplingShopSlots = new List<SCR_BuyableSapling>();
    public List<SCR_BuyableSapling> rareSaplingShopSlots = new List<SCR_BuyableSapling>();
    public List<SCR_BuyableDeco> decoShopSlots = new List<SCR_BuyableDeco>();

    [Header("Rarity fill chances")]
    [Range(0f, 1f)] public float uncommonSlotChance = 0.5f;
    [Range(0f, 1f)] public float rareSlotChance = 0.5f;
    
    public GameObject shopRefreshNotif;
    
    public static event Action<float> OnSaplingShopTimerUpdated;
    public static event Action<float> OnDecoShopTimerUpdated;
    public static event Action OnShopRefreshed;
    
    private void Start()
    {
        SCR_ReworkedSaveSystem.LoadSaplingShopInventory(fruitDatabase, saplingShopSlots, ref saplingShopTimer);
        SCR_ReworkedSaveSystem.LoadDecoShopInventory(decoDatabase, decoShopSlots, ref decoShopTimer);

        if (saplingShopSlots.Count == 0 || saplingShopSlots.TrueForAll(s => s.fruitType == FruitType.Null))
        {
            saplingShopTimer = SCR_ReworkedSaveSystem.LoadSaplingShopTimer();
            decoShopTimer = SCR_ReworkedSaveSystem.LoadDecoShopTimer();
            RefreshSaplingShopInventory();
            RefreshDecoShopInventory();
        }
    }

    private void Update()
    {
        saplingShopTimer -= Time.deltaTime;
        decoShopTimer -= Time.deltaTime;
        OnSaplingShopTimerUpdated?.Invoke(saplingShopTimer);
        OnDecoShopTimerUpdated?.Invoke(decoShopTimer);
        
        if (saplingShopTimer <= 0f)
        {
            shopRefreshNotif.SetActive(true);
            RefreshSaplingShopInventory();
            OnShopRefreshed?.Invoke();
            saplingShopTimer = saplingShopRefreshTime;
            SCR_ReworkedSaveSystem.SaveSaplingShopTimer(saplingShopTimer);
            SCR_ReworkedSaveSystem.SaveSaplingShopInventory(saplingShopSlots, saplingShopTimer);
        }
        
        if (decoShopTimer <= 0f)
        {
            shopRefreshNotif.SetActive(true);
            RefreshDecoShopInventory();
            OnShopRefreshed?.Invoke();
            decoShopTimer = decoShopRefreshTime;
            SCR_ReworkedSaveSystem.SaveDecoShopTimer(decoShopTimer);
            SCR_ReworkedSaveSystem.SaveDecoShopInventory(decoShopSlots, decoShopTimer);
        }
    }

    private void OnApplicationQuit()
    {
        SaveShopTimerCall();
    }

    public void SaveShopTimerCall()
    {
        SCR_ReworkedSaveSystem.SaveSaplingShopTimer(saplingShopTimer);
        SCR_ReworkedSaveSystem.SaveDecoShopTimer(decoShopTimer);
    }
    
    public void RefreshSaplingShopInventory()
    {
        Debug.Log("Calling RefreshSaplingShopInventory");
        
        List<SCR_FruitDatabase.Fruit> common = new List<SCR_FruitDatabase.Fruit>();
        List<SCR_FruitDatabase.Fruit> uncommon = new List<SCR_FruitDatabase.Fruit>();
        List<SCR_FruitDatabase.Fruit> rare = new List<SCR_FruitDatabase.Fruit>();
        
        foreach(var f in fruitDatabase.fruits)
        {
            switch(f.rarity)
            {
                case Rarity.Common:
                    common.Add(f);
                    break;
                case Rarity.Uncommon:
                    uncommon.Add(f);
                    break;
                case Rarity.Rare:
                    rare.Add(f);
                    break;
            }
        }
        
        FillRaritySlots(commonSaplingShopSlots, common, alwaysFill: true);
        FillRaritySlots(uncommonSaplingShopSlots, uncommon, alwaysFill: false);
        FillRaritySlots(rareSaplingShopSlots, rare, alwaysFill: false);
        
        SCR_ReworkedSaveSystem.SaveSaplingShopInventory(saplingShopSlots, saplingShopTimer);
    }

    private void FillRaritySlots(List<SCR_BuyableSapling> slots, List<SCR_FruitDatabase.Fruit> fruitPool,
        bool alwaysFill)
    {
        if (slots.Count != 5)
        {
            Debug.LogWarning("Each rarity slot should have 5 slots, less then 5 detected");
        }

        //If every slot needs to be filled (common)
        if (alwaysFill)
        {
            //For each slot
            for (int i = 0; i < slots.Count; i++)
            {
                //Get fruit
                var fruit = GetWeightedFruit(fruitPool);
                
                //Apply fruit info
                slots[i].fruitType = fruit.type != null ? fruit.type : FruitType.Null;
                slots[i].fruitDatabase = fruitDatabase;
                slots[i].ApplyFruitInfo();
            }

            return;
        }

        //Track amount of filled slots
        int filledSlotsCount = 0;

        //Roll to see if each slot will be filled
        for (int i = 0; i < slots.Count; i++)
        {
            float roll = Random.Range(0f, 1f);

            if (roll > 0.40f)
            {
                filledSlotsCount++;
            }
        }
        
        //For each slot
        for(int i = 0; i < slots.Count; i++)
        {
            //If slot should be filled
            if (i < filledSlotsCount)
            {
                //Get Fruit
                var fruit = GetWeightedFruit(fruitPool);
                
                slots[i].fruitType = fruit != null ? fruit.type : FruitType.Null;
            }
            else
            {
                //If slot should not be filled set it to be of type Null
                slots[i].fruitType = FruitType.Null;
            }
            
            //Apply fruit info
            slots[i].fruitDatabase = fruitDatabase;
            slots[i].ApplyFruitInfo();
        }
    }

    private SCR_FruitDatabase.Fruit GetWeightedFruit(List<SCR_FruitDatabase.Fruit> fruits)
    {
        if (fruits == null || fruits.Count == 0)
        {
            return null;
        }

        float totalWeight = 0f;

        foreach (var f in fruits)
        {
            totalWeight += Mathf.Max(f.shopSpawnChance, 0.00001f);
        }
        
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var f in fruits)
        {
            cumulativeWeight += Mathf.Max(f.shopSpawnChance, 0.00001f);
            if (randomValue < cumulativeWeight)
            {
                return f;
            }
        }
        
        return fruits[Random.Range(0, fruits.Count)];
    }
    
    public void RefreshDecoShopInventory()
    {
        Debug.Log("Calling RefreshDecoShopInventory");
        
        if (decoShopSlots == null || decoShopSlots.Count == 0)
        {
            Debug.Log("No deco shop slots");
            return;
        }
        
        foreach (SCR_BuyableDeco slot in decoShopSlots)
        {
            if (slot == null) continue;

            var deco = GetRandomDecoBySpawnChance();
            slot.decoType = deco.type;
            slot.decoDatabase = decoDatabase;
            slot.ApplyDecoInfo();
        }
        
        SCR_ReworkedSaveSystem.SaveDecoShopInventory(decoShopSlots, decoShopTimer);
        Debug.Log($"Saved {decoShopSlots.Count} shop slots to save data");
    }
    
    private SCR_FruitDatabase.Fruit GetRandomFruitBySpawnChance()
    {
        var fruits = fruitDatabase.fruits;
        if (fruits == null || fruits.Length == 0) return null;

        float totalWeight = 0f;
        foreach (var fruit in fruits)
            totalWeight += Mathf.Max(fruit.shopSpawnChance, 0.0001f);

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var fruit in fruits)
        {
            cumulative += Mathf.Max(fruit.shopSpawnChance, 0.0001f);
            if (randomValue <= cumulative)
                return fruit;
        }

        return fruits[Random.Range(0, fruits.Length)];
    }
    
    private SCR_DecoDatabase.Deco GetRandomDecoBySpawnChance()
    {
        var decos = decoDatabase.decos;
        if (decos == null || decos.Length == 0) return null;

        float totalWeight = 0f;
        foreach (var deco in decos)
            totalWeight += Mathf.Max(deco.shopSpawnChance, 0.0001f);

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var deco in decos)
        {
            cumulative += Mathf.Max(deco.shopSpawnChance, 0.0001f);
            if (randomValue <= cumulative)
                return deco;
        }

        return decos[Random.Range(0, decos.Length)];
    }
    
    // public void RefreshSaplingShopInventory()
    // {
    //     Debug.Log("Calling RefreshSaplingShopInventory");
    //     if (saplingShopSlots == null || saplingShopSlots.Count == 0)
    //     {
    //         Debug.Log("No sapling shop slots");
    //         return;
    //     }
    //
    //     foreach (SCR_BuyableSapling slot in saplingShopSlots)
    //     {
    //         if (slot == null) continue;
    //
    //         var fruit = GetRandomFruitBySpawnChance();
    //         slot.fruitType = fruit.type;
    //         slot.fruitDatabase = fruitDatabase;
    //         slot.ApplyFruitInfo();
    //     }
    //     
    //     SCR_ReworkedSaveSystem.SaveSaplingShopInventory(saplingShopSlots, saplingShopTimer);
    //     Debug.Log($"Saved {saplingShopSlots.Count} shop slots to save data");
    // }
}
