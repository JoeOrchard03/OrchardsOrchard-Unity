using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SCR_ShopInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SCR_FruitDatabase fruitDatabase;

    [Header("Shop settings")]
    public float shopRefreshTime = 30f;
    private float shopTimer;

    [Header("Shop slots")]
    public List<SCR_BuyableSapling> shopSlots = new List<SCR_BuyableSapling>();

    public GameObject shopRefreshNotif;
    
    public static event Action<float> OnShopTimerUpdated;
    public static event Action OnShopRefreshed;
    
    private void Start()
    {
        SCR_SaveSystem.LoadShopInventory(fruitDatabase, shopSlots, ref shopTimer);

        if (shopSlots.Count == 0 || shopSlots.TrueForAll(s => s.fruitType == FruitType.Null))
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
            SCR_SaveSystem.SaveShopInventory(shopSlots, shopTimer);
        }
    }

    public void RefreshShopInventory()
    {
        if (shopSlots == null || shopSlots.Count == 0) return;

        foreach (SCR_BuyableSapling slot in shopSlots)
        {
            if (slot == null) continue;

            var fruit = GetRandomFruitBySpawnChance();
            slot.fruitType = fruit.type;
            slot.fruitDatabase = fruitDatabase;
            slot.ApplyFruitInfo();
        }
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
}
