using UnityEngine;
using TMPro;

public class PuntuacionesConstruccionUI : MonoBehaviour
{
    public TextMeshProUGUI textoXP, textoRacha;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        textoXP.text = ProgresoGlobal.XP.ToString();
        textoRacha.text = ProgresoGlobal.RachaDias.ToString();
    }
}
