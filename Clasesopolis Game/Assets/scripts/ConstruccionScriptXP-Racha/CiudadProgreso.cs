using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Maneja el "progreso de ciudad" del modo Construcción.
///
/// Modelo simple para MVP/demo:
///   - Existe un contador estático con la cantidad de construcciones colocadas
///     en esta sesión de juego.
///   - Cuando el contador llega a 'totalParaCompletar' (default 10),
///     la ciudad está al 100% y se dispara el evento opcional 'onCiudadCompletada'.
///   - El contador es estático, así que sobrevive a recargas de la escena
///     dentro de la misma sesión (sale a fase y vuelve → el slider se mantiene).
///
/// Wiring esperado:
///   - Pon este componente en un GameObject vacío en la escena Construcción
///     (por ejemplo 'SistemaCiudad').
///   - Arrastra el Slider y, opcionalmente, un TextMeshProUGUI para el contador "3/10".
///   - Llama a CiudadProgreso.RegistrarConstruccion() desde el código que coloca
///     una construcción (lo hace ZonaConstruccion.Construir()).
///
/// Notas:
///   - No persiste en disco. Se reinicia al cerrar el juego.
///   - El reset manual está disponible desde el menú contextual del componente
///     y desde ConfiguracionPruebas (toggle 'reiniciarProgresoCiudadEnMemoria').
/// </summary>
public class CiudadProgreso : MonoBehaviour
{
    // ---- Estado global (cross-scene, en memoria) ----
    /// <summary>
    /// Construcciones colocadas en esta sesión. Estático para que el slider se
    /// mantenga aunque el jugador entre y salga de la escena Construcción.
    /// </summary>
    public static int Construcciones = 0;

    /// <summary>
    /// Instancia activa en la escena actual. Se usa para refrescar la UI
    /// desde el llamador estático RegistrarConstruccion().
    /// </summary>
    public static CiudadProgreso Instance { get; private set; }

    [Header("Meta de la ciudad")]
    [Tooltip("Cuántas construcciones colocar para llenar el slider al 100%. " +
             "Para el MVP/demo usar 10.")]
    public int totalParaCompletar = 10;

    [Header("UI")]
    [Tooltip("Slider visual que se llenará entre 0 y 1.")]
    public Slider sliderProgreso;

    [Tooltip("Texto opcional para mostrar el contador, ej '3 / 10'.")]
    public TextMeshProUGUI textoContador;

    [Tooltip("Formato del texto. {0} = construcciones actuales, {1} = total para completar.")]
    public string formatoTexto = "{0} / {1}";

    [Header("Eventos")]
    [Tooltip("Se dispara una sola vez al alcanzar el 100% de la ciudad.")]
    public UnityEvent onCiudadCompletada;

    // Bandera local para no disparar onCiudadCompletada varias veces.
    private bool yaCompletada = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[CiudadProgreso] Ya existe un CiudadProgreso en " +
                             $"'{Instance.gameObject.name}'. Este nuevo ('{gameObject.name}') lo reemplaza.");
        }
        Instance = this;
    }

    private void OnEnable()
    {
        RefrescarUI();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ============================================================
    //  API ESTÁTICA — llamable desde cualquier script
    // ============================================================

    /// <summary>
    /// Suma una construcción al contador y refresca la UI si hay una instancia
    /// del componente en la escena actual.
    /// </summary>
    public static void RegistrarConstruccion()
    {
        Construcciones++;
        Debug.Log($"[CiudadProgreso] Construcción registrada. Total = {Construcciones}");

        if (Instance != null)
        {
            Instance.RefrescarUI();
        }
    }

    /// <summary>
    /// Resetea el contador a 0 (en memoria). Útil para pruebas.
    /// </summary>
    public static void Reiniciar()
    {
        Construcciones = 0;
        Debug.Log("[CiudadProgreso] Contador de ciudad reiniciado.");

        if (Instance != null)
        {
            Instance.yaCompletada = false;
            Instance.RefrescarUI();
        }
    }

    /// <summary>
    /// Porcentaje entre 0 y 1 del progreso actual.
    /// </summary>
    public static float PorcentajeActual(int meta)
    {
        if (meta <= 0) return 0f;
        return Mathf.Clamp01((float)Construcciones / meta);
    }

    // ============================================================
    //  UI
    // ============================================================

    public void RefrescarUI()
    {
        int meta = Mathf.Max(1, totalParaCompletar);
        float pct = Mathf.Clamp01((float)Construcciones / meta);

        if (sliderProgreso != null)
        {
            sliderProgreso.value = pct;
        }

        if (textoContador != null)
        {
            textoContador.text = string.Format(formatoTexto, Construcciones, meta);
        }

        // Evento de ciudad completada (una sola vez por sesión)
        if (!yaCompletada && Construcciones >= meta)
        {
            yaCompletada = true;
            Debug.Log("[CiudadProgreso] ¡Ciudad completada al 100%!");
            onCiudadCompletada?.Invoke();
        }
    }

    // ============================================================
    //  CONTEXT MENU (debug rápido en el Inspector)
    // ============================================================

    [ContextMenu("Sumar 1 construcción (debug)")]
    private void DebugSumar()
    {
        RegistrarConstruccion();
    }

    [ContextMenu("Reiniciar contador (debug)")]
    private void DebugReiniciar()
    {
        Reiniciar();
    }
}
