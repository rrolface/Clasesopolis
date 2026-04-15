using UnityEngine;
using TMPro;

public class MostrarXP : MonoBehaviour
{
    [Header("Referencia al texto UI")]
    public TextMeshProUGUI textoXP; // ?? Lo arrastras desde Unity

    void OnEnable()
    {
        ActualizarTexto();
    }

    public void ActualizarTexto()
    {
        if (textoXP != null)
        {
            textoXP.text = "Total XP: " + ProgresoGlobal.XP.ToString();
        }
    }
}