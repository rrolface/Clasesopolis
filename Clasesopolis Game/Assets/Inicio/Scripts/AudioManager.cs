using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Referencias de Audio")]
    public AudioSource sfxSource;       // Reproductor de efectos
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
