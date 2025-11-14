using System;
using UnityEngine;

public class SCR_DecoPlacer : MonoBehaviour
{
    [Header("Deco information")]
    public SCR_DecoDatabase decoDatabase;
    public DecoType decorationType;
    public Sprite decorationSprite;
    
    [Header("References")]
    private Transform decoInventory;
    public SpriteRenderer spriteRenderer;
    public GameObject placedDecoHolder;

    [Header("Misc")] 
    private GameObject decoInventoryBox;
    public bool editingExistingDecos = false;
    private bool dragging = false;
    private Vector3 offset;
    
    public void InitiateDecoPlacer(DecoType decoType, Sprite decoSprite, GameObject decoInventoryBoxRef)
    {
        if (decoInventory == null)
        {
            decoInventory = decoInventoryBoxRef.transform.parent;
        }

        if (placedDecoHolder == null)
        {
            placedDecoHolder = GameObject.Find("PlacedDecoHolder");
        }
        
        decorationType = decoType;
        decorationSprite = decoSprite;
        decoInventoryBox = decoInventoryBoxRef;
        
        spriteRenderer.sprite = decorationSprite;
    }

    private void Update()
    {
        //If dragging set the transform to be the mouse position
        if (dragging)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        }
    }

    public void PlaceDeco()
    {
        GameObject decoObj = decoDatabase.GetDeco(decorationType).decoPrefab;
        GameObject instantiatedDecoObj = Instantiate(decoObj, transform.position, transform.rotation);
        instantiatedDecoObj.transform.parent = placedDecoHolder.transform;
        instantiatedDecoObj.GetComponent<SCR_PlacedDeco>().decoType = decorationType;
        Debug.Log("Placing deco: " + instantiatedDecoObj.name + " at: " + transform.position);
        
        SCR_SaveData data = SCR_ReworkedSaveSystem.LoadGame();
        data.placedDecoData = SCR_ReworkedSaveSystem.GetPlacedDecoData(placedDecoHolder.transform);
        SCR_ReworkedSaveSystem.SaveGame(data);
        
        RemoveDecoFromInventory();
        Destroy(this.gameObject);
    }

    public void RemoveDecoFromInventory()
    {
        Debug.Log("Removing " + decoInventoryBox.name + " from inventory");
        DestroyImmediate(decoInventoryBox);
        
        SCR_SaveData data = SCR_ReworkedSaveSystem.LoadGame();
        data.decos = SCR_ReworkedSaveSystem.GetInventoryDecoData(decoInventory);
        SCR_ReworkedSaveSystem.SaveGame(data);
    }

    public void CancelDecoPlacement()
    {
        Debug.Log("Deco placement cancelled");
        Destroy(this.gameObject);
    }

    #region Drag

    //Start dragging
    private void OnMouseDown()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragging = true;
    }

    //Stop Dragging
    private void OnMouseUp()
    {
        dragging = false;
    }

    #endregion
}
