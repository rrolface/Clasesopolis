using UnityEngine;
using UnityEngine.EventSystems;

public class ZonaDrop : MonoBehaviour, IDropHandler
{
    public ObjetoClasificable.TipoCategoria tipoDeEstaZona;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            ObjetoClasificable objeto = eventData.pointerDrag.GetComponent<ObjetoClasificable>();
            if (objeto != null)
            {
                // El objeto se queda en el centro de la zona
                eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
                objeto.categoriaActual = tipoDeEstaZona;
            }
        }
    }
}