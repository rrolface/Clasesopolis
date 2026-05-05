using UnityEngine;

/// <summary>
/// Panel de control para la fase de pruebas. Pega este componente a un
/// GameObject vacío en la PRIMERA escena que carga (por ejemplo "Inicio")
/// y desde el Inspector puedes apagar la persistencia y reiniciar el
/// estado de recompensas sin tocar Firebase ni el resto del flujo.
///
/// IMPORTANTE: solo afecta a las recompensas que añadimos recientemente
/// (XP, racha, insignias en memoria + inventario de construcciones).
/// NO toca login, Firebase, FaseManager ni configuración de audio.
///
/// Se ejecuta antes que cualquier otro script (DefaultExecutionOrder = -1000)
/// para que cuando los paneles arranquen ya tengan el flag aplicado.
/// </summary>
[DefaultExecutionOrder(-1000)]
public class ConfiguracionPruebas : MonoBehaviour
{
    [Header("Persistencia")]
    [Tooltip("Si está activo, el inventario de construcciones NO se guarda ni se " +
             "carga de PlayerPrefs durante esta sesión. Todo vive solo en memoria.")]
    public bool desactivarPersistenciaInventario = true;

    [Header("Limpieza al iniciar")]
    [Tooltip("Borra XP, racha e insignias en memoria al arrancar la escena.")]
    public bool reiniciarRecompensasEnMemoria = true;

    [Tooltip("Vacía el inventario de construcciones en memoria al arrancar.")]
    public bool reiniciarInventarioEnMemoria = true;

    [Tooltip("Borra TAMBIÉN los datos guardados en disco (PlayerPrefs) del inventario. " +
             "Útil para empezar de cero después de varias sesiones de prueba.")]
    public bool borrarDatosGuardadosEnDisco = false;

    [Header("Logs")]
    [Tooltip("Imprime un resumen de la configuración aplicada al iniciar.")]
    public bool mostrarLogResumen = true;

    void Awake()
    {
        // 1) Aplicamos el flag de persistencia ANTES de cualquier carga.
        InventarioConstrucciones.PersistenciaHabilitada = !desactivarPersistenciaInventario;

        // 2) Borrar disco si se pidió (antes de cualquier lectura).
        if (borrarDatosGuardadosEnDisco)
        {
            InventarioConstrucciones.BorrarDatosGuardados();
        }

        // 3) Limpiar memoria si se pidió.
        if (reiniciarRecompensasEnMemoria)
        {
            ProgresoGlobal.ReiniciarTodo();
        }

        if (reiniciarInventarioEnMemoria)
        {
            InventarioConstrucciones.ReiniciarEnMemoria();
        }

        if (mostrarLogResumen)
        {
            Debug.Log(
                "[ConfiguracionPruebas] Aplicada -> " +
                $"persistencia={(desactivarPersistenciaInventario ? "OFF" : "ON")}, " +
                $"reiniciarRecompensas={reiniciarRecompensasEnMemoria}, " +
                $"reiniciarInventario={reiniciarInventarioEnMemoria}, " +
                $"borrarDisco={borrarDatosGuardadosEnDisco}");
        }
    }

    // ----------------------------------------------------------
    // Atajos del menú contextual (botón derecho sobre el componente)
    // útiles para reiniciar el estado en runtime sin parar el juego.
    // ----------------------------------------------------------

    [ContextMenu("Reiniciar todo AHORA (memoria)")]
    private void ReiniciarTodoAhora()
    {
        ProgresoGlobal.ReiniciarTodo();
        InventarioConstrucciones.ReiniciarEnMemoria();
        Debug.Log("[ConfiguracionPruebas] Recompensas e inventario reiniciados en memoria.");
    }

    [ContextMenu("Borrar inventario guardado en disco")]
    private void BorrarDiscoAhora()
    {
        InventarioConstrucciones.BorrarDatosGuardados();
        Debug.Log("[ConfiguracionPruebas] PlayerPrefs del inventario borrado.");
    }

    [ContextMenu("Apagar persistencia (inventario)")]
    private void ApagarPersistencia()
    {
        InventarioConstrucciones.PersistenciaHabilitada = false;
        Debug.Log("[ConfiguracionPruebas] Persistencia del inventario: OFF.");
    }

    [ContextMenu("Encender persistencia (inventario)")]
    private void EncenderPersistencia()
    {
        InventarioConstrucciones.PersistenciaHabilitada = true;
        Debug.Log("[ConfiguracionPruebas] Persistencia del inventario: ON.");
    }
}
