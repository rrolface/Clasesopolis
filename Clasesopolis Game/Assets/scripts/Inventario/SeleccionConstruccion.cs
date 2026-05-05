using UnityEngine;

/// <summary>
/// Estado global mínimo: cuál es la construcción que el jugador eligió
/// del inventario para colocar en la próxima ZonaConstruccion.
///
/// Vive como clase estática para que cualquier ZonaConstruccion pueda
/// consultarla sin tener referencias cruzadas.
/// </summary>
public static class SeleccionConstruccion
{
    public static Construccion Actual;

    public static void Limpiar()
    {
        Actual = null;
    }
}
