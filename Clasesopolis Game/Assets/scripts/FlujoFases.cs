using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FlujoFases : MonoBehaviour
{
    [System.Serializable]
    public class PasoDeFase
    {
        public string nombrePaso;
        [TextArea(3, 5)] public string mensajeTexto;
        public AudioClip audioPaso;
        public GameObject panelPrincipal; // El panel que contiene la UI de este paso
        public TextMeshProUGUI textoUI;

        [Header("Configuración de Retos")]
        public bool esRetoInteractivos;
        public GameObject objetoDelReto;

        [Header("Configuración de Salida")]
        public bool esPasoFinal;
        public string escenaDestino;

        [Header("Acciones de Objetos")]
        public List<GameObject> activarAlEntrar;
        public List<GameObject> desactivarAlEntrar;
    }

    [Header("Referencias de UI General")]
    public Button botonContinuar;
    public AudioSource parlanteVoces;

    [Header("Lista de Pasos (Configurar en Inspector)")]
    public List<PasoDeFase> pasos;

    private int indiceActual = -1;

    void Start()
    {
        // Si hay un usuario logueado, personalizamos el primer paso
        if (GlobalSession.IsAuthenticated() && pasos.Count > 0)
        {
            pasos[0].mensajeTexto = pasos[0].mensajeTexto.Replace("{user}", GlobalSession.user.userName);
        }

        // Reseteamos el estado visual inicial
        foreach (var p in pasos)
        {
            if (p.panelPrincipal != null) p.panelPrincipal.SetActive(false);
        }

        AvanzarPaso();
    }

    public void AvanzarPaso()
    {
        // 1. Antes de avanzar, checamos si el paso que estamos dejando es el final
        if (indiceActual >= 0 && indiceActual < pasos.Count)
        {
            if (pasos[indiceActual].esPasoFinal)
            {
                ProgresoGlobal.RegistrarFinDeFase();
                CargarSiguienteEscena(pasos[indiceActual].escenaDestino);
                return;
            }
        }

        // 2. Limpieza de audio y paneles anteriores
        if (parlanteVoces != null) parlanteVoces.Stop();

        GameObject panelAnterior = (indiceActual >= 0) ? pasos[indiceActual].panelPrincipal : null;

        indiceActual++;

        // 3. Verificamos si nos pasamos del límite
        if (indiceActual >= pasos.Count)
        {
            Debug.Log("Se terminaron los pasos de esta fase.");
            return;
        }

        PasoDeFase pasoActual = pasos[indiceActual];

        // 4. LÓGICA DE PANELES
        if (panelAnterior != null && panelAnterior != pasoActual.panelPrincipal)
            panelAnterior.SetActive(false);

        if (pasoActual.panelPrincipal != null)
            pasoActual.panelPrincipal.SetActive(true);

        // 5. BLOQUEO POR RETO
        if (pasoActual.esRetoInteractivos)
        {
            botonContinuar.gameObject.SetActive(false);
            if (pasoActual.objetoDelReto != null) pasoActual.objetoDelReto.SetActive(true);
        }
        else
        {
            botonContinuar.gameObject.SetActive(true);
        }

        // 6. ACTIVACIÓN / DESACTIVACIÓN DE OBJETOS
        foreach (GameObject obj in pasoActual.activarAlEntrar) if (obj != null) obj.SetActive(true);
        foreach (GameObject obj in pasoActual.desactivarAlEntrar) if (obj != null) obj.SetActive(false);

        // 7. TEXTO Y AUDIO (Con protección contra nulos)
        if (pasoActual.textoUI != null) pasoActual.textoUI.text = pasoActual.mensajeTexto;

        if (pasoActual.audioPaso != null && parlanteVoces != null)
        {
            parlanteVoces.clip = pasoActual.audioPaso;
            parlanteVoces.Play();
        }
    }

    public void HabilitarBotonContinuar()
    {
        if (botonContinuar != null) botonContinuar.gameObject.SetActive(true);
    }

    private void CargarSiguienteEscena(string nombre)
    {
        if (!string.IsNullOrEmpty(nombre))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nombre);
        }
    }
}