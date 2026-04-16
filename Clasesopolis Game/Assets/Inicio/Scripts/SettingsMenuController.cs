using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Referencias de Paneles")]
    public GameObject panelAjustes;
    public GameObject panelAyuda;

    [Header("Referencias de Audio")]
    public AudioManager audioManager;

    [Header("UI: Controles de Volumen")]
    public Slider sliderMusica;
    public Slider sliderSfx;

    [Header("UI: Botones (Componente Image)")]
    public Image iconoBotonMusica;
    public Image iconoBotonSfx;

    [Header("Sprites (Imágenes On/Off)")]
    public Sprite spriteMusicaOn;
    public Sprite spriteMusicaOff;
    public Sprite spriteSfxOn;
    public Sprite spriteSfxOff;

    // Variables internas para saber si está muteado o no
    private bool musicaMuteada = false;
    private bool sfxMuteado = false;

    void Start()
    {
        // 1. Apaga los paneles al inicio
        panelAjustes.SetActive(false);
        if (panelAyuda != null) panelAyuda.SetActive(false);

        // 2. Conecta los sliders por código para que actualicen el volumen en tiempo real
        if (sliderMusica != null) sliderMusica.onValueChanged.AddListener(ActualizarVolumenMusica);
        if (sliderSfx != null) sliderSfx.onValueChanged.AddListener(ActualizarVolumenSfx);

        // 3. Cargar el volumen. Si es la primera vez que se juega, por defecto será 0.5f (mitad).
        if (sliderMusica != null)
        {
            sliderMusica.value = PlayerPrefs.GetFloat("VolMusica", 0.5f);
            ActualizarVolumenMusica(sliderMusica.value); // Sincroniza el AudioSource real
        }

        if (sliderSfx != null)
        {
            sliderSfx.value = PlayerPrefs.GetFloat("VolSfx", 0.5f);
            ActualizarVolumenSfx(sliderSfx.value);
        }

        // 4. Carga el estado del botón Mute (Silencio)
        musicaMuteada = PlayerPrefs.GetInt("MusicaMute", 0) == 1; // 1 es true, 0 es false
        sfxMuteado = PlayerPrefs.GetInt("SfxMute", 0) == 1;

        // Actualiza los íconos visuales y el estado físico del audio
        if (iconoBotonMusica != null) iconoBotonMusica.sprite = musicaMuteada ? spriteMusicaOff : spriteMusicaOn;
        if (audioManager != null && audioManager.musicSource != null) audioManager.musicSource.mute = musicaMuteada;

        if (iconoBotonSfx != null) iconoBotonSfx.sprite = sfxMuteado ? spriteSfxOff : spriteSfxOn;
        if (audioManager != null && audioManager.sfxSource != null) audioManager.sfxSource.mute = sfxMuteado;
    }

    // --- FUNCIONES PARA ABRIR/CERRAR PANELES ---
    public void AbrirPanelAjustes() { panelAjustes.SetActive(true); }
    public void CerrarPanelAjustes() { panelAjustes.SetActive(false); }

    public void AbrirPanelAyuda() { panelAyuda.SetActive(true); }
    public void CerrarPanelAyuda() { panelAyuda.SetActive(false); }

    // --- FUNCIONES DE MUTE (Cambio de Íconos) ---
    public void ToggleMusica()
    {
        musicaMuteada = !musicaMuteada; // Invierte el estado
        iconoBotonMusica.sprite = musicaMuteada ? spriteMusicaOff : spriteMusicaOn;
        if (audioManager != null && audioManager.musicSource != null)
            audioManager.musicSource.mute = musicaMuteada;
    }

    public void ToggleSfx()
    {
        sfxMuteado = !sfxMuteado; // Invierte el estado
        iconoBotonSfx.sprite = sfxMuteado ? spriteSfxOff : spriteSfxOn;
        if (audioManager != null && audioManager.sfxSource != null)
            audioManager.sfxSource.mute = sfxMuteado;
    }

    // --- FUNCIONES DE VOLUMEN (Sliders) ---
    public void ActualizarVolumenMusica(float volumen)
    {
        if (audioManager != null && audioManager.musicSource != null)
            audioManager.musicSource.volume = volumen;
    }

    public void ActualizarVolumenSfx(float volumen)
    {
        if (audioManager != null && audioManager.sfxSource != null)
            audioManager.sfxSource.volume = volumen;
    }

    // --- FUNCIÓN APLICAR ---
    public void AplicarAjustes()
    {
        // Guarda la configuración usando PlayerPrefs (se guarda aunque se cierre el juego)
        PlayerPrefs.SetFloat("VolMusica", sliderMusica.value);
        PlayerPrefs.SetFloat("VolSfx", sliderSfx.value);
        PlayerPrefs.SetInt("MusicaMute", musicaMuteada ? 1 : 0);
        PlayerPrefs.SetInt("SfxMute", sfxMuteado ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Ajustes Aplicados y Guardados.");
        CerrarPanelAjustes();
    }
}
