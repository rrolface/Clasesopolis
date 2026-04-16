using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialIntroFlowController : MonoBehaviour
{
    [Header("Botón principal")]
    public Button continueButton;

    [Header("Inicio Byte")]
    public GameObject inicioByte;
    public Text byteIntroText;

    [Header("Presentación de fases")]
    public GameObject presentacionFases;
    public GameObject panelFase1;
    public GameObject panelFase2;
    public GameObject panelFase3;
    public GameObject panelFase4;

    [Header("Presentación final: Rachas, XP e Insignias")]
    public GameObject bytePresentationSystem;
    public Text bytePresentationDialogText;
    public GameObject panelRachas;
    public GameObject panelXP;
    public GameObject panelInsignias;

    [Header("Configuración")]
    public float initialDelay = 1f;

    private int currentStep = 0;

    private string[] introMessages =
    {
        "",
        "¡Bienvenid@ ve! Soy Inspector Byte, tu guía en esta ciudad que apenas está tomando forma: Clasesópolis.",
        "Aquí, nada existe hasta que alguien como tú lo imagine, lo modele… ¡y lo construya!",
        "La ciudad se levantará paso a paso a través de 4 fases de construcción, cada una con un tema clave que te acercará a comprender POO.",
        "Al finalizar cada fase, deberás completar un reto interactivo para continuar con la construcción de tu ciudad. No te quedes atrás oís.",
        "A medida que vas avanzando, te vas poder ganar insignias, sumarte tus puntos de experiencia y mantener tus rachas activas. ¡Entre más constante seas, más crecerá tu progreso!"
    };

    void Start()
    {
        // --- CAMBIO PARA PERSISTENCIA ---
        // Verificamos si hay un usuario en la sesión global para personalizar la bienvenida
        if (GlobalSession.IsAuthenticated())
        {
            string nombreUsuario = GlobalSession.user.userName;
            // Personalizamos el mensaje 0 con el nombre del usuario
            introMessages[0] = $"¡Bienvenid@ {nombreUsuario}! ya estas a un paso de comenzar";
        }
        // --------------------------------


        if (continueButton != null)
            continueButton.interactable = false;

        if (inicioByte != null)
            inicioByte.SetActive(true);

        if (presentacionFases != null)
            presentacionFases.SetActive(false);

        if (bytePresentationSystem != null)
            bytePresentationSystem.SetActive(false);

        if (panelFase1 != null) panelFase1.SetActive(false);
        if (panelFase2 != null) panelFase2.SetActive(false);
        if (panelFase3 != null) panelFase3.SetActive(false);
        if (panelFase4 != null) panelFase4.SetActive(false);

        if (panelRachas != null) panelRachas.SetActive(false);
        if (panelXP != null) panelXP.SetActive(false);
        if (panelInsignias != null) panelInsignias.SetActive(false);

        if (byteIntroText != null)
        {
            if (byteIntroText.transform.parent != null)
                byteIntroText.transform.parent.gameObject.SetActive(true);

            byteIntroText.text = "";
        }

        if (bytePresentationDialogText != null)
        {
            if (bytePresentationDialogText.transform.parent != null)
                bytePresentationDialogText.transform.parent.gameObject.SetActive(false);

            bytePresentationDialogText.text = "";
        }

        StartCoroutine(BeginIntro());
    }

    IEnumerator BeginIntro()
    {
        yield return new WaitForSeconds(initialDelay);

        if (byteIntroText != null)
            byteIntroText.text = introMessages[0];

        if (continueButton != null)
            continueButton.interactable = true;
    }

    public void NextStep()
    {
        currentStep++;

        // InicioByte: mensajes 2 al 5
        if (currentStep >= 1 && currentStep <= 4)
        {
            if (byteIntroText != null)
                byteIntroText.text = introMessages[currentStep];
        }

        // Mostrar presentación de fases
        else if (currentStep == 5)
        {
            if (inicioByte != null)
                inicioByte.SetActive(false);

            if (presentacionFases != null)
                presentacionFases.SetActive(true);

            if (panelFase1 != null) panelFase1.SetActive(false);
            if (panelFase2 != null) panelFase2.SetActive(false);
            if (panelFase3 != null) panelFase3.SetActive(false);
            if (panelFase4 != null) panelFase4.SetActive(false);
        }
        else if (currentStep == 6)
        {
            if (panelFase1 != null) panelFase1.SetActive(true);
            if (panelFase2 != null) panelFase2.SetActive(false);
            if (panelFase3 != null) panelFase3.SetActive(false);
            if (panelFase4 != null) panelFase4.SetActive(false);
        }
        else if (currentStep == 7)
        {
            if (panelFase1 != null) panelFase1.SetActive(false);
            if (panelFase2 != null) panelFase2.SetActive(true);
            if (panelFase3 != null) panelFase3.SetActive(false);
            if (panelFase4 != null) panelFase4.SetActive(false);
        }
        else if (currentStep == 8)
        {
            if (panelFase1 != null) panelFase1.SetActive(false);
            if (panelFase2 != null) panelFase2.SetActive(false);
            if (panelFase3 != null) panelFase3.SetActive(true);
            if (panelFase4 != null) panelFase4.SetActive(false);
        }
        else if (currentStep == 9)
        {
            if (panelFase1 != null) panelFase1.SetActive(false);
            if (panelFase2 != null) panelFase2.SetActive(false);
            if (panelFase3 != null) panelFase3.SetActive(false);
            if (panelFase4 != null) panelFase4.SetActive(true);
        }

        // Mostrar presentación final de gamificación
        else if (currentStep == 10)
        {
            if (presentacionFases != null)
                presentacionFases.SetActive(false);

            if (bytePresentationSystem != null)
                bytePresentationSystem.SetActive(true);

            if (panelRachas != null) panelRachas.SetActive(false);
            if (panelXP != null) panelXP.SetActive(false);
            if (panelInsignias != null) panelInsignias.SetActive(false);

            if (bytePresentationDialogText != null)
            {
                if (bytePresentationDialogText.transform.parent != null)
                    bytePresentationDialogText.transform.parent.gameObject.SetActive(true);

                bytePresentationDialogText.text = "Ahora te voy a mostrar las rachas, la XP y las insignias.";
            }
        }
        else if (currentStep == 11)
        {
            if (panelRachas != null) panelRachas.SetActive(true);
            if (panelXP != null) panelXP.SetActive(false);
            if (panelInsignias != null) panelInsignias.SetActive(false);

            if (bytePresentationDialogText != null)
                bytePresentationDialogText.text = "Estas son las rachas. Te muestran tu progreso continuo.";
        }
        else if (currentStep == 12)
        {
            if (panelRachas != null) panelRachas.SetActive(false);
            if (panelXP != null) panelXP.SetActive(true);
            if (panelInsignias != null) panelInsignias.SetActive(false);

            if (bytePresentationDialogText != null)
                bytePresentationDialogText.text = "Este es el XP. Te indica la experiencia que vas acumulando.";
        }
        else if (currentStep == 13)
        {
            if (panelRachas != null) panelRachas.SetActive(false);
            if (panelXP != null) panelXP.SetActive(false);
            if (panelInsignias != null) panelInsignias.SetActive(true);

            if (bytePresentationDialogText != null)
                bytePresentationDialogText.text = "Estas son las insignias. Representan logros que puedes desbloquear.";
        }
        else if (currentStep == 14)
        {
            if (bytePresentationSystem != null)
                bytePresentationSystem.SetActive(false);

            Debug.Log("Fin del flujo inicial del tutorial. Listo para continuar con lo siguiente.");
        }
    }
}