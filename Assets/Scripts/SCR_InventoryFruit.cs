using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class SCR_InventoryFruit : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SCR_FruitDatabase fruitDatabase; 
    public FruitType fruitType;
    public UnityEngine.UI.Image fruitImage;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector2 cursorOffset;
    
    
    [HideInInspector] public Transform returnParent;
    private int returnSiblingIndex;

    void Awake()
    {
        fruitImage.sprite = fruitDatabase.GetFruit(fruitType).fruitSprite;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        returnParent = rectTransform.parent;
        returnSiblingIndex = rectTransform.GetSiblingIndex();

        // Move under the canvas (or a dedicated drag layer inside it) so it stays visible
        rectTransform.SetParent(canvas.rootCanvas.transform, true);
        rectTransform.SetAsLastSibling();

        // Let drop targets behind this receive raycasts during drag
        canvasGroup.blocksRaycasts = false;
        
        Vector2 size = rectTransform.rect.size;
        Vector2 pivot = rectTransform.pivot;
        cursorOffset = new Vector2(size.x / 2f, size.y / 2f);
    }


    
    public void OnDrag(PointerEventData eventData)
    {
        // Convert screen → canvas local space (works in Screen Space - Camera)
        var cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.rootCanvas.transform as RectTransform,
                eventData.position,
                cam,
                out Vector2 localPoint))
        {
            rectTransform.localPosition = localPoint - cursorOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.SetParent(returnParent, true);
        rectTransform.SetSiblingIndex(returnSiblingIndex);
        canvasGroup.blocksRaycasts = true;
    }
}