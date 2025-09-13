using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SCR_ShopInventory : MonoBehaviour
{
    public float shopRefreshTime = 30f; // seconds per refresh
    private float shopTimer;

    public static event Action OnShopRefresh; 
    public static event Action<float> OnShopTimerUpdated;

    private void Start()
    {
        shopTimer = shopRefreshTime;
    }

    private void Update()
    {
        shopTimer -= Time.deltaTime;
        OnShopTimerUpdated?.Invoke(shopTimer);

        if (shopTimer <= 0f)
        {
            OnShopRefresh?.Invoke();
            shopTimer = shopRefreshTime;
        }
    }
}
