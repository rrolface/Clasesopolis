using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DropZoneMultiple : MonoBehaviour, IDropHandler
{
    public List<TipoDato> items = new List<TipoDato>();

    public void OnDrop(PointerEventData eventData)
    {
        DragItem item = eventData.pointerDrag.GetComponent<DragItem>();

        if (item != null)
        {
            item.transform.SetParent(transform);

            TipoDato tipo = item.GetComponent<TipoDato>();
            items.Add(tipo);
        }
    }
}