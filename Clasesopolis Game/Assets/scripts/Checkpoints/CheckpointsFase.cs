using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema de checkpoints por fase y por usuario.
///
/// Diseño:
/// - Usa PlayerPrefs (con clave que incluye el nombre de usuario logueado),
///   así dos jugadores en la misma máquina no se pisan los progresos.
/// - Si no hay usuario logueado en GlobalSession, usa "_anon" como nombre.
/// - Tiene flag PersistenciaHabilitada (igual que InventarioConstrucciones)
///   para apagar todo en disco durante pruebas y trabajar solo en memoria.
/// - Cuando migremos a base de datos, basta con cambiar el get/set por dentro
///   y mantener la misma API pública.
/// </summary>
public static class CheckpointsFase
{
    /// <summary>
    /// Si está en false, todo vive solo en memoria. No se lee ni se escribe en PlayerPrefs.
    /// </summary>
    public static bool PersistenciaHabilitada = true;

    // ---- Estado en memoria (se usa siempre, y es lo único que se usa cuando persistencia OFF) ----
    private static readonly Dictionary<string, int> checkpointsMem = new Dictionary<string, int>();
    private static readonly HashSet<string> completadasMem = new HashSet<string>();

    // ---- Helpers de claves ----
    private static string Usuario()
    {
        return GlobalSession.IsAuthenticated() && !string.IsNullOrEmpty(GlobalSession.user.userName)
            ? GlobalSession.user.userName
            : "_anon";
    }

    private static string KeyCheckpoint(int numeroFase) => $"chk_{Usuario()}_fase{numeroFase}";
    private static string KeyCompletada(int numeroFase) => $"done_{Usuario()}_fase{numeroFase}";

    // ============================================================
    //  CHECKPOINTS
    // ============================================================

    /// <summary>
    /// Guarda el índice del paso como checkpoint para esta fase.
    /// </summary>
    public static void GuardarCheckpoint(int numeroFase, int indicePaso)
    {
        if (numeroFase <= 0) return;

        string key = KeyCheckpoint(numeroFase);
        checkpointsMem[key] = indicePaso;

        if (PersistenciaHabilitada)
        {
            PlayerPrefs.SetInt(key, indicePaso);
            PlayerPrefs.Save();
        }

        Debug.Log($"[CheckpointsFase] Guardado fase={numeroFase}, paso={indicePaso}, user={Usuario()}");
    }

    /// <summary>
    /// Devuelve el índice del paso guardado o -1 si no hay.
    /// </summary>
    public static int ObtenerCheckpoint(int numeroFase)
    {
        if (numeroFase <= 0) return -1;

        string key = KeyCheckpoint(numeroFase);

        if (checkpointsMem.TryGetValue(key, out int valorMem))
            return valorMem;

        if (PersistenciaHabilitada)
        {
            int valor = PlayerPrefs.GetInt(key, -1);
            if (valor >= 0) checkpointsMem[key] = valor; // cache
            return valor;
        }

        return -1;
    }

    public static bool TieneCheckpoint(int numeroFase)
    {
        return ObtenerCheckpoint(numeroFase) >= 0;
    }

    /// <summary>
    /// Borra el checkpoint de una fase (típicamente al completarla o al reiniciar).
    /// </summary>
    public static void LimpiarCheckpoint(int numeroFase)
    {
        if (numeroFase <= 0) return;

        string key = KeyCheckpoint(numeroFase);
        checkpointsMem.Remove(key);

        if (PersistenciaHabilitada)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }

    // ============================================================
    //  FASE COMPLETADA
    // ============================================================

    public static void MarcarCompletada(int numeroFase)
    {
        if (numeroFase <= 0) return;

        string key = KeyCompletada(numeroFase);
        completadasMem.Add(key);

        if (PersistenciaHabilitada)
        {
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }

        Debug.Log($"[CheckpointsFase] Fase {numeroFase} marcada como COMPLETADA para {Usuario()}");
    }

    public static bool EstaCompletada(int numeroFase)
    {
        if (numeroFase <= 0) return false;

        string key = KeyCompletada(numeroFase);

        if (completadasMem.Contains(key)) return true;

        if (PersistenciaHabilitada)
        {
            bool persistida = PlayerPrefs.GetInt(key, 0) == 1;
            if (persistida) completadasMem.Add(key); // cache
            return persistida;
        }

        return false;
    }

    public static void LimpiarCompletada(int numeroFase)
    {
        if (numeroFase <= 0) return;

        string key = KeyCompletada(numeroFase);
        completadasMem.Remove(key);

        if (PersistenciaHabilitada)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }

    // ============================================================
    //  LIMPIEZA GENERAL (pruebas)
    // ============================================================

    /// <summary>
    /// Borra TODO lo del usuario actual en memoria. Si la persistencia está activa,
    /// también barre las claves PlayerPrefs asociadas (hasta 20 fases por seguridad).
    /// </summary>
    public static void LimpiarTodoDelUsuario()
    {
        checkpointsMem.Clear();
        completadasMem.Clear();

        if (PersistenciaHabilitada)
        {
            for (int n = 1; n <= 20; n++)
            {
                PlayerPrefs.DeleteKey(KeyCheckpoint(n));
                PlayerPrefs.DeleteKey(KeyCompletada(n));
            }
            PlayerPrefs.Save();
        }

        Debug.Log($"[CheckpointsFase] Limpieza total para {Usuario()}");
    }

    /// <summary>
    /// Borra solo el estado en memoria (sin tocar disco). Útil si quieres
    /// resetear la sesión actual y dejar lo guardado en disco intacto.
    /// </summary>
    public static void ReiniciarEnMemoria()
    {
        checkpointsMem.Clear();
        completadasMem.Clear();
    }
}
