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

    [Header("Desbloqueo provisional de fases (testing)")]
    [Tooltip("Si está activo, al iniciar marca las fases anteriores como completadas " +
             "para que la fase indicada quede desbloqueada en el menú de Construccion. " +
             "Ejemplo: 4 desbloquea las fases 1, 2, 3 y 4 al mismo tiempo. " +
             "Dejar en 0 para no forzar nada.")]
    public int desbloquearFasesHasta = 0;

    [Tooltip("Si está activo, RESETEA el progreso de fases al arrancar — solo Fase 1 queda accesible. " +
             "Útil para empezar pruebas desde cero. Si combinas con 'desbloquear hasta', " +
             "primero resetea y luego aplica el desbloqueo.")]
    public bool resetearProgresoFases = false;

    [Tooltip("Si está activo, TODOS los botones de fase quedan interactuables sin importar " +
             "el progreso real. NO toca PlayerPrefs: el progreso real queda intacto. " +
             "Úsalo para saltar directo a la fase que estás trabajando (ej. Fase 4) sin pasar por las anteriores.")]
    public bool ignorarBloqueosFases = false;

    [Header("Logs")]
    [Tooltip("Imprime un resumen de la configuración aplicada al iniciar.")]
    public bool mostrarLogResumen = true;

    void Awake()
    {
        // 1) PRIMERO borrar disco si se pidió, mientras la persistencia aún está
        //    en su valor por defecto (ON). Si lo hacemos después de tocar los flags,
        //    podríamos quedar atrapados en el caso donde "borrar" se pide al mismo
        //    tiempo que "desactivar persistencia" — la limpieza no surtiría efecto
        //    sobre el disco si la persistencia ya está apagada.
        if (borrarDatosGuardadosEnDisco)
        {
            InventarioConstrucciones.BorrarDatosGuardados();
        }
        if (borrarCheckpointsEnDisco)
        {
            CheckpointsFase.LimpiarTodoDelUsuario();
        }

        // 2) Ahora sí aplicamos los flags de persistencia que regirán el resto de la sesión.
        InventarioConstrucciones.PersistenciaHabilitada = !desactivarPersistenciaInventario;
        CheckpointsFase.PersistenciaHabilitada = !desactivarPersistenciaCheckpoints;

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

        // 4) Estado de fases (desbloqueo provisional para testing).
        //    El reset SIEMPRE va antes del desbloqueo, así no se pisan.
        if (resetearProgresoFases)
        {
            FaseManager.ResetearTodo();
        }

        if (desbloquearFasesHasta > 1)
        {
            FaseManager.DesbloquearHasta(desbloquearFasesHasta);
        }

        // Bypass de bloqueo (sesión only, no toca PlayerPrefs).
        FaseManager.IgnorarBloqueos = ignorarBloqueosFases;

        if (mostrarLogResumen)
        {
            Debug.Log(
                "[ConfiguracionPruebas] Aplicada -> " +
                $"persistInv={(desactivarPersistenciaInventario ? "OFF" : "ON")}, " +
                $"persistChk={(desactivarPersistenciaCheckpoints ? "OFF" : "ON")}, " +
                $"reiniciarRecompensas={reiniciarRecompensasEnMemoria}, " +
                $"reiniciarInv={reiniciarInventarioEnMemoria}, " +
                $"reiniciarChk={reiniciarCheckpointsEnMemoria}, " +
                $"borrarDisco(inv/chk)={borrarDatosGuardadosEnDisco}/{borrarCheckpointsEnDisco}, " +
                $"resetearFases={resetearProgresoFases}, desbloquearHasta={desbloquearFasesHasta}");
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

    [ContextMenu("Fases: resetear progreso (solo Fase 1)")]
    private void ResetearFasesAhora()
    {
        FaseManager.ResetearTodo();
    }

    [ContextMenu("Fases: desbloquear hasta Fase 4 (1+4 testeo)")]
    private void DesbloquearHastaFase4()
    {
        FaseManager.DesbloquearHasta(4);
    }

    [ContextMenu("Fases: desbloquear todas (1..4)")]
    private void DesbloquearTodas()
    {
        FaseManager.DesbloquearHasta(4);
    }

    [ContextMenu("Fases: ignorar bloqueos ON (testing)")]
    private void IgnorarBloqueosOn()
    {
        FaseManager.IgnorarBloqueos = true;
        Debug.Log("[ConfiguracionPruebas] FaseManager.IgnorarBloqueos = ON. Todas las fases entrables.");
    }

    [ContextMenu("Fases: ignorar bloqueos OFF")]
    private void IgnorarBloqueosOff()
    {
        FaseManager.IgnorarBloqueos = false;
        Debug.Log("[ConfiguracionPruebas] FaseManager.IgnorarBloqueos = OFF.");
    }
}
