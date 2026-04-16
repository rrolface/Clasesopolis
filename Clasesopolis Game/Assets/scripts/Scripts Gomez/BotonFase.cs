using UnityEngine;
using UnityEngine.UI;

public class BotonFase : MonoBehaviour
{
    [Header("Número de esta fase (1, 2, 3 o 4)")]
    public int numeroFase = 1;

    [Header("Imagen del candado (asignar desde el Inspector)")]
    public GameObject iconoCandado;

    private Button boton;

    private void Start()
    {
        boton = GetComponent<Button>();
        ActualizarEstado();

        // Conectar el click al FaseManager
        boton.onClick.AddListener(() => FaseManager.Instance.EntrarFase(numeroFase));
    }

    public void ActualizarEstado()
    {
        bool desbloqueada = FaseManager.Instance.FaseDesbloqueada(numeroFase);

        // Activar o desactivar interacción
        boton.interactable = desbloqueada;

        // Mostrar/ocultar candado
        if (iconoCandado != null)
            iconoCandado.SetActive(!desbloqueada);
    }
}