using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZonaConstruccion : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelUI;
    public Button botonConstruir;

    [Tooltip("Texto opcional donde se muestra el costo de la construcción seleccionada " +
             "y los mensajes de error (ej. 'Necesitas 30 XP'). Si no se asigna, solo se " +
             "imprime en consola.")]
    public TextMeshProUGUI textoMensaje;

    [Tooltip("Cuántos segundos se mantiene visible el mensaje de error antes de borrarse.")]
    public float duracionMensajeError = 2.5f;

    [Header("Construcción a colocar")]
    [Tooltip("Prefab de respaldo. Solo se usa si el jugador NO tiene una construcción seleccionada " +
             "del inventario. Útil para escenas de prueba o cuando el inventario está vacío.")]
    public GameObject edificioPrefab;

    [Tooltip("Costo de XP que se usa cuando se construye con el prefab de respaldo " +
             "(porque no hay Construccion seleccionada del catálogo y por tanto no hay 'costoXP'). " +
             "Si la construcción viene del inventario, manda el costoXP de Construccion.cs.")]
    public int costoXPFallback = 30;

    [Tooltip("Punto donde se instanciará la construcción.")]
    public Transform puntoConstruccion;

    [Header("Visuales")]
    public GameObject ZonaVisualPiso;
    public GameObject PanelFases;


    private bool jugadorDentro = false;
    private bool yaConstruido = false;

    // Coroutine handle para no acumular mensajes que se pisan.
    private Coroutine rutinaMensaje;

    void Start()
    {
        if (panelUI != null) panelUI.SetActive(false);
        if (botonConstruir != null) botonConstruir.onClick.AddListener(Construir);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaConstruido)
        {
            jugadorDentro = true;
            if (panelUI != null) panelUI.SetActive(true);

            // Refrescar el mensaje con el costo de la construcción actual.
            ActualizarMensajeCosto();

            // Desbloquear el mouse para interactuar con la UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            if (panelUI != null) panelUI.SetActive(false);

            // Cancelar cualquier mensaje pendiente (errores temporales con coroutine)
            // y limpiar el texto, así no queda colgado en pantalla al salir de la zona.
            if (rutinaMensaje != null)
            {
                StopCoroutine(rutinaMensaje);
                rutinaMensaje = null;
            }
            if (textoMensaje != null)
            {
                textoMensaje.text = "";
            }

            // Bloquear el mouse otra vez
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Construir()
    {
        if (yaConstruido) return;

        // ----- Resolver qué prefab usar y qué costo aplica -----
        // Prioridad: la construcción que el jugador eligió en el inventario.
        // Si no hay, caemos al prefab fijo del Inspector con el costoXPFallback.
        GameObject prefabAUsar = null;
        int costo = 0;
        string nombreConstruccion = "";

        if (SeleccionConstruccion.Actual != null && SeleccionConstruccion.Actual.prefab != null)
        {
            prefabAUsar = SeleccionConstruccion.Actual.prefab;
            costo = Mathf.Max(0, SeleccionConstruccion.Actual.costoXP);
            nombreConstruccion = SeleccionConstruccion.Actual.nombre;
        }
        else if (edificioPrefab != null)
        {
            prefabAUsar = edificioPrefab;
            costo = Mathf.Max(0, costoXPFallback);
            nombreConstruccion = edificioPrefab.name;
        }

        if (prefabAUsar == null)
        {
            Debug.LogWarning("ZonaConstruccion: no hay construcción seleccionada en el inventario " +
                             "ni prefab de respaldo asignado. No se construye nada.");
            MostrarMensajeError("No tienes ninguna construcción seleccionada.");
            return;
        }

        // ----- Validar XP -----
        if (!ProgresoGlobal.TieneXP(costo))
        {
            int faltan = costo - ProgresoGlobal.XP;
            Debug.Log($"[ZonaConstruccion] XP insuficiente. Tienes {ProgresoGlobal.XP}, " +
                      $"necesitas {costo} (faltan {faltan}).");
            MostrarMensajeError($"Necesitas {costo} XP para construir esto. Te faltan {faltan} XP.");
            return;
        }

        // ----- Restar XP -----
        ProgresoGlobal.RestarXP(costo);

        // ----- Colocar la construcción -----
        Vector3 pos = (puntoConstruccion != null) ? puntoConstruccion.position : transform.position;
        Quaternion rot = (puntoConstruccion != null) ? puntoConstruccion.rotation : transform.rotation;

        Instantiate(prefabAUsar, pos, rot);
        yaConstruido = true;

        // El inventario NO se reduce: la construcción sigue disponible para usarse en otras zonas.

        // ----- Registrar progreso de ciudad (slider) -----
        CiudadProgreso.RegistrarConstruccion();

        Debug.Log($"[ZonaConstruccion] Construido '{nombreConstruccion}' por {costo} XP. " +
                  $"XP restante: {ProgresoGlobal.XP}");

        if (panelUI != null) panelUI.SetActive(false);
        if (ZonaVisualPiso != null) ZonaVisualPiso.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ActivarPanel()
    {
        if (PanelFases != null) PanelFases.SetActive(true);
    }

    // ============================================================
    //  Mensajes en pantalla
    // ============================================================

    /// <summary>
    /// Refresca el texto del panel con el costo de la construcción actual
    /// y el XP disponible. Solo se ejecuta si textoMensaje está asignado.
    /// </summary>
    private void ActualizarMensajeCosto()
    {
        if (textoMensaje == null) return;

        int costo;
        string nombre;
        if (SeleccionConstruccion.Actual != null)
        {
            costo = Mathf.Max(0, SeleccionConstruccion.Actual.costoXP);
            nombre = SeleccionConstruccion.Actual.nombre;
        }
        else
        {
            costo = costoXPFallback;
            nombre = edificioPrefab != null ? edificioPrefab.name : "—";
        }

        textoMensaje.text = $"{nombre}: cuesta {costo} XP   (tienes {ProgresoGlobal.XP})";
    }

    private void MostrarMensajeError(string msg)
    {
        if (textoMensaje == null)
        {
            // Sin UI, al menos lo deja en consola.
            Debug.LogWarning($"[ZonaConstruccion] {msg}");
            return;
        }

        if (rutinaMensaje != null) StopCoroutine(rutinaMensaje);
        rutinaMensaje = StartCoroutine(RutinaMensajeError(msg));
    }

    private IEnumerator RutinaMensajeError(string msg)
    {
        textoMensaje.text = msg;
        yield return new WaitForSeconds(duracionMensajeError);

        // Solo restablece si el jugador sigue dentro de la zona; si ya salió,
        // el panel está oculto y no hay nada que actualizar.
        if (jugadorDentro)
        {
            ActualizarMensajeCosto();
        }
    }
}
