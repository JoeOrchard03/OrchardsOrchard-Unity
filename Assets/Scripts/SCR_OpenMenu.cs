using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_OpenMenu : MonoBehaviour, INT_Interactable
{
    public void Interact(GameObject interactor)
    {
        SceneManager.LoadScene("MainMenu");
    }
}
