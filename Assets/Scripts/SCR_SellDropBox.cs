using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SCR_SellDropBox : MonoBehaviour, IDropHandler
{
    public FruitType? storedFruitType;
    public int storedAmount = 0;

    public void OnDrop(PointerEventData eventData)
    {
        SCR_InventoryFruit fruit = eventData.pointerDrag.GetComponent<SCR_InventoryFruit>();
        if (fruit != null)
        {
            storedFruitType = fruit.fruitType;
            
            fruit.transform.SetParent(transform);
            fruit.transform.localPosition = Vector3.zero;
        }
    }
}
