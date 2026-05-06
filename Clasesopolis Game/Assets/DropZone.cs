using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public string tipoCorrecto;
    public TipoDato itemActual;

    public void OnDrop(PointerEventData eventData)
    {
        DragItem item = eventData.pointerDrag.GetComponent<DragItem>();

        if (item != null)
        {
            item.transform.SetParent(transform);
            item.transform.position = transform.position;

            itemActual = item.GetComponent<TipoDato>();
        }
    }
}