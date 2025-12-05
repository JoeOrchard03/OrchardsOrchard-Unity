using UnityEngine;

public class SCR_PlacedDeco : MonoBehaviour, INT_Interactable
{
    public DecoType decoType;
    public GameObject decoPlacerPrefab;
    private SCR_PlayerManager playerManagerScriptRef;
    public bool flipped = false;
    
    public void Interact(GameObject interactor)
    {
        GetPlayerRef();
        playerManagerScriptRef.hoveredInteractable = null;
        GameObject instantiatedDecoPlacer = Instantiate(decoPlacerPrefab, Vector2.zero, Quaternion.identity);
        instantiatedDecoPlacer.GetComponent<SCR_DecoPlacer>().InitiateDecoEditing(this.gameObject);
        Debug.Log("Editing " + this.gameObject.name);
        
    }
    
    private void GetPlayerRef()
    {
        if (playerManagerScriptRef == null)
        {
            playerManagerScriptRef = FindFirstObjectByType<SCR_PlayerManager>();
        }
    }
}
