using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class IntroSequenceManager : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer reproductorFondo;
    public GameObject panelBienvenido;

    // Referencia al Canvas Group que controla la transparencia
    public CanvasGroup grupoCanvasBienvenido;

    [Header("Ajustes")]
    public AudioSource musicaDeFondo;
    public float duracionFade = 1.5f; // Tiempo en segundos que tardará en aparecer

    void Start()
    {
        // Apaga el panel
        panelBienvenido.SetActive(false);

        //  Inicia la transparencia en 0 
        if (grupoCanvasBienvenido != null)
        {
            grupoCanvasBienvenido.alpha = 0f;
        }

        // Suscribe el evento al terminar el video
        reproductorFondo.loopPointReached += MostrarUI;
    }

    void MostrarUI(VideoPlayer vp)
    {
        // Enciende el objeto 
        panelBienvenido.SetActive(true);

        // Inicia la Corrutina
        StartCoroutine(EfectoFadeIn());

        // Reproduce la música
        if (musicaDeFondo != null)
        {
            musicaDeFondo.Play();
        }
    }

    // --- FADE-IN ---
    IEnumerator EfectoFadeIn()
    {
        float tiempoPasado = 0f;

        while (tiempoPasado < duracionFade)
        {
            tiempoPasado += Time.deltaTime;

            grupoCanvasBienvenido.alpha = Mathf.Clamp01(tiempoPasado / duracionFade);

            yield return null;
        }
        grupoCanvasBienvenido.alpha = 1f;
    }
}
