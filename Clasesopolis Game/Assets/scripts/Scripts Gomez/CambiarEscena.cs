using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    public string nombreEscena;

    public void Ir()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }
}