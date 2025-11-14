using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_OpenMenu : MonoBehaviour, INT_Interactable
{
    public SCR_ShopInventory shopInventoryRef;
    
    public void Interact(GameObject interactor)
    {
        shopInventoryRef.SaveShopTimerCall();
        SceneManager.LoadScene("MainMenu");
    }
}
