using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Referencias de Paneles")]
    public GameObject panelBienvenida;
    public GameObject panelInicioSesion;
    public GameObject panelRegistro;
    public GameObject panelConfirmacionSalir;

    void Start()
    {
        // 1. Apaga los paneles secundarios
        if (panelInicioSesion != null) panelInicioSesion.SetActive(false);
        if (panelRegistro != null) panelRegistro.SetActive(false);
        if (panelConfirmacionSalir != null) panelConfirmacionSalir.SetActive(false);
    }

    // ---FLUJO: BIENVENIDA <-> INICIO DE SESIÓN ---

    // Lo llamará el botón "Jugar" / "Iniciar" del menú de bienvenida
    public void AbrirInicioSesion()
    {
        panelBienvenida.SetActive(false);
        panelInicioSesion.SetActive(true);
    }

    // Lo llamará la "X" del panel de Inicio de Sesión
    public void VolverABienvenidaDesdeInicio()
    {
        panelInicioSesion.SetActive(false);
        panelBienvenida.SetActive(true);
    }

    // --- FLUJO: INICIO DE SESIÓN <-> REGISTRO ---

    // Lo llamará el texto/botón "Regístrate" en el panel de Inicio
    public void AbrirRegistro()
    {
        panelInicioSesion.SetActive(false);
        panelRegistro.SetActive(true);
    }

    // Lo llamará la "X" del panel de Registro
    public void VolverABienvenidaDesdeRegistro()
    {
        panelRegistro.SetActive(false);
        panelBienvenida.SetActive(true);
    }

    // Lo llamará el botón "Ya tengo cuenta" o "Iniciar Sesión" en el panel de Registro
    public void VolverAInicioDesdeRegistro()
    {
        panelRegistro.SetActive(false);
        panelInicioSesion.SetActive(true);
    }

    // --- FLUJO: SALIDA DEL JUEGO ---

    public void MostrarMenuConfirmacion()
    {
        panelConfirmacionSalir.SetActive(true);
    }

    public void CancelarSalida()
    {
        panelConfirmacionSalir.SetActive(false);
    }

    public void ConfirmarSalida()
    {
        Debug.Log("Cerrando la aplicación...");
        Application.Quit();
    }
}
