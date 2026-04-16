using UnityEngine;
using TMPro; 
using Firebase;
using Firebase.Auth;
using Firebase.Extensions; 
using System.Collections;
using System.Threading.Tasks;

public class AuthManager : MonoBehaviour
{
    // Variables Firebase
    private FirebaseAuth auth;
    private FirebaseUser user;

    // Variables Login
    [Header("UI: Inicio de Sesión")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;

    // Variables Registro
    [Header("UI: Registro")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    [Header("Navegación")]
    public MainMenuController mainMenuController; // Sistema de menús
    public GameObject panelBienvenidaPostLogin; // El panel que quieres mostrar
    public TMP_Text textoBienvenidaNombre;     // El texto: "Bienvenido, [Nombre]"
    public string nombreEscenaTutorial = "Tutorial"; // Nombre de tu escena de tutorial

    void Start()
    {
        // Limpia los textos de advertencia al inicio
        if (warningLoginText != null) warningLoginText.text = "";
        if (warningRegisterText != null) warningRegisterText.text = "";

        // Verifica dependencias e inicializa la Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase inicializado correctamente.");
            }
            else
            {
                Debug.LogError("No se pudieron resolver las dependencias de Firebase: " + dependencyStatus);
            }
        });
    }


    // Esta función se encarga de preparar el panel antes de mostrarlo
    private void MostrarPanelBienvenida(string nombreUsuario)
    {
        if (panelBienvenidaPostLogin != null)
        {
            // Personalizamos el mensaje
            if (textoBienvenidaNombre != null)
                textoBienvenidaNombre.text = $"<b>¡¡¡Bienvenido!!!</b> \n" +
                    $"{nombreUsuario} La experiencia está lista para comenzar." +
                    $"Vamos pues que Clasesopolis te espera";

            // Activamos el panel (asegúrate de que esté encima de todo en el Canvas)
            panelBienvenidaPostLogin.SetActive(true);
        }
    }

    // ESTA FUNCIÓN VA EN EL BOTÓN "COMENZAR" DEL PANEL DE BIENVENIDA
    public void BotonIrAlTutorial()
    {
        // Antes de irnos, asegúrate de que el tiempo corra (por si acaso)
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscenaTutorial);
    }

    // --- BOTONES ---
    public void BotonIniciarSesion()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void BotonRegistrarse()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    // --- CORRUTINA DE LOGIN ---
    private IEnumerator Login(string _email, string _password)
    {
        warningLoginText.text = "Conectando...";

        // Iniciamos la tarea de Firebase
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            // --- MANEJO DE ERRORES ---
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Error al iniciar sesión.";
            switch (errorCode)
            {
                case AuthError.MissingEmail: message = "Falta el correo."; break;
                case AuthError.MissingPassword: message = "Falta la contraseña."; break;
                case AuthError.WrongPassword: message = "Contraseña incorrecta."; break;
                case AuthError.InvalidEmail: message = "Correo inválido."; break;
                case AuthError.UserNotFound: message = "La cuenta no existe."; break;
            }
            warningLoginText.text = message;
        }
        else
        {
            // --- ÉXITO DE LOGUEO ---
            user = LoginTask.Result.User;
            warningLoginText.text = "¡Logueado con éxito!";

            // 1. EL PUENTE (Paso 1): Pasamos el nombre de Firebase a tu sistema Global
            // Creamos la instancia del objeto 'user' dentro de GlobalSession
            GlobalSession.user = new user { userName = user.DisplayName };

            Debug.LogFormat("Persistencia activada: {0} guardado en GlobalSession", GlobalSession.user.userName);

            // 2. LA BIENVENIDA (Paso 2): Llamamos a la función que activa el panel
            // Le pasamos el nombre que recibimos de Firebase
            MostrarPanelBienvenida(user.DisplayName);
        }
    }

    // --- CORRUTINA DE REGISTRO ---
    private IEnumerator Register(string _email, string _password, string _username)
    {
        // 1. Validaciones locales (antes de ir a internet)
        if (string.IsNullOrEmpty(_username))
        {
            warningRegisterText.text = "Falta el nombre de usuario.";
            yield break; // Detiene la corrutina
        }
        if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Las contraseñas no coinciden.";
            yield break;
        }

        warningRegisterText.text = "Creando cuenta...";

        // 2. Crear el usuario en Firebase
        Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

        if (RegisterTask.Exception != null)
        {
            // Manejo de errores detallado
            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Error al registrar.";
            switch (errorCode)
            {
                case AuthError.MissingEmail: message = "Falta el correo."; break;
                case AuthError.MissingPassword: message = "Falta la contraseña."; break;
                case AuthError.WeakPassword: message = "La contraseña es muy débil."; break;
                case AuthError.EmailAlreadyInUse: message = "El correo ya está en uso."; break;
            }
            warningRegisterText.text = message;
        }
        else
        {
            // 3. Si se creó, le asigna el Nombre de Usuario (Perfil)
            user = RegisterTask.Result.User;

            if (user != null)
            {
                UserProfile profile = new UserProfile { DisplayName = _username };
                Task ProfileTask = user.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                if (ProfileTask.Exception != null)
                {
                    warningRegisterText.text = "Error al asignar el nombre.";
                }
                else
                {
                    // 4. ÉXITO TOTAL: Vuelve al panel de Login
                    warningRegisterText.text = "";
                    emailRegisterField.text = "";
                    passwordRegisterField.text = "";
                    passwordRegisterVerifyField.text = "";
                    usernameRegisterField.text = "";

                    // Usa elsistema de navegación para volver
                    mainMenuController.VolverAInicioDesdeRegistro();
                }
            }
        }
    }
}
