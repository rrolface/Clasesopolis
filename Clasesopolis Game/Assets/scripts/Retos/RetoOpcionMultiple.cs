using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Reto de opción múltiple basado en PANELES PRE-DISEÑADOS por el usuario.
///
/// Cada pregunta tiene su propio panel armado a mano en el Inspector con la
/// estética que quiera. El script no genera nada visualmente: solo activa el
/// panel que toca, escucha los clics de los botones y maneja el flujo de
/// resultados.
///
/// Estructura:
///   - Lista de preguntas. Por cada pregunta:
///       · panelPregunta:        GameObject pre-diseñado.
///       · botonCorrecto:        el Button de la opción correcta.
///       · botonesIncorrectos:   los demás botones del panel.
///
///   - Paneles de feedback compartidos:
///       · panelCorrecto         (se muestra tras una respuesta acertada).
///       · panelIncorrecto       (se muestra tras una respuesta fallada).
///
///   - Paneles finales (uno u otro según el resultado):
///       · panelFinalPerfecto    (cero errores, solo botón Continuar).
///       · panelFinalConErrores  (al menos un error, botones Reintentar y Continuar).
///
/// Lo que NO cambia respecto a la versión anterior:
///   - Sistema de puntos proporcional al puntajeMaximo.
///   - Suma de XP al ProgresoGlobal (que después leerá PanelResumenFase).
///   - Regreso al flujo vía FlujoFases.HabilitarBotonContinuar + AvanzarPaso.
/// </summary>
public class RetoOpcionMultiple : MonoBehaviour
{
    // ============================================================
    //  DATOS DE UNA PREGUNTA
    // ============================================================
    [Serializable]
    public class PreguntaPanel
    {
        [Tooltip("Solo para identificarla en el Inspector. No se muestra al usuario.")]
        public string titulo;

        [Tooltip("Panel pre-diseñado de esta pregunta. Se activa cuando le toca y se desactiva al responder.")]
        public GameObject panelPregunta;

        [Tooltip("El botón de la opción correcta. Al pulsarlo cuenta como acierto.")]
        public Button botonCorrecto;

        [Tooltip("Los botones de las opciones incorrectas. Pulsar cualquiera cuenta como error.")]
        public List<Button> botonesIncorrectos = new List<Button>();
    }

    // ============================================================
    //  CONFIGURACIÓN
    // ============================================================
    [Header("Integración con flujo")]
    public FlujoFases flujoPrincipal;

    [Header("Preguntas del reto")]
    public List<PreguntaPanel> preguntas = new List<PreguntaPanel>();

    [Tooltip("Puntaje máximo si se aciertan TODAS las preguntas. Se reparte proporcional a los aciertos.")]
    public int puntajeMaximo = 100;

    // ============================================================
    //  PANELES DE FEEDBACK (compartidos para todas las preguntas)
    // ============================================================
    [Header("Paneles de feedback")]
    [Tooltip("Panel que se muestra cuando el usuario responde correctamente.")]
    public GameObject panelCorrecto;

    [Tooltip("Panel que se muestra cuando el usuario responde incorrectamente.")]
    public GameObject panelIncorrecto;

    // ============================================================
    //  PANELES FINALES
    // ============================================================
    [Header("Paneles finales del minijuego")]
    [Tooltip("Panel mostrado al terminar si NO hubo errores. Suele tener solo el botón Continuar.")]
    public GameObject panelFinalPerfecto;

    [Tooltip("Panel mostrado al terminar si HUBO al menos un error. Suele tener Reintentar y Continuar.")]
    public GameObject panelFinalConErrores;

    [Header("Texto de resultado (opcional)")]
    [Tooltip("Si lo asignas, se rellena con el resumen del reto al llegar al panel final.")]
    public TextMeshProUGUI textoResultado;

    [TextArea]
    [Tooltip("Plantilla del texto. {0}=aciertos, {1}=total, {2}=incorrectas, {3}=puntaje.")]
    public string formatoResultado = "Correctas: {0}/{1}\nIncorrectas: {2}\nGanaste {3} XP";

    // ============================================================
    //  ESTADO INTERNO
    // ============================================================
    private int indiceActual = 0;
    private int aciertos = 0;
    private int puntajeTemporal = 0;
    private bool retoTerminado = false;

    // ============================================================
    //  CICLO DE VIDA
    // ============================================================
    void OnEnable()
    {
        IniciarReto();
    }

    /// <summary>
    /// Resetea el estado y arranca desde la primera pregunta.
    /// También cablea TODOS los botones de TODAS las preguntas (una sola vez).
    /// </summary>
    public void IniciarReto()
    {
        indiceActual = 0;
        aciertos = 0;
        puntajeTemporal = 0;
        retoTerminado = false;

        // Cablear cada botón de cada pregunta a Responder(true/false)
        for (int i = 0; i < preguntas.Count; i++)
        {
            PreguntaPanel p = preguntas[i];

            if (p.botonCorrecto != null)
            {
                p.botonCorrecto.onClick.RemoveAllListeners();
                p.botonCorrecto.onClick.AddListener(() => Responder(true));
            }
            else
            {
                Debug.LogWarning($"[RetoOpcionMultiple] La pregunta '{p.titulo}' no tiene botonCorrecto asignado.");
            }

            if (p.botonesIncorrectos != null)
            {
                foreach (Button btn in p.botonesIncorrectos)
                {
                    if (btn == null) continue;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => Responder(false));
                }
            }
        }

