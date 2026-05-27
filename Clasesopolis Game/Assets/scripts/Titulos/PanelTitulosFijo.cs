using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Versión del panel de títulos con SLOTS FIJOS (sin ScrollView ni instanciado dinámico).
///
/// Diseño:
///   - El panel tiene N slots pre-posicionados manualmente en la escena (ej. 6).
///   - Cada slot tiene un componente SlotInsigniaFijo con su nombreInsignia asignado.
///   - El panel solo recorre la lista de slots y le pide a cada uno que se refresque.
///   - No genera nada en runtime — toda la distribución visual está bajo control del diseñador.
///
/// Ventaja vs. el panel dinámico:
///   - No hay superposiciones porque las posiciones están fijas en el editor.
///   - Soporte fácil de "insignia bloqueada" (silueta) sin lógica especial.
///   - Más limpio visualmente.
///
/// Desventaja:
///   - Si añades una insignia nueva al juego, tienes que añadir un slot a mano.
///   - Cada nombre en el slot debe coincidir EXACTAMENTE con el string usado al ganar la insignia.
/// </summary>
public class PanelTitulosFijo : MonoBehaviour
{
    [Header("Slots fijos")]
    [Tooltip("Arrastra aquí cada uno de los slots del panel (los GameObjects con SlotInsigniaFijo). " +
             "El orden no importa porque cada slot ya sabe qué insignia representa.")]
    public List<SlotInsigniaFijo> slots = new List<SlotInsigniaFijo>();

    [Header("Cierre")]
    [Tooltip("Botón opcional para cerrar el panel manualmente.")]
    public Button botonCerrar;

    [Tooltip("Si está marcado, el panel se cierra automáticamente cuando el jugador " +
             "selecciona un título.")]
    public bool cerrarAlSeleccionar = true;

    [Header("Referencia al display principal")]
    [Tooltip("Componente que muestra el título activo en el listón principal. " +
             "Se refresca automáticamente cuando se selecciona un título.")]
    public TituloActivoUI tituloActivoUI;

    private bool cableado = false;

    void Awake()
    {
        // Inicializar slots aquí (no en OnEnable) para que el wiring sobreviva
        // aunque el panel se abra y cierre varias veces — el listener se añade una sola vez.
        foreach (var s in slots)
        {
            if (s != null) s.Inicializar(this);
        }
    }

    void OnEnable()
    {
        if (!cableado && botonCerrar != null)
        {
            botonCerrar.onClick.AddListener(Cerrar);
            cableado = true;
        }
        Refrescar();
    }

    /// <summary>
    /// Pide a cada slot que vuelva a leer ProgresoGlobal y actualice su visual.
    /// Llamar después de cambios en insignias o título activo.
    /// </summary>
    public void Refrescar()
    {
        foreach (var s in slots)
        {
            if (s != null) s.Refrescar();
        }
    }

    /// <summary>
    /// Llamado por un SlotInsigniaFijo cuando el jugador lo presiona.
    /// </summary>
    public void SeleccionarTitulo(string nombreInsignia)
    {
        bool ok = ProgresoGlobal.EstablecerTituloActivo(nombreInsignia);
        if (!ok) return;

        if (tituloActivoUI != null) tituloActivoUI.Refrescar();
        Refrescar(); // actualizar marcos de selección

        if (cerrarAlSeleccionar) Cerrar();
    }

    public void Cerrar()
    {
        gameObject.SetActive(false);
    }

    // ============================================================
    //  CONTEXT MENU (debug)
    // ============================================================

    [ContextMenu("Refrescar slots ahora")]
    private void RefrescarDebug()
    {
        Refrescar();
    }
}
