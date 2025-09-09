using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SCR_InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        SCR_InventoryFruit draggedFruit = dropped.GetComponent<SCR_InventoryFruit>();
        draggedFruit.returnParent = transform;
    }
}
