using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FlujoTutorial : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string nombrePaso;
        [TextArea(3, 5)] public string mensajeTexto;
        public AudioClip audioPaso;
        public GameObject panelActivo;
        public Text componenteTextoUI;


        [Header("Configuración de Retos")]
        public bool esPasoDeEjercicio; // NUEVO: Bloquea el botón Continuar
        public GameObject objetoDelEjercicio; // El panel o lógica del minijuego


        [Header("Configuración de Salida")]
        public bool esPasoFinal; // El "chulito" para marcar si este paso cierra el tutorial
        public string escenaACargar; // Nombre de la escena: "ZonaJuego"

        [Header("Control de Sub-Elementos")]
        public List<GameObject> objetosAActivar;
        public List<GameObject> objetosADesactivar;
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

       


        //-------------------------------
        // LÓGICA DE SALTO INTELIGENTE
        if (!string.IsNullOrEmpty(TutorialDataBridge.nombrePasoAIniciar))
        {
            // Buscamos la posición del paso que tenga ese nombre exacto
            int indexEncontrado = pasosTutorial.FindIndex(p => p.nombrePaso == TutorialDataBridge.nombrePasoAIniciar);

            if (indexEncontrado != -1)
            {
                currentStep = indexEncontrado - 1;
            }
            else
            {
                Debug.LogWarning("No se encontró el paso: " + TutorialDataBridge.nombrePasoAIniciar);
                currentStep = -1;
            }
        }
        else
        {
            currentStep = TutorialDataBridge.indiceDeInicio - 1;
        }

        // Limpiamos el puente para futuros usos
        TutorialDataBridge.nombrePasoAIniciar = "";
        TutorialDataBridge.indiceDeInicio = 0;

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

    // --- FUNCIÓN DE CARGA INTERNA ---
    private void EjecutarCambioDeEscena(string nombreEscena)
    {
        if (!string.IsNullOrEmpty(nombreEscena))
        {
            Debug.Log("Saliendo del tutorial hacia: " + nombreEscena);
            Time.timeScale = 1f; // Nos aseguramos de que el tiempo no esté en 0
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError("No escribiste el nombre de la escena en el Inspector.");
        }
    }

    public void NextStep()
    {
        // 1. CHEQUEO DE SALIDA: ¿El paso que estamos viendo ahora tiene el chulito de 'esPasoFinal'?
        if (currentStep >= 0 && currentStep < pasosTutorial.Count)
        {
            if (pasosTutorial[currentStep].esPasoFinal)
            {
                // Si el usuario le dio a "Continuar" y este era el paso final, ejecutamos la función
                EjecutarCambioDeEscena(pasosTutorial[currentStep].escenaACargar);
                return; // Bloqueamos el resto del código para que no busque un paso que no existe
            }
        }

        // --- LÓGICA NORMAL DE AVANCE ---
        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();

        GameObject panelQueDejamos = (currentStep >= 0 && currentStep < pasosTutorial.Count)
            ? pasosTutorial[currentStep].panelActivo : null;

        currentStep++;

        // Verificación de seguridad por si se acaba la lista y no marcaste ningún paso final
        if (currentStep >= pasosTutorial.Count)
        {
            if (panelQueDejamos != null) panelQueDejamos.SetActive(false);
            Debug.Log("Fin del tutorial de Clasesópolis.");
            return;
        }

        TutorialStep pasoActual = pasosTutorial[currentStep];

        // 2. LÓGICA DE BLOQUEO POR EJERCICIO
        if (pasoActual.esPasoDeEjercicio)
        {
            continueButton.gameObject.SetActive(false); // Escondemos el botón de seguir
            if (pasoActual.objetoDelEjercicio != null)
                pasoActual.objetoDelEjercicio.SetActive(true); // Encendemos el minijuego
        }
        else
        {
            continueButton.gameObject.SetActive(true);
        }

        // Manejo de paneles principales
        if (panelQueDejamos != null && panelQueDejamos != pasoActual.panelActivo)
        {
            panelQueDejamos.SetActive(false);
        }

        if (pasoActual.panelActivo != null)
            pasoActual.panelActivo.SetActive(true);

        // Activar / Desactivar sub-elementos
        foreach (GameObject obj in pasoActual.objetosAActivar) if (obj != null) obj.SetActive(true);
        foreach (GameObject obj in pasoActual.objetosADesactivar) if (obj != null) obj.SetActive(false);

        // Texto y Audio
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
        // Tu lógica de contenedores se mantiene intacta
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

    // FUNCIÓN CLAVE: La llama el minijuego cuando el usuario gana
    public void HabilitarContinuar()
    {
        continueButton.gameObject.SetActive(true);
        Debug.Log("Reto superado. El usuario puede continuar.");
    }
}
