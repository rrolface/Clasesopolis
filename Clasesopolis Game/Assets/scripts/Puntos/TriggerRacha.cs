using UnityEngine;

public class TriggerRacha : MonoBehaviour
{
    void OnEnable()
    {
        // Llama a la función que ya tenemos para validar las 24h y sumar +1
        ProgresoGlobal.RegistrarFinDeFase();
        Debug.Log("Racha actualizada en el sistema global.");
    }
}