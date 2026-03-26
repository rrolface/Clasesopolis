using UnityEngine;
using UnityEngine.UI;

public class BytePresentationController : MonoBehaviour
{
    public Text dialogText;
    public GameObject panelRachas;
    public GameObject panelXP;
    public GameObject panelInsignias;
    public Button continueButton;

    private int currentStep = 0;

    void Start()
    {
        panelRachas.SetActive(false);
        panelXP.SetActive(false);
        panelInsignias.SetActive(false);

        dialogText.text = "Hola, te voy a mostrar las rachas, la XP y las insignias.";
    }

    public void NextStep()
    {
        currentStep++;

        if (currentStep == 1)
        {
            panelRachas.SetActive(true);
            panelXP.SetActive(false);
            panelInsignias.SetActive(false);
            dialogText.text = "Estas son las rachas. Te muestran tu progreso continuo.";
        }
        else if (currentStep == 2)
        {
            panelRachas.SetActive(false);
            panelXP.SetActive(true);
            panelInsignias.SetActive(false);
            dialogText.text = "Este es el XP. Te indica la experiencia que vas acumulando.";
        }
        else if (currentStep == 3)
        {
            panelRachas.SetActive(false);
            panelXP.SetActive(false);
            panelInsignias.SetActive(true);
            dialogText.text = "Estas son las insignias. Representan logros que puedes desbloquear.";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}