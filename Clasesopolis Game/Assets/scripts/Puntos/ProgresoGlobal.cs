using UnityEngine;
using System.Collections.Generic;

public static class ProgresoGlobal
{
    // --- Totales acumulados ---
    public static int XP = 0;
    public static int RachaDias = 0; // ahora cuenta retos completados, no días
    public static List<string> Insignias = new List<string>();

    // --- Última recompensa obtenida (para que la UI pueda mostrar "+X XP" / "Insignia: ..." ) ---
    public static int UltimaXPGanada = 0;
    public static string UltimaInsignia = "";
    public static bool UltimaInsigniaEsNueva = false;

    // -------------------- XP --------------------
    public static void SumarXP(int cantidad)
    {
        XP += cantidad;
        UltimaXPGanada = cantidad;
        Debug.Log($"¡Ganaste {cantidad} XP! Total: {XP}");
    }

    // -------------------- RACHA --------------------
    /// <summary>
    /// Sube la racha en 1. Llamar una sola vez por reto/ejercicio completado.
    /// Es independiente de la fecha — la racha cuenta retos, no días.
    /// </summary>
    public static void IncrementarRacha()
    {
        RachaDias++;
        Debug.Log($"Racha actualizada: {RachaDias} reto(s) completado(s)");
    }

    /// <summary>
    /// Reinicia la racha (por ejemplo cuando se decide penalizar abandonar).
    /// </summary>
    public static void ReiniciarRacha()
    {
        RachaDias = 0;
    }

    /// <summary>
    /// Limpia TODO el estado de recompensas en memoria: XP, racha, insignias
    /// y los marcadores de "última". Útil entre pruebas para empezar limpio
    /// sin reiniciar el juego. No toca PlayerPrefs ni el inventario de
    /// construcciones (eso se maneja desde InventarioConstrucciones).
    /// </summary>
    public static void ReiniciarTodo()
    {
        XP = 0;
        RachaDias = 0;
        Insignias.Clear();
        UltimaXPGanada = 0;
        UltimaInsignia = "";
        UltimaInsigniaEsNueva = false;
        Debug.Log("ProgresoGlobal: estado de recompensas reiniciado en memoria.");
    }

    // -------------------- FASE COMPLETADA --------------------
    /// <summary>
    /// Marca la fase como completada en FaseManager (persiste en PlayerPrefs).
    /// Ya NO toca la racha — la racha se incrementa por reto vía IncrementarRacha.
    /// </summary>
    public static void RegistrarFinDeFase(int numeroFase = 0)
    {
        if (numeroFase > 0 && FaseManager.Instance != null)
        {
            FaseManager.Instance.CompletarFase(numeroFase);
        }
        else if (numeroFase > 0)
        {
            Debug.LogWarning($"FaseManager no encontrado. Fase {numeroFase} no completada.");
        }
    }

    // -------------------- INSIGNIAS --------------------
    public static void GanarInsignia(string nombreInsignia)
    {
        if (string.IsNullOrEmpty(nombreInsignia)) return;

        UltimaInsignia = nombreInsignia;

        if (!Insignias.Contains(nombreInsignia))
        {
            Insignias.Add(nombreInsignia);
            UltimaInsigniaEsNueva = true;
            Debug.Log($"Nueva Insignia desbloqueada: {nombreInsignia}");
        }
        else
        {
            UltimaInsigniaEsNueva = false;
            Debug.Log($"Insignia ya obtenida: {nombreInsignia}");
        }
    }
}
