using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager de fases.
///
/// Tiene DOS APIs:
///   - De INSTANCIA: usadas desde la escena donde vive este GameObject
///     (típicamente Construccion / ZonaJuego). Las usa BotonFase y demás UI.
///   - ESTÁTICAS: funcionan sin necesidad de que el GameObject exista en la
///     escena. Las llama ProgresoGlobal.RegistrarFinDeFase desde la escena
///     de la fase (donde FaseManager NO está presente).
///
/// La persistencia es PlayerPrefs con claves "fase{N}_completada".
/// </summary>
public class FaseManager : MonoBehaviour
{
    public static FaseManager Instance;

    [Header("Índices de escena (confirmados en Build Settings)")]
    public int escenaFase1 = 4;
    public int escenaFase2 = 5;
    public int escenaFase3 = 6;
    public int escenaFase4 = -1;

    private void Awake()
    {
        // Detección de duplicados: si ya hay un Instance distinto, avisamos.
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[FaseManager] DUPLICADO detectado. Ya existe un FaseManager en " +
                             $"'{Instance.gameObject.name}'. Este nuevo ('{gameObject.name}') va a reemplazarlo. " +
                             "Esto suele causar que veas valores 'congelados' — borra el FaseManager duplicado.");
        }

        Instance = this;

        // Log de los valores actuales — útil para confirmar qué índices realmente tiene este FaseManager.
        Debug.Log($"[FaseManager] Awake en '{gameObject.name}'. " +
                  $"Fase1={escenaFase1}, Fase2={escenaFase2}, Fase3={escenaFase3}, Fase4={escenaFase4}");
    }

    // ============================================================
    //  NAVEGACIÓN ENTRE FASES (instancia, requiere los índices)
    // ============================================================
    public void EntrarFase(int numeroFase)
    {
        Debug.Log($"[FaseManager] EntrarFase({numeroFase}) llamado en GameObject '{gameObject.name}'.");

        if (!FaseDesbloqueada(numeroFase))
        {
            Debug.Log($"[FaseManager] Fase {numeroFase} bloqueada. No se carga.");
            return;
        }

        int indice = ObtenerIndice(numeroFase);
        Debug.Log($"[FaseManager] Fase {numeroFase} -> índice de escena resuelto: {indice}");

        if (indice < 0)
        {
            Debug.LogWarning($"[FaseManager] Fase {numeroFase} no tiene escena asignada aún (índice {indice}). " +
                             $"Revisa el campo 'Escena Fase {numeroFase}' del componente en la escena Construccion.");
            return;
        }

        SceneManager.LoadScene(indice);
    }

    private int ObtenerIndice(int numeroFase)
    {
        switch (numeroFase)
        {
            case 1: return escenaFase1;
            case 2: return escenaFase2;
            case 3: return escenaFase3;
            case 4: return escenaFase4;
            default: return -1;
        }
    }

    // ============================================================
    //  ESTADO DE FASES — API DE INSTANCIA (delega a estática)
    // ============================================================
    public bool FaseDesbloqueada(int numeroFase) => EstaDesbloqueada(numeroFase);
    public void CompletarFase(int numeroFase) => MarcarCompletada(numeroFase);

    // ============================================================
    //  ESTADO DE FASES — API ESTÁTICA
    //  Llamable desde cualquier escena, no necesita Instance.
    // ============================================================

    /// <summary>
    /// Si está en true, EstaDesbloqueada devuelve siempre true (todas las fases
    /// quedan accesibles sin importar el progreso real). NO toca PlayerPrefs,
    /// es solo un bypass de sesión para testing. La controla ConfiguracionPruebas.
    /// </summary>
    public static bool IgnorarBloqueos = false;

    /// <summary>
    /// True si la fase N puede entrarse (la N-1 está completada, o N=1).
    /// </summary>
    public static bool EstaDesbloqueada(int numeroFase)
    {
        if (IgnorarBloqueos) return true;
        if (numeroFase <= 1) return true;
        return PlayerPrefs.GetInt($"fase{numeroFase - 1}_completada", 0) == 1;
    }

    /// <summary>
    /// True si la fase N ya fue completada por el jugador.
    /// </summary>
    public static bool EstaCompletada(int numeroFase)
    {
        if (numeroFase <= 0) return false;
        return PlayerPrefs.GetInt($"fase{numeroFase}_completada", 0) == 1;
    }

    /// <summary>
    /// Marca la fase N como completada. Esto desbloquea automáticamente la N+1.
    /// Llamable desde cualquier escena — no necesita Instance.
    /// </summary>
    public static void MarcarCompletada(int numeroFase)
    {
        if (numeroFase <= 0) return;
        PlayerPrefs.SetInt($"fase{numeroFase}_completada", 1);
        PlayerPrefs.Save();
        Debug.Log($"[FaseManager] Fase {numeroFase} completada. Fase {numeroFase + 1} desbloqueada.");
    }

    /// <summary>
    /// Desbloquea hasta la fase N inclusive marcando las 1..N-1 como completadas.
    /// Ej: DesbloquearHasta(4) marca fase1, fase2 y fase3 como completadas.
    /// </summary>
    public static void DesbloquearHasta(int numeroFase)
    {
        for (int n = 1; n < numeroFase; n++)
        {
            PlayerPrefs.SetInt($"fase{n}_completada", 1);
        }
        PlayerPrefs.Save();
        Debug.Log($"[FaseManager] Desbloqueado hasta Fase {numeroFase}.");
    }

    /// <summary>
    /// Borra el progreso de TODAS las fases.
    /// Tras esto, solo Fase 1 queda accesible.
    /// </summary>
    public static void ResetearTodo()
    {
        for (int n = 1; n <= 10; n++)
        {
            PlayerPrefs.DeleteKey($"fase{n}_completada");
        }
        PlayerPrefs.Save();
        Debug.Log("[FaseManager] Progreso de fases reseteado a cero.");
    }

    // ============================================================
    //  CONTEXT MENU
    // ============================================================
    [ContextMenu("Resetear Progreso de Fases")]
    public void ResetearProgreso()
    {
        ResetearTodo();
    }

    [ContextMenu("Desbloquear todas las fases (1..4)")]
    public void DesbloquearTodas()
    {
        DesbloquearHasta(4);
    }
}
