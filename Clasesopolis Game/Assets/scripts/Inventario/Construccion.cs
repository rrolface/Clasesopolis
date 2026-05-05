using System;
using UnityEngine;

/// <summary>
/// Datos de una construcción que se puede ganar y colocar en el modo libre.
/// El campo 'id' es el que se persiste en el inventario, así que NO debe cambiar
/// una vez que el juego esté en uso (cambiarlo borraría las construcciones que
/// los jugadores ya tengan guardadas).
/// </summary>
[Serializable]
public class Construccion
{
    [Tooltip("Identificador único persistente. Ej: 'casa_basica', 'tienda_neon'. " +
             "No debe cambiarse después de publicar el juego.")]
    public string id;

    [Tooltip("Nombre legible para mostrar en UI.")]
    public string nombre;

    [TextArea]
    [Tooltip("Descripción opcional para mostrar en el inventario o panel de recompensa.")]
    public string descripcion;

    [Tooltip("Icono usado en el inventario y panel de recompensa.")]
    public Sprite icono;

    [Tooltip("Prefab que se instanciará en el escenario al construir.")]
    public GameObject prefab;
}
