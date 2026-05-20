using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CiudadXP : MonoBehaviour
{
    public static CiudadXP instancia;

    [Header("XP")]
    public int xpActual = 0;
    public int xpMaxima = 100;

    [Header("UI")]
    public Slider barraXP;
    public TextMeshProUGUI textoXP;

    private void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        ActualizarUI();
    }

    public void AgregarXP(int cantidad)
    {
        xpActual += cantidad;

        // Evitar que pase del máximo
        if (xpActual > xpMaxima)
            xpActual = xpMaxima;

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (barraXP != null)
        {
            barraXP.maxValue = xpMaxima;
            barraXP.value = xpActual;
        }

        if (textoXP != null)
        {
            textoXP.text = xpActual + " / " + xpMaxima + " XP";
        }
    }
}