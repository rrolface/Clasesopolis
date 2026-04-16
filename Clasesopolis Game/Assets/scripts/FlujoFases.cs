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
        public GameObject panelPrincipal;
        public TextMeshProUGUI textoUI;

        [Header("Configuración de Retos")]
        public bool esRetoInteractivos;
        public GameObject objetoDelReto;

        [Header("Configuración de Salida")]
        public bool esPasoFinal;
        public string escenaDestino;

        [Header("Número de fase para desbloqueo (solo en paso final)")]
        public int numeroFase = 1;

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
        if (GlobalSession.IsAuthenticated() && pasos.Count > 0)
        {
            pasos[0].mensajeTexto = pasos[0].mensajeTexto.Replace("{user}", GlobalSession.user.userName);
        }

        foreach (var p in pasos)
        {
            if (p.panelPrincipal != null) p.panelPrincipal.SetActive(false);
        }

        AvanzarPaso();
    }

    public void AvanzarPaso()
    {
        // 1. Chequeo del paso final
        if (indiceActual >= 0 && indiceActual < pasos.Count)
        {
            PasoDeFase pasoQueTermina = pasos[indiceActual];
            if (pasoQueTermina.esPasoFinal)
            {
                // FIX: le pasamos el número de fase para que desbloquee la siguiente
                ProgresoGlobal.RegistrarFinDeFase(pasoQueTermina.numeroFase);
                CargarSiguienteEscena(pasoQueTermina.escenaDestino);
                return;
            }
        }

        // 2. Limpieza
        if (parlanteVoces != null) parlanteVoces.Stop();
        GameObject panelAnterior = (indiceActual >= 0) ? pasos[indiceActual].panelPrincipal : null;

        indiceActual++;

        // 3. Límite
        if (indiceActual >= pasos.Count)
        {
            Debug.Log("Se terminaron los pasos de esta fase.");
            return;
        }

        PasoDeFase pasoActual = pasos[indiceActual];

        // 4. Paneles
        if (panelAnterior != null && panelAnterior != pasoActual.panelPrincipal)
            panelAnterior.SetActive(false);

        if (pasoActual.panelPrincipal != null)
            pasoActual.panelPrincipal.SetActive(true);

        // 5. Bloqueo por reto
        if (pasoActual.esRetoInteractivos)
        {
            botonContinuar.gameObject.SetActive(false);
            if (pasoActual.objetoDelReto != null) pasoActual.objetoDelReto.SetActive(true);
        }
        else
        {
            botonContinuar.gameObject.SetActive(true);
        }

        // 6. Activación / desactivación
        foreach (GameObject obj in pasoActual.activarAlEntrar) if (obj != null) obj.SetActive(true);
        foreach (GameObject obj in pasoActual.desactivarAlEntrar) if (obj != null) obj.SetActive(false);

        // 7. Texto y audio
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