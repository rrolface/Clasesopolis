using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CiudadXP : MonoBehaviour
{
    public static CiudadXP instancia;

    [Header("UI")]
    public Slider barraXP;
    public TextMeshProUGUI textoXP;

    private void Awake()
    {
        instancia = this;
    }

    void Start()
    {
       
    }

}