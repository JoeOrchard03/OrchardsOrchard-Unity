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
    public List<SCR_BuyableDeco> decoShopSlots = new List<SCR_BuyableDeco>();

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
        if (saplingShopSlots == null || saplingShopSlots.Count == 0)
        {
            Debug.Log("No sapling shop slots");
            return;
        }

        foreach (SCR_BuyableSapling slot in saplingShopSlots)
        {
            if (slot == null) continue;

            var fruit = GetRandomFruitBySpawnChance();
            slot.fruitType = fruit.type;
            slot.fruitDatabase = fruitDatabase;
            slot.ApplyFruitInfo();
        }
        
        SCR_ReworkedSaveSystem.SaveSaplingShopInventory(saplingShopSlots, saplingShopTimer);
        Debug.Log($"Saved {saplingShopSlots.Count} shop slots to save data");
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
}
