using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentOriginal;
    private CanvasGroup canvasGroup;
    private Vector3 posicionOriginal;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        parentOriginal = transform.parent;
        posicionOriginal = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentOriginal = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentOriginal);
        canvasGroup.blocksRaycasts = true;
    } 

    public void ResetItem()
    {
        transform.SetParent(parentOriginal);
        transform.position = posicionOriginal;
        canvasGroup.blocksRaycasts = true;
     
    }
}