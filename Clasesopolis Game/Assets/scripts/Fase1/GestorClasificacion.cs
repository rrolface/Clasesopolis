using UnityEngine;
using System.Collections.Generic;

public class GestorClasificacion : MonoBehaviour
{
    public FlujoFases flujoPrincipal;
    public List<ObjetoClasificable> todosLosObjetos;
    

    [Header("Paneles de Resultado")]
    public GameObject panelConfirmacion;
    public GameObject panelError;
    public GameObject panelExito;

    private int puntajeTemporal = 0; // Puntos calculados en el intento actual

    public void BotonComprobar()
    {
        panelConfirmacion.SetActive(true);
    }

    public void ConfirmarValidacion()
    {
        panelConfirmacion.SetActive(false);
        int aciertos = 0;
        int total = todosLosObjetos.Count;

        foreach (var obj in todosLosObjetos)
        {
            if (obj.categoriaActual == obj.categoriaCorrecta) aciertos++;
        }

        // Cálculo proporcional redondeado
        float calculo = ((float)aciertos / total) * 100f;
        puntajeTemporal = Mathf.RoundToInt(calculo);

        if (aciertos == total)
        {
            panelExito.SetActive(true);
        }
        else
        {
            panelError.SetActive(true);
            // Podrías mostrar en el panel de error cuántos aciertos lleva
            Debug.Log($"Aciertos: {aciertos}/{total}. Puntaje potencial: {puntajeTemporal}");
        }
    }

    public void Reintentar()
    {
        puntajeTemporal = 0; // IMPORTANTE: Reseteamos para que no se acumule
        foreach (var obj in todosLosObjetos) obj.ResetearPosicion();
        panelError.SetActive(false);
    }

    public void OmitirYSeguir()
    {
        // El usuario se rinde o decide seguir con lo que sacó
        FinalizarReto();
    }

    public void FinalizarConExito()
    {
        // El usuario logró el 100%
        FinalizarReto();
    }

    private void FinalizarReto()
    {
        // 1. Guardamos los puntos
        ProgresoGlobal.SumarXP(puntajeTemporal);

        // 2. Apagamos todo lo visual del reto
        panelError.SetActive(false);
        panelExito.SetActive(false);

        if (flujoPrincipal != null)
        {
            // 3. Liberamos el botón (por si acaso)
            flujoPrincipal.HabilitarBotonContinuar();

            // 4. ¡LA CLAVE! Le pedimos al flujo que pase al siguiente paso INMEDIATAMENTE
            flujoPrincipal.AvanzarPaso();
        }

        // 5. Matamos el objeto del reto
        gameObject.SetActive(false);
    }
}