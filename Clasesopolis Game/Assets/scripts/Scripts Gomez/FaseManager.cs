using UnityEngine;
using UnityEngine.SceneManagement;

public class FaseManager : MonoBehaviour
{
    public static FaseManager Instance;

    [Header("Índices de escena (confirmados en Build Settings)")]
    public int escenaFase1 = 4;
    public int escenaFase2 = 5;
    public int escenaFase3 = 6;
    public int escenaFase4 = -1;

    private void Awake()
    {
        Instance = this;
    }

    public void EntrarFase(int numeroFase)
    {
        if (!FaseDesbloqueada(numeroFase))
        {
            Debug.Log($"Fase {numeroFase} bloqueada.");
            return;
        }

        int indice = ObtenerIndice(numeroFase);
        if (indice < 0)
        {
            Debug.LogWarning($"Fase {numeroFase} no tiene escena asignada aún.");
            return;
        }

        SceneManager.LoadScene(indice);
    }

    private int ObtenerIndice(int numeroFase)
    {
        switch (numeroFase)
        {
            case 1: return escenaFase1;
            case 2: return escenaFase2;
            case 3: return escenaFase3;
            case 4: return escenaFase4;
            default: return -1;
        }
    }

    public bool FaseDesbloqueada(int numeroFase)
    {
        if (numeroFase == 1) return true;
        return PlayerPrefs.GetInt($"fase{numeroFase - 1}_completada", 0) == 1;
    }

    public void CompletarFase(int numeroFase)
    {
        PlayerPrefs.SetInt($"fase{numeroFase}_completada", 1);
        PlayerPrefs.Save();
        Debug.Log($"Fase {numeroFase} completada. Fase {numeroFase + 1} desbloqueada.");
    }

    [ContextMenu("Resetear Progreso de Fases")]
    public void ResetearProgreso()
    {
        PlayerPrefs.DeleteKey("fase1_completada");
        PlayerPrefs.DeleteKey("fase2_completada");
        PlayerPrefs.DeleteKey("fase3_completada");
        PlayerPrefs.DeleteKey("fase4_completada");
        PlayerPrefs.Save();
        Debug.Log("Progreso reseteado.");
    }
}