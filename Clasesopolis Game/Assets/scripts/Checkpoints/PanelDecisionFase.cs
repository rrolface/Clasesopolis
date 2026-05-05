using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Panel de decisión que se muestra dentro del flujo de la fase (típicamente
/// después de un momento de recompensa). Ofrece dos opciones:
///
///   - Continuar:    avanza al siguiente paso del flujo.
///   - SalirDeFase:  carga otra escena (por defecto la zona de construcción).
///                   El checkpoint ya fue guardado por FlujoFases al ENTRAR
///                   a este paso (siempre que esté marcado esCheckpoint=true),
///                   así que al volver a entrar a la fase se retoma desde aquí.
///
/// Cómo usarlo en el Inspector:
///   1) En tu FlujoFases agrega un PasoDeFase nuevo donde quieras que aparezca
///      el panel.
///   2) Marca esRetoInteractivos = true (para esconder el botón Continuar global).
///   3) Marca esCheckpoint = true (para que se guarde el progreso al llegar aquí).
///   4) Pon este script en el panelPrincipal del paso.
///   5) Asigna 'flujoPrincipal' arrastrando el GameObject que tiene FlujoFases.
///   6) Cablea los dos botones del panel a Continuar() y SalirDeFase().
/// </summary>
public class PanelDecisionFase : MonoBehaviour
{
    [Header("Referencia al flujo")]
    public FlujoFases flujoPrincipal;

    [Header("Salida")]
    [Tooltip("Nombre de la escena a cargar al pulsar 'Salir/Probar construcción'. " +
             "Por defecto la zona de construcción/libre.")]
    public string escenaSalida = "Construccion";

    [Tooltip("Si está activo, asegura Time.timeScale=1 antes de cargar la escena " +
             "(por si el juego estaba pausado).")]
    public bool reanudarTiempoAlSalir = true;

    /// <summary>
    /// Botón "Continuar": avanza al siguiente paso del flujo.
    /// </summary>
    public void Continuar()
    {
        if (flujoPrincipal == null)
        {
            Debug.LogWarning("[PanelDecisionFase] No hay FlujoFases asignado en 'flujoPrincipal'.");
            return;
        }

        // Cerramos el panel para que no quede visible mientras el flujo cambia de paso.
        gameObject.SetActive(false);

        flujoPrincipal.AvanzarPaso();
    }

    /// <summary>
    /// Botón "Salir / Probar construcción": carga la escena configurada.
    /// El checkpoint ya quedó guardado por FlujoFases al entrar al paso
    /// (siempre que el paso tenga esCheckpoint = true).
    /// </summary>
    public void SalirDeFase()
    {
        if (string.IsNullOrEmpty(escenaSalida))
        {
            Debug.LogWarning("[PanelDecisionFase] 'escenaSalida' está vacío. No se puede salir.");
            return;
        }

        if (reanudarTiempoAlSalir) Time.timeScale = 1f;
        SceneManager.LoadScene(escenaSalida);
    }

    /// <summary>
    /// Variante de salida que permite especificar la escena por código
    /// (por ejemplo desde un menú de pausa).
    /// </summary>
    public void SalirAEscena(string nombreEscena)
    {
        if (string.IsNullOrEmpty(nombreEscena)) return;
        if (reanudarTiempoAlSalir) Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }
}
