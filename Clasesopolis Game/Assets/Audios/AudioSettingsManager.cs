using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Controla los tres canales del AudioMixer (Música, Voces, SFX) desde el
/// menú de pausa: sliders de volumen y botones de muteo individual.
///
/// El mute funciona por encima del slider:
///   - Cuando un canal está MUTEADO el mixer queda a -80 dB pase lo que pase
///     con el slider.
///   - Al desmutear, vuelve al valor que tenga el slider en ese momento.
///
/// Persistencia (PlayerPrefs):
///   - Volúmenes: MusicVol, VoiceVol, SFXVol  (float 0..1)
///   - Mutes:     MusicMute, VoiceMute, SFXMute  (int 0/1)
/// </summary>
public class AudioSettingsManager : MonoBehaviour
{
    // ============================================================
    //  REFERENCIAS DEL MIXER
    // ============================================================
    [Header("Mixer")]
    public AudioMixer mainMixer;

    [Tooltip("Nombres EXACTOS de los parámetros expuestos en el AudioMixer.")]
    public string parametroMusica = "musicVol";
    public string parametroVoces = "voiceVol";
    public string parametroSFX = "sfxVol";

    // ============================================================
    //  SLIDERS DE VOLUMEN
    // ============================================================
    [Header("Sliders")]
    public Slider sliderMusica;
    public Slider sliderVoces;
    public Slider sliderSFX;

    // ============================================================
    //  BOTONES DE MUTE
    // ============================================================
    [Header("Botones de mute (opcionales)")]
    [Tooltip("Si está activo, conecto los botones por código a sus métodos. " +
             "Si lo dejas en false, debes cablear OnClick → ToggleMutearXX desde el Inspector.")]
    public bool autoConectarBotones = true;

    public Button botonMuteMusica;
    public Button botonMuteVoces;
    public Button botonMuteSFX;

    // ============================================================
    //  ICONOS / SPRITES (placeholder hasta que existan los diseños)
    // ============================================================
    [Header("Iconos de estado (opcionales)")]
    [Tooltip("Image que cambia su sprite según el estado activo/muteado. " +
             "Puede ser el propio Image del Button, o un hijo dentro del botón.")]
    public Image iconoMuteMusica;
    public Image iconoMuteVoces;
    public Image iconoMuteSFX;

    [Header("Sprites — Música")]
    public Sprite spriteMusicaActiva;
    public Sprite spriteMusicaMuteada;

    [Header("Sprites — Voces")]
    public Sprite spriteVocesActivas;
    public Sprite spriteVocesMuteadas;

    [Header("Sprites — SFX")]
    public Sprite spriteSFXActivos;
    public Sprite spriteSFXMuteados;

    // ============================================================
    //  ESTADO INTERNO
    // ============================================================
    private bool musicaMuteada;
    private bool vocesMuteadas;
    private bool sfxMuteados;

    // dB que se aplica al mixer cuando un canal está muteado.
    private const float DB_SILENCIO = -80f;

    // ============================================================
    //  CICLO DE VIDA
    // ============================================================
    void Start()
    {
        // 1) Cargar volúmenes guardados (default 0.75)
        if (sliderMusica != null) sliderMusica.value = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        if (sliderVoces != null) sliderVoces.value = PlayerPrefs.GetFloat("VoiceVol", 0.75f);
        if (sliderSFX != null) sliderSFX.value = PlayerPrefs.GetFloat("SFXVol", 0.75f);

        // 2) Cargar estado de mute guardado
        musicaMuteada = PlayerPrefs.GetInt("MusicMute", 0) == 1;
        vocesMuteadas = PlayerPrefs.GetInt("VoiceMute", 0) == 1;
        sfxMuteados = PlayerPrefs.GetInt("SFXMute", 0) == 1;

        // 3) Aplicar todo al mixer
        AplicarVolumenMusica();
        AplicarVolumenVoces();
        AplicarVolumenSFX();

        // 4) Conectar botones por código si se pidió
        if (autoConectarBotones)
        {
            if (botonMuteMusica != null)
            {
                botonMuteMusica.onClick.RemoveListener(ToggleMutearMusica);
                botonMuteMusica.onClick.AddListener(ToggleMutearMusica);
            }
            if (botonMuteVoces != null)
            {
                botonMuteVoces.onClick.RemoveListener(ToggleMutearVoces);
                botonMuteVoces.onClick.AddListener(ToggleMutearVoces);
            }
            if (botonMuteSFX != null)
            {
                botonMuteSFX.onClick.RemoveListener(ToggleMutearSFX);
                botonMuteSFX.onClick.AddListener(ToggleMutearSFX);
            }
        }

        // 5) Refrescar iconos
        ActualizarIconoMusica();
        ActualizarIconoVoces();
        ActualizarIconoSFX();
    }

