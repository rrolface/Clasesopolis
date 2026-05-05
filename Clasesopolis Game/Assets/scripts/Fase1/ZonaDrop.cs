using UnityEngine;
using UnityEngine.EventSystems;

public class ZonaDrop : MonoBehaviour, IDropHandler
{
    [Header("Categoría que representa esta zona")]
    public ObjetoClasificable.TipoCategoria tipoDeEstaZona;

    [Header("Anti-superposición")]
    [Tooltip("Distancia (en píxeles del canvas) que se aplica al objeto en cada intento " +
             "de empuje cuando se está superponiendo con otro.")]
    public float pasoNudge = 8f;

    [Tooltip("Cantidad máxima de intentos antes de rendirse y dejar el objeto en su sitio.")]
    public int maxIntentosNudge = 40;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        ObjetoClasificable objeto = eventData.pointerDrag.GetComponent<ObjetoClasificable>();
        if (objeto == null) return;

        RectTransform rtObjeto = eventData.pointerDrag.GetComponent<RectTransform>();
        RectTransform rtZona = GetComponent<RectTransform>();

        // 1) El objeto pasa a ser hijo de la zona conservando su posición de mundo.
        //    Esto evita que vuelva al origen, y al usar SetAsLastSibling queda renderizado
        //    encima de la zona y por lo tanto vuelve a recibir clicks/drags.
        rtObjeto.SetParent(rtZona, true);
        rtObjeto.SetAsLastSibling();

        // 2) Marcamos a qué categoría pertenece ahora (lo lee GestorClasificacion).
        objeto.categoriaActual = tipoDeEstaZona;

        // 3) Si quedó pisando a otro objeto de la misma zona, lo empujamos.
        EvitarSuperposicion(rtObjeto);

        // 4) Por seguridad, lo mantenemos dentro del rectángulo de la zona.
        ClampDentroDeZona(rtObjeto, rtZona);
    }

    // --- Empuje progresivo cuando dos objetos se solapan en la misma zona ---
    private void EvitarSuperposicion(RectTransform nuevo)
    {
        ObjetoClasificable[] hermanos = GetComponentsInChildren<ObjetoClasificable>();

        for (int intento = 0; intento < maxIntentosNudge; intento++)
        {
            bool huboColision = false;
            Rect rectNuevo = ObtenerRectMundo(nuevo);

            foreach (var h in hermanos)
            {
                if (h == null) continue;
                if (h.transform == nuevo) continue;

                RectTransform rh = h.GetComponent<RectTransform>();
                Rect rectH = ObtenerRectMundo(rh);

                if (rectNuevo.Overlaps(rectH))
                {
                    huboColision = true;

                    // Empujamos en la dirección que aleja a 'nuevo' de 'h'
                    Vector2 dir = (Vector2)(nuevo.position - rh.position);
                    if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right; // caso degenerado: misma posición
                    dir.Normalize();

                    nuevo.position += (Vector3)(dir * pasoNudge);
                    break; // recalculamos colisiones en el siguiente intento
                }
            }

            if (!huboColision) break;
        }
    }

    // --- Mantiene al objeto dentro del rectángulo de la zona ---
    private void ClampDentroDeZona(RectTransform objeto, RectTransform zona)
    {
        Vector3[] esquinasZona = new Vector3[4];
        zona.GetWorldCorners(esquinasZona); // 0:BL  1:TL  2:TR  3:BR

        Vector3[] esquinasObjeto = new Vector3[4];
        objeto.GetWorldCorners(esquinasObjeto);

        Vector2 desplazamiento = Vector2.zero;
        if (esquinasObjeto[0].x < esquinasZona[0].x) desplazamiento.x += esquinasZona[0].x - esquinasObjeto[0].x;
        if (esquinasObjeto[2].x > esquinasZona[2].x) desplazamiento.x += esquinasZona[2].x - esquinasObjeto[2].x;
        if (esquinasObjeto[0].y < esquinasZona[0].y) desplazamiento.y += esquinasZona[0].y - esquinasObjeto[0].y;
        if (esquinasObjeto[2].y > esquinasZona[2].y) desplazamiento.y += esquinasZona[2].y - esquinasObjeto[2].y;

        objeto.position += (Vector3)desplazamiento;
    }

    private Rect ObtenerRectMundo(RectTransform rt)
    {
        Vector3[] esquinas = new Vector3[4];
        rt.GetWorldCorners(esquinas);
        return new Rect(esquinas[0].x, esquinas[0].y,
                        esquinas[2].x - esquinas[0].x,
                        esquinas[2].y - esquinas[0].y);
    }
}
