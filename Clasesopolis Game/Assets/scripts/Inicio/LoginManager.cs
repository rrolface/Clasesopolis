using UnityEngine;
using TMPro;
using System.IO;
using System.Linq; // Para buscar fácil en la lista

public class LoginManager : MonoBehaviour
{
    [Header("UI Inputs")]
    public TMP_InputField loginUserInput;
    public TMP_InputField loginPasswordInput;

    [Header("Feedback Panels")]
    public GameObject loginSuccessPanel;
    public TextMeshProUGUI welcomeText;
    public TextMeshProUGUI errorText; // Texto para errores

    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "local_db.json");
    }

    public void AttemptLogin()
    {
        string inputUser = loginUserInput.text;
        string inputPass = loginPasswordInput.text;

        if (!File.Exists(filePath))
        {
            errorText.text = "No hay usuarios registrados en el sistema.";
            return;
        }

        // Leer la "base de datos" local
        string json = File.ReadAllText(filePath);
        UserListWrapper database = JsonUtility.FromJson<UserListWrapper>(json);

        // 1. Buscar si el usuario existe
        user foundUser = database.users.Find(u => u.userName == inputUser);

        if (foundUser == null)
        {
            errorText.text = "El usuario no es correcto o no está registrado.";
            return;
        }

        // 2. Si existe, comparar la contraseña
        if (foundUser.password != inputPass)
        {
            errorText.text = "Contraseña incorrecta. Intente nuevamente.";
            return;
        }

        // 3. ÉXITO: Establecer persistencia y mostrar saludo
        Success(foundUser);
    }

    private void Success(user user)
    {
        errorText.text = ""; // Limpiar errores

        // --- AQUÍ OCURRE LA PERSISTENCIA ---
        GlobalSession.user = user;

        welcomeText.text = $"Bienvenido de nuevo, {user.userName}";
        loginSuccessPanel.SetActive(true);

        Debug.Log($"Sesión iniciada: {GlobalSession.user.userName}");
    }
}