using UnityEngine;
using TMPro;

public class PanelTrofeos : MonoBehaviour
{
    public TextMeshProUGUI textoInsignias;

    void OnEnable()
    {
        ActualizarInsignias();
    }

    public void ActualizarInsignias()
    {
        if (textoInsignias == null) return;

        if (ProgresoGlobal.Insignias == null || ProgresoGlobal.Insignias.Count == 0)
        {
            textoInsignias.text = "Aún no has obtenido insignias.";
            return;
        }

        string lista = "?? Insignias obtenidas:\n\n";
        foreach (string insignia in ProgresoGlobal.Insignias)
        {
            lista += "• " + insignia + "\n";
        }

        textoInsignias.text = lista;
    }
}