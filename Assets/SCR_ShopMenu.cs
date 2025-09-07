using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_ShopMenu : MonoBehaviour
{
    public Canvas saplingCanvas;
    public Canvas decorCanvas;
    public Canvas sellCanvas;

    private void Awake()
    {
        saplingCanvas.enabled = true;
        decorCanvas.enabled = false;
        sellCanvas.enabled = false;
    }
}
