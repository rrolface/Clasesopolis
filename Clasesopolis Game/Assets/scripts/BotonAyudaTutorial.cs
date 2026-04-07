using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonAyudaTutorial : MonoBehaviour
{
    public void IrATemaTutorial(string nombreDelPaso)
    {
        // 1. Le decimos al puente qué tema queremos ver
        TutorialDataBridge.ConfigurarSaltoPorNombre(nombreDelPaso);

        // 2. Cargamos la escena del tutorial
        SceneManager.LoadScene("Tutorial");
    }
}