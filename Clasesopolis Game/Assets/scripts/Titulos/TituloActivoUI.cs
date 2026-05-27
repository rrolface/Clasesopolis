using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Componente para la interfaz principal del modo Construcción.
///
/// Va pegado al "listoncito/trofeo" que ya existe manualmente en la escena.
/// Su responsabilidad:
///   - Mostrar el nombre del título actualmente equipado (ProgresoGlobal.TituloActivo).
///   - Al hacer click sobre el botón asociado, abrir el PanelSeleccionTitulos.
///
/// No toca el sprite del listón — eso ya viene puesto manualmente en la escena.
/// Solo refresca el texto.
/// </summary>
public class TituloActivoUI : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Texto donde se mostrará el nombre del título activo. Si está vacío " +
             "se muestra 'textoSinTitulo'.")]
    public TextMeshProUGUI textoTituloActivo;

    [Tooltip("Texto a mostrar cuando el jugador no tiene títulos desbloqueados " +
             "o no ha equipado ninguno.")]
    public string textoSinTitulo = "Sin título";

    [Tooltip("Botón que abre el panel de selección de títulos. Suele cubrir " +
             "el área del listón/trofeo. Opcional: si no se asigna, no se abre el panel.")]
    public Button botonAbrirPanel;

    [Header("Panel de selección — versión NUEVA con slots fijos (preferida)")]
    [Tooltip("Referencia al PanelTitulosFijo (slots pre-diseñados sin ScrollView). " +
             "Si está asignado, este tiene prioridad sobre 'panelSeleccion'.")]
    public PanelTitulosFijo panelTitulosFijo;

    [Header("Panel de selección — versión LEGACY dinámica (opcional)")]
    [Tooltip("Referencia al PanelSeleccionTitulos antiguo (basado en ScrollView e " +
             "instanciado dinámico). Solo se usa si 'panelTitulosFijo' está vacío. " +
             "Mantener para compatibilidad si todavía no migraste a slots fijos.")]
    public PanelSeleccionTitulos panelSeleccion;

    [Tooltip("Si está marcado, al iniciar la escena el panel queda oculto " +
             "(se muestra solo cuando el jugador hace click).")]
    public bool ocultarPanelAlIniciar = true;

    private bool cableado = false;

    void OnEnable()
    {
        // Cablear UNA sola vez aunque OnEnable se llame varias.
        if (!cableado && botonAbrirPanel != null)
        {
            botonAbrirPanel.onClick.AddListener(AbrirPanel);
            cableado = true;
        }

        // Ocultar al inicio el panel que esté en uso.
        if (ocultarPanelAlIniciar)
        {
            GameObject panelActivo = ObtenerPanelGO();
            if (panelActivo != null) panelActivo.SetActive(false);
        }

        Refrescar();
    }

    /// <summary>
    /// Devuelve el GameObject del panel a usar: prioriza la versión fija (nueva);
    /// si no está asignada, usa el panel dinámico legacy.
    /// </summary>
    private GameObject ObtenerPanelGO()
    {
        if (panelTitulosFijo != null) return panelTitulosFijo.gameObject;
        if (panelSeleccion != null) return panelSeleccion.gameObject;
        return null;
    }

    /// <summary>
    /// Vuelve a leer ProgresoGlobal.TituloActivo y actualiza el texto.
    /// Llamar después de cambiar el título (lo hace PanelSeleccionTitulos
    /// automáticamente cuando el jugador elige uno).
    /// </summary>
    public void Refrescar()
    {
        if (textoTituloActivo == null) return;

        string activo = ProgresoGlobal.TituloActivo;
        textoTituloActivo.text = string.IsNullOrEmpty(activo)
            ? textoSinTitulo
            : activo;
    }

    private void AbrirPanel()
    {
        GameObject panelActivo = ObtenerPanelGO();
        if (panelActivo != null)
        {
            panelActivo.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[TituloActivoUI] No hay ningún panel asignado (ni panelTitulosFijo ni panelSeleccion).");
        }
    }
}
