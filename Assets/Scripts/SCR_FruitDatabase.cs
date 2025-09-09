using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fruit Database")]
public class SCR_FruitDatabase : ScriptableObject
{
    [System.Serializable]
    public class Fruit
    {
        public FruitType type;  // Uses your existing enum
        public float value;
    }

    public Fruit[] fruits;

    // Helper function to get value by type
    public float GetValue(FruitType type)
    {
        foreach (var fruit in fruits)
        {
            if (fruit.type == type)
                return fruit.value;
        }
        Debug.LogWarning($"No value defined for fruit type {type}");
        return 0f;
    }
}
