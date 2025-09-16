using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FruitType
{
    Apple,
    Cherry,
    Orange,
    Peach,
    Lemon,
    Lime,
    Lyche,
    Mullbery,
    Plum,
    Papaya,
    Olive,
    Cocoa,
    Date,
    Avocado,
    CrabApple,
    Kumquat,
    Banana,
    Pear,
    Coconut,
    Pomelo,
    Grapefruit
}

[CreateAssetMenu(menuName = "Fruit Database")]
public class SCR_FruitDatabase : ScriptableObject
{
    [System.Serializable]
    public class Fruit
    {
        public FruitType type;
        public Sprite saplingSprite;
        public Sprite fruitSprite;
        public float sellValue;
        public float saplingPrice;
        public float shopSpawnChance;
    }

    public Fruit[] fruits;

    // Helper function to get value by type
    public float GetValue(FruitType type)
    {
        foreach (var fruit in fruits)
        {
            if (fruit.type == type)
                return fruit.sellValue;
        }
        Debug.LogWarning($"No value defined for fruit type {type}");
        return 0f;
    }

    public Fruit GetFruit(FruitType type)
    {
        foreach (var fruit in fruits)
        {
            if (fruit.type == type)
                return fruit;
        }
        Debug.LogWarning($"No fruit defined for fruit type {type}");
        return null;
    }
    
    public float GetSaplingPrice(FruitType type)
    {
        foreach (var fruit in fruits)
        {
            if (fruit.type == type)
                return fruit.saplingPrice;
        }
        Debug.LogWarning($"No sapling price defined for fruit type {type}");
        return 0f;
    }
}
