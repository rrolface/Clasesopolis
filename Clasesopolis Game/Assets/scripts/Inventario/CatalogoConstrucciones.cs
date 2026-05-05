using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Catálogo central con TODAS las construcciones del juego.
/// Es un ScriptableObject: créalo desde el menú
/// Assets > Create > Clasesopolis > Catálogo de Construcciones
/// y rellena la lista en el Inspector. Una sola instancia compartida por
/// todo el proyecto.
/// </summary>
[CreateAssetMenu(
    fileName = "CatalogoConstrucciones",
    menuName = "Clasesopolis/Catálogo de Construcciones")]
public class CatalogoConstrucciones : ScriptableObject
{
    [Tooltip("Todas las construcciones disponibles en el juego.")]
    public List<Construccion> construcciones = new List<Construccion>();

    /// <summary>
    /// Devuelve la construcción con ese id, o null si no está en el catálogo.
    /// </summary>
    public Construccion BuscarPorId(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        for (int i = 0; i < construcciones.Count; i++)
        {
            if (construcciones[i] != null && construcciones[i].id == id)
                return construcciones[i];
        }
        return null;
    }
}
