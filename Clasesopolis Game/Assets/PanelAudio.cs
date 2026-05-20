using UnityEngine;

public class PanelAudio : MonoBehaviour
{
    public AudioClip audioDelPanel;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = FindFirstObjectByType<AudioSource>();
    }

    private void OnEnable()
    {
        if (audioSource != null && audioDelPanel != null)
        {
            audioSource.Stop();

            audioSource.clip = audioDelPanel;
            audioSource.Play();
        }
    }
}