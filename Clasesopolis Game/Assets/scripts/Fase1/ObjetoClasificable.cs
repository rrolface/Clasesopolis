using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjetoClasificable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum TipoCategoria { Clase, NoClase }

    [Header("Configuración")]
    public TipoCategoria categoriaCorrecta;

    [HideInInspector] public TipoCategoria? categoriaActual = null; // null si está afuera
    [HideInInspector] public Vector3 posicionInicial;
    [HideInInspector] public Transform parentInicial;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        // Guardamos posición y padre originales para poder volver con Reintentar
        posicionInicial = rectTransform.position;
        parentInicial = rectTransform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; // Permite que el mouse "vea" la zona debajo
        categoriaActual = null;

        // Subimos al canvas raíz para arrastrar libremente entre zonas
        // y para que se renderice por encima de todo durante el drag.
        if (canvas != null)
        {
            rectTransform.SetParent(canvas.transform, true);
            rectTransform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Mueve el objeto con el mouse ajustado a la escala del Canvas
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Si al soltar no cayó en ninguna zona, vuelve a casa (padre y posición originales)
        if (categoriaActual == null)
        {
            if (parentInicial != null) rectTransform.SetParent(parentInicial, true);
            rectTransform.position = posicionInicial;
        }
    }

    public void ResetearPosicion()
    {
        // Restablecemos jerarquía y posición para el botón Reintentar
        if (parentInicial != null) rectTransform.SetParent(parentInicial, true);
        rectTransform.position = posicionInicial;
        categoriaActual = null;
    }
}
