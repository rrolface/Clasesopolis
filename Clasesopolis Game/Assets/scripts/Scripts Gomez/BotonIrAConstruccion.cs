using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonIrAConstruccion : MonoBehaviour
{
    public string nombreEscena = "Construccion";

    public void IrAConstruccion()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }
}