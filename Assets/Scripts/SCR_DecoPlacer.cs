using System;
using UnityEngine;
using UnityEngine.Serialization;

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
    public GameObject inventoryDecoPrefab;

    [Header("Misc")] 
    private GameObject decoInventoryBox;
    private bool dragging = false;
    private Vector3 offset;

    [Header("Deco Editing")] 
    public GameObject toBackPackBox;
    public bool editingExistingDecos = false;
    public GameObject decoToEdit;
    private Vector2 originalPos;
    
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

    public void PlaceDeco()
    {
        if (editingExistingDecos)
        {
            decoToEdit.transform.position = transform.position;
            
            //Enable the deco again
            decoToEdit.GetComponent<SpriteRenderer>().enabled = true;
            decoToEdit.GetComponent<BoxCollider2D>().enabled = true;
            decoToEdit.SetActive(true);
            
            SCR_SaveData data = SCR_ReworkedSaveSystem.LoadGame();
            data.placedDecoData = SCR_ReworkedSaveSystem.GetPlacedDecoData(placedDecoHolder.transform);
            SCR_ReworkedSaveSystem.SaveGame(data);

            Destroy(this.gameObject);
            return;
        }
        
        GameObject decoObj = decoDatabase.GetDeco(decorationType).decoPrefab;
        GameObject instantiatedDecoObj = Instantiate(decoObj, transform.position, transform.rotation);
        instantiatedDecoObj.transform.parent = placedDecoHolder.transform;
        instantiatedDecoObj.GetComponent<SCR_PlacedDeco>().decoType = decorationType;
        Debug.Log("Placing deco: " + instantiatedDecoObj.name + " at: " + transform.position);
        
        SCR_SaveData data2 = SCR_ReworkedSaveSystem.LoadGame();
        data2.placedDecoData = SCR_ReworkedSaveSystem.GetPlacedDecoData(placedDecoHolder.transform);
        SCR_ReworkedSaveSystem.SaveGame(data2);
        
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
        if (editingExistingDecos)
        {
            decoToEdit.transform.position = originalPos;
            
            //Enable the deco again
            decoToEdit.GetComponent<SpriteRenderer>().enabled = true;
            decoToEdit.GetComponent<BoxCollider2D>().enabled = true;
            decoToEdit.SetActive(true);
            
            Destroy(this.gameObject);
        }
        
        Debug.Log("Deco placement cancelled");
        Destroy(this.gameObject);
    }

    public void InitiateDecoEditing(GameObject placedDeco)
    {
        editingExistingDecos = true;
        toBackPackBox.SetActive(true);
        
        if (placedDecoHolder == null)
        {
            placedDecoHolder = GameObject.Find("PlacedDecoHolder");
        }
        
        if (decoInventory == null)
        {
            decoInventory = GameObject.FindFirstObjectByType<SCR_DecoInventory>(FindObjectsInactive.Include).transform;
        }
        
        decoToEdit = placedDeco;
        originalPos = placedDeco.transform.position;
        
        // Hide the original deco so the player can't click it again
        decoToEdit.GetComponent<SpriteRenderer>().enabled = false;
        decoToEdit.GetComponent<BoxCollider2D>().enabled = false;
        decoToEdit.SetActive(false);
        
        var placedScript = placedDeco.GetComponent<SCR_PlacedDeco>();
        decorationType = placedScript.decoType;
        decorationSprite = decoDatabase.GetDeco(decorationType).decoSprite;
        
        spriteRenderer.sprite = decorationSprite;
        
        transform.position = originalPos;
    }

    public void ReturnEditingDecoToInventory()
    {
        Debug.Log("Returning deco to inventory");

        if (decoInventory == null)
        {
            decoInventory = GameObject.FindFirstObjectByType<SCR_DecoInventory>(FindObjectsInactive.Include).transform;
        }
        
        GameObject newInventoryBox = Instantiate(inventoryDecoPrefab, decoInventory);
        newInventoryBox.GetComponent<SCR_DecoMenuBox>().decoType = decorationType;
        
        // Remove from placed decos before destroying
        if (decoToEdit != null)
        {
            decoToEdit.SetActive(false);
            decoToEdit.transform.parent = null;
            Destroy(decoToEdit);
        }
        
        SCR_SaveData data = SCR_ReworkedSaveSystem.LoadGame();
        data.decos = SCR_ReworkedSaveSystem.GetInventoryDecoData(decoInventory);

        if (placedDecoHolder != null)
        {
            data.placedDecoData = SCR_ReworkedSaveSystem.GetPlacedDecoData(placedDecoHolder.transform);
        }
        else
        {
            data.placedDecoData = new System.Collections.Generic.List<PlacedDecoData>();
        }
        
        SCR_ReworkedSaveSystem.SaveGame(data);

        Destroy(this.gameObject);
    }
    
    #region Drag

    private void Update()
    {
        //If dragging set the transform to be the mouse position
        if (dragging)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        }
    }
    
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
