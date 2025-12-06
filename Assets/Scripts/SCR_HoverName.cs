using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_HoverName : MonoBehaviour
{
    public Camera cam;
    public LayerMask hoverMask;
    public GameObject hoverNameOBJ;
    public TextMeshProUGUI hoverNameText;

    public Vector2 offsetVector;

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        hoverNameOBJ.SetActive(false);
    }

    private void Update()
    {
        //Make a ray from the mouse position
        RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1f, hoverMask);

        if (hit)
        {
            //If raycast hits a hover object and its not already active
            if (!hoverNameOBJ.activeSelf)
            {
                //Make hover tag active
                hoverNameOBJ.SetActive(true);
            }
            
            hoverNameOBJ.transform.position = (hit.point + offsetVector);

            if(hoverNameText.text == "")
            {
                hoverNameText.text = ApplyText(hit.transform.gameObject);
            }
        }
        //If not hitting anything with raycast
        else
        {
            //If hover tag is active disable it
            if (hoverNameOBJ.activeSelf)
            {
                hoverNameOBJ.SetActive(false);
                hoverNameText.text = "";
            }
        }
    }

    private string ApplyText(GameObject hoveredOBJ)
    {
        //If object is a buyable deco
        if (hoveredOBJ.GetComponent<SCR_BuyableDeco>() != null)
        {
            string returnText;
            //Set return text to be deco's name
            string decoName = hoveredOBJ.GetComponent<SCR_BuyableDeco>().decoName;
            returnText = decoName;
            return returnText;
        }
        //If object is a buyable sapling
        else if(hoveredOBJ.GetComponent<SCR_BuyableSapling>() != null)
        {
            string returnText;
            var type = hoveredOBJ.GetComponent<SCR_BuyableSapling>().fruitType;
            //Set return text to be fruit type with sapling appended to the end
            returnText = type.ToString() + " sapling";
            return returnText;
        }
        else
        {
            //Debug.Log("No component found returning null");
            return "";
        }
    }
}
