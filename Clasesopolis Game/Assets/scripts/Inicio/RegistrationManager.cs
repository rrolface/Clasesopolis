using System.IO; // Para manejar archivos
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions; // Necesario para validar el email
using UnityEngine.SceneManagement;
public class RegistrationManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField nameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI statusText; // Para mensajes de error/éxito
    [Header("Success Panel UI")]
    public GameObject successPanel; // Arrastra el panel pequeño aquí
    public TextMeshProUGUI welcomeText; // El texto dentro del panel

    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "local_db.json");
        statusText.text = ""; // Limpiar mensaje al iniciar
    }

    public void RegisterUser()
    {
        Debug.Log("1. Botón presionado");

        // Vamos a ver qué contienen los inputs realmente
        Debug.Log($"Datos recibidos -> Nombre: '{nameInput.text}', Email: '{emailInput.text}', Pass: '{passwordInput.text}'");

        // 1. Validación de campos vacíos
        if (string.IsNullOrEmpty(nameInput.text) ||
            string.IsNullOrEmpty(emailInput.text) ||
            string.IsNullOrEmpty(passwordInput.text))
        {
            Debug.Log("Detenido en: Campos vacíos");
            ShowMessage("Todos los campos son obligatorios", Color.red);
            return;
        }

        // 2. Validación de formato de Email
        if (!IsValidEmail(emailInput.text))
        {
            Debug.Log("Detenido en: Formato de Email inválido");
            ShowMessage("El formato del correo no es válido", Color.red);
            return;
        }

        // 3. Validación de longitud de contraseña
        if (passwordInput.text.Length < 6)
        {
            Debug.Log("Detenido en: Contraseña corta");
            ShowMessage("La contraseña debe tener al menos 6 caracteres", Color.red);
            return;
        }

        Debug.Log("4. Pasó todas las validaciones. Guardando...");
        SaveUserLocal();
    }

    private void SaveUserLocal()
    {
        // Creamos un contenedor para la lista de usuarios
        UserListWrapper database = new UserListWrapper();

        // SI EL ARCHIVO YA EXISTE: Leemos lo que tiene para no borrar a los anteriores
        if (File.Exists(filePath))
        {
            string currentJson = File.ReadAllText(filePath);
            // Convertimos el texto del archivo en nuestra lista de usuarios
            database = JsonUtility.FromJson<UserListWrapper>(currentJson);
        }

        // Creamos el nuevo usuario con los datos de los inputs
        user newUser = new user
        {
            userName = nameInput.text,
            email = emailInput.text,
            password = passwordInput.text
        };

        // AÑADIMOS el nuevo usuario a la lista (sin borrar los que ya estaban)
        database.users.Add(newUser);

        // 1. Guardamos la LISTA COMPLETA en el disco duro
        string json = JsonUtility.ToJson(database, true);
        File.WriteAllText(filePath, json);

        // 2. Persistencia en RAM (para esta sesión)
        GlobalSession.user = newUser;

        // 3. UI: Mostramos el panel de éxito y el mensaje neón
        ShowSuccessUI(newUser.userName);
        ShowMessage("¡Registro completado!", Color.cyan);

        ClearFields();
    }

    // --- Funciones de Utilidad (Helpers) ---

    private bool IsValidEmail(string email)
    {
        // Expresión regular estándar para validar correos
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private void ShowMessage(string message, Color color)
    {
        statusText.text = message;
        statusText.color = color;
    }

    private void ClearFields()
    {
        nameInput.text = "";
        emailInput.text = "";
        passwordInput.text = "";
    }

    

    private void ShowSuccessUI(string name)
    {
        successPanel.SetActive(true); // Aparece el panel
        welcomeText.text = $"Te has registrado correctamente, {name}";

        // El botón "Continuar" del panel simplemente cerrará el panel 
        // o te llevará a la siguiente escena.
    }

    public void IrAlMundoVirtual()
    {
        // Cargamos la escena por su nombre exacto como aparece en Build Settings
        SceneManager.LoadScene("Tutorial");
    }

}
