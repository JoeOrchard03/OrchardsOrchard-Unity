using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SCR_InventorySlot : MonoBehaviour, IDropHandler
{
    private SCR_ShopMenu shopMenuScriptRef;
    public bool shopSellBox = false;
    public SCR_InventoryFruit fruitInBox;

    private void Awake()
    {
        shopMenuScriptRef = GameObject.FindGameObjectWithTag("ShopMenu").GetComponent<SCR_ShopMenu>();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        fruitInBox = eventData.pointerDrag.GetComponent<SCR_InventoryFruit>();

        if (fruitInBox != null)
        {
            fruitInBox.returnParent = transform;
            fruitInBox.transform.SetParent(transform,true);

            shopMenuScriptRef.UpdateTotal();
        }
        
        GameObject dropped = eventData.pointerDrag;
        SCR_InventoryFruit draggedFruit = dropped.GetComponent<SCR_InventoryFruit>();
        draggedFruit.returnParent = transform;
    }
}
