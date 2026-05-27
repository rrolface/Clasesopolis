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

    // --- Título actualmente equipado por el jugador ---
    /// <summary>
    /// Nombre del título/insignia que el jugador eligió como su "título activo".
    /// Si está vacío, el jugador no tiene título equipado.
    /// La UI de Construcción lo muestra junto al listón/trofeo.
    /// </summary>
    public static string TituloActivo = "";

    // -------------------- XP --------------------
    public static void SumarXP(int cantidad)
    {
        XP += cantidad;
        UltimaXPGanada = cantidad;
        Debug.Log($"¡Ganaste {cantidad} XP! Total: {XP}");
    }

    /// <summary>
    /// Devuelve true si el jugador tiene al menos 'cantidad' XP disponible.
    /// Útil para validar antes de gastar (por ejemplo al construir).
    /// </summary>
    public static bool TieneXP(int cantidad)
    {
        return XP >= cantidad;
    }

    /// <summary>
    /// Resta XP al total del jugador. Nunca baja de 0 (clamp).
    /// No toca UltimaXPGanada — ese marcador es solo para "recompensas ganadas",
    /// no para gastos. Devuelve la cantidad realmente descontada.
    /// </summary>
    public static int RestarXP(int cantidad)
    {
        if (cantidad <= 0) return 0;

        int descontado = Mathf.Min(cantidad, XP);
        XP -= descontado;
        if (XP < 0) XP = 0;

        Debug.Log($"Se gastaron {descontado} XP. Total restante: {XP}");
        return descontado;
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
        TituloActivo = "";
        Debug.Log("ProgresoGlobal: estado de recompensas reiniciado en memoria.");
    }

    // -------------------- FASE COMPLETADA --------------------
    /// <summary>
    /// Marca la fase como completada en PlayerPrefs (vía FaseManager estático).
    /// Funciona desde CUALQUIER escena, no requiere que FaseManager esté
    /// presente en la jerarquía actual.
    /// Ya NO toca la racha — la racha se incrementa por reto vía IncrementarRacha.
    /// </summary>
    public static void RegistrarFinDeFase(int numeroFase = 0)
    {
        if (numeroFase > 0)
        {
            FaseManager.MarcarCompletada(numeroFase);
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

            // Si el jugador todavía no tiene título equipado, la primera insignia
            // que gana queda como activa por defecto. Así el listón en la UI nunca
            // está vacío después de ganar al menos una.
            if (string.IsNullOrEmpty(TituloActivo))
            {
                TituloActivo = nombreInsignia;
                Debug.Log($"[ProgresoGlobal] Título activo auto-equipado: {nombreInsignia}");
            }
        }
        else
        {
            UltimaInsigniaEsNueva = false;
            Debug.Log($"Insignia ya obtenida: {nombreInsignia}");
        }
    }

    /// <summary>
    /// Establece el título activo del jugador. Solo lo aplica si la insignia
    /// ya está desbloqueada (presente en la lista Insignias). Si se pasa
    /// string vacío o null, desequipa el título activo. Devuelve true si se aplicó.
    /// </summary>
    public static bool EstablecerTituloActivo(string nombreInsignia)
    {
        if (string.IsNullOrEmpty(nombreInsignia))
        {
            TituloActivo = "";
            Debug.Log("[ProgresoGlobal] Título activo desequipado.");
            return true;
        }

        if (!Insignias.Contains(nombreInsignia))
        {
            Debug.LogWarning($"[ProgresoGlobal] Intento de equipar título no desbloqueado: '{nombreInsignia}'");
            return false;
        }

        TituloActivo = nombreInsignia;
        Debug.Log($"[ProgresoGlobal] Título activo: {nombreInsignia}");
        return true;
    }
}
