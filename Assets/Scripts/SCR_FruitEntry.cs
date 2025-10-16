using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_FruitEntry : MonoBehaviour
{
    public FruitType fruitType;
    public GameObject standardFruit;
    public GameObject goldFruit;
    public GameObject iridescentFruit;

    [HideInInspector]
    public bool standardCollected = false;
    [HideInInspector]
    public bool goldCollected = false;
    [HideInInspector]
    public bool iridescentCollected = false;

    private void Awake()
    {
        RunSetUp();
    }

    public void RunSetUp()
    {
        standardFruit = transform.GetChild(0).gameObject;
        goldFruit = transform.GetChild(1).gameObject;
        iridescentFruit = transform.GetChild(2).gameObject;
    }

    public void RefreshEntries()
    {
        standardFruit.SetActive(standardCollected);
        goldFruit.SetActive(goldCollected);
        iridescentFruit.SetActive(iridescentCollected);
    }

    public void MarkStandardFruit()
    {
        if (!standardCollected)
        {
            standardCollected = true;
        }
    }

    public void MarkGoldFruit()
    {
        if (!goldCollected)
        {
            goldCollected = true;
        }
    }

    public void MarkIridescentFruit()
    {
        if (!iridescentCollected)
        {
            iridescentCollected = true;
        }
    }
}