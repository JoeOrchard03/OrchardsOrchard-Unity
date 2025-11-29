using UnityEngine;

public class SCR_TutorialScreen : MonoBehaviour
{
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject page4;
    public GameObject page5;
    public GameObject page6;
    public GameObject page7;
    public GameObject page8;

    private int currentPage = 1;

    private void Start()
    {
        currentPage = 1;
    }
    
    public void NextPage()
    {
        switch (currentPage)
        {
            case 1:
                page2.SetActive(true);
                currentPage = 2;
                page1.SetActive(false);
                break;
            case 2:
                page3.SetActive(true);
                currentPage = 3;
                page2.SetActive(false);
                break;
            case 3:
                page4.SetActive(true);
                currentPage = 4;
                page3.SetActive(false);
                break;
            case 4:
                page5.SetActive(true);
                currentPage = 5;
                page4.SetActive(false);
                break;
            case 5:
                page6.SetActive(true);
                currentPage = 6;
                page5.SetActive(false);
                break;
            case 6:
                page7.SetActive(true);
                currentPage = 7;
                page6.SetActive(false);
                break;
            case 7:
                page8.SetActive(true);
                currentPage = 8;
                page7.SetActive(false);
                break;
        }
        Debug.Log("Next Page");
    }

    public void PreviousPage()
    {
        switch (currentPage)
        {
            case 2:
                page1.SetActive(true);
                currentPage = 1;
                page2.SetActive(false);
                break;
            case 3:
                page2.SetActive(true);
                currentPage = 2;
                page3.SetActive(false);
                break;
            case 4:
                page3.SetActive(true);
                currentPage = 3;
                page4.SetActive(false);
                break;
            case 5:
                page4.SetActive(true);
                currentPage = 4;
                page5.SetActive(false);
                break;
            case 6:
                page5.SetActive(true);
                currentPage = 5;
                page6.SetActive(false);
                break;
            case 7:
                page6.SetActive(true);
                currentPage = 6;
                page7.SetActive(false);
                break;
            case 8:
                page7.SetActive(true);
                currentPage = 7;
                page8.SetActive(false);
                break;
        }
        Debug.Log("Previous Page");
    }
}
