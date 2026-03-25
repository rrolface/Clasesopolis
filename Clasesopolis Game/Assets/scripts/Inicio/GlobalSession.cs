using UnityEngine;

public static class GlobalSession
{
    // Esta es la "caja" donde vive el usuario actual
    public static user user;

    // Para saber si hay alguien jugando
    public static bool IsAuthenticated() => user != null;

    // Para cuando el usuario quiera salir al menú principal
    public static void Logout()
    {
        user = null;
    }
}