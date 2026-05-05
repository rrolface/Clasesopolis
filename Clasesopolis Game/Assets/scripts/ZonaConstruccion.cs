using UnityEngine;
using UnityEngine.UI;

public class ZonaConstruccion : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelUI;
    public Button botonConstruir;

    [Header("Construcción a colocar")]
    [Tooltip("Prefab de respaldo. Solo se usa si el jugador NO tiene una construcción seleccionada " +
             "del inventario. Útil para escenas de prueba o cuando el inventario está vacío.")]
    public GameObject edificioPrefab;

    [Tooltip("Punto donde se instanciará la construcción.")]
    public Transform puntoConstruccion;

    [Header("Visuales")]
    public GameObject ZonaVisualPiso;
    public GameObject PanelFases;

    private bool jugadorDentro = false;
    private bool yaConstruido = false;

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

            // Bloquear el mouse otra vez
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Construir()
    {
        if (yaConstruido) return;

        // Prioridad: la construcción que el jugador eligió en el inventario.
        // Si no hay, caemos al prefab fijo del Inspector.
        GameObject prefabAUsar = null;

        if (SeleccionConstruccion.Actual != null && SeleccionConstruccion.Actual.prefab != null)
        {
            prefabAUsar = SeleccionConstruccion.Actual.prefab;
        }
        else if (edificioPrefab != null)
        {
            prefabAUsar = edificioPrefab;
        }

        if (prefabAUsar == null)
        {
            Debug.LogWarning("ZonaConstruccion: no hay construcción seleccionada en el inventario " +
                             "ni prefab de respaldo asignado. No se construye nada.");
            return;
        }

        Vector3 pos = (puntoConstruccion != null) ? puntoConstruccion.position : transform.position;
        Quaternion rot = (puntoConstruccion != null) ? puntoConstruccion.rotation : transform.rotation;

        Instantiate(prefabAUsar, pos, rot);
        yaConstruido = true;

        // El inventario NO se reduce: la construcción sigue disponible para usarse en otras zonas.

        if (panelUI != null) panelUI.SetActive(false);
        if (ZonaVisualPiso != null) ZonaVisualPiso.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ActivarPanel()
    {
        if (PanelFases != null) PanelFases.SetActive(true);
    }
}
