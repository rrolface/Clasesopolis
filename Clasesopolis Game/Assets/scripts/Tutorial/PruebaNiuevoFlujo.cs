using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PruebaNiuevoFlujo : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string nombrePaso;
        [TextArea(3, 5)] public string mensajeTexto;
        public AudioClip audioPaso;
        public GameObject panelActivo; // Panel Principal
        public Text componenteTextoUI;

        [Header("Control de Sub-Elementos")]
        public List<GameObject> objetosAActivar;   // Cosas que se prenden en este paso
        public List<GameObject> objetosADesactivar; // Cosas que se apagan en este paso
    }

    [Header("Referencias de UI")]
    public Button continueButton;
    public AudioSource audioSource;

    [Header("Configuración de Pasos")]
    public List<TutorialStep> pasosTutorial;

    [Header("Contenedores Principales")]
    public GameObject inicioByte;
    public GameObject presentacionFases;
    public GameObject bytePresentationSystem;

    private int currentStep = -1;

    void Start()
    {
        if (GlobalSession.IsAuthenticated() && pasosTutorial.Count > 0)
        {
            pasosTutorial[0].mensajeTexto = $"¡Bienvenid@ {GlobalSession.user.userName}! ya estás a un paso de comenzar.";
        }

        PrepararEscena();
        NextStep();
    }

    void PrepararEscena()
    {
        foreach (var paso in pasosTutorial)
        {
            if (paso.panelActivo != null) paso.panelActivo.SetActive(false);
        }
    }

    public void NextStep()
    {
        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();

        // Guardamos referencia al panel que estamos dejando actualmente
        GameObject panelQueDejamos = (currentStep >= 0 && currentStep < pasosTutorial.Count)
            ? pasosTutorial[currentStep].panelActivo : null;

        currentStep++;

        // Verificar si llegamos al final
        if (currentStep >= pasosTutorial.Count)
        {
            if (panelQueDejamos != null) panelQueDejamos.SetActive(false);
            Debug.Log("Fin del tutorial de Clasesópolis.");
            return;
        }

        TutorialStep pasoActual = pasosTutorial[currentStep];

        // --- LÓGICA INTELIGENTE DE PANELES ---
        // Solo apagamos el panel anterior si el nuevo paso usa uno DIFERENTE
        if (panelQueDejamos != null && panelQueDejamos != pasoActual.panelActivo)
        {
            panelQueDejamos.SetActive(false);
        }

        // Activar el panel principal de este paso
        if (pasoActual.panelActivo != null)
            pasoActual.panelActivo.SetActive(true);

        // --- ACTIVAR / DESACTIVAR SUB-ELEMENTOS ---
        foreach (GameObject obj in pasoActual.objetosAActivar)
        {
            if (obj != null) obj.SetActive(true);
        }

        foreach (GameObject obj in pasoActual.objetosADesactivar)
        {
            if (obj != null) obj.SetActive(false);
        }

        // --- TEXTO Y AUDIO ---
        if (pasoActual.componenteTextoUI != null)
            pasoActual.componenteTextoUI.text = pasoActual.mensajeTexto;

        if (pasoActual.audioPaso != null && audioSource != null)
        {
            audioSource.clip = pasoActual.audioPaso;
            audioSource.Play();
        }

        ManejarContenedores(currentStep);
    }

    private void ManejarContenedores(int index)
    {
        // Esta lógica de contenedores grandes (inicioByte, etc.) se mantiene igual
        if (index == 0)
        {
            if (inicioByte) inicioByte.SetActive(true);
            if (presentacionFases) presentacionFases.SetActive(false);
            if (bytePresentationSystem) bytePresentationSystem.SetActive(false);
        }
        else if (index == 5)
        {
            if (inicioByte) inicioByte.SetActive(false);
            if (presentacionFases) presentacionFases.SetActive(true);
        }
    }
}
