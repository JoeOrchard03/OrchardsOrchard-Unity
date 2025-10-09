using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SCR_InventorySlot : MonoBehaviour, IDropHandler
{
    private SCR_ShopMenu shopMenuScriptRef;
    public bool shopSellBox = false;
    public SCR_InventoryFruit fruitInBox;
    public AudioSource shopMenuAudioSource;
    public AudioClip placeAudio;

    private void Awake()
    {
        shopMenuScriptRef = GameObject.FindGameObjectWithTag("ShopMenu").GetComponent<SCR_ShopMenu>();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        shopMenuAudioSource.PlayOneShot(placeAudio);
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        SCR_InventoryFruit draggedFruit = dropped.GetComponent<SCR_InventoryFruit>();
        if (draggedFruit == null) return;

        // Store the original parent before we move the fruit
        Transform originalParent = draggedFruit.returnParent;

        // Assign fruit to this slot
        fruitInBox = draggedFruit;
        draggedFruit.returnParent = transform;
        draggedFruit.transform.SetParent(transform, true);

        // Update shop totals
        shopMenuScriptRef.UpdateTotal();

        // If this is a sell box, destroy the original inventory box
        if (shopSellBox && originalParent != null)
        {
            Destroy(originalParent.gameObject);
        }
    }
}
