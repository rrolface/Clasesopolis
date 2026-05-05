using UnityEngine;

/// <summary>
/// Componente "disparador": al activarse el GameObject que lo lleva,
/// le suma +1 a la racha. Se usa colocándolo en una de las listas
/// activarAlEntrar de un PasoDeFase.
/// </summary>
public class TriggerRacha : MonoBehaviour
{
    void OnEnable()
    {
        ProgresoGlobal.IncrementarRacha();
    }
}
