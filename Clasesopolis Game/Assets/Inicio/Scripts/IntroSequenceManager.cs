using UnityEngine;
using UnityEngine.Video;

public class IntroSequenceManager : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer reproductorFondo;
    public GameObject panelBienvenido;

    // Referencia a la música
    public AudioSource musicaDeFondo;

    // Para pruebas comentar cuando termine
    [Header("Debug")]
    public bool saltarIntroParaPruebas = true;
    void Start()
    {
        // 1. Asegura de que la UI esté apagada al arrancar
        panelBienvenido.SetActive(false);

        // 2. Al terminar el video Muesstra la UI"
        reproductorFondo.loopPointReached += MostrarUI;

        // Para pruebas únicamente
        panelBienvenido.SetActive(false);

        // PRUEBA
        if (saltarIntroParaPruebas)
        {
            reproductorFondo.Stop();
            MostrarUI(reproductorFondo);
            return;
        }

        reproductorFondo.loopPointReached += MostrarUI;
    }
    void MostrarUI(VideoPlayer vp)
    {
        // 3. Enciende el panel principal
        panelBienvenido.SetActive(true);

        // Reproduce la música cuando se muestra la UI
        if (musicaDeFondo != null)
        {
            musicaDeFondo.Play();
        }

        //Debug.Log("El video de introducción ha terminado. Pantalla de bienvenida activa.");
    }
}
