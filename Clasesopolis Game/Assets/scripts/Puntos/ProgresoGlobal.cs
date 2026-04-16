using UnityEngine;
using System;
using System.Collections.Generic;

public static class ProgresoGlobal
{
    public static int XP = 0;
    public static int RachaDias = 0;
    public static List<string> Insignias = new List<string>();

    private static string UltimaConexionKey = "UltimaConexion";

    public static void SumarXP(int cantidad)
    {
        XP += cantidad;
        Debug.Log($"¡Ganaste {cantidad} XP! Total: {XP}");
    }

    public static void RegistrarFinDeFase(int numeroFase = 0)
    {
        string fechaGuardada = PlayerPrefs.GetString(UltimaConexionKey, "");
        DateTime ahora = DateTime.Now;

        if (!string.IsNullOrEmpty(fechaGuardada))
        {
            DateTime ultimaVez = DateTime.Parse(fechaGuardada);
            TimeSpan diferencia = ahora - ultimaVez;
            if (diferencia.TotalHours <= 24)
            {
                RachaDias++;
                Debug.Log("¡Racha mantenida!");
            }
            else
            {
                RachaDias = 1;
            }
        }
        else { RachaDias = 1; }

        PlayerPrefs.SetString(UltimaConexionKey, ahora.ToString());

        if (numeroFase > 0 && FaseManager.Instance != null)
        {
            FaseManager.Instance.CompletarFase(numeroFase);
        }
        else if (numeroFase > 0)
        {
            Debug.LogWarning($"FaseManager no encontrado. Fase {numeroFase} no completada.");
        }
    }

    public static void GanarInsignia(string nombreInsignia)
    {
        if (!Insignias.Contains(nombreInsignia))
        {
            Insignias.Add(nombreInsignia);
            Debug.Log($"Nueva Insignia desbloqueada: {nombreInsignia}");
        }
    }
}