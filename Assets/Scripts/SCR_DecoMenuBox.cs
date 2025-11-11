using UnityEngine;
using UnityEngine.UI;

public class SCR_DecoMenuBox : MonoBehaviour
{
    public SCR_DecoDatabase decoDatabase;
    public DecoType decoType;

    private GameObject player;
    public Image decoImage;
    private GameObject decoInventory;
    public SCR_ReworkedSaveSystem saveSystem;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        decoInventory = transform.parent.gameObject;
        saveSystem = GameObject.Find("SaveManager").GetComponent<SCR_ReworkedSaveSystem>();
        LoadImage();
    }

    public void LoadImage()
    {
        decoImage.sprite = decoDatabase.GetDeco(decoType).decoSprite;
    }
    
    public void Place()
    {
        player.GetComponent<SCR_PlayerManager>().hoveredInteractable = null;
        Debug.Log("Entering placement mode");
    }
}
