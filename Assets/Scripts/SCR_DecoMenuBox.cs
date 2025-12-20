using UnityEngine;
using UnityEngine.UI;

public class SCR_DecoMenuBox : MonoBehaviour
{
    [Header("Deco info")]
    public SCR_DecoDatabase decoDatabase;
    public DecoType decoType;
    public Image decoImage;
    private Sprite decoSprite;

    [Header("References")]
    private GameObject player;
    private GameObject decoInventory;
    public SCR_ReworkedSaveSystem saveSystem;
    public GameObject decoPlacerPrefab;
    public SCR_OpenDecoMenu openDecoMenuScriptRef;
    
    private void Awake()
    {
        openDecoMenuScriptRef = FindFirstObjectByType<SCR_OpenDecoMenu>();
        player = GameObject.FindGameObjectWithTag("Player");
        decoInventory = transform.parent.gameObject;
        saveSystem = GameObject.Find("SaveManager").GetComponent<SCR_ReworkedSaveSystem>();
    }

    private void Start()
    {
        LoadImage();
    }

    public void LoadImage()
    {
        decoImage.sprite = decoDatabase.GetDeco(decoType).decoSprite;
        Debug.Log("Sprite loaded is: " + decoImage.sprite.name);
        decoSprite = decoImage.sprite;
    }
    
    public void Place()
    {
        player.GetComponent<SCR_PlayerManager>().hoveredInteractable = null;
        openDecoMenuScriptRef.CloseMenu();
        GameObject instantiatedDecoPlacer = Instantiate(decoPlacerPrefab, Vector2.zero, Quaternion.identity);
        instantiatedDecoPlacer.GetComponent<SCR_DecoPlacer>().InitiateDecoPlacer(decoType, decoSprite, this.gameObject);
        Debug.Log("Entering placement mode");
    }
}
