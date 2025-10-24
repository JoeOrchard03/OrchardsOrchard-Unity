using UnityEngine;

public class SCR_OpenDecoMenu : MonoBehaviour, INT_Interactable
{
    private GameObject player;
    public GameObject decoMenu;
    public bool decoMenuOpen = false;
    private AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    public void Interact(GameObject interactor)
    {
        Debug.Log("Interacting with deco HB");
        if (!decoMenuOpen)
        {
            //audioSource.Play();
            OpenMenu();
        }
        else
        {
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        Debug.Log("Opening deco menu");
        player.GetComponent<SCR_PlayerManager>().decoMenuOpen = true;
        decoMenuOpen = true;
        decoMenu.SetActive(true);
    }
    
    public void CloseMenu()
    {
        Debug.Log("Closing deco menu");
        player.GetComponent<SCR_PlayerManager>().shopMenuOpen = false;
        decoMenuOpen = false;
        decoMenu.SetActive(false);
    }
}