    // ============================================================
    //  SLIDERS — métodos públicos para wirear desde el Inspector
    // ============================================================
    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVol", value);
        AplicarVolumenMusica();
    }

    public void SetVoiceVolume(float value)
    {
        PlayerPrefs.SetFloat("VoiceVol", value);
        AplicarVolumenVoces();
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVol", value);
        AplicarVolumenSFX();
    }

    // ============================================================
    //  MUTE TOGGLES — métodos públicos para wirear desde el Inspector
    // ============================================================
    public void ToggleMutearMusica()
    {
        musicaMuteada = !musicaMuteada;
        PlayerPrefs.SetInt("MusicMute", musicaMuteada ? 1 : 0);
        AplicarVolumenMusica();
        ActualizarIconoMusica();
    }

    public void ToggleMutearVoces()
    {
        vocesMuteadas = !vocesMuteadas;
        PlayerPrefs.SetInt("VoiceMute", vocesMuteadas ? 1 : 0);
        AplicarVolumenVoces();
        ActualizarIconoVoces();
    }

    public void ToggleMutearSFX()
    {
        sfxMuteados = !sfxMuteados;
        PlayerPrefs.SetInt("SFXMute", sfxMuteados ? 1 : 0);
        AplicarVolumenSFX();
        ActualizarIconoSFX();
    }

    /// <summary>
    /// Lectura pública del estado actual (útil si algo más quiere consultarlo).
    /// </summary>
    public bool MusicaEstaMuteada => musicaMuteada;
    public bool VocesEstanMuteadas => vocesMuteadas;
    public bool SFXEstanMuteados => sfxMuteados;

    // ============================================================
    //  APLICACIÓN AL MIXER
    // ============================================================
    private void AplicarVolumenMusica()
    {
        float val = sliderMusica != null ? sliderMusica.value : 0.75f;
        AplicarAlMixer(parametroMusica, val, musicaMuteada);
    }

    private void AplicarVolumenVoces()
    {
        float val = sliderVoces != null ? sliderVoces.value : 0.75f;
        AplicarAlMixer(parametroVoces, val, vocesMuteadas);
    }

    private void AplicarVolumenSFX()
    {
        float val = sliderSFX != null ? sliderSFX.value : 0.75f;
        AplicarAlMixer(parametroSFX, val, sfxMuteados);
    }

    private void AplicarAlMixer(string parametro, float valorSlider, bool muteado)
    {
        if (mainMixer == null || string.IsNullOrEmpty(parametro)) return;

        float db;
        if (muteado)
        {
            db = DB_SILENCIO;
        }
        else
        {
            // Evita Log10(0) = -infinito
            float v = Mathf.Max(valorSlider, 0.0001f);
            db = Mathf.Log10(v) * 20f;
        }

        mainMixer.SetFloat(parametro, db);
    }

    // ============================================================
    //  ACTUALIZACIÓN VISUAL
    // ============================================================
    private void ActualizarIconoMusica()
    {
        SetSprite(iconoMuteMusica,
                  musicaMuteada ? spriteMusicaMuteada : spriteMusicaActiva);
    }

    private void ActualizarIconoVoces()
    {
        SetSprite(iconoMuteVoces,
                  vocesMuteadas ? spriteVocesMuteadas : spriteVocesActivas);
    }

    private void ActualizarIconoSFX()
    {
        SetSprite(iconoMuteSFX,
                  sfxMuteados ? spriteSFXMuteados : spriteSFXActivos);
    }

    /// <summary>
    /// Cambia el sprite de una Image solo si el sprite asignado no es null.
    /// Así, mientras no haya diseños, el Image se queda con su sprite actual y
    /// la lógica de audio sigue funcionando.
    /// </summary>
    private void SetSprite(Image img, Sprite nuevo)
    {
        if (img == null) return;
        if (nuevo != null) img.sprite = nuevo;
    }
}
