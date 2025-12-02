using Unity.VisualScripting;
using UnityEngine;

public class SCR_HoverName : MonoBehaviour
{
    private GameObject hoverNameOBJ;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!hoverNameOBJ.activeInHierarchy)
        {
            hoverNameOBJ.SetActive(true);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Keep at same depth
        transform.position = mousePos;
    }
}
