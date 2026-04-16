using UnityEngine;
using TMPro;
using Firebase.Auth;

public class MostrarCorreoUsuario : MonoBehaviour
{
    public TMP_Text textoCorreo;

    void Start()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            string correo = auth.CurrentUser.Email;
            textoCorreo.text = "Correo: " + correo;
        }
        else
        {
            textoCorreo.text = "No hay usuario logueado";
        }
    }
}
