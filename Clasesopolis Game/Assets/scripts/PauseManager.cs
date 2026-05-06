using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class PauseManager : MonoBehaviour
{
    [Header("UI del Menú Principal")]
    public GameObject panelPausa;
    public bool estaPausado = false;
    public AudioSource audioSourceVoces;

    [Header("Paneles de Confirmación")]
    public GameObject panelConfirmarZonaLibre; // Asignar en Inspector
    public GameObject panelConfirmarLogout;    // Asignar en Inspector

    void Update()
    {
        // Uso del New Input System para la tecla Escape
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (estaPausado) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        estaPausado = true;
        panelPausa.SetActive(true);
        Time.timeScale = 0f;

        if (audioSourceVoces != null && audioSourceVoces.isPlaying)
        {
            audioSourceVoces.Pause();
        }
    }

    public void Reanudar()
    {
        estaPausado = false;
        panelPausa.SetActive(false);
        // Cerramos cualquier sub-panel de confirmación por si acaso
        if (panelConfirmarZonaLibre) panelConfirmarZonaLibre.SetActive(false);
        if (panelConfirmarLogout) panelConfirmarLogout.SetActive(false);

        Time.timeScale = 1f;

        if (audioSourceVoces != null)
        {
            audioSourceVoces.UnPause();
        }
    }

    // --- LÓGICA DE ZONA LIBRE ---

    public void AbrirConfirmacionZonaLibre()
    {
        // Activamos el panel que pregunta "¿Estás seguro de ir a la zona libre?"
        if (panelConfirmarZonaLibre) panelConfirmarZonaLibre.SetActive(true);
    }

    public void ConfirmarIrAZonaLibre()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Construccion");
    }

    // --- LÓGICA DE CERRAR SESIÓN ---

    public void AbrirConfirmacionLogout()
    {
        // Activamos el panel que pregunta "¿Seguro que quieres cerrar sesión?"
        if (panelConfirmarLogout) panelConfirmarLogout.SetActive(true);
    }

    public void ConfirmarLogout()
    {
        Time.timeScale = 1f;

        // Usamos el método que ya existía en tu GlobalSession
        GlobalSession.Logout();

        Debug.Log("Sesión cerrada. Usuario es ahora: " + GlobalSession.user);
        SceneManager.LoadScene("Inicio");
    }

    // --- UTILIDAD (Para usar en el Login) ---
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo de Clasesópolis...");
        Application.Quit();
    }

    // Método extra para cerrar los paneles de confirmación y volver al menú de pausa
    public void CancelarAccion()
    {
        if (panelConfirmarZonaLibre) panelConfirmarZonaLibre.SetActive(false);
        if (panelConfirmarLogout) panelConfirmarLogout.SetActive(false);
    }
}