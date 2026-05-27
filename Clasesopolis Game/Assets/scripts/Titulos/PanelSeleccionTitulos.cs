using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel desplegable que muestra todas las insignias/títulos desbloqueados por el
/// jugador y le permite seleccionar uno como su "título activo".
///
/// Funcionamiento:
///   - Lee ProgresoGlobal.Insignias (lista de strings).
///   - Por cada insignia desbloqueada, instancia un BotonTituloItem en el contenedor.
///   - Cada botón muestra el mismo icono compartido (un listón/trofeo) + nombre.
///   - Al hacer click sobre un botón, el título queda equipado y el panel se cierra.
///
/// La misma instancia se reutiliza: cada vez que se activa (OnEnable) regenera la lista.
/// </summary>
public class PanelSeleccionTitulos : MonoBehaviour
{
    [Header("UI - Lista de títulos")]
    [Tooltip("Contenedor (usualmente con VerticalLayoutGroup o GridLayoutGroup) " +
             "donde se instanciarán los botones de título.")]
    public Transform contenedorBotones;

    [Tooltip("Prefab del botón de título (debe tener BotonTituloItem).")]
    public BotonTituloItem prefabBoton;

    [Tooltip("Icono compartido por TODOS los títulos (el listón/trofeo). " +
             "Es el mismo sprite para cada item porque visualmente todos son iguales.")]
    public Sprite iconoPorDefecto;

    [Header("UI - Estado y cierre")]
    [Tooltip("GameObject que se muestra cuando el jugador no tiene ningún título " +
             "desbloqueado (ej. 'Completa fases para desbloquear títulos').")]
    public GameObject mensajeVacio;

    [Tooltip("Botón opcional para cerrar el panel manualmente (X o 'Cerrar').")]
    public Button botonCerrar;

    [Header("Referencia al display principal")]
    [Tooltip("Componente que muestra el título activo en la UI principal. " +
             "Se refresca automáticamente cuando el jugador elige un título.")]
    public TituloActivoUI tituloActivoUI;

    [Header("Comportamiento")]
    [Tooltip("Si está marcado, después de seleccionar un título el panel se cierra solo. " +
             "Si lo desmarcas, el jugador puede ver el cambio reflejado y cerrar manualmente.")]
    public bool cerrarAlSeleccionar = true;

    // Botones instanciados actualmente (para limpiarlos al refrescar).
    private readonly List<BotonTituloItem> botones = new List<BotonTituloItem>();

    private bool cableado = false;

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
    /// Reconstruye la lista de botones a partir de ProgresoGlobal.Insignias.
    /// Llamar manualmente si las insignias cambian mientras el panel está abierto.
    /// </summary>
    public void Refrescar()
    {
        // Limpia botones previos
        for (int i = botones.Count - 1; i >= 0; i--)
        {
            if (botones[i] != null) Destroy(botones[i].gameObject);
        }
        botones.Clear();

        List<string> insignias = ProgresoGlobal.Insignias;

        if (mensajeVacio != null)
            mensajeVacio.SetActive(insignias.Count == 0);

        if (contenedorBotones == null || prefabBoton == null) return;

        foreach (string nombre in insignias)
        {
            BotonTituloItem item = Instantiate(prefabBoton, contenedorBotones);
            item.Configurar(nombre, iconoPorDefecto, this);
            botones.Add(item);
        }

        // Refrescar marcos de selección por si TituloActivo ya estaba seteado.
        ActualizarMarcosSeleccion();
    }

    /// <summary>
    /// Llamado por cada BotonTituloItem al hacerle click.
    /// Aplica el título, refresca la UI principal y cierra el panel.
    /// </summary>
    public void SeleccionarTitulo(string nombreInsignia)
    {
        bool ok = ProgresoGlobal.EstablecerTituloActivo(nombreInsignia);
        if (!ok) return;

        if (tituloActivoUI != null) tituloActivoUI.Refrescar();
        ActualizarMarcosSeleccion();

        if (cerrarAlSeleccionar) Cerrar();
    }

    public void Cerrar()
    {
        gameObject.SetActive(false);
    }

    private void ActualizarMarcosSeleccion()
    {
        foreach (var b in botones)
        {
            if (b != null) b.ActualizarMarcoSeleccion();
        }
    }
}
