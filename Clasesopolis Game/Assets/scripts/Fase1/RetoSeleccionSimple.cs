using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // <--- VITAL: Para detectar el clic

public class RetoSeleccionSimple : MonoBehaviour
{
    [Header("Referencias Generales")]
    public FlujoFases flujoPrincipal;
    public TextMeshProUGUI textoFeedback;
    public int puntosXP = 50;

    [Header("Efectos de Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoError;

    [Header("Opción 1: Materiales")]
    public GameObject botonMateriales;
    [TextArea(2, 3)] public string mensajeMateriales;
    public bool materialesEsCorrecto;

    [Header("Opción 2: Trabajadores")]
    public GameObject botonTrabajadores;
    [TextArea(2, 3)] public string mensajeTrabajadores;
    public bool trabajadoresEsCorrecto;

    [Header("Opción 3: Planos")]
    public GameObject botonPlanos;
    [TextArea(2, 3)] public string mensajePlanos;
    public bool planosEsCorrecto;

    // --- FUNCIONES EXPLÍCITAS PARA EL INSPECTOR ---

    public void OpcionMateriales(GameObject visualFeedback)
    {
        ProcesarRespuesta(botonMateriales, visualFeedback, mensajeMateriales, materialesEsCorrecto);
    }

    public void OpcionTrabajadores(GameObject visualFeedback)
    {
        ProcesarRespuesta(botonTrabajadores, visualFeedback, mensajeTrabajadores, trabajadoresEsCorrecto);
    }

    public void OpcionPlanos(GameObject visualFeedback)
    {
        ProcesarRespuesta(botonPlanos, visualFeedback, mensajePlanos, planosEsCorrecto);
    }

    // --- LÓGICA INTERNA (Privada para no ensuciar el Inspector) ---

    private void ProcesarRespuesta(GameObject boton, GameObject visual, string mensaje, bool esCorrecto)
    {
        // 1. Intercambio visual
        if (boton != null) boton.SetActive(false);
        if (visual != null) visual.SetActive(true);

        // 2. Feedback de texto
        if (textoFeedback != null) textoFeedback.text = mensaje;

        // 3. Resultado
        if (esCorrecto)
        {
            if (sonidoCorrecto && audioSource) audioSource.PlayOneShot(sonidoCorrecto);
            ProgresoGlobal.SumarXP(puntosXP);
            if (flujoPrincipal != null) flujoPrincipal.HabilitarBotonContinuar();
            Debug.Log("¡Respuesta Correcta detectada!");
        }
        else
        {
            if (sonidoError && audioSource) audioSource.PlayOneShot(sonidoError);
            Debug.Log("Respuesta Incorrecta: " + mensaje);
        }
    }
}