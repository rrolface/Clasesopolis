using UnityEngine;
using UnityEngine.SceneManagement;

public class ModoJuegoUIController : MonoBehaviour
{
    public GameObject panelFases;
    public string nombreEscenaConstruccion = "Construccion";

    public void AbrirPanelFases()
    {
        if (panelFases != null)
        {
            panelFases.SetActive(true);
        }
    }

    public void CerrarPanelFases()
    {
        if (panelFases != null)
        {
            panelFases.SetActive(false);
        }
    }

    public void IrAModoLibre()
    {
        SceneManager.LoadScene(nombreEscenaConstruccion);
    }
}