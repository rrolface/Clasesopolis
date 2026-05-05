using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Pega este script al panel de resumen de la fase. Cuando se activa:
///   1) (Opcional) otorga insignia y aumenta racha.
///   2) (Opcional) otorga una construcción aleatoria del catálogo.
///   3) Rellena los textos / iconos con XP, racha, insignia y construcción.
/// </summary>
public class PanelResumenFase : MonoBehaviour
{
    // ============================================================
    //  TEXTOS UI
    // ============================================================
    [Header("Textos UI (TextMeshPro)")]
    public TextMeshProUGUI textoXPGanada;     // Ej: "+50 XP"
    public TextMeshProUGUI textoXPTotal;      // Ej: "Total: 250 XP"
    public TextMeshProUGUI textoRacha;        // Ej: "Racha: 3"
    public TextMeshProUGUI textoInsignia;     // Ej: "¡Nueva insignia: Constructor Inicial!"
    public TextMeshProUGUI textoConstruccion; // Ej: "¡Ganaste: Casa básica!"

    // ============================================================
    //  ICONOS / PREVIEWS
    // ============================================================
    [Header("Iconos")]
    public Image iconoInsignia;
    public Sprite spriteInsignia;
    public Image iconoConstruccion; // se rellena con la construcción otorgada

    [Header("Preview 3D opcional de la construcción")]
    [Tooltip("Si lo asignas, se instancia el prefab de la construcción ganada como hijo de este Transform " +
             "para mostrar una vista previa real (rotada/escalada como hayas configurado el padre).")]
    public Transform slotPreviewConstruccion;
    [Tooltip("Escala uniforme aplicada al preview instanciado.")]
    public float escalaPreview = 1f;

    private GameObject instanciaPreviewActual;

    // ============================================================
    //  AUTO-OTORGAR AL MOSTRARSE
    // ============================================================
    [Header("Insignia y racha (auto-otorgar opcional)")]
    [Tooltip("Nombre de la insignia que se otorga al activarse este panel. " +
             "Déjalo vacío si ya la otorgas con un TriggerInsignia aparte.")]
    public string insigniaAOtorgar = "";

    [Tooltip("Si está activo, suma +1 a la racha al aparecer este panel. " +
             "Desactívalo si ya usas un TriggerRacha aparte para evitar contar dos veces.")]
    public bool incrementarRachaAlMostrar = true;

    [Header("Construcción (auto-otorgar opcional)")]
    [Tooltip("Catálogo central de construcciones del juego. Asignar el ScriptableObject " +
             "creado vía Assets > Create > Clasesopolis > Catálogo de Construcciones.")]
    public CatalogoConstrucciones catalogoConstrucciones;

    [Tooltip("Si está activo, otorga una construcción aleatoria del catálogo al aparecer.")]
    public bool otorgarConstruccionAlMostrar = true;

    [Tooltip("Si está activo, puede repetir construcciones que el jugador ya tenga. " +
             "Si está desactivado, primero intenta darle una nueva.")]
    public bool permitirConstruccionRepetida = false;

    // ============================================================
    //  PLANTILLAS DE TEXTO
    // ============================================================
    [Header("Plantillas de texto (con marcadores {0})")]
    public string formatoXPGanada = "+{0} XP";
    public string formatoXPTotal = "Total: {0} XP";
    public string formatoRacha = "Racha: {0}";
    public string formatoInsigniaNueva = "¡Nueva insignia: {0}!";
    public string formatoInsigniaRepetida = "Insignia: {0}";
    public string formatoConstruccion = "¡Ganaste: {0}!";

    // ============================================================
    //  CICLO DE VIDA
    // ============================================================
    void OnEnable()
    {
        // 1) Asignamos el catálogo al inventario (lo necesitan otorgar y consultas posteriores).
        if (catalogoConstrucciones != null)
            InventarioConstrucciones.Catalogo = catalogoConstrucciones;

        // 2) Otorgamos premios primero, así los textos leen el estado más reciente.
        if (!string.IsNullOrEmpty(insigniaAOtorgar))
            ProgresoGlobal.GanarInsignia(insigniaAOtorgar);

        if (incrementarRachaAlMostrar)
            ProgresoGlobal.IncrementarRacha();

        if (otorgarConstruccionAlMostrar)
            InventarioConstrucciones.OtorgarAleatoria(permitirConstruccionRepetida);

        // 3) Refrescamos textos e iconos.
        ActualizarTextos();
    }

    void OnDisable()
    {
        // Limpiamos el preview instanciado al cerrarse el panel para no dejar basura.
        if (instanciaPreviewActual != null)
        {
            Destroy(instanciaPreviewActual);
            instanciaPreviewActual = null;
        }
    }

    /// <summary>
    /// Refresca manualmente los textos e iconos.
    /// </summary>
    public void ActualizarTextos()
    {
        // XP
        if (textoXPGanada != null)
            textoXPGanada.text = string.Format(formatoXPGanada, ProgresoGlobal.UltimaXPGanada);

        if (textoXPTotal != null)
            textoXPTotal.text = string.Format(formatoXPTotal, ProgresoGlobal.XP);

        // Racha
        if (textoRacha != null)
            textoRacha.text = string.Format(formatoRacha, ProgresoGlobal.RachaDias);

        // Insignia
        if (textoInsignia != null)
        {
            if (string.IsNullOrEmpty(ProgresoGlobal.UltimaInsignia))
            {
                textoInsignia.text = "";
            }
            else
            {
                string plantilla = ProgresoGlobal.UltimaInsigniaEsNueva
                    ? formatoInsigniaNueva
                    : formatoInsigniaRepetida;
                textoInsignia.text = string.Format(plantilla, ProgresoGlobal.UltimaInsignia);
            }
        }

        if (iconoInsignia != null && spriteInsignia != null)
        {
            iconoInsignia.sprite = spriteInsignia;
            iconoInsignia.enabled = true;
        }

        // Construcción
        Construccion ganada = InventarioConstrucciones.UltimaOtorgada;
        if (textoConstruccion != null)
        {
            textoConstruccion.text = (ganada != null && !string.IsNullOrEmpty(ganada.nombre))
                ? string.Format(formatoConstruccion, ganada.nombre)
                : "";
        }

        if (iconoConstruccion != null)
        {
            if (ganada != null && ganada.icono != null)
            {
                iconoConstruccion.sprite = ganada.icono;
                iconoConstruccion.enabled = true;
            }
            else
            {
                iconoConstruccion.enabled = false;
            }
        }

        // Preview 3D opcional
        if (slotPreviewConstruccion != null)
        {
            if (instanciaPreviewActual != null)
            {
                Destroy(instanciaPreviewActual);
                instanciaPreviewActual = null;
            }

            if (ganada != null && ganada.prefab != null)
            {
                instanciaPreviewActual = Instantiate(ganada.prefab, slotPreviewConstruccion);
                instanciaPreviewActual.transform.localPosition = Vector3.zero;
                instanciaPreviewActual.transform.localRotation = Quaternion.identity;
                instanciaPreviewActual.transform.localScale = Vector3.one * escalaPreview;
            }
        }
    }
}
