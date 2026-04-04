using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public struct TutorialStep
    {
        public string nombreDelPaso; // Solo para organizar en el Inspector
        public GameObject panelUI;   // El panel que debe aparecer
        public AudioClip clipAudio;  // El audio que debe sonar
    }

    [Header("Configuración de Tutorial")]
    public List<TutorialStep> pasosTutorial;
    public AudioSource audioSource;

    private int indiceActual = 0;

    void Start()
    {
        // Inicializamos ocultando todos los paneles por si acaso
        foreach (var paso in pasosTutorial)
        {
            paso.panelUI.SetActive(false);
        }

        // Si quieres que empiece apenas cargue la escena:
        EjecutarPaso(0);
    }

    public void AvanzarPaso()
    {
        // 1. Detener el audio actual si está sonando
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // 2. Apagar el panel actual
        pasosTutorial[indiceActual].panelUI.SetActive(false);

        // 3. Aumentar el índice
        indiceActual++;

        // 4. Verificar si aún quedan pasos
        if (indiceActual < pasosTutorial.Count)
        {
            EjecutarPaso(indiceActual);
        }
        else
        {
            TerminarTutorial();
        }
    }

    void EjecutarPaso(int indice)
    {
        // Activar el nuevo panel
        pasosTutorial[indice].panelUI.SetActive(true);

        // Asignar y reproducir el audio
        if (pasosTutorial[indice].clipAudio != null)
        {
            audioSource.clip = pasosTutorial[indice].clipAudio;
            audioSource.Play();
        }
    }

    void TerminarTutorial()
    {
        Debug.Log("Fin del tutorial de Clasesopolis");
        // Aquí podrías cargar la Zona Libre o desbloquear el movimiento del jugador
    }
}