        ApagarTodosLosPaneles();
        MostrarPreguntaActual();
    }

    // ============================================================
    //  FLUJO DE PREGUNTAS
    // ============================================================
    private void MostrarPreguntaActual()
    {
        if (preguntas == null || preguntas.Count == 0)
        {
            Debug.LogWarning("[RetoOpcionMultiple] No hay preguntas configuradas.");
            MostrarFinal();
            return;
        }

        if (indiceActual >= preguntas.Count)
        {
            MostrarFinal();
            return;
        }

        PreguntaPanel p = preguntas[indiceActual];
        if (p.panelPregunta != null)
            p.panelPregunta.SetActive(true);
    }

    /// <summary>
    /// La llama internamente el listener cableado en cada botón.
    /// Apaga el panel de la pregunta y muestra el feedback correspondiente.
    /// </summary>
    private void Responder(bool fueCorrecto)
    {
        if (retoTerminado) return;
        if (indiceActual >= preguntas.Count) return;

        // Apaga el panel de la pregunta actual
        PreguntaPanel p = preguntas[indiceActual];
        if (p.panelPregunta != null) p.panelPregunta.SetActive(false);

        if (fueCorrecto)
        {
            aciertos++;
            if (panelCorrecto != null) panelCorrecto.SetActive(true);
        }
        else
        {
            if (panelIncorrecto != null) panelIncorrecto.SetActive(true);
        }
    }

    /// <summary>
    /// Botón "Continuar" de los paneles de respuesta correcta/incorrecta.
    /// Apaga el feedback y muestra la siguiente pregunta (o el final).
    /// </summary>
    public void Avanzar()
    {
        if (panelCorrecto != null) panelCorrecto.SetActive(false);
        if (panelIncorrecto != null) panelIncorrecto.SetActive(false);

        indiceActual++;

        if (indiceActual >= preguntas.Count)
            MostrarFinal();
        else
            MostrarPreguntaActual();
    }

    // ============================================================
    //  FINAL DEL MINIJUEGO
    // ============================================================
    private void MostrarFinal()
    {
        retoTerminado = true;

        int total = preguntas != null ? preguntas.Count : 0;
        int incorrectas = total - aciertos;

        // Cálculo proporcional de puntos (mismo criterio que GestorClasificacion)
        if (total > 0)
        {
            float proporcion = (float)aciertos / total;
            puntajeTemporal = Mathf.RoundToInt(proporcion * puntajeMaximo);
        }
        else
        {
            puntajeTemporal = 0;
        }

        if (textoResultado != null)
            textoResultado.text = string.Format(formatoResultado, aciertos, total, incorrectas, puntajeTemporal);

        ApagarTodosLosPaneles();

        if (incorrectas == 0)
        {
            if (panelFinalPerfecto != null) panelFinalPerfecto.SetActive(true);
        }
        else
        {
            if (panelFinalConErrores != null) panelFinalConErrores.SetActive(true);
        }
    }

    /// <summary>Botón "Volver a intentar" del panel final con errores.</summary>
    public void Reintentar()
    {
        IniciarReto();
    }

    /// <summary>Botón "Continuar" de los paneles finales.</summary>
    public void Continuar()
    {
        FinalizarReto();
    }

    /// <summary>
    /// Lógica de cierre: suma XP, vuelve al flujo y se apaga.
    /// IDÉNTICA a la del reto anterior — sistema de puntos y persistencia intactos.
    /// </summary>
    private void FinalizarReto()
    {
        ProgresoGlobal.SumarXP(puntajeTemporal);

        ApagarTodosLosPaneles();

        if (flujoPrincipal != null)
        {
            flujoPrincipal.HabilitarBotonContinuar();
            flujoPrincipal.AvanzarPaso();
        }

        gameObject.SetActive(false);
    }

    // ============================================================
    //  UTILIDAD
    // ============================================================
    private void ApagarTodosLosPaneles()
    {
        foreach (PreguntaPanel p in preguntas)
        {
            if (p != null && p.panelPregunta != null)
                p.panelPregunta.SetActive(false);
        }

        if (panelCorrecto != null) panelCorrecto.SetActive(false);
        if (panelIncorrecto != null) panelIncorrecto.SetActive(false);
        if (panelFinalPerfecto != null) panelFinalPerfecto.SetActive(false);
        if (panelFinalConErrores != null) panelFinalConErrores.SetActive(false);
    }

    // ============================================================
    //  ACCESOS PÚBLICOS
    // ============================================================
    public int Aciertos => aciertos;
    public int PuntajeTemporal => puntajeTemporal;
    public int IndiceActual => indiceActual;
    public int TotalPreguntas => preguntas != null ? preguntas.Count : 0;
}
