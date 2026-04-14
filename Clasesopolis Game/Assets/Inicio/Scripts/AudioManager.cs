using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Reproductores de Audio")]
    public AudioSource musicSource;     // Para la música de fondo
    public AudioSource sfxSource;       // Reproductor de efectos

    [Header("Clips de Efectos")]
    public AudioClip sonidoHover;       // Archivo de audio para hover
    public AudioClip sonidoClick;       // Archivo de audio para click

    public void ReproducirHover()
    {
        if (sfxSource != null && sonidoHover != null)
        {
            sfxSource.PlayOneShot(sonidoHover);
        }
    }

    // Función al hacer click
    public void ReproducirClick()
    {
        if (sfxSource != null && sonidoClick != null)
        {
            sfxSource.PlayOneShot(sonidoClick);
        }
    }
}
