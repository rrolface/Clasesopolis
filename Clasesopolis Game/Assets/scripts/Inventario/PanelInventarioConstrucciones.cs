using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel del modo construcción/libre que muestra al jugador su inventario
/// de construcciones desbloqueadas. Cada construcción aparece como un botón
/// y al hacerle click queda seleccionada para colocarse en la próxima
/// ZonaConstruccion.
/// </summary>
public class PanelInventarioConstrucciones : MonoBehaviour
{
    [Header("Catálogo")]
    [Tooltip("Mismo ScriptableObject que usa PanelResumenFase para otorgar premios.")]
    public CatalogoConstrucciones catalogo;

    [Header("UI")]
    [Tooltip("Contenedor con un GridLayoutGroup donde se instanciarán los botones.")]
    public Transform contenedorBotones;

    [Tooltip("Prefab del botón de inventario (debe tener BotonConstruccionItem).")]
    public BotonConstruccionItem prefabBoton;

    [Header("Texto de selección actual (opcional)")]
    public TextMeshProUGUI textoNombreSeleccionada;
    public Image iconoSeleccionada;
    public string formatoNombre = "Seleccionada: {0}";

    [Header("Mensaje cuando el inventario está vacío")]
    public GameObject panelInventarioVacio;

    // Botones actualmente instanciados (para limpiarlos al refrescar).
    private readonly List<BotonConstruccionItem> botones = new List<BotonConstruccionItem>();

    void OnEnable()
    {
        if (catalogo != null)
            InventarioConstrucciones.Catalogo = catalogo;

        Refrescar();
        ActualizarTextoSeleccion();
    }

    /// <summary>
    /// Reconstruye la lista de botones a partir del inventario actual.
    /// Llamar manualmente si el inventario cambia mientras el panel está abierto.
    /// </summary>
    public void Refrescar()
    {
        // Limpia botones previos
        for (int i = botones.Count - 1; i >= 0; i--)
        {
            if (botones[i] != null) Destroy(botones[i].gameObject);
        }
        botones.Clear();

        if (contenedorBotones == null || prefabBoton == null) return;

        List<Construccion> poseidos = InventarioConstrucciones.ObtenerPoseidos();

        if (panelInventarioVacio != null)
            panelInventarioVacio.SetActive(poseidos.Count == 0);

        foreach (Construccion c in poseidos)
        {
            BotonConstruccionItem item = Instantiate(prefabBoton, contenedorBotones);
            item.Configurar(c, this);
            botones.Add(item);
        }
    }

    /// <summary>
    /// Llamado por cada BotonConstruccionItem cuando se hace click.
    /// </summary>
    public void NotificarSeleccion(Construccion seleccionada)
    {
        // Refresca el marco de cada botón
        foreach (var b in botones)
            if (b != null) b.ActualizarMarcoSeleccion();

        ActualizarTextoSeleccion();
    }

    private void ActualizarTextoSeleccion()
    {
        Construccion sel = SeleccionConstruccion.Actual;

        if (textoNombreSeleccionada != null)
            textoNombreSeleccionada.text = (sel != null)
                ? string.Format(formatoNombre, sel.nombre)
                : "";

        if (iconoSeleccionada != null)
        {
            if (sel != null && sel.icono != null)
            {
                iconoSeleccionada.sprite = sel.icono;
                iconoSeleccionada.enabled = true;
            }
            else
            {
                iconoSeleccionada.enabled = false;
            }
        }
    }
}
