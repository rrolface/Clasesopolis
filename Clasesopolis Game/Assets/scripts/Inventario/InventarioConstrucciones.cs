using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventario persistente de construcciones del jugador.
///
/// Diseño:
/// - Solo guardamos los IDs en disco (PlayerPrefs/JSON). Los datos completos
///   (nombre, icono, prefab) se reconstruyen consultando al catálogo en runtime.
/// - Esto está pensado para migrarse a una base de datos remota más adelante:
///   con cambiar Cargar/Guardar (y mantener la misma API pública) basta.
/// - Es estático para que todas las escenas compartan el mismo estado, igual
///   que ProgresoGlobal y GlobalSession.
/// </summary>
public static class InventarioConstrucciones
{
    private const string KeyInventario = "InventarioConstrucciones";

    private static readonly List<string> idsPoseidos = new List<string>();
    private static bool cargado = false;

    /// <summary>
    /// Si está en false, el inventario funciona SOLO en memoria:
    /// Cargar() y Guardar() no leen ni escriben en PlayerPrefs.
    /// Útil para fases de prueba donde no quieres que lo guardado de
    /// sesiones anteriores ensucie las pruebas. Por defecto está activa
    /// para no romper el flujo existente — apágala desde ConfiguracionPruebas.
    /// </summary>
    public static bool PersistenciaHabilitada = true;

    /// <summary>
    /// Catálogo de construcciones. Debe asignarse en runtime (por ejemplo
    /// desde PanelResumenFase o PanelInventarioConstrucciones) antes de
    /// usar OtorgarAleatoria u ObtenerPoseidos.
    /// </summary>
    public static CatalogoConstrucciones Catalogo { get; set; }

    /// <summary>
    /// Última construcción otorgada al jugador (para que la UI de recompensa la lea).
    /// </summary>
    public static Construccion UltimaOtorgada { get; private set; }

    // -------------------- PERSISTENCIA --------------------
    public static void Cargar()
    {
        idsPoseidos.Clear();

        // Si la persistencia está apagada, arrancamos vacío y no tocamos disco.
        if (!PersistenciaHabilitada)
        {
            cargado = true;
            return;
        }

        string json = PlayerPrefs.GetString(KeyInventario, "");
        if (!string.IsNullOrEmpty(json))
        {
            ListaIdsWrapper wrapper = JsonUtility.FromJson<ListaIdsWrapper>(json);
            if (wrapper != null && wrapper.ids != null)
                idsPoseidos.AddRange(wrapper.ids);
        }
        cargado = true;
    }

    public static void Guardar()
    {
        // Si la persistencia está apagada, no escribimos en disco.
        if (!PersistenciaHabilitada) return;

        ListaIdsWrapper wrapper = new ListaIdsWrapper { ids = new List<string>(idsPoseidos) };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(KeyInventario, json);
        PlayerPrefs.Save();
    }

    private static void AsegurarCargado()
    {
        if (!cargado) Cargar();
    }

    /// <summary>
    /// Vacía el inventario en memoria SIN tocar el PlayerPrefs.
    /// Útil para reiniciar el estado de pruebas dentro de la misma sesión.
    /// </summary>
    public static void ReiniciarEnMemoria()
    {
        idsPoseidos.Clear();
        UltimaOtorgada = null;
        cargado = true; // marca como cargado vacío así no intenta leer disco después
    }

    /// <summary>
    /// Borra los datos guardados en disco (PlayerPrefs) sin tocar el estado en memoria.
    /// Para una limpieza total puedes combinarlo con ReiniciarEnMemoria.
    /// </summary>
    public static void BorrarDatosGuardados()
    {
        PlayerPrefs.DeleteKey(KeyInventario);
        PlayerPrefs.Save();
    }

    // -------------------- LECTURA --------------------
    /// <summary>
    /// IDs en bruto que el jugador posee (solo lectura).
    /// </summary>
    public static IReadOnlyList<string> IdsPoseidos
    {
        get { AsegurarCargado(); return idsPoseidos; }
    }

    /// <summary>
    /// Devuelve los datos completos de cada construcción que el jugador posee,
    /// resolviéndolos contra el catálogo. Si no hay catálogo asignado, devuelve vacío.
    /// </summary>
    public static List<Construccion> ObtenerPoseidos()
    {
        AsegurarCargado();
        List<Construccion> resultado = new List<Construccion>();
        if (Catalogo == null) return resultado;

        foreach (string id in idsPoseidos)
        {
            Construccion c = Catalogo.BuscarPorId(id);
            if (c != null) resultado.Add(c);
        }
        return resultado;
    }

    public static bool TieneConstruccion(string id)
    {
        AsegurarCargado();
        return idsPoseidos.Contains(id);
    }

    // -------------------- ESCRITURA --------------------
    /// <summary>
    /// Agrega una construcción al inventario por id.
    /// No agrega duplicados: si ya está, se queda igual.
    /// </summary>
    public static void Agregar(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        AsegurarCargado();
        if (!idsPoseidos.Contains(id))
        {
            idsPoseidos.Add(id);
            Guardar();
        }
    }

    /// <summary>
    /// Otorga una construcción aleatoria del catálogo y la agrega al inventario.
    /// Por defecto evita repetir las que ya posee. Si ya las tiene todas, devuelve
    /// una repetida (sin duplicar el id internamente).
    /// </summary>
    /// <param name="permitirRepetidas">
    /// Si true, escoge entre TODAS las del catálogo, incluyendo las que ya posee.
    /// </param>
    /// <returns>Datos de la construcción otorgada, o null si el catálogo está vacío.</returns>
    public static Construccion OtorgarAleatoria(bool permitirRepetidas = false)
    {
        AsegurarCargado();
        if (Catalogo == null || Catalogo.construcciones == null || Catalogo.construcciones.Count == 0)
        {
            Debug.LogWarning("InventarioConstrucciones: catálogo no asignado o vacío.");
            UltimaOtorgada = null;
            return null;
        }

        List<Construccion> candidatas = new List<Construccion>();
        if (permitirRepetidas)
        {
            foreach (var c in Catalogo.construcciones)
                if (c != null) candidatas.Add(c);
        }
        else
        {
            foreach (var c in Catalogo.construcciones)
                if (c != null && !idsPoseidos.Contains(c.id))
                    candidatas.Add(c);

            // Fallback: si ya las tiene todas, escoge cualquiera del catálogo.
            if (candidatas.Count == 0)
            {
                foreach (var c in Catalogo.construcciones)
                    if (c != null) candidatas.Add(c);
            }
        }

        if (candidatas.Count == 0)
        {
            UltimaOtorgada = null;
            return null;
        }

        Construccion elegida = candidatas[Random.Range(0, candidatas.Count)];
        Agregar(elegida.id); // idempotente: no duplica
        UltimaOtorgada = elegida;
        return elegida;
    }

    /// <summary>
    /// Borra el inventario (útil para testing o un botón "Reiniciar progreso").
    /// </summary>
    public static void Reiniciar()
    {
        idsPoseidos.Clear();
        UltimaOtorgada = null;
        Guardar();
    }

    // Wrapper privado para que JsonUtility pueda serializar la lista.
    [System.Serializable]
    private class ListaIdsWrapper
    {
        public List<string> ids = new List<string>();
    }
}
