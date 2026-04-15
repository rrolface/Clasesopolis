using UnityEngine;

public class TriggerInsignia : MonoBehaviour
{
    public string nombreDeEstaInsignia;

    void OnEnable()
    {
        // Lo guarda en la lista global
        ProgresoGlobal.GanarInsignia(nombreDeEstaInsignia);
    }
}