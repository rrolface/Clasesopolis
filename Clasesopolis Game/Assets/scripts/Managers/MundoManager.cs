using UnityEngine;
using TMPro;

public class MundoManager : MonoBehaviour
{
    public TextMeshProUGUI saludoText;

    void Start()
    {
        // Aquí ocurre la magia de la persistencia:
        // Accedemos a la clase estática sin importar que cambiamos de escena
        if (GlobalSession.IsAuthenticated())
        {
            string nombre = GlobalSession.user.userName;
            saludoText.text = $"{nombre}";
            Debug.Log("Persistencia exitosa. Usuario actual: " + nombre);
        }
        else
        {
            // Si alguien intenta entrar a esta escena sin loguearse
            saludoText.text = "Error: Sesión no iniciada.";
        }
    }
}