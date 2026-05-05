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

    [Tooltip("Si está activo, los checkpoints y el estado 'fase completada' NO se " +
             "guardan ni se cargan de PlayerPrefs durante esta sesión.")]
    public bool desactivarPersistenciaCheckpoints = true;

    [Header("Limpieza al iniciar")]
    [Tooltip("Borra XP, racha e insignias en memoria al arrancar la escena.")]
    public bool reiniciarRecompensasEnMemoria = true;

    [Tooltip("Vacía el inventario de construcciones en memoria al arrancar.")]
    public bool reiniciarInventarioEnMemoria = true;

    [Tooltip("Vacía los checkpoints y 'fase completada' en memoria al arrancar.")]
    public bool reiniciarCheckpointsEnMemoria = true;

    [Tooltip("Borra TAMBIÉN los datos guardados en disco (PlayerPrefs) del inventario. " +
             "Útil para empezar de cero después de varias sesiones de prueba.")]
    public bool borrarDatosGuardadosEnDisco = false;

    [Tooltip("Borra TAMBIÉN los checkpoints y 'fase completada' guardados en disco " +
             "para el usuario actual.")]
    public bool borrarCheckpointsEnDisco = false;

    [Header("Logs")]
    [Tooltip("Imprime un resumen de la configuración aplicada al iniciar.")]
    public bool mostrarLogResumen = true;

    void Awake()
    {
        // 1) Aplicamos los flags de persistencia ANTES de cualquier carga.
        InventarioConstrucciones.PersistenciaHabilitada = !desactivarPersistenciaInventario;
        CheckpointsFase.PersistenciaHabilitada = !desactivarPersistenciaCheckpoints;

        // 2) Borrar disco si se pidió (antes de cualquier lectura).
        if (borrarDatosGuardadosEnDisco)
        {
            InventarioConstrucciones.BorrarDatosGuardados();
        }
        if (borrarCheckpointsEnDisco)
        {
            CheckpointsFase.LimpiarTodoDelUsuario();
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

        if (reiniciarCheckpointsEnMemoria)
        {
            CheckpointsFase.ReiniciarEnMemoria();
        }

        if (mostrarLogResumen)
        {
            Debug.Log(
                "[ConfiguracionPruebas] Aplicada -> " +
                $"persistInv={(desactivarPersistenciaInventario ? "OFF" : "ON")}, " +
                $"persistChk={(desactivarPersistenciaCheckpoints ? "OFF" : "ON")}, " +
                $"reiniciarRecompensas={reiniciarRecompensasEnMemoria}, " +
                $"reiniciarInv={reiniciarInventarioEnMemoria}, " +
                $"reiniciarChk={reiniciarCheckpointsEnMemoria}, " +
                $"borrarDisco(inv/chk)={borrarDatosGuardadosEnDisco}/{borrarCheckpointsEnDisco}");
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
        CheckpointsFase.ReiniciarEnMemoria();
        Debug.Log("[ConfiguracionPruebas] Recompensas, inventario y checkpoints reiniciados en memoria.");
    }

    [ContextMenu("Borrar inventario guardado en disco")]
    private void BorrarDiscoAhora()
    {
        InventarioConstrucciones.BorrarDatosGuardados();
        Debug.Log("[ConfiguracionPruebas] PlayerPrefs del inventario borrado.");
    }

    [ContextMenu("Borrar checkpoints guardados en disco (usuario actual)")]
    private void BorrarCheckpointsDiscoAhora()
    {
        CheckpointsFase.LimpiarTodoDelUsuario();
        Debug.Log("[ConfiguracionPruebas] PlayerPrefs de checkpoints borrado.");
    }

    [ContextMenu("Apagar persistencia (inventario)")]
    private void ApagarPersistenciaInv()
    {
        InventarioConstrucciones.PersistenciaHabilitada = false;
        Debug.Log("[ConfiguracionPruebas] Persistencia del inventario: OFF.");
    }

    [ContextMenu("Encender persistencia (inventario)")]
    private void EncenderPersistenciaInv()
    {
        InventarioConstrucciones.PersistenciaHabilitada = true;
        Debug.Log("[ConfiguracionPruebas] Persistencia del inventario: ON.");
    }

    [ContextMenu("Apagar persistencia (checkpoints)")]
    private void ApagarPersistenciaChk()
    {
        CheckpointsFase.PersistenciaHabilitada = false;
        Debug.Log("[ConfiguracionPruebas] Persistencia de checkpoints: OFF.");
    }

    [ContextMenu("Encender persistencia (checkpoints)")]
    private void EncenderPersistenciaChk()
    {
        CheckpointsFase.PersistenciaHabilitada = true;
        Debug.Log("[ConfiguracionPruebas] Persistencia de checkpoints: ON.");
    }
}
