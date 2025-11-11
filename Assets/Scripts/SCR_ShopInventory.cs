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
    public float shopRefreshTime = 30f;
    private float shopTimer;

    [Header("Shop slots")]
    public List<SCR_BuyableSapling> saplingShopSlots = new List<SCR_BuyableSapling>();
    public List<SCR_BuyableDeco> decoShopSlots = new List<SCR_BuyableDeco>();

    public GameObject shopRefreshNotif;
    
    public static event Action<float> OnShopTimerUpdated;
    public static event Action OnShopRefreshed;
    
    private void Start()
    {
        SCR_ReworkedSaveSystem.LoadSaplingShopInventory(fruitDatabase, saplingShopSlots, ref shopTimer);
        SCR_ReworkedSaveSystem.LoadDecoShopInventory(decoDatabase, decoShopSlots, ref shopTimer);

        if (saplingShopSlots.Count == 0 || saplingShopSlots.TrueForAll(s => s.fruitType == FruitType.Null))
        {
            shopTimer = shopRefreshTime;
            RefreshShopInventory();
        }
    }

    private void Update()
    {
        shopTimer -= Time.deltaTime;
        OnShopTimerUpdated?.Invoke(shopTimer);
        
        if (shopTimer <= 0f)
        {
            shopRefreshNotif.SetActive(true);
            RefreshShopInventory();
            OnShopRefreshed?.Invoke();
            shopTimer = shopRefreshTime;
            SCR_ReworkedSaveSystem.SaveSaplingShopInventory(saplingShopSlots, shopTimer);
            SCR_ReworkedSaveSystem.SaveDecoShopInventory(decoShopSlots, shopTimer);
        }
    }

    public void RefreshShopInventory()
    {
        Debug.Log("Calling RefreshShopInventory");
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
        
        SCR_ReworkedSaveSystem.SaveSaplingShopInventory(saplingShopSlots, shopTimer);
        SCR_ReworkedSaveSystem.SaveDecoShopInventory(decoShopSlots, shopTimer);
        Debug.Log($"Saved {saplingShopSlots.Count} shop slots to save data");
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
