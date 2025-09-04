using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_MenuBox : MonoBehaviour, INT_Interactable
{
    [SerializeField] private string SaplingName;
    [SerializeField] private Sprite SaplingIcon;
    [SerializeField] private SpriteRenderer SaplingRenderer;
    private GameObject selectedPlot;

    private void Start()
    {
        SaplingRenderer.sprite = SaplingIcon;
    }

    public void Interact(GameObject interactor)
    {
        if (interactor.GetComponent<SCR_Interact>().selectedPlot == null) { return; }
        GetComponent<SCR_Highlightable>().highlightEffect.SetActive(false);
        selectedPlot = interactor.GetComponent<SCR_Interact>().selectedPlot;
        selectedPlot.GetComponent<SCR_Plot>().SaplingToPlant(SaplingName);
        interactor.GetComponent<SCR_Interact>().hoveredInteractable = null;
    }
}
