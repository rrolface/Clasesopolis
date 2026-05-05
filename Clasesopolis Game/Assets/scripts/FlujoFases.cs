using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FlujoFases : MonoBehaviour
{
    [System.Serializable]
    public class PasoDeFase
    {
        public string nombrePaso;
        [TextArea(3, 5)] public string mensajeTexto;
        public AudioClip audioPaso;
        public GameObject panelPrincipal;
        public TextMeshProUGUI textoUI;

        [Header("Configuración de Retos")]
        public bool esRetoInteractivos;
        public GameObject objetoDelReto;

        [Header("Configuración de Salida")]
        public bool esPasoFinal;
        public string escenaDestino;

        [Header("Número de fase para desbloqueo (solo en paso final)")]
        public int numeroFase = 1;

        [Header("Checkpoint")]
        [Tooltip("Si está activo, al ENTRAR a este paso se guarda como checkpoint. " +
                 "Si el usuario sale de la fase y vuelve, retomará desde aquí.")]
        public bool esCheckpoint;

        [Header("Acciones de Objetos")]
        public List<GameObject> activarAlEntrar;
        public List<GameObject> desactivarAlEntrar;
    }

    [Header("Identificador de fase")]
    [Tooltip("Número de esta fase (1, 2, 3...). Se usa para guardar/recuperar el checkpoint " +
             "y para marcarla como completada por usuario. Debe coincidir con el número usado " +
             "en FaseManager. Si lo dejas en 0, los checkpoints quedan deshabilitados para esta fase.")]
    public int numeroFase = 0;

    [Tooltip("Si está activo, al iniciar la escena salta automáticamente al checkpoint guardado " +
             "para esta fase y este usuario. Apágalo si quieres forzar un inicio desde el principio.")]
    public bool usarCheckpointsAlIniciar = true;

    [Header("Referencias de UI General")]
    public Button botonContinuar;
    public AudioSource parlanteVoces;

    [Header("Lista de Pasos (Configurar en Inspector)")]
    public List<PasoDeFase> pasos;

    private int indiceActual = -1;

    void Start()
    {
        if (GlobalSession.IsAuthenticated() && pasos.Count > 0)
        {
            pasos[0].mensajeTexto = pasos[0].mensajeTexto.Replace("{user}", GlobalSession.user.userName);
        }

        // Apaga todos los paneles por defecto
        foreach (var p in pasos)
        {
            if (p.panelPrincipal != null) p.panelPrincipal.SetActive(false);
        }

        // ----- Salto a checkpoint -----
        if (usarCheckpointsAlIniciar && numeroFase > 0)
        {
            int chk = CheckpointsFase.ObtenerCheckpoint(numeroFase);
            if (chk >= 0 && chk < pasos.Count)
            {
                // -1 porque AvanzarPaso() incrementa antes de aplicar
                indiceActual = chk - 1;
                Debug.Log($"[FlujoFases] Retomando fase {numeroFase} desde checkpoint #{chk} ({pasos[chk].nombrePaso})");
            }
        }

        AvanzarPaso();
    }

    public void AvanzarPaso()
    {
        // 1. Chequeo del paso final
        if (indiceActual >= 0 && indiceActual < pasos.Count)
        {
            PasoDeFase pasoQueTermina = pasos[indiceActual];
            if (pasoQueTermina.esPasoFinal)
            {
                ProgresoGlobal.RegistrarFinDeFase(pasoQueTermina.numeroFase);

                // Marcamos la fase como completada para este usuario y limpiamos checkpoint
                // así un nuevo intento empieza desde el principio.
                if (numeroFase > 0)
                {
                    CheckpointsFase.MarcarCompletada(numeroFase);
                    CheckpointsFase.LimpiarCheckpoint(numeroFase);
                }

                CargarSiguienteEscena(pasoQueTermina.escenaDestino);
                return;
            }
        }

        // 2. Limpieza
        if (parlanteVoces != null) parlanteVoces.Stop();
        GameObject panelAnterior = (indiceActual >= 0 && indiceActual < pasos.Count)
            ? pasos[indiceActual].panelPrincipal : null;

        indiceActual++;

        // 3. Límite
        if (indiceActual >= pasos.Count)
        {
            Debug.Log("Se terminaron los pasos de esta fase.");
            return;
        }

        PasoDeFase pasoActual = pasos[indiceActual];

        // 4. Paneles
        if (panelAnterior != null && panelAnterior != pasoActual.panelPrincipal)
            panelAnterior.SetActive(false);

        if (pasoActual.panelPrincipal != null)
            pasoActual.panelPrincipal.SetActive(true);

        // 5. Bloqueo por reto
        if (pasoActual.esRetoInteractivos)
        {
            if (botonContinuar != null) botonContinuar.gameObject.SetActive(false);
            if (pasoActual.objetoDelReto != null) pasoActual.objetoDelReto.SetActive(true);
        }
        else
        {
            if (botonContinuar != null) botonContinuar.gameObject.SetActive(true);
        }

        // 6. Activación / desactivación
        foreach (GameObject obj in pasoActual.activarAlEntrar) if (obj != null) obj.SetActive(true);
        foreach (GameObject obj in pasoActual.desactivarAlEntrar) if (obj != null) obj.SetActive(false);

        // 7. Texto y audio
        if (pasoActual.textoUI != null) pasoActual.textoUI.text = pasoActual.mensajeTexto;

        if (pasoActual.audioPaso != null && parlanteVoces != null)
        {
            parlanteVoces.clip = pasoActual.audioPaso;
            parlanteVoces.Play();
        }

        // 8. Checkpoint: si este paso está marcado como punto de control, lo guardamos.
        if (pasoActual.esCheckpoint && numeroFase > 0)
        {
            CheckpointsFase.GuardarCheckpoint(numeroFase, indiceActual);
        }
    }

    public void HabilitarBotonContinuar()
    {
        if (botonContinuar != null) botonContinuar.gameObject.SetActive(true);
    }

    private void CargarSiguienteEscena(string nombre)
    {
        if (!string.IsNullOrEmpty(nombre))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nombre);
        }
    }

    // ============================================================
    //  ACCESOS PÚBLICOS (para PanelDecisionFase, botones, etc.)
    // ============================================================

    public int IndiceActual => indiceActual;

    /// <summary>
    /// Borra el checkpoint de esta fase y reinicia el flujo desde el principio.
    /// Útil para un botón "Reiniciar fase" en el menú de selección.
    /// </summary>
    public void ReiniciarFaseDesdeCero()
    {
        if (numeroFase > 0)
            CheckpointsFase.LimpiarCheckpoint(numeroFase);

        indiceActual = -1;
        foreach (var p in pasos)
            if (p.panelPrincipal != null) p.panelPrincipal.SetActive(false);

        AvanzarPaso();
    }
}
