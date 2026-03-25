using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource MusicaRecorrido;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); 
    }
}
