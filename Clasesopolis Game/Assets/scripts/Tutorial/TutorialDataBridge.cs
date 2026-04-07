using UnityEngine;

public static class TutorialDataBridge
{
    public static int indiceDeInicio = 0;
    public static string nombrePasoAIniciar = ""; // Nueva variable para buscar por nombre

    public static void ConfigurarSaltoPorNombre(string nombre)
    {
        nombrePasoAIniciar = nombre;
        indiceDeInicio = -1; // Desactivamos el índice para priorizar el nombre
    }
}