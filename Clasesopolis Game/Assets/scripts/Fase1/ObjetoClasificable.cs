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
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        posicionInicial = rectTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; // Permite que el mouse "vea" la zona debajo
        categoriaActual = null;
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

        // Si al soltar no cayó en ninguna zona, vuelve a casa
        if (categoriaActual == null)
        {
            rectTransform.position = posicionInicial;
        }
    }

    public void ResetearPosicion()
    {
        rectTransform.position = posicionInicial;
        categoriaActual = null;
    }
}