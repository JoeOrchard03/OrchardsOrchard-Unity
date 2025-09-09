using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_ShopMenu : MonoBehaviour
{
    public GameObject saplingCanvas;
    public GameObject sellCanvas;

    private void Awake()
    {
        saplingCanvas.SetActive(true);
        sellCanvas.SetActive(false);
    }

    public void OpenSaplingsTab()
    {
        if (saplingCanvas.active == true)
        {
            return;
        }
        
        saplingCanvas.SetActive(true);
        sellCanvas.SetActive(false);
    }

    public void OpenSellTab()
    {
        if (sellCanvas.active == true)
        {
            return;
        }

        saplingCanvas.SetActive(false);
        sellCanvas.SetActive(true);
    }
}
