using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VozAyudaInspector : MonoBehaviour
{
    [Header("Configuración de Voces")]
    public AudioSource audioSourceVoces; // El "parlante" de Byte
    public List<AudioClip> frasesDeAyuda; // Lista de audios: "¡Claro!", "¡Dime!", "¿En qué ayudo?"

    // Esta función la llamaremos desde el botón
    public void SaludarYAyudar()
    {
        if (frasesDeAyuda.Count == 0) return;

        // Si Byte ya está hablando, cortamos la frase anterior para decir la nueva
        if (audioSourceVoces.isPlaying) audioSourceVoces.Stop();

        // Elegimos un índice al azar
        int indiceAleatorio = Random.Range(0, frasesDeAyuda.Count);

        // Asignamos y reproducimos
        audioSourceVoces.clip = frasesDeAyuda[indiceAleatorio];
        audioSourceVoces.Play();

        Debug.Log("Inspector Byte dice: " + frasesDeAyuda[indiceAleatorio].name);
    }
}