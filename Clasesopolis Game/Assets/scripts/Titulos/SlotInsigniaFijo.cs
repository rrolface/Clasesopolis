using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Un espacio FIJO del panel de títulos. Va pegado a cada slot que ya está
/// posicionado manualmente en la jerarquía del panel.
///
/// Cada slot representa UNA insignia concreta — el nombre se asigna en el Inspector
/// y debe coincidir EXACTAMENTE con el string usado en ProgresoGlobal.GanarInsignia(...).
///
/// El slot tiene DOS estados visuales que el diseñador arma en el editor:
///   - Bloqueado:   silueta en negro/gris, sin nombre (o "???").
///                  Se muestra cuando el jugador NO tiene esa insignia.
///   - Desbloqueado: listón a color + nombre del título.
///                   Se muestra cuando el jugador SÍ tiene esa insignia.
///
/// El script SOLO alterna la visibilidad de esos dos GameObjects.
/// No instancia nada, no toca posiciones. Por eso no hay problemas de
/// superposición o layout — cada slot vive en su lugar pre-diseñado.
/// </summary>
public class SlotInsigniaFijo : MonoBehaviour
{
    [Header("Datos del slot")]
    [Tooltip("Nombre EXACTO de la insignia que representa este slot. " +
             "Debe coincidir con el string usado en ProgresoGlobal.GanarInsignia(\"...\"). " +
             "Ej: 'Maestro de Clasificación'.")]
    public string nombreInsignia;

    [Header("Visuales - estado BLOQUEADO")]
    [Tooltip("GameObject que se muestra cuando el jugador NO tiene la insignia. " +
             "Típicamente la silueta en negro/gris del listón.")]
    public GameObject visualBloqueado;

    [Tooltip("Texto opcional para el slot bloqueado, ej. '???' o 'Aún no desbloqueado'.")]
    public TextMeshProUGUI textoBloqueado;

    [Header("Visuales - estado DESBLOQUEADO")]
    [Tooltip("GameObject que se muestra cuando el jugador SÍ tiene la insignia. " +
             "Listón a color.")]
    public GameObject visualDesbloqueado;

    [Tooltip("Texto donde se mostrará el nombre del título. Se rellena automáticamente " +
             "con 'nombreInsignia' cuando se desbloquea.")]
    public TextMeshProUGUI textoNombre;

    [Header("Selección")]
    [Tooltip("Botón que el jugador presiona para elegir este título. Solo se vuelve " +
             "interactuable cuando la insignia está desbloqueada.")]
    public Button botonSeleccionar;

    [Tooltip("GameObject opcional (borde, glow, marco) que se activa cuando este título " +
             "es el actualmente equipado por el jugador.")]
    public GameObject marcoSeleccion;

    // ---- internos ----
    private PanelTitulosFijo panelDueño;
    private bool cableado = false;

    /// <summary>
    /// Llamado por PanelTitulosFijo en su Awake/OnEnable para que el slot sepa a
    /// quién avisar cuando lo presionen.
    /// </summary>
    public void Inicializar(PanelTitulosFijo panel)
    {
        panelDueño = panel;

        if (!cableado && botonSeleccionar != null)
        {
            botonSeleccionar.onClick.AddListener(OnClick);
            cableado = true;
        }
    }

    /// <summary>
    /// Lee ProgresoGlobal y actualiza el estado visual del slot (bloqueado/desbloqueado
    /// y marco de selección). Llamarlo siempre que cambien las insignias o el TituloActivo.
    /// </summary>
    public void Refrescar()
    {
        bool desbloqueada = !string.IsNullOrEmpty(nombreInsignia)
                            && ProgresoGlobal.Insignias.Contains(nombreInsignia);

        if (visualBloqueado != null) visualBloqueado.SetActive(!desbloqueada);
        if (visualDesbloqueado != null) visualDesbloqueado.SetActive(desbloqueada);

        if (textoNombre != null)
        {
            textoNombre.text = desbloqueada ? nombreInsignia : "";
        }

        if (botonSeleccionar != null)
        {
            botonSeleccionar.interactable = desbloqueada;
        }

        if (marcoSeleccion != null)
        {
            bool esActivo = desbloqueada
                            && nombreInsignia == ProgresoGlobal.TituloActivo;
            marcoSeleccion.SetActive(esActivo);
        }
    }

    private void OnClick()
    {
        if (string.IsNullOrEmpty(nombreInsignia))
        {
            Debug.LogWarning($"[SlotInsigniaFijo] El slot '{gameObject.name}' no tiene nombreInsignia asignado.");
            return;
        }

        if (panelDueño != null)
        {
            panelDueño.SeleccionarTitulo(nombreInsignia);
        }
        else
        {
            // Fallback: si no hay panel dueño, intentar aplicar igual.
            ProgresoGlobal.EstablecerTituloActivo(nombreInsignia);
        }
    }
}
