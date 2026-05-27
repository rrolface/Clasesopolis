using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Item individual del PanelSeleccionTitulos.
///
/// Estructura del prefab:
///   - GameObject raíz con Button (este componente lo requiere).
///   - Hijos sugeridos:
///       * Image (icono del listón/trofeo)
///       * TextMeshProUGUI (nombre del título)
///       * GameObject opcional 'marcoSeleccion' que se activa cuando este título
///         es el activo (un borde, brillo, glow, etc.).
///
/// El PanelSeleccionTitulos lo configura llamando a Configurar(...).
/// </summary>
[RequireComponent(typeof(Button))]
public class BotonTituloItem : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Imagen del listón/trofeo. Recibe el sprite compartido del panel.")]
    public Image icono;

    [Tooltip("Texto donde se mostrará el nombre del título.")]
    public TextMeshProUGUI nombreTexto;

    [Tooltip("GameObject opcional que se activa cuando este título es el activo. " +
             "Sirve como marco/borde de selección. Puede ser un Outline, un glow, etc.")]
    public GameObject marcoSeleccion;

    // Datos asignados al configurar.
    private string nombreInsignia;
    private PanelSeleccionTitulos panel;

    private Button boton;

    void Awake()
    {
        boton = GetComponent<Button>();
    }

    /// <summary>
    /// Llamado por PanelSeleccionTitulos justo después de instanciar el prefab.
    /// </summary>
    public void Configurar(string nombre, Sprite iconoSprite, PanelSeleccionTitulos panelDueño)
    {
        nombreInsignia = nombre;
        panel = panelDueño;

        if (nombreTexto != null) nombreTexto.text = nombre;

        if (icono != null)
        {
            if (iconoSprite != null)
            {
                icono.sprite = iconoSprite;
                icono.enabled = true;
            }
            else
            {
                // Si no se pasó icono, deja la imagen como esté en el prefab.
                // Útil si en el prefab ya hay un sprite por defecto.
            }
        }

        if (boton == null) boton = GetComponent<Button>();
        if (boton != null)
        {
            boton.onClick.RemoveAllListeners();
            boton.onClick.AddListener(OnClick);
        }

        ActualizarMarcoSeleccion();
    }

    /// <summary>
    /// Activa el marco de selección si este título coincide con el TituloActivo
    /// actualmente equipado en ProgresoGlobal.
    /// </summary>
    public void ActualizarMarcoSeleccion()
    {
        if (marcoSeleccion == null) return;

        bool esActivo = !string.IsNullOrEmpty(nombreInsignia)
                        && nombreInsignia == ProgresoGlobal.TituloActivo;
        marcoSeleccion.SetActive(esActivo);
    }

    private void OnClick()
    {
        if (panel != null)
        {
            panel.SeleccionarTitulo(nombreInsignia);
        }
    }
}
