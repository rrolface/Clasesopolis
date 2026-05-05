using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Componente que se pega a un prefab de botón del inventario. Cada botón
/// representa una construcción del inventario del jugador y, al hacer click,
/// la marca como seleccionada en SeleccionConstruccion.
///
/// Estructura mínima esperada del prefab:
///   - Button (componente)
///   - Image hijo para el icono (asignar al campo 'icono')
///   - TMP_Text hijo para el nombre (asignar al campo 'nombre')
/// </summary>
[RequireComponent(typeof(Button))]
public class BotonConstruccionItem : MonoBehaviour
{
    [Header("UI")]
    public Image icono;
    public TextMeshProUGUI nombre;

    [Header("Resaltado de selección (opcional)")]
    [Tooltip("Si lo asignas, se activa cuando esta es la construcción seleccionada.")]
    public GameObject marcoSeleccionado;

    private Construccion data;
    private PanelInventarioConstrucciones panel;

    public Construccion Datos => data;

    public void Configurar(Construccion construccion, PanelInventarioConstrucciones panelPadre)
    {
        data = construccion;
        panel = panelPadre;

        if (icono != null)
        {
            icono.sprite = construccion.icono;
            icono.enabled = construccion.icono != null;
        }
        if (nombre != null)
        {
            nombre.text = construccion.nombre;
        }

        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);

        ActualizarMarcoSeleccion();
    }

    private void OnClick()
    {
        if (data == null) return;
        SeleccionConstruccion.Actual = data;
        if (panel != null) panel.NotificarSeleccion(data);
    }

    /// <summary>
    /// Lo llama el panel cuando cambia la selección global, para que cada botón
    /// muestre/oculte su marco "Seleccionado".
    /// </summary>
    public void ActualizarMarcoSeleccion()
    {
        if (marcoSeleccionado == null) return;
        bool soyElSeleccionado = data != null
            && SeleccionConstruccion.Actual != null
            && SeleccionConstruccion.Actual.id == data.id;
        marcoSeleccionado.SetActive(soyElSeleccionado);
    }
}
