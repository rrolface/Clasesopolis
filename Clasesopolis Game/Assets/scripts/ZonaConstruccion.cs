using UnityEngine;
using UnityEngine.UI;

public class ZonaConstruccion : MonoBehaviour
{
    public GameObject panelUI;
    public Button botonConstruir;

    public GameObject edificioPrefab;
    public Transform puntoConstruccion;

    private bool jugadorDentro = false;
    private bool yaConstruido = false;

    public GameObject ZonaVisualPiso;

    void Start()
    {
        panelUI.SetActive(false);
        botonConstruir.onClick.AddListener(Construir);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaConstruido)
        {
            jugadorDentro = true;
            panelUI.SetActive(true);

            //desbloquear el mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            panelUI.SetActive(false);

            //bloquear el mouse
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Construir()
    {
        if (!yaConstruido)
        {

            Instantiate(edificioPrefab, puntoConstruccion.position, puntoConstruccion.rotation);
            yaConstruido = true;

            panelUI.SetActive(false);

            if (ZonaVisualPiso != null)
                ZonaVisualPiso.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}