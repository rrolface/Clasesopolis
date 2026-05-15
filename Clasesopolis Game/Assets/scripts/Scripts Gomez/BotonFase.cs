using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Botón de selección de fase para la UI del modo construcción.
/// Lee el estado de desbloqueo desde FaseManager (estático, así también
/// funciona si el FaseManager Instance aún no se inicializó).
///
/// Se refresca cada vez que el GameObject se activa (OnEnable), por si el
/// panel de fases se muestra/oculta varias veces dentro de la misma escena.
/// </summary>
[RequireComponent(typeof(Button))]
public class BotonFase : MonoBehaviour
{
    [Header("Número de esta fase (1, 2, 3 o 4)")]
    public int numeroFase = 1;

    [Header("Imagen del candado (asignar desde el Inspector)")]
    public GameObject iconoCandado;

    private Button boton;
    private bool cableado = false;

    private void Awake()
    {
        boton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        // Cablear el listener UNA sola vez aunque OnEnable se llame muchas veces.
        if (!cableado && boton != null)
        {
            boton.onClick.AddListener(OnClick);
            cableado = true;
        }
        ActualizarEstado();
    }

    private void OnClick()
    {
        // Para entrar a la fase sí necesitamos la Instance (porque tiene los
        // índices de escena de cada fase en sus campos del Inspector).
        if (FaseManager.Instance != null)
        {
            FaseManager.Instance.EntrarFase(numeroFase);
        }
        else
        {
            Debug.LogWarning("[BotonFase] No hay FaseManager.Instance en la escena. " +
                             "Asegúrate de tener un GameObject con el componente FaseManager.");
        }
    }

    public void ActualizarEstado()
    {
        // Usa la versión estática: no depende de Instance, lee PlayerPrefs directo.
        bool desbloqueada = FaseManager.EstaDesbloqueada(numeroFase);

        if (boton != null)
            boton.interactable = desbloqueada;

        if (iconoCandado != null)
            iconoCandado.SetActive(!desbloqueada);
    }
}
